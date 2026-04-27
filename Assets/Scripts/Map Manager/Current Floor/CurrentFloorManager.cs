using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CurrentFloorManager : MonoBehaviour
{
    [Header("Dungeon Prefabs")]
    [SerializeField] private List<GameObject> dungeon_start;
    [SerializeField] private List<GameObject> dungeon_end;
    [SerializeField] private List<GameObject> dungeon_basic;
    [SerializeField] private List<GameObject> dungeon_gift;
    [SerializeField] private List<GameObject> dungeon_trap;

    [Header("Item Prefabs")]
    [SerializeField] private GameObject item_coin;
    [SerializeField] private GameObject item_water;

    public MapManager mapManager;

    private Dictionary<string, Point> dir = new Dictionary<string, Point>
    {
        { "door_left", new Point(0, -1) },
        { "door_right", new Point(0, 1) },
        { "door_down", new Point(1, 0) },
        { "door_up", new Point(-1, 0) }
    };

    private void Awake()
    {
        mapManager = MapManager.Instance;
    }

    public void MakeDungeons()
    {
        if (mapManager == null)
        {
            Debug.LogError("1. mapManager 변수 자체가 null입니다.");
            return;
        }

        mapManager.currentFloorInstances.Clear();

        int floor = mapManager.currentFloor;
        Dungeon[,] currentFloorData = mapManager.fullDungeonData[floor];

        int col = currentFloorData.GetLength(0); // Column
        int row = currentFloorData.GetLength(1); // Row

        for (int r = 0; r < row; r++)
        {
            for (int c = 0; c < col; c++)
            {
                Dungeon dungeon = currentFloorData[c, r];
                // 1. 타입에 맞는 리스트 선택
                List<GameObject> targetList = dungeon.type switch
                {
                    MapType.START => dungeon_start,
                    MapType.END => dungeon_end,
                    MapType.BASIC => dungeon_basic,
                    MapType.GIFT => dungeon_gift,
                    MapType.TRAP => dungeon_trap,
                    _ => null // EMPTY나 정의되지 않은 타입
                };

                // 2. 리스트가 비어있거나 null이면 생성 건너뛰기
                if (targetList == null || targetList.Count == 0) continue;

                // 3. 위치 계산 및 생성 (중복 제거)
                Vector3 spawnPos = new Vector3(dungeon.C * MapManager.Instance.col_distance, dungeon.R * MapManager.Instance.row_distance, 0);
                GameObject prefab = targetList[Random.Range(0, targetList.Count)];
                GameObject instance = Instantiate(prefab, spawnPos, Quaternion.identity, transform);

                List<GameObject> doors = new List<GameObject>();
                doors.Add(instance.transform.Find("door_left").gameObject);
                doors.Add(instance.transform.Find("door_right").gameObject);
                doors.Add(instance.transform.Find("door_down").gameObject);
                doors.Add(instance.transform.Find("door_up").gameObject);

                foreach(GameObject door in doors)
                {
                    if (door.GetComponent<Collider2D>() != null) door.GetComponent<Collider2D>().enabled = false;

                    door.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().enabled = false;
                    door.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().enabled = false;
                    door.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().enabled = true;
                }

                TryActivateDoor(instance, c, r, "left", currentFloorData, dungeon.left_door);
                TryActivateDoor(instance, c, r, "right", currentFloorData, dungeon.right_door);
                TryActivateDoor(instance, c, r, "down", currentFloorData, dungeon.down_door);
                TryActivateDoor(instance, c, r, "up", currentFloorData, dungeon.up_door);

                SpriteRenderer bg = instance.transform.Find("Background").GetComponent<SpriteRenderer>();
                switch (MapManager.Instance.currentFloor)
                {
                    case 0:
                        bg.color = new Color(1, 1, 1);
                        break;
                    case 1:
                        bg.color = new Color(1, 0.58f, 0.9f);
                        break;
                    case 2:
                        bg.color = new Color(0.8f, 0.4f, 1f);
                        break;
                    case 3:
                        bg.color = new Color(0.4f, 0.55f, 1f);
                        break;
                    case 4:
                        bg.color = new Color(0.42f, 1f, 0.9f);
                        break;
                }

                // instance의 하위 요소로 아이템을 넣어주자
                if (dungeon.type == MapType.BASIC)
                {
                    // 1. 아이템 개수 0~1 50% / 2 30% / 3 20%  
                    float random = Random.value;

                    int count = 0;
                    if (random < 0.2) count = 3;
                    else if (random < 0.5) count = 2;
                    else if (random < 0.75) count = 1;

                    // 2. 아이템 위치
                    var shuffled = ShuffleList(instance.GetComponent<DungeonPrefab>().itemPlace);
                    for(int i = 0; i < count; i++)
                    {
                        // 3. 아이템 종류
                        GameObject item = item_coin;
                        if (Random.value < 0.1) item = item_water;

                        Instantiate(item, instance.transform.position + shuffled[i], Quaternion.identity, instance.transform);
                    }
                }

                mapManager.currentFloorInstances.Add(new Point(dungeon.R, dungeon.C), instance);

                if (dungeon.type == MapType.END)
                {
                    instance.transform.Find("load_down").gameObject.SetActive(true);
                    doors[2].GetComponent<Collider2D>().enabled = true;
                    doors[2].transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
                    doors[2].transform.GetChild(2).GetComponent<SpriteRenderer>().enabled = false;
                }
                if (dungeon.type != MapType.START)
                {
                    instance.SetActive(false);
                }
                else
                {
                    instance.transform.Find("load_up").gameObject.SetActive(true);
                }
            }
        }
    }

    private void TryActivateDoor(GameObject instance, int c, int r, string name, Dungeon[,] currentFloorData, bool condition)
    {
        if (condition)
        {
            string doorName = "door_" + name;
            Transform door = instance.transform.Find(doorName);
            if (door != null)
            {
                if (door.GetComponent<Collider2D>() != null) door.GetComponent<Collider2D>().enabled = true; ;

                door.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
                door.GetChild(2).GetComponent<SpriteRenderer>().enabled = false;

                int nextC = c + dir[doorName].C;
                int nextR = r + dir[doorName].R;

                if (currentFloorData[nextC, nextR].type == MapType.TRAP || currentFloorData[nextC, nextR].type == MapType.GIFT)
                {
                    // door.GetComponent<SpriteRenderer>().color = new Color(0, 0.8f, 1);
                    door.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(0.95f, 1, 0.45f);
                    door.GetChild(1).GetComponent<SpriteRenderer>().color = new Color(0.95f, 1, 0.45f);
                    door.GetChild(2).GetComponent<SpriteRenderer>().color = new Color(0.95f, 1, 0.45f);

                    door.GetComponent<DoorClick>().isUnknown = true;
                }
            }

            string loadName = "load_" + name;
            Transform load = instance.transform.Find(loadName);
            if (load != null)
            {
                load.gameObject.SetActive(true);
            }
        }
    }

    private List<T> ShuffleList<T>(List<T> list)
    {
        int random1, random2;
        T temp;

        for (int i = 0; i < list.Count; ++i)
        {
            random1 = Random.Range(0, list.Count);
            random2 = Random.Range(0, list.Count);

            temp = list[random1];
            list[random1] = list[random2];
            list[random2] = temp;
        }

        return list;
    }
}
