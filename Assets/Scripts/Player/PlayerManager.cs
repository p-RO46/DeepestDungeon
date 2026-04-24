using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerManager : MonoBehaviour
{
    public Animator playerAnimator;
    private MapManager mapManager;

    public int playerR;
    public int playerC;

    // public float initX;
    public float initY = -2.4f;

    private Dictionary<Door, Vector2Int> direction = new Dictionary<Door, Vector2Int>()
    {
        { Door.LEFT, new Vector2Int(0, -1) },
        { Door.RIGHT, new Vector2Int(0, 1) },
        { Door.DOWN, new Vector2Int(1, 0) },
        { Door.UP, new Vector2Int(-1, 0) }
    };

    private void Start()
    {
        mapManager = MapManager.Instance;
    }

    public void MoveDungeon(Door dir)
    {
        int nextRow = playerR + direction[dir].x;
        int nextCol = playerC + direction[dir].y;

        if (nextRow >= 0 && nextRow < mapManager.floorData[mapManager.currentFloor].row &&
            nextCol >= 0 && nextCol < mapManager.floorData[mapManager.currentFloor].column)
        {
            playerR = nextRow;
            playerC = nextCol;
        }
        else if (nextRow == mapManager.floorData[mapManager.currentFloor].row &&
            nextCol >= 0 && nextCol < mapManager.floorData[mapManager.currentFloor].column) // 다음 층
        {
            // 다음 층으로 넘어가겠냐는 메시지와 함께
            // 씬 전환
            Debug.Log("다음 층으로 갑니까?");
            return;
        }
        else if (nextRow < 0 && nextCol == mapManager.initCol) // 이전 층으로 넘어가려고 시도
        {
            // 이전 층으로 못 돌아간다는 메시지 출력
            Debug.Log("이전 층으로는 이동 불가!");
            return;
        }
        else // 뭐가 잘못된 경우
        {
            Debug.Log("PlayerManager/MoveDungeon : 존재하지 않는 던전으로 접근.");
            return;
        }
    }

    public void MovePlayer()
    {
        // 애니메이션 필요! 코루틴?
        Vector3 newPos = new Vector3(playerC * MapManager.col_distance, playerR * MapManager.row_distance, 0);

        transform.position = newPos + new Vector3(0, initY, 0);
        Camera.main.transform.position = newPos + new Vector3(0, 0, -10);
    }

    public IEnumerator WalkLeftRoutine()
    {
        // 왼쪽을 바라보도록
        transform.localScale = new Vector3(1, 1, 1);

        playerAnimator.SetBool("Run", true);
        while (playerC * MapManager.col_distance < transform.position.x)
        {
            transform.Translate(new Vector3(-0.05f, 0, 0));
            yield return null;
        }
        playerAnimator.SetBool("Run", false);

        MovePlayer();
    }
    public IEnumerator WalkRightRoutine()
    {
        // 오른쪽을 바라보도록
        transform.localScale = new Vector3(-1, 1, 1);

        playerAnimator.SetBool("Run", true);
        while (transform.position.x < playerC * MapManager.col_distance)
        {
            transform.Translate(new Vector3(0.05f, 0, 0));
            yield return null;
        }
        playerAnimator.SetBool("Run", false);

        MovePlayer();
    }

    public IEnumerator WalkDownRoutine()
    {
        playerAnimator.SetBool("Jump", true);
        while (playerR * MapManager.row_distance < transform.position.y)
        {
            transform.Translate(new Vector3(0, -0.05f, 0));
            yield return null;
        }
        playerAnimator.SetBool("Jump", false);

        MovePlayer();
    }

    public IEnumerator WalkUpRoutine()
    {
        playerAnimator.SetBool("Jump", true);
        while (transform.position.y < playerR * MapManager.row_distance)
        {
            transform.Translate(new Vector3(0, 0.05f, 0));
            yield return null;
        }
        playerAnimator.SetBool("Jump", false);

        MovePlayer();
    }
}
