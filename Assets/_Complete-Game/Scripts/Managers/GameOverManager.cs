using UnityEngine;
using Photon.Pun;
namespace CompleteProject
{
    public class GameOverManager : MonoBehaviour
    {
        bool following = false;
        Animator anim;                          // Reference to the animator component.
        void Awake()
        {
            // Set up the reference.
            anim = GetComponent<Animator>();
        }
        void Update()
        {
            if (!GameController.Instance.IsGameOver())
            {
                if ((bool)PhotonNetwork.LocalPlayer.CustomProperties["isAlive"] == false && !following)
                {
                    // ... tell the animator the game is over.
                    anim.ResetTrigger("Reset");
                    anim.SetTrigger("GameOver");
                    following = true;
                }
            }

        }

        public void ResetGameOverScreen()
        {
            anim.ResetTrigger("GameOver");
            anim.SetTrigger("Reset");
        }

        public void OnGameOverEnd()
        {
            GameController.Instance.GameOverEnd();
        }
    }
}