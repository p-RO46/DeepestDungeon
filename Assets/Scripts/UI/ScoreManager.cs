using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private GameData gameData;
    public int currentScore = 0;

    private void Start()
    {
        // DB에서 score 읽어오기!
        currentScore = gameData.totalScore;
        scoreText.text = $"Score: {currentScore}";
    }

    public void AddScore(int score)
    {
        currentScore += score;
        scoreText.text = $"Score: {currentScore}";
    }

    public void SaveScore()
    {
        gameData.totalScore = currentScore;
    }
}
