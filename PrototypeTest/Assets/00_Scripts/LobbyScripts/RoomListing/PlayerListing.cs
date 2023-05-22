using Photon.Realtime;
using TMPro;
using UnityEngine;

public class PlayerListing : MonoBehaviour
{
    public Player Player { get; private set; }

    [SerializeField]
    private TMP_Text _playerName;
    public TMP_Text PlayerName { get { return _playerName; } }

    public void ApplyPhotonPlayer(Player newPlayer)
    {
        Player = newPlayer;
        PlayerName.text = newPlayer.NickName;
    }
}