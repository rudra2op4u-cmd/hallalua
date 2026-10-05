using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class quitbutton : MonoBehaviourPun
{
    public void quit()
    {
        PhotonNetwork.LeaveRoom(); 
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}
