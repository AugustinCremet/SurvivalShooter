using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class WaitingRoomInitializer : MonoBehaviourPun, IPunInstantiateMagicCallback
{
    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        GameObject parent = GameObject.Find("Canvas/Players");
        transform.SetParent(WaitingRoomController.instance.GetParent());
        transform.SetSiblingIndex(photonView.Owner.ActorNumber - 1);

        //gameObject.name = "Player " + photonView.Owner.ActorNumber + "Panel";
        gameObject.name = photonView.Owner.NickName;

        TMP_Text text = gameObject.GetComponentInChildren<TMP_Text>();
        //text.text = "Player " + photonView.Owner.ActorNumber;
        text.text = photonView.Owner.NickName;
    }
}
