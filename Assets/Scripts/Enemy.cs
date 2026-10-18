using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("属性调整")]

    [Label("生命值")] public int health = 200;
    [Label("攻击力")] public int attack = 4;
    [Label("攻击间隔")] public float attactInterval = 0.04f;
    [Label("移速")] public float speed;

    [Header("受击闪白")]
    [Tooltip("闪白变化速度，数值越大越快")]
    [SerializeField] protected float flush_speed = 1f;

    [Tooltip("闪白最高强度")]
    [SerializeField, Range(0f, 1f)]
    protected float flush_peak = 0.1f;

    protected Animator animator;
    protected SpriteRenderer spriteRenderer;

    private MaterialPropertyBlock propertyBlock;
    private Coroutine flushCoroutine;

    private static readonly int FlashAmountID =
        Shader.PropertyToID("_FlashAmount");

    protected virtual void Awake()
    {
        Initialize();
    }

    protected void Initialize()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            Debug.LogError(
                $"{gameObject.name} 没有找到 SpriteRenderer 组件。",
                gameObject
            );

            return;
        }

        propertyBlock = new MaterialPropertyBlock();
        SetFlashAmount(0f);
    }

    public virtual void Hurt(int damage)
    {
        health -= damage;
        StartFlush();
    }

    public virtual void Die() { GameObject.Destroy(gameObject); }

    private void StartFlush()
    {
        if (spriteRenderer == null)
            return;

        // 连续受击时，停止上一次闪白并重新开始
        if (flushCoroutine != null)
        {
            StopCoroutine(flushCoroutine);
        }

        flushCoroutine = StartCoroutine(FlushCoroutine());
    }

    private IEnumerator FlushCoroutine()
    {
        float amount = GetFlashAmount();

        // 逐渐变白：当前值 → 最高值
        while (amount < flush_peak)
        {
            amount = Mathf.MoveTowards(
                amount,
                flush_peak,
                flush_speed * Time.deltaTime
            );

            SetFlashAmount(amount);

            yield return null;
        }

        // 逐渐恢复：最高值 → 0
        while (amount > 0f)
        {
            amount = Mathf.MoveTowards(
                amount,
                0f,
                flush_speed * Time.deltaTime
            );

            SetFlashAmount(amount);

            yield return null;
        }

        SetFlashAmount(0f);
        flushCoroutine = null;
    }

    private float GetFlashAmount()
    {
        if (spriteRenderer == null || propertyBlock == null)
            return 0f;

        spriteRenderer.GetPropertyBlock(propertyBlock);

        return propertyBlock.GetFloat(FlashAmountID);
    }

    private void SetFlashAmount(float amount)
    {
        if (spriteRenderer == null || propertyBlock == null)
            return;

        spriteRenderer.GetPropertyBlock(propertyBlock);

        propertyBlock.SetFloat(
            FlashAmountID,
            Mathf.Clamp01(amount)
        );

        spriteRenderer.SetPropertyBlock(propertyBlock);
    }

    protected virtual void OnDisable()
    {
        if (flushCoroutine != null)
        {
            StopCoroutine(flushCoroutine);
            flushCoroutine = null;
        }

        SetFlashAmount(0f);
    }
}