using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float speed;
    public int damage;
    float radius;
    LayerMask blockMask;
    GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        radius = GetComponent<SphereCollider>().radius * transform.localScale.x;
        blockMask = LayerMask.GetMask("Wall") | LayerMask.GetMask("Ground") | LayerMask.GetMask("Player");

        Destroy(gameObject, 5f);
    }

    void Update()
    {
        float moveDist = speed * Time.deltaTime;
        if (Physics.SphereCast(transform.position, radius, transform.forward, out RaycastHit hit, moveDist, blockMask)) {
            ObjectHit(hit.transform.gameObject);
        }
        transform.Translate(moveDist * transform.forward, Space.World);
    }

    void ObjectHit(GameObject hit)
    {
        Destroy(gameObject);
        if (hit.CompareTag("Player")) gameManager.TakeDamage(damage);
    }
}
