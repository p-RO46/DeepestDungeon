using System.Collections;
using UnityEngine;

public abstract class ItemClick : MonoBehaviour, MapObjClick
{
    [SerializeField] private float fadeTime = 0.5f;
    [SerializeField] private SpriteRenderer textSprite;

    private bool isClicked = false; // 중복 클릭 방지 플래그
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void ObjClicked()
    {
        if (isClicked) return;

        isClicked = true;
        // Item 고유 효과
        GetItem();
        StartCoroutine(FadeOutAndDestroy());
    }

    public abstract void GetItem();

    private IEnumerator FadeOutAndDestroy()
    {
        if(textSprite != null)
        {
            textSprite.enabled = true;
        }

        yield return FadeRoutine(false);

        if (textSprite != null)
        {
            textSprite.enabled = false;
        }

        spriteRenderer.enabled = false;

        // 부모 오브젝트가 있는지 확인 후 파괴
        if (transform.parent != null)
        {
            Destroy(transform.parent.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public IEnumerator FadeRoutine(bool fadeIn)
    {
        float elapsedTime = 0;
        float startAlpha = fadeIn ? 0f : 1f;
        float endAlpha = fadeIn ? 1f : 0f;

        Color newColor = spriteRenderer.color;

        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeTime);

            newColor.a = newAlpha;
            spriteRenderer.color = newColor;

            yield return null;
        }

        // 최종 알파값 고정
        newColor.a = endAlpha;
        spriteRenderer.color = newColor;
    }
}
