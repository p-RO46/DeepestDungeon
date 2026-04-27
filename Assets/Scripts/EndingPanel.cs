using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class EndingPanel : MonoBehaviour
{
    [SerializeField] private float fadeTime = 1.5f;
    [SerializeField] private DialogueManager dlgManager;
    private GameObject mapManager;
    private Image panelImage;

    void Start()
    {
        panelImage = GetComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 1);

        mapManager = GameObject.Find("MapManager");

        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        yield return new WaitForSeconds(2f);

        float elapsedTime = 0;
        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0.82f, elapsedTime / fadeTime);
            panelImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        yield return dlgManager.EndingRoutine();
        yield return new WaitForSeconds(1f);

        elapsedTime = 0;
        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0.82f, 1f, elapsedTime / fadeTime);
            panelImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        Destroy(mapManager);
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("MainScene");
    }
}
