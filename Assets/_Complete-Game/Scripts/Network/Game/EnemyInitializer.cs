using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using CompleteProject;

public class EnemyInitializer : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback
{
    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        transform.SetParent(GameController.Instance.enemiesContainer.transform);
    }
}
