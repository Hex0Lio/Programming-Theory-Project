using System.Collections;
using UnityEngine;

public class RangedEnemy : Enemy
{
    [Header("Ranged Enemy Variables")]
    public GameObject bulletPrefab;
    public int shotAmount;
    public float timeBetweenShots;
    public float recoilKnockback;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        attackRadius = bulletPrefab.GetComponent<SphereCollider>().radius * bulletPrefab.transform.localScale.x + 0.05f;
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
    }

    protected override IEnumerator Attack()
    {
        Vector3 shotDir = (transform.forward + Vector3.up * enemyToPlayerDir.y).normalized;
        yield return new WaitForSeconds(attackDelay);

        do {
            for (int i = 0; i < shotAmount; i++) {
                // Shoot
                GameObject bullet = Instantiate(bulletPrefab, enemyHeadPos.position, Quaternion.LookRotation(shotDir));
                bullet.GetComponent<Bullet>().damage = damage;
                // Recoil
                rb.AddForce(-shotDir * recoilKnockback, ForceMode.Impulse);

                LookAtPlayer();

                yield return new WaitForSeconds(timeBetweenShots);
                shotDir = (transform.forward + Vector3.up * enemyToPlayerDir.y).normalized;
            }

            yield return new WaitForSeconds(attackCooldown);

            UpdateState();
        } while (state == State.Attacking);
    }
}
