using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    [SerializeField] GameObject gun;
    [SerializeField] ParticleSystem explosion;
    [SerializeField] Camera cam;

    [SerializeField] float maxDist;
    [SerializeField] float recoil;
    [SerializeField] int damage;
    [SerializeField] float firerate;
    float timer = 0;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && timer <= 0) {
            timer = firerate;
            Shoot();
        } else {
            timer -= Time.deltaTime;
        }
    }

    void Shoot()
    {
        ParticleSystem explosionClone = Instantiate(explosion, GameObject.Find("Gun").transform);
        explosionClone.Play();
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, maxDist)) {
            if (hit.collider.gameObject.CompareTag("Enemy")) {
                hit.collider.gameObject.GetComponent<Enemy>().TakDamage(damage);
            }
        }
        iTween.PunchRotation(cam.gameObject, Vector3.right * recoil, firerate);
    }
}
