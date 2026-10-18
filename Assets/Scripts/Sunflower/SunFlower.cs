using System.Collections;
using UnityEngine;

public class SunFlower : Plant
{
    [Header("生产设置")]

    [SerializeField, Label("动画持续时间")] 
    private float creatingSpeed = 5f;
    [SerializeField, Label("阳光预制体")] 
    private GameObject sunlightPrefab;
    [SerializeField, Label("阳光放大速度")] 
    private float enlargeSpeed = 1f;

    private Animator animator;
    private float timer;
    private bool creating;
    private Coroutine createSunlightCoroutine;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if(!isPlanted)
        {
            return; 
        }
        timer += Time.deltaTime;

        // 等待生产间隔
        if (!creating && timer >= Interval)
        {
            animator.SetBool("creating", true);

            creating = true;
            timer = 0f;
        }

        // 等待生产动画结束
        if (creating && timer >= creatingSpeed)
        {
            animator.SetBool("creating", false);

            if (createSunlightCoroutine != null)
            {
                StopCoroutine(createSunlightCoroutine);
            }

            createSunlightCoroutine = StartCoroutine(CreateSunlight());

            creating = false;
            timer = 0f;
        }
    }

    private IEnumerator CreateSunlight()
    {
        if (sunlightPrefab == null)
        {
            Debug.LogError("没有设置阳光预制体", gameObject);
            yield break;
        }

        GameObject sunlight = Instantiate(
            sunlightPrefab,
            transform.position,
            Quaternion.identity
        );

        Vector3 targetScale = sunlight.transform.localScale;

        sunlight.transform.localScale = Vector3.zero;

        while (sunlight != null &&
               sunlight.transform.localScale != targetScale)
        {
            sunlight.transform.localScale = Vector3.MoveTowards(
                sunlight.transform.localScale,
                targetScale,
                enlargeSpeed * Time.deltaTime
            );

            // 等待下一帧，避免卡死
            yield return null;
        }

        if (sunlight != null)
        {
            sunlight.transform.localScale = targetScale;
        }

        createSunlightCoroutine = null;
    }

    private void OnDisable()
    {
        if (createSunlightCoroutine != null)
        {
            StopCoroutine(createSunlightCoroutine);
            createSunlightCoroutine = null;
        }
    }
}