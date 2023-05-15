using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;

namespace CompleteProject
{
    public class PlayerInitializer : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback
    {
        enum COLOR
        {
            DEFAULT,
            WHITE,
            RED,
            GREEN,
            BLUE,
        }

        public void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            gameObject.name = photonView.Owner.NickName;
            gameObject.transform.SetParent(GameController.Instance.PlayerContainer.transform);
            photonView.Owner.SetScore(0);
            ChangeColor(photonView.Owner.ActorNumber);

            if (photonView.IsMine)
            {
                CameraFollow cameraFollow = Camera.main.GetComponent<CameraFollow>();
                cameraFollow.SetTarget(gameObject.transform.GetChild(0).transform);
                cameraFollow.enabled = true;
                //var hash = PhotonNetwork.LocalPlayer.CustomProperties;
                //hash["Score"] = 0;
                //hash["Health"] = 100;
                //PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
            }
        }

        private void ChangeColor(int actorID)
        {
            SkinnedMeshRenderer mesh = gameObject.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>();

            switch (actorID)
            {
                case (int)COLOR.WHITE:
                    mesh.material.color = Color.white;
                    break;
                case (int)COLOR.RED:
                    mesh.material.color = Color.red;
                    break;
                case (int)COLOR.GREEN:
                    mesh.material.color = Color.green;
                    break;
                case (int)COLOR.BLUE:
                    mesh.material.color = Color.blue;
                    break;
                default:
                    break;
            }
        }
    }
}
