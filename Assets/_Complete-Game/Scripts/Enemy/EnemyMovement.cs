using UnityEngine;
using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.AI;

namespace CompleteProject
{
    public class EnemyMovement : MonoBehaviourPun
    {
        GameObject player;               // Reference to the player's position.
        PlayerHealth playerHealth;      // Reference to the player's health.
        EnemyHealth enemyHealth;        // Reference to this enemy's health.
        UnityEngine.AI.NavMeshAgent nav;               // Reference to the nav mesh agent.

        EnemyAttack enemyAttack;
        [SerializeField] SphereCollider detectionCollider;
        LayerMask playerMask = 1 << 3;

        void Awake ()
        {
            // Set up the references.
            enemyHealth = GetComponent <EnemyHealth> ();
            nav = GetComponent <UnityEngine.AI.NavMeshAgent> (); 
            enemyAttack = GetComponent <EnemyAttack> ();
            
        }
        
        void Update ()
        {
            // If the enemy and the player have health left...
            EnemyBehaviour();
        }

        //[PunRPC]
        void EnemyBehaviour()
        {
            player = CheckForPlayerInRange();

            if (enemyHealth.currentHealth > 0)
            {
                // ... set the destination of the nav mesh agent to the player.
                if (player != null)
                {
                    if (nav.enabled)
                    {
                        nav.SetDestination(player.transform.position);
                    } 
                }
                else 
                {
                    string targetName = CheckHighestScore();
                    GameObject target = GameObject.Find(targetName);
                    GameObject realTarget = null;

                    if(target != null)
                    {
                        
                        realTarget = target.transform.Find("Player").gameObject;
                        if (nav.enabled)
                        {
                            nav.SetDestination(realTarget.transform.position);
                        }
                    }

                }
            }
            // Otherwise...
            else
            {
                // ... disable the nav mesh agent.
                nav.enabled = false;
            }
        }

        string CheckHighestScore()
        {
            string bestPlayer = null;
            int score = 0;

            foreach(var player in PhotonNetwork.PlayerList)
            {
                if (!player.CustomProperties.ContainsKey("isAlive") ||
                    !(bool)player.CustomProperties["isAlive"] ||
                    !player.CustomProperties.ContainsKey("Score") ||
                    score > (int)player.CustomProperties["Score"])
                {
                    Debug.Log("continue");
                    continue;
                }
                Debug.Log("Loop");
                bestPlayer = (string)player.CustomProperties["GameObject"];
                score = (int)player.CustomProperties["Score"];
            }

            return bestPlayer;
        }
        
        GameObject CheckForPlayerInRange()
        {
            float distanceToPlayer = Mathf.Infinity;
            GameObject target = null;

            Collider[] colliders = Physics.OverlapSphere(transform.position, detectionCollider.radius, playerMask);

            if (colliders.Length > 0)
            {
                foreach (var playerInRange in colliders)
                {
                    if (playerInRange.GetComponent<PlayerHealth>().IsDead)
                    {
                        return null;
                    }
                    float newDis = Vector3.Distance(transform.position, playerInRange.transform.position);

                    if (distanceToPlayer < newDis) continue;

                    distanceToPlayer = newDis;
                    target = playerInRange.gameObject;
                }

                return target;
            }
            else
            {
                return null;
            }
        }
    }
}