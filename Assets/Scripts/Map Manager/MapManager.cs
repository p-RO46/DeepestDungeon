using System;
using System.Collections.Generic;
using UnityEngine;

// 처음부터 모든 맵을 만들고 그 정보를 저장하는 싱글톤 객체.

[Serializable]
public struct Floor
{
    public int floor;
    public int row;
    public int column;
    public int maxLength;
    public int branchCount;
    public float probability;
}

public class MapManager : MonoBehaviour
{
    private static MapManager instance = null;

    public List<Dungeon[,]> fullDungeonData = new List<Dungeon[,]>();
    public Dictionary<Point, GameObject> currentFloorInstances = new Dictionary<Point, GameObject>();

    [Header("Map Settings")]
    public List<Floor> floorData;

    public int currentFloor = -1;
    public int initRow = 0;
    public int initCol;

    public int maxFloor;

    [Header("Instantiate Settings")]
    public float row_distance = -12f;
    public float col_distance = 20f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public static MapManager Instance
    {
        get
        {
            if (instance == null)
            {
                return null;
            }
            return instance;
        }
    }

    public void InitializeMap()
    {
        MapGenerator mapGenerator = new MapGenerator();

        foreach(Floor floor in floorData)
        {
            fullDungeonData.Add(mapGenerator.GenerateFloor(floor.row, floor.column, floor.branchCount, floor.probability, floor.maxLength));
        }

        maxFloor = fullDungeonData.Count;
    }

    public void SetCurrentFloor(PlayerManager playerManager)
    {
        currentFloor++;
        foreach (Dungeon dungeon in fullDungeonData[currentFloor])
        {
            if (dungeon.type == MapType.START)
            {
                initRow = dungeon.R;
                initCol = dungeon.C;

                playerManager.playerR = dungeon.R;
                playerManager.playerC = dungeon.C;
                return;
            }
        }

        // 여기까지 도달한 경우 오류
        Debug.LogError("오류: START 지점을 발견하지 못함");
    }
}
