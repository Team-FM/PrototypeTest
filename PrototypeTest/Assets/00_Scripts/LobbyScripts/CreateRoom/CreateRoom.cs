using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class CreateRoom : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private TMP_Text _roomName;
    public TMP_Text RoomName
    {
        get { return _roomName;}
    }

    public void OnClick_CreateRoom()
    {
        RoomOptions roomOptions = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = 4 };

        if(PhotonNetwork.CreateRoom(RoomName.text, roomOptions, TypedLobby.Default))
        {
            Debug.Log("Create Room Successfully sent.");
        }
        else
        {
            Debug.Log("Craete Room failed to send");
        }
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log($"CreatedRoomFailed ! returnCode {returnCode} / message {message}");
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Room Created Successfully");
    }
}