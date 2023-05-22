using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLayoutGroup : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject _playerListingPrefab;
    private GameObject PlayerListingPrefab { get { return _playerListingPrefab; } }

    private List<PlayerListing> _playerListing = new List<PlayerListing>();
    public List<PlayerListing> PlayerListings { get { return _playerListing; } }


    // Called by photon whenever you join a room.
    public override void OnJoinedRoom()
    {
        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        MainCanvasManager.Instance.CurrentRoomCanvas.transform.SetAsLastSibling();

        Player[] players = PhotonNetwork.PlayerList;
        for(int i = 0; i < players.Length; i++)
        {
            PlayerJoinedRoom(players[i]);
        }
    }

    private void PlayerJoinedRoom(Player player)
    {
        if (player == null) return;

        PlayerLeftRoom(player);

        GameObject playerListingObj = Instantiate(PlayerListingPrefab);
        playerListingObj.transform.SetParent(transform, false);

        PlayerListing pl = playerListingObj.GetComponent<PlayerListing>();
        pl.ApplyPhotonPlayer(player);

        PlayerListings.Add(pl);
    }

    private void PlayerLeftRoom(Player player)
    {
        int index = PlayerListings.FindIndex(x => x.Player == player);
        if (index != -1)
        {
            Destroy(PlayerListings[index].gameObject);
            PlayerListings.RemoveAt(index);
        }
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        PlayerJoinedRoom(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        PlayerLeftRoom(otherPlayer); 
    }

    public void OnClickRoomState()
    {
        if(!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        PhotonNetwork.CurrentRoom.IsOpen = !PhotonNetwork.CurrentRoom.IsOpen;
        PhotonNetwork.CurrentRoom.IsVisible= !PhotonNetwork.CurrentRoom.IsOpen;
    }

    public void OnClickLeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
}