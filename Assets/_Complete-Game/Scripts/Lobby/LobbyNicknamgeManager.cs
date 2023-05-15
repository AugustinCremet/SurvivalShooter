using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyNicknamgeManager : MonoBehaviour
{
    Animator animator;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    public void ResetWiggle()
    {
        animator.SetBool("Wiggle", false);
        LobbyUiManager.feedBack();
    }
}
