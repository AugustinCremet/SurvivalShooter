using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using TMPro;
using System.Collections;

public class WaitingRoomController : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject playerList;
    [SerializeField] GameObject playerNamePrefab;
    [SerializeField] TMP_Text blackText;
    [SerializeField] TMP_Text whiteText;
    [SerializeField] Button startGameButton;
    [SerializeField] GameObject transitionCanvas;
    [SerializeField] GameObject amountReadyText;
    [SerializeField] GameObject _lobbyManager;

    [SerializeField, Range(2, 4)] int minNumberPlayers;

    private int nbOfPlayersReady;

    private GameObject playerName;
    private bool ready;
    private bool created = false;
    public static WaitingRoomController instance;

    public void Awake()
    {
        Debug.Assert(instance == null);
        instance = this;
    }

    public Transform GetParent()
    {
        return playerList.transform;
    }

    private void Update()
    {
        nbOfPlayersReady = CheckIfReady();
        amountReadyText.GetComponentInChildren<TMP_Text>().text = nbOfPlayersReady + " / " + PhotonNetwork.PlayerList.Length;

        if (nbOfPlayersReady >= minNumberPlayers && PhotonNetwork.IsMasterClient)
        {
            startGameButton.gameObject.SetActive(true);
        }
        else
        {
            startGameButton.gameObject.SetActive(false);
        }
    }

    public void Create()
    {
        playerName = PhotonNetwork.Instantiate(playerNamePrefab.name, Vector3.zero, Quaternion.identity);

        var hash = PhotonNetwork.LocalPlayer.CustomProperties;
        hash["Ready"] = false;
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }
    private int CheckIfReady()
    {
        int readyNB = 0;

        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (!player.CustomProperties.ContainsKey("Ready")) continue;

            if ((bool)player.CustomProperties["Ready"])
            {
                readyNB++;
            }
        }

        return readyNB;
    }

    public void LoadGame()
    {
        PhotonNetwork.LoadLevel("Game");
    }

    public void ReadyButton()
    {
        var hash = PhotonNetwork.LocalPlayer.CustomProperties;

        if (!ready)
        {
            ready = true;
            hash["Ready"] = true;
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

            playerName.GetComponent<PhotonView>().RPC("ReadyBehaviour", RpcTarget.All);
            blackText.text = "UNREADY";
            whiteText.text = "UNREADY";
        }
        else
        {
            ready = false;
            hash["Ready"] = false;
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

            playerName.GetComponent<PhotonView>().RPC("UnreadyBehaviour", RpcTarget.All);
            blackText.text = "READY";
            whiteText.text = "READY";
        }
    }

    public void BackToLobby()
    {
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("Lobby");
    }
}
