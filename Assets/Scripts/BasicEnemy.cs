using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BasicEnemy : Enemy
{
    Rigidbody rb;
    bool charging;

    [Header("Basic Enemy Variables")]
    public float attackForce;
    public float attackKnockback;
    public float attackTime;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
    }

    protected override IEnumerator Attack()
    {
        attacking = true;
        yield return new WaitForSeconds(attackDelay);

        while (CanAttack()) {

            // Attack
            rb.AddForce(transform.forward * attackForce, ForceMode.Impulse);
            charging = true;

            yield return new WaitForSeconds(attackTime);
            charging = false;

            yield return new WaitForSeconds(attackCooldown);
        }

        attacking = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && charging) {
            Debug.Log("HIT");
            playerRb.AddForce(enemyToPlayerDirFlat * attackKnockback + Vector3.up * (attackKnockback / 2f), ForceMode.Impulse);
        }
    }
}
