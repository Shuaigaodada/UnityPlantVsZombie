using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeaShooter : Plant
{
    [Header("Ö²Îï×ÔÓÐÉèÖÃ")]
    [SerializeField, Label("µ¯Ò©ÀàÐÍ")] private GameObject Bullet;
    [SerializeField, Label("µ¯Ò©Éä»÷Î»ÖÃÆ«ÒÆ-X")] private float offsetX;
    [SerializeField, Label("µ¯Ò©Éä»÷Î»ÖÃÆ«ÒÆ-Y")] private float offsetY;
    [SerializeField, Label("µ¯Ò©Éä»÷Î»ÖÃÆ«ÒÆ-Z")] private float offsetZ;
    private float timer;

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer >= Interval && isPlanted)
        {
            timer = 0;
            Instantiate(Bullet, new Vector3(
                gameObject.transform.position.x + offsetX,
                gameObject.transform.position.y + offsetY,
                gameObject.transform.position.z + offsetZ), Quaternion.identity);
        }
    }


}
