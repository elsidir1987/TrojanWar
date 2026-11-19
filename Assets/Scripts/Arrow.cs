using UnityEngine;

public class Arrow : MonoBehaviour
{
    [Header("Flight Settings")]
    public float speed = 12f;
    public float damage = 1f;

    [Header("Tags to hit")]
    public string playerTag = "Player"; 
    public string groundTag = "Ground"; 

    private Rigidbody2D rb;
    private Collider2D coll;
    private bool hasHit = false; 

    // Awake is called when the script instance is being loaded.
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Shoot(Transform target)
    {
        if (target == null) return;

        // calculate direction from arrow to target
        Vector2 direction = (Vector2)target.position - (Vector2)transform.position;
        direction.Normalize();

        // 2. Set velocity in the direction of the target
        rb.linearVelocity = direction * speed;

        // 3. Optional: Set gravity scale to 0 for straight flight, or a small value for a parabolic arc
        rb.gravityScale = 0.5f; 

        Destroy(gameObject, 5f);
    }
    void Update()
    {
        if (hasHit) return;
        // Rotate the arrow to point in the direction of travel
        float angle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerMovement player = collision.GetComponent<PlayerMovement>();
            
            // Add null check (if != null) to prevent crashes!
            if (player != null)
            {
                if (player.isBlocking)
                {
                    Debug.Log("Blocked!");
                    Destroy(gameObject);
                    return;
                }
                player.TakeDamage(1f);
            }
            Destroy(gameObject);
        }
    }

    void StickAndFade(Transform targetHit)
    {
        hasHit = true;
        rb.linearVelocity = Vector2.zero; // Stop moving
        rb.bodyType = RigidbodyType2D.Kinematic;// Lock in place
        
        if (coll != null) coll.enabled = false; // Disable collider
        
        transform.SetParent(targetHit); // Attach to target
        Destroy(gameObject, 3f); // Destroy after 3 seconds
    }
}