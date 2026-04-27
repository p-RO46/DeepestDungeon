using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainButtonManager : MonoBehaviour
{
    public Image panelImage;
    public AudioSource audioSource;
    public AudioClip button;

    private void Start()
    {
        panelImage.color = new Color(0, 0, 0, 0);
    }

    public void OnStartClick()
    {
        panelImage.gameObject.SetActive(true);
        StartCoroutine(Sound());
    }

    private IEnumerator Sound()
    {
        audioSource.PlayOneShot(button);
        float elapsedTime = 0;
        float fadeTime = 2f;
        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0, 1, elapsedTime / fadeTime);
            panelImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        SceneManager.LoadScene("StartScene");
    }
    public void Quit()
    {
#if UNITY_EDITOR
        audioSource.PlayOneShot(button);
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
