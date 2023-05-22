using Photon.Pun;
using UnityEngine;

public class CurrentRoomCanvas : MonoBehaviour
{
    public void OnClickStartSync()
    {
        if (PhotonNetwork.IsMasterClient == false) return;

        PhotonNetwork.LoadLevel(1);
    }

    public void OnClickStartDelayed()
    {
        if (PhotonNetwork.IsMasterClient == false) return;

        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;
        PhotonNetwork.LoadLevel(1);
    }
}