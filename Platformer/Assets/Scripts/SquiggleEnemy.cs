using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SquiggleEnemy : MonoBehaviour
{
    Transform target;
    [SerializeField] float lookRadius = 5f;
    [SerializeField] float attackRate = 1.5f;
    [SerializeField] float attackDistance = 1f;
    [SerializeField] LayerMask obstacleLayers;
    NavMeshAgent agent;
    bool isChasing = false;
    float lastAttackTime = 0f;

    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        target = PlayerManager.instance.transform;
        animator = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }




    // Update is called once per frame
    void Update()
    {
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
                        agent.SetDestination(target.position);
                    }
                }
            }
        }
        else
        {
            agent.SetDestination(target.position);
            if (distance <= attackDistance)
            {
                //attack the target
                if (Time.time - lastAttackTime >= attackRate)
                {
                    animator.SetTrigger("Attack");
                    PlayerManager.instance.TakeDamage();
                    lastAttackTime = Time.time;
                }
            }
        }
        
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
        Gizmos.DrawWireSphere(transform.position, attackDistance);
    }
}
