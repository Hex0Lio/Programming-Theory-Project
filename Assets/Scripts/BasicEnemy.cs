using System.Collections;
using UnityEngine;

public class BasicEnemy : Enemy
{
    bool canDealDamage;
    GameManager gameManager;

    [Header("Basic Enemy Variables")]
    public float attackForce;
    public float attackKnockback;
    public float attackTime;

    // Start is called before the first frame update
    new void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        base.Start();
        attackRadius = radius;
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
    }

    protected override IEnumerator Attack()
    {
        yield return new WaitForSeconds(attackDelay);

        do {
            // Attack
            rb.AddForce(transform.forward * attackForce + Vector3.up * enemyToPlayerDir.y, ForceMode.Impulse);
            canDealDamage = true;

            yield return new WaitForSeconds(attackTime);
            canDealDamage = false;

            yield return new WaitForSeconds(attackCooldown);

            UpdateState();
        } while (state == State.Attacking);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && canDealDamage) {
            canDealDamage = false;

            gameManager.TakeDamage(damage);

            playerRb.AddForce(enemyToPlayerDirFlat * attackKnockback + Vector3.up * (attackKnockback / 2f), ForceMode.Impulse);

            rb.velocity = new Vector3(0, rb.velocity.y, 0);
            rb.AddForce(-enemyToPlayerDir * 2, ForceMode.Impulse);
        }
    }
}
