using System.Collections;
using UnityEngine;

public class Attack : MonoBehaviour
{
    private Zombie zombie;
    private Plant currentPlant;
    private Coroutine attackCoroutine;

    private void Start()
    {
        zombie = transform.parent.GetComponent<Zombie>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("plant"))
            return;

        Plant plant = collision.GetComponent<Plant>();

        if (plant == null)
            return;

        if (plant.Tigger)
            return;

        currentPlant = plant;

        zombie.Attack();

        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
        }

        attackCoroutine = StartCoroutine(AttackPlant());
    }

    private IEnumerator AttackPlant()
    {
        while (currentPlant != null && currentPlant.health > 0)
        {
            currentPlant.Hurt(zombie.attack);

            yield return new WaitForSeconds(
                zombie.attactInterval
            );
        }

        zombie.Move();

        currentPlant = null;
        attackCoroutine = null;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (currentPlant == null)
            return;

        Plant plant = collision.GetComponent<Plant>();

        if (plant != currentPlant)
            return;

        StopAttack();
        zombie.Move();
    }

    private void StopAttack()
    {
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }

        currentPlant = null;
    }

    private void OnDisable()
    {
        StopAttack();
    }
}