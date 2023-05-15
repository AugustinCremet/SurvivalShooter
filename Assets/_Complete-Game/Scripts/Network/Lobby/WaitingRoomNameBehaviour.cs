using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;

public class WaitingRoomNameBehaviour : MonoBehaviour
{
    [PunRPC]
    private void ReadyBehaviour()
    {
        //gameObject.GetComponent<Image>().color = Color.green;
        gameObject.GetComponent<Image>().color = Color.white;
        gameObject.transform.GetChild(0).GetComponent<Image>().color = Color.black;
        gameObject.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
    }

    [PunRPC]
    private void UnreadyBehaviour()
    {
        gameObject.GetComponent<Image>().color = Color.black;
        gameObject.transform.GetChild(0).GetComponent<Image>().color = Color.white;
        gameObject.GetComponentInChildren<TextMeshProUGUI>().color = Color.black;
    }
}
