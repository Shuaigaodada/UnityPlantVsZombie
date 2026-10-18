using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using Unity.VisualScripting;
using UnityEditor.U2D.Sprites;
using UnityEngine;

public class Card : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    [SerializeField, Label("卡面")] public Sprite card;
    [SerializeField, Label("卡面-禁用")] public Sprite cardDisable;
    [SerializeField, Label("碰撞箱")] private BoxCollider2D boxCollider;
    [SerializeField, Label("卡面索引")] private int cardIndex;

    private bool AllowBuy;


    public void FitSpriteToCollider()
    {
        if (boxCollider == null || spriteRenderer == null)
        {
            boxCollider = GetComponent<BoxCollider2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        spriteRenderer.drawMode = SpriteDrawMode.Sliced;
        spriteRenderer.sprite = cardDisable;
        spriteRenderer.size = boxCollider.size;
    }

    public void SetActive(bool status)
    {
        AllowBuy = status;
        spriteRenderer.sprite = AllowBuy ? card : cardDisable; 
    }

    public bool IsActive()
    {
        return AllowBuy;
    }

    public void SetCardIndex(int index)
    {
        cardIndex = index;
    }

    private void OnMouseDown()
    {
        if(!IsActive())
        {
            return;
        }

        GameManager.Instance.BuyPlant(cardIndex - 1);
    }

}
