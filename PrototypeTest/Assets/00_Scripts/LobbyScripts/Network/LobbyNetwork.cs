using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class LobbyNetwork : MonoBehaviourPunCallbacks
{
    private static readonly string _gameVersion = "1";
    private void Start()
    {
        Debug.Log("Connecting to server..");
        PhotonNetwork.GameVersion = _gameVersion;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to master...");
        PhotonNetwork.AutomaticallySyncScene = false;
        PhotonNetwork.NickName = PlayerNetwork.Instance.PlayerName;

        PhotonNetwork.JoinLobby(TypedLobby.Default);
    }

    public override void OnJoinedLobby()
    {
        if(!PhotonNetwork.InRoom)
        {
            MainCanvasManager.Instance.LobbyCanvas.transform.SetAsLastSibling();
        }
    }
}