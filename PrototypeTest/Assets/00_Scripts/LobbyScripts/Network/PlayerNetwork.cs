using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerNetwork : MonoBehaviour
{
    public static PlayerNetwork Instance;
    public string PlayerName { get; private set; }

    private PhotonView PhotonView;
    private int PlayersInGame = 0;

    private PlayerMovement CurrentPlayer;

    private void Awake()
    {
        PhotonView = GetComponent<PhotonView>();

        Instance = this;
        PlayerName = "Distul#" + Random.Range(1000, 9999);

        //PhotonNetwork.SendRate = 60;
        //PhotonNetwork.SerializationRate = 30;


        SceneManager.sceneLoaded += OnSceneFinishedLoading;
    }

    private void OnSceneFinishedLoading(Scene scene, LoadSceneMode lcm)
    {
        if (scene.name == "Game")
        {
            if (PhotonNetwork.IsMasterClient)
            {
                MasterLoadedGame();
            }
            else
            {
                NonMasterLoadedGame();
            }
        }
    }

    private void MasterLoadedGame()
    {
        PhotonView.RPC("RPC_LoadedGameScene", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer);
        PhotonView.RPC("RPC_LoadGameOthers", RpcTarget.Others);
    }

    private void NonMasterLoadedGame()
    {
        PhotonView.RPC("RPC_LoadedGameScene", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer);
    }

    [PunRPC]
    private void RPC_LoadGameOthers()
    {
        PhotonNetwork.LoadLevel(1);
    }

    [PunRPC]
    private void RPC_LoadedGameScene(Player photonPlayer)
    {
        PlayerManagement.Instance.AddPlayerStats(photonPlayer);
        PlayersInGame++;
        if(PlayersInGame == PhotonNetwork.PlayerList.Length)
        {
            Debug.Log("All players are in the game scene");
            PhotonView.RPC("RPC_CreatePlayer", RpcTarget.All);
        }
    }

    public void NewHealth(Player photonPlayer, int health)
    {
        PhotonView.RPC("RPC_NewHealth", photonPlayer, health);
    }

    [PunRPC]
    private void RPC_NewHealth(int health)
    {
        if (CurrentPlayer == null) return;

        if(health <= 0)
        {
            PhotonNetwork.Destroy(CurrentPlayer.gameObject);
        }
        else
        {
            CurrentPlayer.Health = health;
        }
    }

    [PunRPC]
    private void RPC_CreatePlayer()
    {
        float randomValue = Random.Range(0, 5f);
        int randomInteger = Random.Range(0, 10);
        string playerPrefabName = "";
        if (5 > randomInteger)
            playerPrefabName = Path.Combine("Prefab", "Player_Rio");
        else
            playerPrefabName = Path.Combine("Prefab", "Player_Nicky");
        GameObject obj = PhotonNetwork.Instantiate(playerPrefabName, new Vector3(0f, 1f, -9f), Quaternion.identity, 0);
        CurrentPlayer = obj.GetComponent<PlayerMovement>();
    }
}