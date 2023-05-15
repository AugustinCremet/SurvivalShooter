using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Photon.Pun;

namespace CompleteProject
{
    public class ScoreManager : MonoBehaviour
    {
        public int score;        // The player's score.

        Text text;                      // Reference to the Text component.

        void Awake ()
        {
            // Set up the reference.
            text = GetComponent <Text> ();

            // Reset the score.
            score = 0;
        }

        void Update ()
        {
            if (!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Score"))
                return;
            text.text = "Score: " + (int)PhotonNetwork.LocalPlayer.CustomProperties["Score"];
        }
    }
}