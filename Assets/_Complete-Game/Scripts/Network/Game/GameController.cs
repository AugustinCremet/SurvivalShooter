using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;

namespace CompleteProject
{
    public class GameController : MonoBehaviourPunCallbacks
    {
        public GameObject enemiesContainer;
        [SerializeField] GameObject playerPrefab;
        [SerializeField] GameObject playerContainer;

        [SerializeField] TMP_Text spectateText;
        [SerializeField] GameObject CameraChoice;

        [SerializeField] Transform[] spawnPositions;
        [SerializeField] GameOverManager gameOverManager;

        private bool loadScene = false;
        public static GameController Instance { get; private set; }

        [SerializeField] GlobalGameOver globalGameOver;

        [SerializeField] GameObject onlineManagerObject;
        OnlineManager onlineManager;

        public GameObject PlayerContainer
        { get { return playerContainer; } }
       

        public void Awake()
        {
            Debug.Assert(Instance == null);
            Instance = this;
            onlineManager = onlineManagerObject.GetComponent<OnlineManager>();
        }

        void Start()
        {
            int index = PhotonNetwork.LocalPlayer.ActorNumber - 1;

            GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPositions[index].position, spawnPositions[index].rotation);

            if (player.GetComponent<PhotonView>().IsMine)
            {
                gameOverManager.enabled = true;
            }

            var hash = PhotonNetwork.LocalPlayer.CustomProperties;

            hash["Score"] = 0;
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

            hash["isAlive"] = true;
		    PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

            hash["Health"] = 100;
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

            hash["GameObject"] = player.name;
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

            hash["Ready"] = false;
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

            OnlineManager.Instance.AddPlayerToList(PhotonNetwork.LocalPlayer);
        }

        public bool IsGameOver()
        {
            bool gameOver = true;
            foreach (var player in PhotonNetwork.PlayerList)
            {
                if (!player.CustomProperties.ContainsKey("isAlive")) continue;

                if ((bool)player.CustomProperties["isAlive"])
                {
                    gameOver = false;
                    break;
                }
            }
            return gameOver;
        }

        private void Update()
        {       
            if (IsGameOver() && !loadScene)
            {
                globalGameOver.gameObject.SetActive(true);
                globalGameOver.PlayAnimation();
                loadScene = true;
            }
        }

        [PunRPC]
        private void StopGame()
        {
            Time.timeScale = 0; 
        }

        public void GameOverEnd()
        {
            int j = 0;
            for (int i = 0; i < playerContainer.transform.childCount; i++)
            {
                if (!(bool)playerContainer.transform.GetChild(i).gameObject.GetComponent<PhotonView>().Owner.CustomProperties["isAlive"])
                    continue;

                CameraChoice.transform.GetChild(j).gameObject.SetActive(true);
                CameraChoice.transform.GetChild(j).transform.GetChild(0).gameObject.GetComponentInChildren<TMP_Text>().text = playerContainer.transform.GetChild(i).gameObject.name;
                CameraChoice.transform.GetChild(j).SetParent(CameraChoice.transform);
                CameraChoice.transform.GetChild(j).gameObject.name = playerContainer.transform.GetChild(i).gameObject.name + "Camera";
                int playerNum = i;//stupid but this is the easy solution for anonymous delegate scope problem
                CameraChoice.transform.GetChild(j).gameObject.GetComponent<Button>().onClick.AddListener(
                    () => ShowPlayerCamera(playerNum)//if you pass i here, it gonna be the last value of i when loop finish
                    );
                j++;
            }
            spectateText.gameObject.SetActive(true);
        }
        public void ShowPlayerCamera(int i)
        {
            gameOverManager.ResetGameOverScreen();
            Transform target = playerContainer.transform.GetChild(i).transform.GetChild(0);
            Vector3 startPos = Camera.main.transform.position;
            startPos.x = target.position.x;
            Camera.main.transform.position = startPos;
            CameraFollow cameraFollow = Camera.main.GetComponent<CameraFollow>();
            cameraFollow.SetTarget(target);
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            base.OnDisconnected(cause);
            SceneManager.LoadScene("Lobby");
        }
    }
    
}
