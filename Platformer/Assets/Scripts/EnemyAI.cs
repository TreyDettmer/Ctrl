using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    Transform target;
    [SerializeField] float lookRadius = 5f;
    [SerializeField] LayerMask obstacleLayers;
    NavMeshAgent agent;
    // Start is called before the first frame update
    void Start()
    {
        target = PlayerManager.instance.transform;
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector2.Distance(target.position, transform.position);
        if (distance <= lookRadius)
        {

            agent.SetDestination(target.position);

            if (distance <= agent.stoppingDistance)
            {
                //attack the target
            }
            //RaycastHit2D hit = Physics2D.Raycast(transform.position, (target.position - transform.position).normalized, lookRadius, obstacleLayers);
            //if (hit.collider != null)
            //{
            //    if (hit.collider.gameObject.GetComponent<PlayerManager>())
            //    {
            //        agent.SetDestination(target.position);

            //        if (distance <= agent.stoppingDistance)
            //        {
            //            //attack the target
            //        }
            //    }
            //}
            


        }
        
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }
}
