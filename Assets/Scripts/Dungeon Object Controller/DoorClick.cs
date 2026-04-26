using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Door
{
    LEFT,
    RIGHT,
    DOWN,
    UP
}

public class DoorClick : MonoBehaviour, MapObjClick
{
    public Door doorDir;
    public PlayerManager playerManager;
    public DialogueManager dlgManager;

    [SerializeField] private SpriteRenderer doorRenderer;
    [SerializeField] private GameObject mysteryUI;

    private Dictionary<Door, Vector2Int> direction = new Dictionary<Door, Vector2Int>()
    {
        { Door.LEFT, new Vector2Int(0, -1) },
        { Door.RIGHT, new Vector2Int(0, 1) },
        { Door.DOWN, new Vector2Int(1, 0) },
        { Door.UP, new Vector2Int(-1, 0) }
    };

    public bool isUnknown = false;

    private void Start()
    {
        playerManager = GameObject.Find("Player").GetComponent<PlayerManager>();
        doorRenderer = GetComponent<SpriteRenderer>();

        Transform parent = GameObject.Find("Canvas").transform;
        dlgManager = parent.Find("Conv UI").GetComponent<DialogueManager>();
        mysteryUI = parent.Find("Mystery UI").gameObject;
    }

    public void ObjClicked()
    {
        if (playerManager.isBusy) return;
        playerManager.isBusy = true;

        if (isUnknown)
        {
            UnknownDoorClick();
        }
        else
        {
            BasicDoorClick();
        }
    }

    private void BasicDoorClick()
    {
        if (playerManager.playerRoutine != null) return;

        playerManager.MoveDungeon(doorDir);

        Vector2Int nextRoom = playerManager.NextRoom_Move(doorDir);
        Debug.Log(nextRoom);
        Transform nextDoors = null;

        if(doorDir == Door.LEFT)
        {
            nextDoors = MapManager.Instance.currentFloorInstances[new Point(nextRoom.x, nextRoom.y)].transform.Find("door_right");
        }
        else if (doorDir == Door.RIGHT)
        {
            nextDoors = MapManager.Instance.currentFloorInstances[new Point(nextRoom.x, nextRoom.y)].transform.Find("door_left");
        }
        else if (doorDir == Door.DOWN)
        {
            nextDoors = MapManager.Instance.currentFloorInstances[new Point(nextRoom.x, nextRoom.y)].transform.Find("door_up");
        }
        else if (doorDir == Door.UP)
        {
            nextDoors = MapManager.Instance.currentFloorInstances[new Point(nextRoom.x, nextRoom.y)].transform.Find("door_down");
        }

        nextDoors.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
        nextDoors.GetChild(1).GetComponent<SpriteRenderer>().enabled = true;

        playerManager.playerRoutine = StartCoroutine(playerManager.MoveRoutine(doorDir, doorRenderer, nextDoors));
    }

    private void UnknownDoorClick()
    {
        // Malt와의 대화가 시작됨
        // 1) 물어볼 것인지 말 것인지 선택
        // 1-1) 물어보기
        // 1-2) 안물어보고 들어가기
        // 1-3) 좀 더 생각해보기
        mysteryUI.SetActive(true);
        // 1. 버튼 컴포넌트 가져오기
        Transform buttons = mysteryUI.transform.Find("Buttons");
        Button askBtn = buttons.Find("Button_Ask").GetComponent<Button>();
        Button enterBtn = buttons.Find("Button_Enter").GetComponent<Button>();
        Button thinkBtn = buttons.Find("Button_Think").GetComponent<Button>();

        // 2. AddListener를 사용하여 클릭 이벤트 등록
        askBtn.onClick.RemoveAllListeners();
        askBtn.onClick.AddListener(AskMalt);

        enterBtn.onClick.RemoveAllListeners();
        enterBtn.onClick.AddListener(JustGoIn);

        thinkBtn.onClick.RemoveAllListeners();
        thinkBtn.onClick.AddListener(ThinkLittleMore);

        // 2) 물어본 경우: 물어보기 선택지 삭제하고, 들은 결과를 띄워줌 -> 들어가기 or 고민하기 둘 중 하나
        // 3) 안물어본 경우: 똑같은 세 개 선택지 띄움
    }

    public void AskMalt()
    {
        mysteryUI.SetActive(false);

        ThirstManager thirstManager = FindAnyObjectByType<ThirstManager>();
        if (!thirstManager.UsePoint())
        {
            dlgManager.MaltThirsty();
            return;
        }

        dlgManager.AskMalt(playerManager.NextRoom_Smell(doorDir));
    }

    public void JustGoIn()
    {
        mysteryUI.SetActive(false);
        BasicDoorClick();
    }

    public void ThinkLittleMore()
    {
        mysteryUI.SetActive(false);
        playerManager.isBusy = false;
    }
}
