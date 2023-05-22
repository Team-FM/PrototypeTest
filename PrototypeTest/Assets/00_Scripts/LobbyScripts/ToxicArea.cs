using Photon.Pun;
using UnityEngine;

public class ToxicArea : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(!PhotonNetwork.IsMasterClient) return;

        PhotonView pv = other.GetComponent<PhotonView>();
        if(pv != null)
        {
            PlayerManagement.Instance.ModifyHealth(pv.Owner, -10);
        }
    }
}