using System.Collections.Generic;
using UnityEngine;

public class CurrentFloorManager : MonoBehaviour
{
    [Header("Dungeon Prefabs")]
    [SerializeField] private List<GameObject> dungeon_end;
    [SerializeField] private List<GameObject> dungeon_basic;
    [SerializeField] private List<GameObject> dungeon_gift;
    [SerializeField] private List<GameObject> dungeon_trap;

    public MapManager mapManager;
    public List<GameObject> currentInstances = new List<GameObject>();

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
                    MapType.START or MapType.END => dungeon_end,
                    MapType.BASIC => dungeon_basic,
                    MapType.GIFT => dungeon_gift,
                    MapType.TRAP => dungeon_trap,
                    _ => null // EMPTY나 정의되지 않은 타입
                };

                // 2. 리스트가 비어있거나 null이면 생성 건너뛰기
                if (targetList == null || targetList.Count == 0) continue;

                // 3. 위치 계산 및 생성 (중복 제거)
                Vector3 spawnPos = new Vector3(dungeon.C * MapManager.col_distance, dungeon.R * MapManager.row_distance, 0);
                GameObject prefab = targetList[Random.Range(0, targetList.Count)];
                GameObject instance = Instantiate(prefab, spawnPos, Quaternion.identity, transform);

                TryActivateDoor(instance, c, r, "door_left", currentFloorData, dungeon.left_door);
                TryActivateDoor(instance, c, r, "door_right", currentFloorData, dungeon.right_door);
                TryActivateDoor(instance, c, r, "door_down", currentFloorData, dungeon.down_door);
                TryActivateDoor(instance, c, r, "door_up", currentFloorData, dungeon.up_door);
            }
        }
    }

    private void TryActivateDoor(GameObject instance, int c, int r, string doorName, Dungeon[,] currentFloorData, bool condition)
    {
        if (condition)
        {
            Transform door = instance.transform.Find(doorName);
            if (door != null)
            {
                door.gameObject.SetActive(true);
                int nextC = c + dir[doorName].C;
                int nextR = r + dir[doorName].R;

                if (currentFloorData[nextC, nextR].type == MapType.TRAP || currentFloorData[nextC, nextR].type == MapType.GIFT)
                {
                    door.GetComponent<SpriteRenderer>().color = new Color(0.8f, 0, 1);
                    door.GetComponent<DoorClick>().isUnknown = true;
                }
            }
        }
    }
}
