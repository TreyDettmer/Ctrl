using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncyEnemy : MonoBehaviour
{
    Transform target;
    Rigidbody2D rb;
    [SerializeField] float lookRadius = 5f;
    [SerializeField] float speed = 3f;
    [SerializeField] float attackDistance = 3f;
    [SerializeField] float attackSpeedMultiplier = 2f;
    [SerializeField] LayerMask obstacleLayers;
    bool isChasing = false;

    bool damagedPlayer = false;
    // Start is called before the first frame update
    void Start()
    {
        target = PlayerManager.instance.transform;
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (transform.position.y < -100)
        {
            Destroy(gameObject);
        }
        float distance = Vector2.Distance(target.position, transform.position);
        if (!isChasing)
        {
            if (distance <= lookRadius)
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, (target.position - transform.position).normalized, lookRadius, obstacleLayers);
                if (hit.collider != null)
                {
                    if (hit.collider.gameObject.GetComponent<PlayerManager>())
                    {
                        isChasing = true;
                    }
                }
            }
        }
        else
        {
            if (transform.position.x - target.position.x > 0)
            {
                // target is to the left
                if (rb.velocity.x >= -1)
                {
                    rb.AddForce(new Vector2(-speed, 0) * Time.fixedDeltaTime);
                }
            }
            else
            {
                // target is to the right
                if (rb.velocity.x <= 1)
                {
                    rb.AddForce(new Vector2(speed, 0) * Time.fixedDeltaTime);
                }
            }

            // move directly towards target if close enough
            if (Vector2.Distance(transform.position, target.position) < attackDistance)
            {
                rb.AddForce((target.position - transform.position).normalized * speed * attackSpeedMultiplier * Time.fixedDeltaTime);
            }
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.GetComponent<PlayerManager>())
        {
            if (!damagedPlayer)
            {
                // since the player has multiple colliders, we only want to account for one of those colliders
                collision.collider.GetComponent<PlayerManager>().TakeDamage();
                damagedPlayer = true;
            }
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }
}
