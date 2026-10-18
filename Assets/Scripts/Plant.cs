using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour
{
    [Header("属性")]
    [Label("血量")] public int health;
    [Label("间隔")] public float Interval;
    [Label("是否可被攻击")] public bool Tigger = false;
    [SerializeField, Label("费用")] protected int Cost;
    [Header("其他属性")]
    [SerializeField, Label("植物卡面")] public Sprite Card;
    [SerializeField, Label("植物卡面-禁用")] public Sprite CardDisable;
    [SerializeField, Label("植物是否种下")] protected bool isPlanted;

    [SerializeField, Label("所在列")] protected int column;
    [SerializeField, Label("所在行")] protected int row;

    public virtual void Hurt(int damage)
    {
        health -= damage;
        if(health <= 0)
        {
            Die();
        }
    }

    public void SetTrigger(bool status)
    {
        this.Tigger = status;
    }

    public virtual void Die()
    {
        GameObject.Destroy(gameObject);
        GameManager.Instance.PlantDeath(row, column);
    }

    public int GetCost()
    {
        return Cost;
    }

    public void Planted()
    {
        isPlanted = true;
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.speed = 1;
        }
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingLayerName = "GameObject";
        }
        SetTrigger(false);
    }

    public void SetGridPosition(int row, int column)
    {
        this.row = row;
        this.column = column;
    }
}
