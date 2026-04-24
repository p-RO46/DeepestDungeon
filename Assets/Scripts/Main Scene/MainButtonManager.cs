using UnityEngine;
using UnityEngine.SceneManagement;

public class MainButtonManager : MonoBehaviour
{
    public void OnStartClick()
    {
        SceneManager.LoadScene("GameScene");
    }
}
