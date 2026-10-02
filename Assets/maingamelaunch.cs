using UnityEngine;
using Photon.Pun; 
public class maingamelaunch : MonoBehaviourPunCallbacks 
{
    public networkshit networkshit;
    public string playerPrefabName = "MONKE"; 

    void Start()
    {
        // Check if we are ALREADY in the room (The Host usually is)
        if (PhotonNetwork.InRoom)
        {
            SpawnMonke();
        }
    }

    // If the Client wasn't in the room yet during Start(), this triggers the 
    // exact millisecond they finally step through the door.
    public override void OnJoinedRoom()
    {
        SpawnMonke();
    }

    void SpawnMonke()
    {
        Vector3 spawnPosition = new Vector3(Random.Range(300f, 600f), 70f, Random.Range(130f, 800f));
        PhotonNetwork.Instantiate(playerPrefabName, spawnPosition, Quaternion.identity);
    }
}