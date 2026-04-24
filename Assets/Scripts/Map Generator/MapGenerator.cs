using UnityEngine;
using System.Collections.Generic;

public class Point : System.IEquatable<Point>
{
    public int R, C;
    public Point(int r, int c) { R = r; C = c; }

    public override bool Equals(object obj) => obj is Point p && Equals(p);
    public bool Equals(Point p) => p != null && R == p.R && C == p.C;
    public override int GetHashCode() => (R, C).GetHashCode();
    public static bool operator ==(Point a, Point b) => Equals(a, b);
    public static bool operator !=(Point a, Point b) => !Equals(a, b);
}

public class MapGenerator
{
    private Dungeon[,] fullMap;
    private HashSet<Point> walkableTiles;
    private int row, column;

    // -----------------------------------------
    // Public Entry Point
    // -----------------------------------------
    public Dungeon[,] GenerateFloor(int row, int column, int branchCount, float probability, int maxLength)
    {
        this.row = row;
        this.column = column;

        // 1. 초기화
        InitMap();

        // 2. 시작 / 끝 지점
        int startC = Random.Range(0, column);
        int endC = Random.Range(0, column);
        Point start = new Point(0, startC);
        Point end = new Point(row - 1, endC);

        // 3. MainPath 생성
        List<Point> mainPath = GenerateMainPath(start, end);
        foreach (Point p in mainPath)
            PlaceTile(p, MapType.BASIC);

        // START / END 덮어쓰기
        PlaceTile(start, MapType.START);
        PlaceTile(end, MapType.END);

        // 4. SubPath 생성
        AddBranches(mainPath, branchCount, probability, maxLength);

        // 5. 문(door) 연결
        ConnectDoors();

        return fullMap;
    }

    // -----------------------------------------
    // 1. 초기화
    // -----------------------------------------
    private void InitMap()
    {
        walkableTiles = new HashSet<Point>();
        fullMap = new Dungeon[column, row];

        for (int r = 0; r < row; r++)
            for (int c = 0; c < column; c++)
                fullMap[c, r] = new Dungeon(MapType.EMPTY, r, c);
    }

    // -----------------------------------------
    // 2. 타일 배치 헬퍼
    // -----------------------------------------
    private void PlaceTile(Point p, MapType type)
    {
        fullMap[p.C, p.R] = new Dungeon(type, p.R, p.C);
        walkableTiles.Add(p);
    }

    // -----------------------------------------
    // 3. MainPath
    //    start(0행) -> end(row-1행)
    //    진행 방향: 아래 / 좌 / 우 (위는 제외)
    // -----------------------------------------
    private List<Point> GenerateMainPath(Point start, Point end)
    {
        List<Point> path = new List<Point>();
        HashSet<Point> visited = new HashSet<Point>();

        Point current = start;
        path.Add(current);
        visited.Add(current);

        // 목표 행까지 랜덤하게 내려감
        while (current.R < end.R)
        {
            List<Point> candidates = GetNeighbors(current, visited, allowUp: false);

            if (candidates.Count == 0) break; // 막힘 (사실상 발생 안 함)

            // 아래 방향에 60% 가중치
            Point down = new Point(current.R + 1, current.C);
            Point next;
            if (candidates.Contains(down) && Random.value < 0.4f)
                next = down;
           else
                next = candidates[Random.Range(0, candidates.Count)];

            current = next;
            path.Add(current);
            visited.Add(current);
        }

        // 목표 열까지 수평 이동 (이미 방문한 곳도 허용 — 강제 도달)
        while (current.C != end.C)
        {
            int step = end.C > current.C ? 1 : -1;
            current = new Point(current.R, current.C + step);
            if (!visited.Contains(current))
            {
                path.Add(current);
                visited.Add(current);
            }
        }

        return path;
    }

    // -----------------------------------------
    // 4. SubPath
    // -----------------------------------------
    private void AddBranches(List<Point> mainPath, int branchCount, float probability, int maxLength)
    {
        // 앞 20% 제외한 MainPath 노드를 후보로 사용
        int skipCount = mainPath.Count / 5;
        List<Point> candidates = new List<Point>(mainPath.GetRange(skipCount, mainPath.Count - skipCount));

        int actualCount = Mathf.Min(branchCount, candidates.Count);

        for (int i = 0; i < actualCount; i++)
        {
            int idx = Random.Range(0, candidates.Count);
            Point branchStart = candidates[idx];
            candidates.RemoveAt(idx); // 한 점에서 가지가 몰리지 않도록 제거

            CreateSingleBranch(branchStart, probability, maxLength);
        }
    }

    private void CreateSingleBranch(Point startNode, float probability, int maxLength)
    {
        Point current = startNode;
        int length = 0;

        while (length < maxLength)
        {
            // probability 확률로 조기 종료 -> 현재 위치를 GIFT/TRAP으로
            if (length > 0 && Random.value < probability)
            {
                PlaceDeadEnd(current);
                return;
            }

            // walkableTiles에 없는 이웃 탐색 (기존 경로 침범 금지)
            List<Point> neighbors = GetFreeNeighbors(current);

            if (neighbors.Count == 0)
            {
                // 더 이상 뻗을 곳 없음 -> Dead-end
                if (length == 0) return;

                PlaceDeadEnd(current);
                return;
            }

            // 다음 칸으로 이동
            Point next = neighbors[Random.Range(0, neighbors.Count)];
            PlaceTile(next, MapType.BASIC);
            current = next;
            length++;
        }

        // 최대 길이 도달 → Dead-end
        PlaceDeadEnd(current);
    }

    // Dead-end 처리: GIFT 또는 TRAP (이미 BASIC이면 덮어씀)
    private void PlaceDeadEnd(Point p)
    {
        MapType deadType = Random.Range(0, 2) == 0 ? MapType.GIFT : MapType.TRAP;
        fullMap[p.C, p.R].type = deadType; // walkableTiles에는 이미 있으므로 type만 변경
    }

    // -----------------------------------------
    // 5. 문(door) 연결
    // -----------------------------------------
    private void ConnectDoors()
    {
        Point[] dirs =
        {
            new Point( 1,  0), // Down
            new Point(-1,  0), // Up
            new Point( 0, -1), // Left
            new Point( 0,  1), // Right
        };

        foreach (Point tile in walkableTiles)
        {
            for (int i = 0; i < 4; i++)
            {
                int nr = tile.R + dirs[i].R;
                int nc = tile.C + dirs[i].C;

                if (nr >= 0 && nr < row && nc >= 0 && nc < column
                    && fullMap[nc, nr].type != MapType.EMPTY)
                {
                    switch (i)
                    {
                        case 0: fullMap[tile.C, tile.R].down_door = true; break;
                        case 1: fullMap[tile.C, tile.R].up_door = true; break;
                        case 2: fullMap[tile.C, tile.R].left_door = true; break;
                        case 3: fullMap[tile.C, tile.R].right_door = true; break;
                    }
                }
            }
        }
    }

    // -----------------------------------------
    // 유틸: 이웃 탐색
    // -----------------------------------------

    // MainPath용: visited 기준, allowUp으로 방향 제어
    private List<Point> GetNeighbors(Point p, HashSet<Point> visited, bool allowUp)
    {
        List<Point> result = new List<Point>();
        Point[] dirs =
        {
            new Point( 1,  0), // Down
            new Point(-1,  0), // Up
            new Point( 0, -1), // Left
            new Point( 0,  1), // Right
        };

        foreach (Point dir in dirs)
        {
            if (!allowUp && dir.R == -1) continue; // 위 방향 제외

            int nr = p.R + dir.R;
            int nc = p.C + dir.C;

            if (IsInBounds(nr, nc) && !visited.Contains(new Point(nr, nc)))
                result.Add(new Point(nr, nc));
        }
        return result;
    }

    // SubPath용: walkableTiles 기준 (기존 경로 침범 금지)
    private List<Point> GetFreeNeighbors(Point p)
    {
        List<Point> result = new List<Point>();
        Point[] dirs =
        {
            new Point( 1,  0), // Down
            new Point(-1,  0), // Up
            new Point( 0, -1), // Left
            new Point( 0,  1), // Right
        };

        foreach (Point dir in dirs)
        {
            int nr = p.R + dir.R;
            int nc = p.C + dir.C;

            if (IsInBounds(nr, nc) && !walkableTiles.Contains(new Point(nr, nc)))
                result.Add(new Point(nr, nc));
        }
        return result;
    }

    private bool IsInBounds(int r, int c)
        => r >= 0 && r < row && c >= 0 && c < column;
}