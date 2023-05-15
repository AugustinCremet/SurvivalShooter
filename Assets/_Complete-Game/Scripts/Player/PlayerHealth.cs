using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using Photon.Pun;

namespace CompleteProject
{
    public class PlayerHealth : MonoBehaviourPun
    {
        public int startingHealth = 100;                            // The amount of health the player starts the game with.
        public int currentHealth;                                   // The current health the player has.
        public Slider healthSlider;                                 // Reference to the UI's health bar.
        public Image damageImage;                                   // Reference to an image to flash on the screen on being hurt.
        public AudioClip deathClip;                                 // The audio clip to play when the player dies.
        public float flashSpeed = 5f;                               // The speed the damageImage will fade at.
        public Color flashColour = new Color(1f, 0f, 0f, 0.1f);     // The colour the damageImage is set to, to flash.


        Animator anim;                                              // Reference to the Animator component.
        AudioSource playerAudio;                                    // Reference to the AudioSource component.
        PlayerMovement playerMovement;                              // Reference to the player's movement.
        PlayerShooting playerShooting;                              // Reference to the PlayerShooting script.
        bool isDead;                                                // Whether the player is dead.
        bool damaged;                                               // True when the player gets damaged.
        public bool IsDead { get { return isDead; } }
        OnlineManager onlineManager;

        void Awake ()
        {
            // Setting up the references.
            anim = GetComponent <Animator> ();
            playerAudio = GetComponent <AudioSource> ();
            playerMovement = GetComponent <PlayerMovement> ();
            playerShooting = GetComponentInChildren <PlayerShooting> ();
            onlineManager = GameObject.Find("OnlineManager").GetComponent <OnlineManager> ();
            damageImage = GameObject.Find("HUDCanvas/DamageImage").GetComponent<Image> ();
            // Set the initial health of the player.
            currentHealth = startingHealth;

        }


        void Update()
        {
            if (!photonView.IsMine)
                return;

            //If the player has just been damaged...
            if (damaged)
            {
                //... set the colour of the damageImage to the flash colour.
                damageImage.color = flashColour;
            }
            //Otherwise...
            else
            {
                //... transition the colour back to clear.
                damageImage.color = Color.Lerp(damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
            }

            //Reset the damaged flag.
            damaged = false;
        }


        public void TakeDamage (int amount)
        {
            // Set the damaged flag so the screen will flash.
            damaged = true;

            // Reduce the current health by the damage amount.
            currentHealth -= amount;

            SetCustomPropertiesHP();

            // Set the health bar's value to the current health.
            //healthSlider.value = currentHealth;

            // Play the hurt sound effect.
            playerAudio.Play ();

            // If the player has lost all it's health and the death flag hasn't been set yet...
            if(currentHealth <= 0 && !isDead)
            {
                // ... it should die.
                Death ();
            }
        }

        void SetCustomPropertiesHP()
        {
            if (!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Health") || !photonView.IsMine)
                return;

            var hash = PhotonNetwork.LocalPlayer.CustomProperties;
            hash["Health"] = currentHealth;
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }

        void Death ()
        {
            // Set the death flag so this function won't be called again.
            isDead = true;

            // Turn off any remaining shooting effects.
            playerShooting.DisableEffects ();

            // Tell the animator that the player is dead.
            anim.SetTrigger ("Die");

            // Set the audiosource to play the death clip and play it (this will stop the hurt sound from playing).
            playerAudio.clip = deathClip;
            playerAudio.Play ();

            // Turn off the movement and shooting scripts.
            playerMovement.enabled = false;
            playerShooting.enabled = false;

            if (!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("isAlive") || !photonView.IsMine)
                return;

            var hash = PhotonNetwork.LocalPlayer.CustomProperties;
            hash["isAlive"] = false;
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
    }
}