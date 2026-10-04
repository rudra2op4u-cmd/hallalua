using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class quitbutton : MonoBehaviourPun
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void quit()
    {
        PhotonNetwork.LeaveRoom(); 
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}
