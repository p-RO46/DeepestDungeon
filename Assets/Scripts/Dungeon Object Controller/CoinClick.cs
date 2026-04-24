using System.Collections;
using UnityEngine;

public class CoinClick : ItemClick
{
    [SerializeField] private int minScore = 3;
    [SerializeField] private int maxScore = 11;
    public override void GetItem()
    {
        // Score 올려주기
        // 씬의 유일 컴포넌트라서 이렇게 찾아줌
        ScoreManager scoreManager = FindAnyObjectByType<ScoreManager>();
        scoreManager.AddScore(Random.Range(minScore, maxScore));
    }
}
