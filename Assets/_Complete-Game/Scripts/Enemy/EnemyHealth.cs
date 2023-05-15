using UnityEngine;
using Photon.Pun;
using System.Collections;

namespace CompleteProject
{
    public class EnemyHealth : MonoBehaviourPun
    {
        public int startingHealth = 100;            // The amount of health the enemy starts the game with.
        public int currentHealth;                   // The current health the enemy has.
        public float sinkSpeed = 2.5f;              // The speed at which the enemy sinks through the floor when dead.
        public int scoreValue = 10;                 // The amount added to the player's score when the enemy dies.
        public AudioClip deathClip;                 // The sound to play when the enemy dies.


        Animator anim;                              // Reference to the animator.
        AudioSource enemyAudio;                     // Reference to the audio source.
        ParticleSystem hitParticles;                // Reference to the particle system that plays when the enemy is damaged.
        CapsuleCollider capsuleCollider;            // Reference to the capsule collider.
        bool isDead;                                // Whether the enemy is dead.
        bool isSinking;                             // Whether the enemy has started sinking through the floor.
        int lastPlayerHitId;
        private int idOfKill;

        void Awake ()
        {
            // Setting up the references.
            anim = GetComponent <Animator> ();
            enemyAudio = GetComponent <AudioSource> ();
            hitParticles = GetComponentInChildren <ParticleSystem> ();
            capsuleCollider = GetComponent <CapsuleCollider> ();

            // Setting the current health when the enemy first spawns.
            currentHealth = startingHealth;
        }
        
        void Update ()
        {
            // If the enemy should be sinking...
            if (isSinking)
            {
                // ... move the enemy down by the sinkSpeed per second.
                transform.Translate (-Vector3.up * sinkSpeed * Time.deltaTime);
            }
        }

        [PunRPC]
        public void TakeDamage (int amount, Vector3 hitPoint, int id)
        {
            // If the enemy is dead...
            if(isDead)
                // ... no need to take damage so exit the function.
                return;

            lastPlayerHitId = id;

            // Play the hurt sound effect.
            if(!enemyAudio.isPlaying && PhotonNetwork.IsMasterClient)
                enemyAudio.Play ();

            // Reduce the current health by the amount of damage sustained.
            currentHealth -= amount;
            
            // Set the position of the particle system to where the hit was sustained.
            hitParticles.transform.position = hitPoint;

            // And play the particles.
            hitParticles.Play();

            // If the current health is less than or equal to zero...
            if(currentHealth <= 0)
            {
                // ... the enemy is dead.
                Death();
            }
        }
        
        void Death ()
        {
            // The enemy is dead.
            isDead = true;

            // Turn the collider into a trigger so shots can pass through it.
            capsuleCollider.enabled = false;

            // Tell the animator that the enemy is dead.
            anim.SetTrigger ("Dead");

            // Change the audio clip of the audio source to the death clip and play it (this will stop the hurt clip playing).
            enemyAudio.clip = deathClip;
            if(PhotonNetwork.IsMasterClient)
                enemyAudio.Play ();
        }

        public void StartSinking()
        {
            if(PhotonNetwork.IsMasterClient)
                photonView.RPC(nameof(Sink), RpcTarget.All);
        }

        [PunRPC]
        void Sink ()
        {
            // Find and disable the Nav Mesh Agent.
            GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;

            // Find the rigidbody component and make it kinematic (since we use Translate to sink the enemy).
            GetComponent<Rigidbody>().isKinematic = true;

            // The enemy should no sink.
            isSinking = true;

            if (lastPlayerHitId == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                var hash = PhotonNetwork.LocalPlayer.CustomProperties;
                hash["Score"] = scoreValue + (int)PhotonNetwork.LocalPlayer.CustomProperties["Score"];
                PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
            }

            Destroy(gameObject, 2.0f);
        }
    }
}

