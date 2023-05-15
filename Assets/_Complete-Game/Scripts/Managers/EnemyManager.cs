using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace CompleteProject
{
    public class EnemyManager : MonoBehaviourPun//, IPunObservable
    {
        public PlayerHealth playerHealth;       // Reference to the player's heatlh.
        public GameObject enemyPrefab;                // The enemy prefab to be spawned.
        public float spawnTime = 3f;            // How long between each spawn.
        public Transform[] spawnPoints;         // An array of the spawn points this enemy can spawn from.

        private void Start()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                if (PhotonNetwork.PlayerList.Length >= 2)
                {
                    spawnTime /= (float)PhotonNetwork.PlayerList.Length;
                }
                InvokeRepeating(nameof(Spawn), 0.0f, spawnTime);
            }
        }
        void Spawn()
        {
            // Find a random index between zero and one less than the number of spawn points.
            int spawnPointIndex = Random.Range(0, spawnPoints.Length);
        
            var enemy = PhotonNetwork.Instantiate(enemyPrefab.name, spawnPoints[spawnPointIndex].position, spawnPoints[spawnPointIndex].rotation);
            int enemyId = enemy.GetComponent<PhotonView>().ViewID;

            if (Time.time >= 60f && Time.time < 120f)
            {
                photonView.RPC(nameof(ModifyEnemy), RpcTarget.All,enemyId, 0.25f,50,5);
            }
            else if (Time.time >= 120 && Time.time < 180f)
            {
                photonView.RPC(nameof(ModifyEnemy), RpcTarget.All, enemyId, 0.25f, 75, 5);
            }
            else if (Time.time >= 180f)
            {
                photonView.RPC(nameof(ModifyEnemy), RpcTarget.All, enemyId, 0.25f, 100, 5);
            }
        }

        [PunRPC]
        private void ModifyEnemy(int enemyId, float speed, int health, int damage)
        {
            GameObject enemy = PhotonView.Find(enemyId).gameObject;
            enemy.GetComponent<NavMeshAgent>().speed += speed;
            enemy.GetComponent<EnemyHealth>().currentHealth += health;
            Debug.Log(enemy.GetComponent<EnemyHealth>().currentHealth);
            enemy.GetComponent<EnemyAttack>().attackDamage += damage;
        }
    }
}
