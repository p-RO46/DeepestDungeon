using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class StartManager : MonoBehaviour
{
    [SerializeField] private float fadeTime = 1.5f;
    [SerializeField] private DialogueManager dlgManager;
    private Image panelImage;

    void Start()
    {
        panelImage = GetComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 1);
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        yield return new WaitForSeconds(1f);

        yield return dlgManager.StartRoutine();
        yield return new WaitForSeconds(1f);

        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("GameScene");
    }
}
