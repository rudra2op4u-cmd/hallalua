using UnityEngine;
using Photon.Pun; // This tells the script to use the Photon networking library
using UnityEngine.UI;

public class networkshit : MonoBehaviourPunCallbacks 
{
    public Dropdown mapselector;
    public string map = "map1";
    public Button a;
    public InputField b;
    public bool ServerStarted = false;
    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        Debug.Log("Attempting to connect to the server...");
        PhotonNetwork.ConnectUsingSettings();
        a.interactable = false;
    }
    void Update()
    {
        Debug.Log(""+mapselector.value);
        if (b.text != "" && ServerStarted)
        {
            a.interactable = true;
        }
        else
        {
            a.interactable = false;
        }
        if (mapselector.value == 0)
        {
            map = "monke";
        }
        if (mapselector.value == 1)
        {
            map = "map1";
        }
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected successfully! We are online.");
        ServerStarted = true;
    }

    // This runs when you successfully enter a room with other players
    public override void OnJoinedRoom()
    {
        Debug.Log("Joined a room! Ready to spawn the Monke.");
        if (PhotonNetwork.IsMasterClient)
    {
        PhotonNetwork.LoadLevel(map); 
    }
    }
    public void hostCreateJoin()
    {
        a.interactable = false;
        Photon.Realtime.RoomOptions roomRules = new Photon.Realtime.RoomOptions();
        roomRules.MaxPlayers = 4;
        PhotonNetwork.JoinOrCreateRoom(b.text,roomRules,Photon.Realtime.TypedLobby.Default); 
        
    }
}