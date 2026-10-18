using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickManager : MonoBehaviour
{
    [SerializeField, Label("—Ùπ‚Õº≤„")]
    private LayerMask sunlightLayer;

    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (!Input.GetMouseButtonDown(0))
            return;

        Vector3 mouseWorldPosition =
            mainCamera.ScreenToWorldPoint(Input.mousePosition);

        Vector2 point = new Vector2(
            mouseWorldPosition.x,
            mouseWorldPosition.y
        );

        Collider2D hit = Physics2D.OverlapPoint(
            point,
            sunlightLayer
        );

        if (hit == null)
            return;

        Sunlight sunlight = hit.GetComponent<Sunlight>();

        if (sunlight != null)
        {
            sunlight.Collect();
        }
    }
}
