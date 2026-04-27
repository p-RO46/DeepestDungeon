using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextDoorClick : MonoBehaviour, MapObjClick
{
    public PlayerManager playerManager;

    private void Start()
    {
        playerManager = GameObject.Find("Player").GetComponent<PlayerManager>();
    }

    public void ObjClicked()
    {
        if (playerManager.playerRoutine != null) return;
        if (playerManager.isBusy) return;

        playerManager.isBusy = true;
        playerManager.playerRoutine = StartCoroutine(NextFloorLoad());
    }

    private IEnumerator NextFloorLoad()
    {
        SpriteRenderer doorRenderer = GetComponent<SpriteRenderer>();
        yield return playerManager.MoveNextFloorRoutine(doorRenderer);

        Debug.Log(MapManager.Instance.currentFloor);

        if (MapManager.Instance.currentFloor == MapManager.Instance.maxFloor - 1)
        {
            // 엔딩 씬으로 이동
            SceneManager.LoadScene("EndingScene");
        }
        else
        {
            // 로딩씬으로 이동?
            SceneManager.LoadScene("GameScene");
        }
    }
}
