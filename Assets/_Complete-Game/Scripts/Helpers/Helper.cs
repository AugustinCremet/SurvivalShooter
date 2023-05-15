using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helper : MonoBehaviour
{
    public void Enter(GameObject button)
    {
        button.GetComponent<Animator>().ResetTrigger("Exit");
        button.GetComponent<Animator>().SetTrigger("Enter");
    }

    public void Exit(GameObject button)
    {
        button.GetComponent<Animator>().ResetTrigger("Enter");
        button.GetComponent<Animator>().SetTrigger("Exit");
    }

    public void Quit()
    {
        PhotonNetwork.Disconnect();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    public void Mainmenu()
    {
        PhotonNetwork.LoadLevel("Lobby");
    }

    public void Replay()
    {
        PhotonNetwork.LoadLevel("Game");
    }
}
