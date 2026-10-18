using System.Collections;
using UnityEngine;

public class Sunlight : MonoBehaviour
{
    [Label("阳光数值")]
    public int value = 25;

    [Label("存在时间")]
    public float destroyTime = 10f;

    [Label("消失时间")]
    public float fadeDuration = 3f;

    [Header("收集动画")]

    [SerializeField, Label("目标位置")]
    private Transform collectTarget;

    [SerializeField, Label("移动时间")]
    private float collectDuration = 0.8f;

    [SerializeField, Label("开始消失的距离")]
    private float fadeDistance = 1.5f;

    private float timer;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    private bool allowDestroy;
    private bool isCollecting;
    private float collectStartAlpha;
    private Collider2D sunlightCollider;
    private Coroutine collectCoroutine;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        sunlightCollider = GetComponent<Collider2D>();

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }

        if (collectTarget == null)
        {
            GameObject target =
                GameObject.Find("ChooserBackground/sunlight");

            if (target != null)
            {
                collectTarget = target.transform;
            }
        }

    }

    public void AllowDestroy(bool status)
    {
        allowDestroy = status;

        if (!status)
        {
            timer = 0f;
        }
    }

    private void Update()
    {
        if (!allowDestroy || isCollecting)
            return;

        timer += Time.deltaTime;

        float fadeStartTime = destroyTime - fadeDuration;

        if (timer >= fadeStartTime && spriteRenderer != null)
        {
            float fadeProgress = Mathf.Clamp01(
                (timer - fadeStartTime) / fadeDuration
            );

            Color color = originalColor;
            color.a = Mathf.Lerp(
                originalColor.a,
                0f,
                fadeProgress
            );

            spriteRenderer.color = color;
        }

        if (timer >= destroyTime)
        {
            Destroy(gameObject);
        }
    }

    public void Collect()
    {
        if (isCollecting)
            return;

        if (collectTarget == null)
        {
            Debug.LogError("没有设置阳光收集目标。", gameObject);
            return;
        }

        isCollecting = true;
        allowDestroy = false;

        // 记录点击时当前透明度，不要恢复到1
        if (spriteRenderer != null)
        {
            collectStartAlpha = spriteRenderer.color.a;
        }

        // 防止连续点击同一个阳光
        if (sunlightCollider != null)
        {
            sunlightCollider.enabled = false;
        }

        collectCoroutine = StartCoroutine(CollectSunlight());
        GameManager.Instance.AddSunlight(value);
    }

    private IEnumerator CollectSunlight()
    {
        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;

        while (
            elapsedTime < collectDuration &&
            collectTarget != null
        )
        {
            elapsedTime += Time.deltaTime;

            float progress = Mathf.Clamp01(
                elapsedTime / collectDuration
            );

            /*
             * Ease Out：
             * 开始移动快，接近目标时逐渐变慢。
             */
            float easedProgress =
                1f - Mathf.Pow(1f - progress, 3f);

            Vector3 targetPosition =
                collectTarget.position;

            transform.position = Vector3.Lerp(
                startPosition,
                targetPosition,
                easedProgress
            );

            FadeWhenClose(targetPosition);

            yield return null;
        }

        if (collectTarget != null)
        {
            transform.position = collectTarget.position;
        }

        // 确保最后完全消失
        yield return StartCoroutine(FadeOutCompletely());

        Destroy(gameObject);
    }

    private void FadeWhenClose(Vector3 targetPosition)
    {
        if (spriteRenderer == null)
            return;

        float safeFadeDistance = Mathf.Max(fadeDistance, 0.01f);

        float distance = Vector3.Distance(
            transform.position,
            targetPosition
        );

        if (distance > safeFadeDistance)
            return;

        float alphaProgress =
            1f - Mathf.Clamp01(distance / safeFadeDistance);

        float targetAlpha = Mathf.Lerp(
            collectStartAlpha,
            0f,
            alphaProgress
        );

        Color color = spriteRenderer.color;

        // 透明度只能降低，不能突然恢复
        color.a = Mathf.Min(color.a, targetAlpha);

        spriteRenderer.color = color;
    }

    private IEnumerator FadeOutCompletely()
    {
        if (spriteRenderer == null)
            yield break;

        Color startColor = spriteRenderer.color;
        float duration = 0.15f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            float progress = Mathf.Clamp01(
                elapsedTime / duration
            );

            Color color = startColor;
            color.a = Mathf.Lerp(
                startColor.a,
                0f,
                progress
            );

            spriteRenderer.color = color;

            yield return null;
        }
    }
}