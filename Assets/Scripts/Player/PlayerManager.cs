using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    public DialogueManager dlgManager;
    public Animator playerAnimator;
    private MapManager mapManager;

    public int playerR;
    public int playerC;

    private int pastR;
    private int pastC;

    [SerializeField] private float moveTime = 1f;
    [SerializeField] private float jumpTime = 1f;
    [SerializeField] private float jumpHeight = 2f;

    [SerializeField] private float dropTime = 0.2f;
    // public float initX;
    // public float initY;

    [Header("Door Sprite")]
    [SerializeField] private Sprite[] leftDoor = new Sprite[2];
    [SerializeField] private Sprite[] rightDoor = new Sprite[2];
    [SerializeField] private Sprite[] downDoor = new Sprite[2];

    public bool isBusy = false;

    public Coroutine playerRoutine = null;

    [Header("Audio Clip")]
    [SerializeField] private AudioClip door;
    [SerializeField] private AudioClip walk;

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
            pastR = playerR;
            pastC = playerC;

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
        Vector3 newPos = new Vector3(playerC * MapManager.Instance.col_distance, playerR * MapManager.Instance.row_distance, 0);
        transform.position = newPos;
        Camera.main.transform.position = newPos + new Vector3(0, 0, -10);
    }

    public IEnumerator MoveRoutine(Door dir, SpriteRenderer doorRenderer, Transform nextDoors)
    {
        SpriteRenderer openRenderer = doorRenderer.transform.Find("open").gameObject.GetComponent<SpriteRenderer>();
        SpriteRenderer closeRenderer = doorRenderer.transform.Find("close").gameObject.GetComponent<SpriteRenderer>();

        Transform dungeon = doorRenderer.transform.parent;
        DungeonPrefab dungeonPrefab = dungeon.GetComponent<DungeonPrefab>();

        float dist = dungeonPrefab.doorDistance;

        mapManager.currentFloorInstances[new Point(playerR, playerC)].SetActive(true);
        Vector3 newPos = new Vector3(playerC * MapManager.Instance.col_distance, playerR * MapManager.Instance.row_distance, 0);

        // 4.5 만큼 이동 후 Attack -> 그 후 목적지 도달
        float elapsedTime = 0;
        
        float init = transform.position.x;
        float end = playerC * MapManager.Instance.col_distance;


        AudioSource audioSource = GameObject.Find("SE Player").GetComponent<AudioSource>();
        audioSource.clip = walk;

        if (dir == Door.LEFT)
        {
            // 왼쪽을 바라보도록
            transform.localScale = new Vector3(1, 1, 1);
            playerAnimator.SetBool("Run", true);

            elapsedTime = 0;

            audioSource.Play();
            while (elapsedTime < moveTime)
            {
                elapsedTime += Time.deltaTime * 2;

                Vector3 nextPos = transform.position;
                nextPos.x = Mathf.Lerp(init, init - dist, elapsedTime / moveTime);
                transform.position = nextPos;

                yield return null;
            }
            audioSource.Stop();

            playerAnimator.SetBool("Run", false);
            yield return AttackRoutine(dir, doorRenderer);

            playerAnimator.SetBool("Run", true);

            bool camMoved = false;
            init = transform.position.x;
            elapsedTime = 0;
            audioSource.Play();
            while (elapsedTime < moveTime)
            {
                if (!camMoved && elapsedTime >= moveTime / 2)
                {
                    // 딱 한번 실행
                    Camera.main.transform.position = newPos + new Vector3(0, 0, -10);
                    camMoved = true;
                }
                elapsedTime += Time.deltaTime;

                Vector3 nextPos = transform.position;
                nextPos.x = Mathf.Lerp(init, end, elapsedTime / moveTime);
                transform.position = nextPos;

                yield return null;
            }
            audioSource.Stop();
            playerAnimator.SetBool("Run", false);
        }
        else if(dir == Door.RIGHT)
        {
            // 오른쪽을 바라보도록
            transform.localScale = new Vector3(-1, 1, 1);
            playerAnimator.SetBool("Run", true);
            elapsedTime = 0;
            audioSource.Play();
            while (elapsedTime < moveTime)
            {
                elapsedTime += Time.deltaTime * 2;

                Vector3 nextPos = transform.position;
                nextPos.x = Mathf.Lerp(init, init + dist, elapsedTime / moveTime);
                transform.position = nextPos;

                yield return null;
            }
            audioSource.Stop();
            playerAnimator.SetBool("Run", false);
            yield return AttackRoutine(dir, doorRenderer);

            bool camMoved = false;
            playerAnimator.SetBool("Run", true);
            elapsedTime = 0;
            audioSource.Play();
            while (elapsedTime < moveTime)
            {
                if (!camMoved && elapsedTime >= moveTime / 2)
                {
                    // 딱 한번 실행
                    Camera.main.transform.position = newPos + new Vector3(0, 0, -10);
                    camMoved = true;
                }
                elapsedTime += Time.deltaTime;

                Vector3 nextPos = transform.position;
                nextPos.x = Mathf.Lerp(init + dist, end, elapsedTime / moveTime);
                transform.position = nextPos;

                yield return null;
            }
            audioSource.Stop();
            playerAnimator.SetBool("Run", false);
        }
        else if(dir == Door.DOWN)
        {
            yield return AttackRoutine(dir, doorRenderer);

            init = transform.position.y;
            end = playerR * MapManager.Instance.row_distance;

            bool camMoved = false;
            while (elapsedTime < moveTime)
            {
                if (!camMoved && elapsedTime >= moveTime / 2)
                {
                    // 딱 한번 실행
                    Camera.main.transform.position = newPos + new Vector3(0, 0, -10);
                    camMoved = true;
                }
                elapsedTime += Time.deltaTime * 2;

                Vector3 nextPos = transform.position;
                nextPos.y = Mathf.Lerp(init, end, elapsedTime / moveTime);
                transform.position = nextPos;

                yield return null;
            }
        }
        else if(dir == Door.UP)
        {
            yield return JumpRoutine();

            openRenderer.enabled = true;
            closeRenderer.enabled = false;

            init = transform.position.y;
            end = playerR * MapManager.Instance.row_distance;

            bool camMoved = false;
            while (elapsedTime < moveTime)
            {
                if (!camMoved && elapsedTime >= moveTime / 2)
                {
                    // 딱 한번 실행
                    Camera.main.transform.position = newPos + new Vector3(0, 0, -10);
                    camMoved = true;
                }
                elapsedTime += Time.deltaTime;

                Vector3 nextPos = transform.position;
                nextPos.y = Mathf.Lerp(init, end, elapsedTime / moveTime);
                transform.position = nextPos;

                yield return null;
            }
        }

        MovePlayer();

        openRenderer.enabled = false;
        closeRenderer.enabled = true;
        if(nextDoors != null)
        {
            nextDoors.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
            nextDoors.GetChild(1).GetComponent<SpriteRenderer>().enabled = false;
        }

        if(mapManager.fullDungeonData[mapManager.currentFloor][playerC, playerR].type == MapType.GIFT)
        {
            Transform fountain = mapManager.currentFloorInstances[new Point(playerR, playerC)].transform.Find("fountain");
            FountainClick fc = fountain.GetComponent<FountainClick>();

            if (!fc.getted)
            {
                fountain.GetComponent<FountainClick>().GetItem();
            }
        }
        if (mapManager.fullDungeonData[mapManager.currentFloor][playerC, playerR].type == MapType.TRAP)
        {
            yield return 0.5f;
            Transform spike = mapManager.currentFloorInstances[new Point(playerR, playerC)].transform.Find("spike");

            init = 2.3f;
            end = -3f;
            elapsedTime = 0;
            while (elapsedTime < dropTime)
            {
                elapsedTime += Time.deltaTime;

                Vector3 nextPos = spike.localPosition;
                nextPos.y = Mathf.Lerp(init, end, elapsedTime / dropTime);
                spike.localPosition = nextPos;

                yield return null;
            }

            SceneManager.LoadScene("EndingScene");
        }

        mapManager.currentFloorInstances[new Point(pastR, pastC)].SetActive(false);
        playerRoutine = null;
        isBusy = false;
    }

    private IEnumerator JumpRoutine()
    {
        float elapsedTime = 0;
        float init = transform.position.y;
        float end = init + jumpHeight;

        playerAnimator.SetTrigger("Jump");
        yield return new WaitForSeconds(0.3f);

        while (elapsedTime < jumpTime)
        {
            elapsedTime += Time.deltaTime;

            Vector3 nextPos = transform.position;
            nextPos.y = Mathf.Lerp(init, end, elapsedTime / jumpTime);
            transform.position = nextPos;

            yield return null;
        }
    }

    private IEnumerator AttackRoutine(Door dir, SpriteRenderer doorRenderer)
    {
        playerAnimator.SetTrigger("Attack");

        yield return null;

        AnimatorStateInfo stateInfo = playerAnimator.GetCurrentAnimatorStateInfo(0);
        while (stateInfo.normalizedTime < 1.0f)
        {
            stateInfo = playerAnimator.GetCurrentAnimatorStateInfo(0);
            yield return null;
        }
        GameObject.Find("SE Player").GetComponent<AudioSource>().PlayOneShot(door);

        SpriteRenderer openRenderer = doorRenderer.transform.Find("open").gameObject.GetComponent<SpriteRenderer>();
        openRenderer.enabled = true;

        SpriteRenderer closeRenderer = doorRenderer.transform.Find("close").gameObject.GetComponent<SpriteRenderer>();
        closeRenderer.enabled = false;

        yield return new WaitForSeconds(0.5f);
        // Debug.Log("애니메이션 종료!");
    }

    public IEnumerator MoveNextFloorRoutine(SpriteRenderer doorRenderer)
    {
        yield return AttackRoutine(Door.DOWN, doorRenderer);

        SpriteRenderer openRenderer = doorRenderer.transform.Find("open").gameObject.GetComponent<SpriteRenderer>();
        SpriteRenderer closeRenderer = doorRenderer.transform.Find("close").gameObject.GetComponent<SpriteRenderer>();

        float elapsedTime = 0;
        float init = transform.position.y;
        float end = init + MapManager.Instance.row_distance;

        while (elapsedTime < moveTime)
        {
            elapsedTime += Time.deltaTime * 2;

            Vector3 nextPos = transform.position;
            nextPos.y = Mathf.Lerp(init, end, elapsedTime / moveTime);
            transform.position = nextPos;

            yield return null;
        }

        openRenderer.enabled = false;
        closeRenderer.enabled = true;

        yield return new WaitForSeconds(1f);
        playerRoutine = null;
        isBusy = false;
    }

    public IEnumerator MoveStartRoutine()
    {
        isBusy = true;

        Vector3 initPos = transform.position;
        float elapsedTime = 0;
        float end = transform.position.y;
        float init = end - MapManager.Instance.row_distance;

        while (elapsedTime < moveTime)
        {
            elapsedTime += Time.deltaTime * 2;

            Vector3 nextPos = transform.position;
            nextPos.y = Mathf.Lerp(init, end, elapsedTime / moveTime);
            transform.position = nextPos;

            yield return null;
        }

        transform.position = initPos;
        yield return new WaitForSeconds(0.1f);

        playerRoutine = null;

        dlgManager.FloorStartDialogue();
    }

    public Vector2Int NextRoom_Move(Door dir)
    {
        int nextRow = playerR;
        int nextCol = playerC;

        if (nextRow >= 0 && nextRow < mapManager.floorData[mapManager.currentFloor].row &&
            nextCol >= 0 && nextCol < mapManager.floorData[mapManager.currentFloor].column)
        {
            return new Vector2Int(nextRow, nextCol);
            // return mapManager.fullDungeonData[mapManager.currentFloor][nextCol, nextRow].type == MapType.TRAP;
        }
        else // 뭐가 잘못된 경우
        {
            Debug.Log("PlayerManager/MoveDungeon : 존재하지 않는 던전으로 접근.");
            return new Vector2Int(-1, -1);
        }
    }

    public bool NextRoom_Smell(Door dir)
    {
        int nextRow = playerR + direction[dir].x;
        int nextCol = playerC + direction[dir].y;

        if (nextRow >= 0 && nextRow < mapManager.floorData[mapManager.currentFloor].row &&
            nextCol >= 0 && nextCol < mapManager.floorData[mapManager.currentFloor].column)
        {
            // return new Vector2Int(nextRow, nextCol);
            return mapManager.fullDungeonData[mapManager.currentFloor][nextCol, nextRow].type == MapType.TRAP;
        }
        else // 뭐가 잘못된 경우
        {
            Debug.Log("PlayerManager/MoveDungeon : 존재하지 않는 던전으로 접근.");
            return false;
        }
    }
}
