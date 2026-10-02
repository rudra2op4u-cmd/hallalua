using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Photon.Pun;
using UnityEngine;

public class bullet : MonoBehaviourPun
{
    void Start()
    {
        GetComponent<Rigidbody>().velocity = transform.forward * 1000;

        // Only the owner of the bullet is allowed to trigger the self-destruct
        if (photonView.IsMine)
        {
            // Tell the server to delete this bullet after 3 seconds
            Invoke("AutoDestruct", 3f); 
        }
    }
    void OnTriggerEnter(Collider other)
    {
    // 1. Only the computer that fired the bullet is allowed to calculate the hit!
    // This stops enemy screens from calculating duplicate ghost bullets.
    if (!photonView.IsMine) return;

    // 2. Check if we hit a monkey
    sommin targetHealth = other.gameObject.GetComponent<sommin>();

    if (targetHealth != null)
    {
        // 3. PREVENT SELF-HARM: If the monkey we hit is OUR monkey, ignore it!
        if (targetHealth.photonView.IsMine) return; 

        // 4. We successfully hit an ENEMY! Send the damage signal to their body across the network.
        targetHealth.photonView.RPC("SyncDamage", RpcTarget.All, 20);
    }

    // 5. Destroy the bullet on the network after it hits anything (a wall, floor, or enemy)
    PhotonNetwork.Destroy(gameObject);
    }
    void AutoDestruct()
{
    PhotonNetwork.Destroy(gameObject);
}
}
