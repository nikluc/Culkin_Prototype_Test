using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAnimationController : MonoBehaviour
{
    [Header("References")]
    public GameObject icon;
    public GameObject cardFace;

    [Header("Animation Settings")]
    public float flipDuration = 0.2f;
    public float vanishDuration = 0.3f;

    // Use Unity AnimationCurve for easing vanish animation
    public AnimationCurve vanishEaseCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private Coroutine currentAnimation;

    public void FlipFaceUp(System.Action onComplete = null)
    {
        StartAnimation(FlipAnimation(true, onComplete));
    }

    public void FlipFaceDown(System.Action onComplete = null)
    {
        StartAnimation(FlipAnimation(false, onComplete));
    }

    public void Vanish(System.Action onComplete = null)
    {
        StartAnimation(VanishAnimation(onComplete));
    }

    public void ResetImmediate(bool showIcon)
    {
        if (currentAnimation != null)
            StopCoroutine(currentAnimation);
        cardFace.transform.localScale = Vector3.one;
        icon.SetActive(showIcon);
    }

    private void StartAnimation(IEnumerator animation)
    {
        if (currentAnimation != null)
            StopCoroutine(currentAnimation);
        currentAnimation = StartCoroutine(animation);
    }

    private IEnumerator FlipAnimation(bool showFace, System.Action onComplete)
    {
        yield return AnimateScaleX(1f, 0f, flipDuration);
        icon.SetActive(showFace);
        yield return AnimateScaleX(0f, 1f, flipDuration);
        currentAnimation = null;
        onComplete?.Invoke();
    }

    private IEnumerator VanishAnimation(System.Action onComplete)
    {
        float elapsed = 0f;
        Vector3 startScale = transform.localScale;
        Vector3 targetScale = Vector3.zero;

        while (elapsed < vanishDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / vanishDuration);

            // Evaluate scale factor from AnimationCurve
            float easedT = vanishEaseCurve.Evaluate(t);
            transform.localScale = Vector3.Lerp(startScale, targetScale, easedT);

            yield return null;
        }

        transform.localScale = targetScale;
        currentAnimation = null;
        onComplete?.Invoke();
    }

    private IEnumerator AnimateScaleX(float from, float to, float duration)
    {
        float elapsed = 0f;
        Vector3 scale = cardFace.transform.localScale;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float scaleX = Mathf.Lerp(from, to, t);
            cardFace.transform.localScale = new Vector3(scaleX, scale.y, scale.z);
            yield return null;
        }
        cardFace.transform.localScale = new Vector3(to, scale.y, scale.z);
    }
}
