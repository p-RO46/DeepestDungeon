using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EndingPanel : MonoBehaviour
{
    [SerializeField] private float fadeTime = 1.5f;
    private Image panelImage;

    void Start()
    {
        panelImage = GetComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 1);
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
    }
}
