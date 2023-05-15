using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TransitionReplay : MonoBehaviourPun
{
    Animator animator;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        animator.SetTrigger("Enter");
        Invoke(nameof(LoadLevel), 2.0f);
    }

    private void LoadLevel()
    {
        if(PhotonNetwork.IsMasterClient)
            PhotonNetwork.LoadLevel("Game");
    }
}
