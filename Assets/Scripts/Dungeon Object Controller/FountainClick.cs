using UnityEngine;

public class FountainClick : MonoBehaviour
{
    [SerializeField] private int score = 200;
    public bool getted = false;
    private void Start()
    {
        getted = false;
    }

    public void GetItem()
    {
        ScoreManager scoreManager = FindAnyObjectByType<ScoreManager>();
        scoreManager.AddScore(score);

        getted = true;
    }
}
