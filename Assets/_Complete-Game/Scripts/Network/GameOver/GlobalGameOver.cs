using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
public class GlobalGameOver : MonoBehaviourPun
{
    private class ScoreIndex
    {
        public ScoreIndex(string name, int score)
        {
            Name = name;
            Score = score;
        }
        public string Name { get; }
        public int Score { get; }

        private string name;
        private int score;

    }

    [SerializeField] private GameObject resultPanel;
    [SerializeField] private GameObject winLooseText;

    private List<ScoreIndex> playersScores = new List<ScoreIndex>();
    Animator animator;
    private static int nbOfPlayersReady = 0;
    private bool isLoaded = false;
    [SerializeField] GameObject playerContainer;
    [SerializeField] Text amountReadyText;
    [SerializeField] GameObject readyObject;
    [SerializeField] TMP_Text readyTextBlack;
    [SerializeField] TMP_Text readyTextWhite;
    private void Awake()
    {
        animator = GetComponentInParent<Animator>();
    }

    void SetScoreOrder()
    {
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            playersScores.Add(new ScoreIndex(PhotonNetwork.PlayerList[i].NickName, (int)PhotonNetwork.PlayerList[i].CustomProperties["Score"]));
        };

        playersScores.Sort((player1, player2) =>
        {
            return player2.Score - player1.Score;
        });

        for (int i = 0; i < playersScores.Count; i++)
        {
            //Names
            (resultPanel.transform.GetChild(0).transform.GetChild(i).gameObject.GetComponent<TMP_Text>()).text = playersScores[i].Name;
            resultPanel.transform.GetChild(0).transform.GetChild(i).gameObject.SetActive(true);
            //Scores
            (resultPanel.transform.GetChild(1).transform.GetChild(i).gameObject.GetComponent<TMP_Text>()).text = $" {playersScores[i].Score} points ";
            resultPanel.transform.GetChild(1).transform.GetChild(i).gameObject.SetActive(true);
        }

        if ((int)PhotonNetwork.LocalPlayer.CustomProperties["Score"] == playersScores[0].Score)
        {
            winLooseText.GetComponentInChildren<TMP_Text>().text = "You won!";
        }
        else
        {
            winLooseText.GetComponentInChildren<TMP_Text>().text = "You lost!";
        }
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



    public void PlayAnimation()
    {

        animator.SetTrigger("GameOver");
    }

    public void OnGameOverEnd()
    {
        gameObject.transform.GetChild(1).gameObject.SetActive(true);
        resultPanel.SetActive(true);
        winLooseText.SetActive(true);
        SetScoreOrder();
    }

    public void ReplayGame()
    {

        var hash = PhotonNetwork.LocalPlayer.CustomProperties;
        hash["Ready"] = true;
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

        photonView.RPC("ChangeText", RpcTarget.All);


        Debug.Log(nbOfPlayersReady + " / " + PhotonNetwork.PlayerList.Length);
        if (nbOfPlayersReady == PhotonNetwork.PlayerList.Length && !isLoaded && PhotonNetwork.IsMasterClient)
        {
            isLoaded = true;
            PhotonNetwork.LoadLevel("Transition");
        }
    }

    [PunRPC]
    void ChangeText()
    {
        nbOfPlayersReady = CheckIfReady();
        amountReadyText.text = nbOfPlayersReady + " / " + PhotonNetwork.PlayerList.Length;
        if (nbOfPlayersReady == PhotonNetwork.PlayerList.Length && PhotonNetwork.IsMasterClient ||
            nbOfPlayersReady == PhotonNetwork.PlayerList.Length - 1 && PhotonNetwork.IsMasterClient && !(bool)PhotonNetwork.LocalPlayer.CustomProperties["Ready"])
        {
            readyTextBlack.text = "Start";
            readyTextWhite.text = "Start";
        }

        if ((bool)PhotonNetwork.LocalPlayer.CustomProperties["Ready"] && !PhotonNetwork.IsMasterClient)
            readyObject.SetActive(false);

    }

    public void GoToMainMenu()
    {
        PhotonNetwork.LoadLevel("Lobby");
    }

    public void QuitGame()
    {
        PhotonNetwork.Disconnect();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}
