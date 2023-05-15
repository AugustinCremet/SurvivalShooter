using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionCanvas : MonoBehaviour
{
    [SerializeField] GameObject lobbyCanvas;
    [SerializeField] GameObject waitingCanvas;
    public void OnTransitionEnd()
    {
        lobbyCanvas.SetActive(false);
        waitingCanvas.SetActive(true);
    }

    public void OnExitTransitionEnd()
    {
        gameObject.SetActive(false);
    }
}
