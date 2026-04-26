using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private GameData gameData;
    public int currentScore = 0;

    private void Start()
    {
        // DB에서 score 읽어오기!
        currentScore = gameData.totalScore;
        scoreText.text = $"{currentScore}";

        SceneManager.sceneUnloaded += SaveScore;
    }

    public void AddScore(int score)
    {
        currentScore += score;
        scoreText.text = $"{currentScore}";
    }

    public void SaveScore(Scene scene)
    {
        gameData.totalScore = currentScore;
    }
}
