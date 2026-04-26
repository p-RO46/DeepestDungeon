using UnityEngine;

public class GameSceneManager : MonoBehaviour
{
    // 실행 순서 관리해주는 매니저.
    public PlayerManager playerManager;
    public CurrentFloorManager currentFloorManager;

    private void Start()
    {
        MapManager.Instance.InitializeMap();
        MapManager.Instance.SetCurrentFloor(playerManager);
        currentFloorManager.gameObject.SetActive(true);
        currentFloorManager.MakeDungeons();

        // Player 위치 세팅.
        playerManager.playerR = MapManager.Instance.initRow;
        playerManager.playerC = MapManager.Instance.initCol;
        // playerManager.initY = playerManager.transform.position.y;
        // Debug.Log(playerManager.initY);
        playerManager.MovePlayer();

        if (playerManager.playerRoutine == null) playerManager.playerRoutine = StartCoroutine(playerManager.MoveStartRoutine());

        // Camera 위치 세팅.
        Camera.main.transform.position = new Vector3(MapManager.Instance.initCol * MapManager.Instance.col_distance, 0, -10);
    }
}
