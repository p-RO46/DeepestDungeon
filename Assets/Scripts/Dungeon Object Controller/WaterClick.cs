using UnityEngine;

public class WaterClick : ItemClick
{
    public override void GetItem()
    {
        // Thirst 올려주기
        // 씬의 유일 컴포넌트라서 이렇게 찾아줌
        ThirstManager thirstManager = FindAnyObjectByType<ThirstManager>();
        thirstManager.AddPoint();

        SEPlayer sePlayer = GameObject.Find("SE Player").GetComponent<SEPlayer>();
        sePlayer.WaterSound();
    }
}
