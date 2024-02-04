using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIElementAnimations : MonoBehaviour
{
    //public RectTransform uiElemen


    #region scalingAnimation
    Coroutine scaleCoroutine;
    public void SetScaleAnimation(float scaleAmount, float scaleDuration, GameObject GO)
    {
        if (scaleCoroutine != null)
        {
            StopCoroutine(scaleCoroutine);
        }

        // Start the scale up and down animation
        scaleCoroutine = StartCoroutine(ScaleUpAndDown(scaleAmount, scaleDuration, GO));
    }

    public void StopScaleAnimation(GameObject GO)
    {
        if (scaleCoroutine != null)
        {
            StopCoroutine(scaleCoroutine);
            GO.transform.localScale = Vector3.one; // Set the scale to the original scale
        }
    }

    IEnumerator ScaleUpAndDown(float scaleAmount, float scaleDuration, GameObject GO)
    {
        while (true)
        {
            yield return ScaleOverTime(Vector3.one * scaleAmount, scaleDuration, GO);

            yield return ScaleOverTime(Vector3.one, scaleDuration, GO);
        }
    }

    private IEnumerator ScaleOverTime(Vector3 targetScale, float duration, GameObject GO)
    {
        float elapsedTime = 0f;
        Vector3 originalScale = GO.transform.localScale;

        while (elapsedTime < duration)
        {
            GO.transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        GO.transform.localScale = targetScale;
    }
    #endregion


    #region VibrateAnimation
    public void VibrateUI(GameObject GO,
        float vibrationDuration, float vibrationIntensity
        , Vector3 originalPosition)
    {
        

        //Debug.Log("VIBRATION");
        StartCoroutine(Vibrate(
        GO,
        vibrationDuration, vibrationIntensity
        , originalPosition));
    }


    IEnumerator Vibrate(
        GameObject GO,
        float vibrationDuration, float vibrationIntensity
        , Vector3 originalPosition)
    {
        float elapsedTime = 0f;

        RectTransform rectTransform = GO.GetComponent<RectTransform>();
        Vector2 anchorMin = rectTransform.anchorMin;
        Vector2 anchorMax = rectTransform.anchorMax;

        //Debug.Log($"ORIGINAL POS {originalPosition}");
        while (elapsedTime < vibrationDuration)
        {
            // Calculate a random offset based on the vibration intensity
            Vector3 offset = new Vector3(
                Random.Range(-vibrationIntensity, vibrationIntensity),
                Random.Range(-vibrationIntensity, vibrationIntensity),
                0f);

            // Apply the offset to the UI element's position
            GO.transform.position = originalPosition + offset;

            // Wait for the next frame
            yield return null;

            // Update the elapsed time
            elapsedTime += Time.deltaTime;
        }

        // Reset local position considering anchors
        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        rectTransform.anchoredPosition = Vector2.zero;

    }
    #endregion


    #region Fade Animation
    [HideInInspector]
    public Coroutine fadeOutCoroutine;
    public void FadeOutAnimation(CanvasGroup canvasGroup, float fadeDuration)
    {
        fadeOutCoroutine = StartCoroutine(FadeOut(canvasGroup, fadeDuration));
    }
    IEnumerator FadeOut(CanvasGroup canvasGroup, float fadeDuration)
    {
        float elapsedTime = 0f;
        float startAlpha = canvasGroup.alpha;

        while (elapsedTime < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = 0f; // Ensure the alpha is set to the target value
    }

    public void FadeInAnimation(CanvasGroup canvasGroup, float fadeDuration)
    {
        StartCoroutine( FadeIn(canvasGroup, fadeDuration));    
    }
    IEnumerator FadeIn(CanvasGroup canvasGroup, float fadeDuration)
    {
        float elapsedTime = 0f;
        float startAlpha = canvasGroup.alpha;

        while (elapsedTime < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 1f, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = 1f; // Ensure the alpha is set to the target value
    }
    #endregion

}
