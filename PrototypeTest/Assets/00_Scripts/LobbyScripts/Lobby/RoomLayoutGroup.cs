using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public class RoomLayoutGroup : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject _roomListingPrefab;
    private GameObject RoomListingPrefab
    {
        get { return _roomListingPrefab;}
    }

    private List<RoomListing> _roomListingButtons = new List<RoomListing>();
    public List<RoomListing> RoomListingButtons { get { return _roomListingButtons; } }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach(RoomInfo roomInfo in roomList)
        {
            RoomReceived(roomInfo);
        }

        RemoveOldRooms();
    }

    private void RoomReceived(RoomInfo room)
    {
        int index = RoomListingButtons.FindIndex(x => x.RoomName == room.Name);

        if(index == -1)
        {
            if(room.IsVisible && room.PlayerCount < room.MaxPlayers)
            {
                GameObject roomListingObj = Instantiate(RoomListingPrefab);
                roomListingObj.transform.SetParent(transform, false);

                RoomListing roomListing = roomListingObj.GetComponent<RoomListing>();
                RoomListingButtons.Add(roomListing);

                index = (RoomListingButtons.Count - 1);
            }
        }

        if(index != -1)
        {
            RoomListing roomListing = RoomListingButtons[index];
            roomListing.SetRoomNameText(room.Name);
            roomListing.Updated = true;
        }
    }

    private void RemoveOldRooms()
    {
        List<RoomListing> removeRooms = new List<RoomListing>();
        foreach(RoomListing roomListing in RoomListingButtons) 
        {
            if(!roomListing.Updated)
            {
                removeRooms.Add(roomListing);
            }
            else
            {
                roomListing.Updated = false;
            }
        }

        foreach(RoomListing roomListing in removeRooms)
        {
            GameObject roomListingObj = roomListing.gameObject;
            RoomListingButtons.Remove(roomListing);
            Destroy(roomListingObj);
        }
    }
}