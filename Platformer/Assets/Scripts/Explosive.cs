using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosive : MonoBehaviour
{
    bool hasExploded = false;
    [SerializeField] float explosionRadius = 2f;
    [SerializeField] GameObject explosionEffect;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Explode()
    {
        if (!hasExploded)
        {
            hasExploded = true;
            Instantiate(explosionEffect, transform.position, transform.rotation);

            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);        
            bool damagedPlayer = false;
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].GetComponent<PlayerManager>())
                {
                    // since the player has multiple colliders, we only want to account for one of those colliders
                    if (!damagedPlayer)
                    {
                        colliders[i].GetComponent<PlayerManager>().TakeDamage(3);
                        damagedPlayer = true;
                    }
                }
                else if (colliders[i].GetComponent<Explosive>())
                {
                    colliders[i].GetComponent<Explosive>().Explode();
                }
                else if (colliders[i].GetComponent<BouncyEnemy>())
                {
                    Destroy(colliders[i].gameObject);
                }
                else if (colliders[i].GetComponent<SquiggleEnemy>())
                {
                    Destroy(colliders[i].gameObject);
                }
            }
            Destroy(gameObject);

        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        Explode();
        
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
