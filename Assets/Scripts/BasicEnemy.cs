using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BasicEnemy : Enemy
{
    Rigidbody rb;

    [Header("Basic Enemy Variables")]
    public float attackForce;
    public float attackKnockback;

    //public float attackOffset;
    //public float attackRadius;
    //public LayerMask playerMask;

    //int hitNum = 1;
    //Vector3 center;
    //Vector3 halfExtends;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        radius = GetComponent<CapsuleCollider>().radius;

        agent.speed = speed;
    }

    // Update is called once per frame
    void Update()
    {
        CalculateDirectionAndDistance();

        if (!attacking) Move();
        else {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
        }
    }

    protected override IEnumerator Attack()
    {
        attacking = true;
        yield return new WaitForSeconds(attackDelay);

        //center = transform.position + transform.forward * attackOffset;
        //halfExtends = new Vector3(attackRadius, 0.5f, attackRadius);

        //if (Physics.CheckBox(center, halfExtends, transform.rotation, playerMask)) {
        //    Debug.Log("HIT" + hitNum);
        //    hitNum++;
        //}

        while (CanAttack()) {
            // Attack
            rb.AddForce(transform.forward * attackForce, ForceMode.Impulse);

            yield return new WaitForSeconds(attackCooldown);
        }

        attacking = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player")) {
            
        }
    }
}
