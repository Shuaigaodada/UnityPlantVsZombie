using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Head : MonoBehaviour
{

    [SerializeField, Label("ÏûÊ§ËÙ¶È")]
    private float DestorySpeed;

    private bool AllowDestory = false;
    private SpriteRenderer spriteRenderer;

    public void Active()
    {
        gameObject.SetActive(true);
        transform.SetParent(null);
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    

    private void Update()
    {
        if(AllowDestory)
        {
            spriteRenderer.color = new Color(
            spriteRenderer.color.r,
            spriteRenderer.color.g,
            spriteRenderer.color.b,
            spriteRenderer.color.a - Time.deltaTime * DestorySpeed);
            if (spriteRenderer.color.a <= 0)
            {
                GameObject.Destroy(gameObject);
            }
        }
        
        
    }

    public void StartDestroy()
    {
        this.AllowDestory = true;
    }
}
