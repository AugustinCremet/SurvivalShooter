using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
using Photon.Realtime;

public class OnlineManager : MonoBehaviourPun
{
    private class PlayersStats
    {
        public PlayersStats(string name, int hp, int score)
        {
            Name = name;
            Hp = hp;
            Score = score;
        }
        public string Name { get; set; }
        public int Hp { get; set; }
        public int Score { get; set; }

        private string name;
        private int hp;
        private int score;
    }

    public static OnlineManager Instance { get; private set; }
    List<PlayersStats> playersList = new List<PlayersStats>();
    [SerializeField] GameObject panelOfAllPlayers;

    void Awake()
    {
        if(Instance == null)
            Instance = this;
    }

    public void AddPlayerToList(Player newPlayer)
    {
        photonView.RPC(nameof(AddPlayerPun), RpcTarget.All, newPlayer);
    }

    [PunRPC]
    void AddPlayerPun(Player newPlayer)
    {
        playersList.Add(new PlayersStats((string)newPlayer.CustomProperties["GameObject"],
                                         (int)newPlayer.CustomProperties["Health"],
                                         (int)newPlayer.CustomProperties["Score"]));

        int index = newPlayer.ActorNumber - 1;
        panelOfAllPlayers.transform.GetChild(0).transform.GetChild(index).gameObject.SetActive(true);
        panelOfAllPlayers.transform.GetChild(1).transform.GetChild(index).gameObject.SetActive(true);
        panelOfAllPlayers.transform.GetChild(2).transform.GetChild(index).gameObject.SetActive(true);
    }

    void Start()
    {
        photonView.RPC(nameof(Update), RpcTarget.All);
    }

    [PunRPC]
    void Update()
    {
        if (playersList.Count == 0) return;

        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++) // 
        {
            if (!PhotonNetwork.PlayerList[i].CustomProperties.ContainsKey("Score"))
                return;
            playersList[i].Score = (int)PhotonNetwork.PlayerList[i].CustomProperties["Score"];
            playersList[i].Hp = (int)PhotonNetwork.PlayerList[i].CustomProperties["Health"];
            playersList[i].Name = PhotonNetwork.PlayerList[i].NickName;
        }

        playersList.Sort((player1, player2) =>
        {
            return player2.Score - player1.Score;
        });


        string[] name = new string[]{ "" , "", "", ""};
        int[] hp = new int[] { 0, 0, 0, 0 };
        string[] score = new string[]{ "", "", "", "" };

        //Set values
        for (int i = 0; i < playersList.Count; i++)
        {
            //Names
            name[i] = playersList[i].Name;
            //HP
            hp[i] = playersList[i].Hp;

            if (hp[i] <= 0)
            {
                //TO ADD when player will be able to continue playing even while there are dead player
                panelOfAllPlayers.transform.GetChild(1).transform.GetChild(i).gameObject.GetComponent<Text>().enabled = true;
                panelOfAllPlayers.transform.GetChild(1).transform.GetChild(i).GetChild(0).gameObject.SetActive(false);
            }
            else
            {
                panelOfAllPlayers.transform.GetChild(1).transform.GetChild(i).gameObject.GetComponent<Text>().enabled = false;
                panelOfAllPlayers.transform.GetChild(1).transform.GetChild(i).GetChild(0).gameObject.SetActive(true);
            }

            //Scores
            score[i] = playersList[i].Score.ToString();
        }

        //Show values
        for (int i = 0; i < playersList.Count; i++)
        {
            //Names
            (panelOfAllPlayers.transform.GetChild(0).transform.GetChild(i).gameObject.GetComponent<Text>()).text = name[i];
            //HP
            if (hp[i] > 0)
                (panelOfAllPlayers.transform.GetChild(1).transform.GetChild(i).GetComponentInChildren<Slider>()).value = hp[i];
            //Scores
            (panelOfAllPlayers.transform.GetChild(2).transform.GetChild(i).gameObject.GetComponent<Text>()).text = score[i];
        }
    }
}
