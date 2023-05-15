using UnityEngine;
using System.Collections;

namespace CompleteProject
{
    public class EnemyAttack : MonoBehaviour
    {
        public float timeBetweenAttacks = 0.5f;     // The time in seconds between each attack.
        public int attackDamage = 10;               // The amount of health taken away per attack.

        Animator anim;                              // Reference to the animator component.
        GameObject player;                          // Reference to the player GameObject.
        PlayerHealth playerHealth;                  // Reference to the player's health.
        EnemyHealth enemyHealth;                    // Reference to this enemy's health.
        float timer;                                // Timer for counting up to the next attack.

        SphereCollider sphereCollider;
        int playerMask = 1 << 3;

        void Awake ()
        {
            // Setting up the references.
            enemyHealth = GetComponent<EnemyHealth>();
            anim = GetComponent <Animator> ();
            sphereCollider = GetComponent <SphereCollider> ();
        }

        void Update ()
        {
            // Add the time since Update was last called to the timer.
            timer += Time.deltaTime;

            player = CheckForPlayerInAttackRange();

            if (player == null)
                return;

            // If the timer exceeds the time between attacks, the player is in range and this enemy is alive...
            if(timer >= timeBetweenAttacks && enemyHealth.currentHealth > 0)
            {
                // ... attack.
                Attack ();
            }

            // If the player has zero or less health...
            if(playerHealth.currentHealth <= 0)
            {
                // ... tell the animator the player is dead.
                anim.SetTrigger ("PlayerDead");
            }
        }

        void Attack ()
        {
            // Reset the timer.
            timer = 0f;

            // If the player has health to lose...
            if(playerHealth.currentHealth > 0)
            {
                // ... damage the player.
                playerHealth.TakeDamage (attackDamage);
            }
        }
        
        GameObject CheckForPlayerInAttackRange()
        {
            float distanceToPlayer = Mathf.Infinity;
            GameObject target = null;

            Collider[] colliders = Physics.OverlapSphere(transform.position, sphereCollider.radius, playerMask);

            if (colliders.Length > 0)
            {
                foreach (var playerInRange in colliders)
                {
                    float newDis = Vector3.Distance(transform.position, playerInRange.transform.position);

                    if (distanceToPlayer < newDis) continue;

                    distanceToPlayer = newDis;
                    playerHealth = playerInRange.GetComponent<PlayerHealth>();
                    target = playerInRange.gameObject;
                }

                return target;
            }
            else
            {
                playerHealth = null;
                return null;
            }
        }

        public GameObject PlayerToAttack
        {
            get { return player; }
        }

    }
}