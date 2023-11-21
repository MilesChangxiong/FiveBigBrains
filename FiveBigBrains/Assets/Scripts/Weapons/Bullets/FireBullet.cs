using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class FireBullet : Bullet
{
    public float speed = 10f;
    public float explosionRadius = 2f;
    public LayerMask explosionLayers;
    private Rigidbody2D rb;

    private bool hasExploded = false;

    // fireball explosion animation
    public GameObject fireballExplosion;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        explosionLayers = LayerMask.GetMask("Default");
        SetInitialVelocity();
    }

    void SetInitialVelocity()
    {
        float initialSpeedY = Mathf.Tan(Mathf.Deg2Rad * 45) * speed;
        Vector2 initialVelocity = new Vector2(speed, initialSpeedY);

        if (transform.rotation.eulerAngles.z > 90)
        {
            initialVelocity.x *= -1;
        }

        rb.velocity = initialVelocity;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!hasExploded)
        {
            hasExploded = true;
            Explode();
        }
    }

    void Explode()
    {
        // explosion animation
        GameObject explosion = (GameObject)Instantiate (fireballExplosion);
        explosion.transform.position = transform.position;

        string explosionId = System.Guid.NewGuid().ToString();

        // use OverlapCircleAll to detect objects in explosion area
        Collider2D[] objectsInExplosionRadius = Physics2D.OverlapCircleAll(transform.position, explosionRadius, explosionLayers);

        foreach (Collider2D collider in objectsInExplosionRadius)
        {
            PlayerBodyParts playerBodyPart = collider.GetComponent<PlayerBodyParts>();
            Ice iceBlock = collider.GetComponent<Ice>();

            if (playerBodyPart)
            {
                playerBodyPart.transform.parent.GetComponent<Player>().TakeDamageWithEventID(1, explosionId);
                ReportWeaponAction("HitPlayer");
            }

            if (iceBlock)
            {
                Destroy(iceBlock.gameObject);
                ReportWeaponAction("HitIce");
            }
        }

        Destroy(gameObject);
    }
}
