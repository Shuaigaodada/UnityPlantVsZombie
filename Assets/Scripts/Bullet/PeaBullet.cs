using UnityEngine;

public class PeaBullet : MonoBehaviour
{
    [Header(" Ù–‘")]
    [SerializeField, Label("√¸÷–∫ÛÃÊªªæ´¡È")] private Sprite PeaBulletHit;
    [SerializeField, Label("µØ“©≥ıÀŸ")] private float bulletspeed;
    [SerializeField, Label("µØ“©…À∫¶")] private int damage;
    [SerializeField, Label("¥›ªŸÀŸ∂»")] private float destorySpeed;


    private SpriteRenderer spriteRenderer;
    private bool destorying;
    

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void OnHitObject(Collider2D other)
    {
        spriteRenderer.sprite = PeaBulletHit;
        destorying = true;
        Enemy enemy = other.gameObject.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.Hurt(damage);
        }
    }

    private void Update()
    {
        if(destorying)
        {
            spriteRenderer.color = new Color(
                spriteRenderer.color.r,
                spriteRenderer.color.g,
                spriteRenderer.color.b,
                spriteRenderer.color.a - Time.deltaTime * destorySpeed);
            if(spriteRenderer.color.a <= 0)
            {
                GameObject.Destroy(gameObject);
            }
        } else
        {
            gameObject.transform.position = new Vector3(
            gameObject.transform.position.x + Time.deltaTime * bulletspeed,
            gameObject.transform.position.y,
            gameObject.transform.position.z);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        
        if (other.gameObject.tag == "Enemy")
        {
            this.OnHitObject(other);
            
        }
    }
}
