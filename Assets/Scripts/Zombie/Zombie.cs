using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : Enemy
{

    [Header("其他属性")]

    [SerializeField, Label("头部")] private Head head;
    [SerializeField, Label("掉落头部所需血量")] private int lostHead = 100;
    [SerializeField, Label("尸体消失速度")] private float destroy_speed = 1.0f;

    bool losted = false;
    bool isMoving = true;
    bool start_destory = false;
    // Start is called before the first frame update
    void Start()
    {
        this.Initialize();
        // Set Zombie Move Animation
        int method = UnityEngine.Random.Range(1, 3);
        animator.SetInteger("MoveAni", method);

        head = transform.Find("ZombieHead").GetComponent<Head>();

    }

    // Update is called once per frame
    void Update()
    {
        if(isMoving)
        {
            _Move();
        }
        if (start_destory)
        {
            spriteRenderer.color = new Color(
                spriteRenderer.color.r,
                spriteRenderer.color.g,
                spriteRenderer.color.b,
                spriteRenderer.color.a - Time.deltaTime * destroy_speed);
            if (spriteRenderer.color.a <= 0)
            {
                GameObject.Destroy(gameObject);
            }
        }
    }

    public void StartDestory()
    {
        isMoving = false;
        this.start_destory = true;
    }


    public override void Hurt(int damage)
    {
        base.Hurt(damage);
        if (health <= lostHead && !losted)
        {
            head.Active();
            losted = true;
            animator.SetTrigger("LostHead");
        }
        if (health <= 0)
        {
            animator.SetTrigger("Dead");
            isMoving = false;
            GetComponent<Collider2D>().enabled = false;
            transform.Find("Attack").GetComponent<Collider2D>().enabled = false;
        }
    }

    private void _Move()
    {
        gameObject.transform.position = new Vector3(
            gameObject.transform.position.x - Time.deltaTime * speed,
            gameObject.transform.position.y,
            gameObject.transform.position.z);
    }

    public void Attack()
    {
        this.isMoving = false;
        animator.SetBool("Attack", true);
    }

    public void Move()
    {
        if(health > 0)
            this.isMoving = true;
        animator.SetBool("Attack", false);
    }

    
}
