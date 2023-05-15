using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class MainMenuController : MonoBehaviourPunCallbacks
{
    [SerializeField] Button joinButton;
    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    public void Connect()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            PhotonNetwork.LoadLevel("WaitingRoom");
        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        base.OnJoinRandomFailed(returnCode, message);
        PhotonNetwork.CreateRoom(null, new Photon.Realtime.RoomOptions { MaxPlayers = 4 });
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    public void OnPlayerNickNameValueChanged(TMP_InputField playerNickname)
    {
        if (playerNickname.text.Length > 1 && playerNickname.text.Length < 10)
        {
            PhotonNetwork.LocalPlayer.NickName = playerNickname.text;
            joinButton.gameObject.SetActive(true);
        }
        else
        {
            joinButton.gameObject.SetActive(false);
        }

    }
}
