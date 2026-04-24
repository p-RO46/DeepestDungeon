using UnityEngine;

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

    public bool isUnknown = false;

    private void Start()
    {
        playerManager = GameObject.Find("Player").GetComponent<PlayerManager>();
    }

    public void ObjClicked()
    {
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
        playerManager.MoveDungeon(doorDir);

        StartCoroutine(playerManager.MoveRoutine(doorDir));
    }

    private void UnknownDoorClick()
    {
        // Malt와의 대화가 시작됨
        // 1) 물어볼 것인지 말 것인지 선택
        // 1-1) 물어보기
        // 1-2) 안물어보고 들어가기
        // 1-3) 좀 더 생각해보기

        // 2) 물어본 경우: 물어보기 선택지 삭제하고, 들은 결과를 띄워줌 -> 들어가기 or 고민하기 둘 중 하나
        // 3) 안물어본 경우: 똑같은 세 개 선택지 띄움
    }
}
