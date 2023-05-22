using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManagement : MonoBehaviour
{
    public static PlayerManagement Instance;
    private PhotonView photonView;
    private List<PlayerStats> playerStats = new List<PlayerStats>();

    private void Awake()
    {
        Instance = this;
        photonView = GetComponent<PhotonView>();
    }

    public void AddPlayerStats(Player player)
    {
        int idx = playerStats.FindIndex(x => x.PhotonPlayer == player);
        if(idx == -1)
        {
            playerStats.Add(new PlayerStats(player, 30));
        }
    }

    public void ModifyHealth(Player player, int value) 
    {
        int index = playerStats.FindIndex(x=>x.PhotonPlayer == player);
        if(index != -1)
        {
            PlayerStats ps = playerStats[index];
            ps.Health += value;
            PlayerNetwork.Instance.NewHealth(player, ps.Health);
        }
    }
}

public class PlayerStats
{
    public PlayerStats(Player photonPlayer, int health)
    {
        PhotonPlayer= photonPlayer;
        Health= health;
    }

    public readonly Player PhotonPlayer;
    public int Health;
}