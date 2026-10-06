using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class grenadegowheee : MonoBehaviourPun
{
    public float delay = 3f;
    public float blastRadius = 22.5f;
    public int explosionDamage = 75;

    void Start()
    {
        if (photonView.IsMine)
        {
            Invoke("Explode", delay);
        }
    }

    void Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, blastRadius);
        foreach (Collider nearbyObject in colliders)
        {
            sommin targetHealth = nearbyObject.GetComponent<sommin>();
            
            if (targetHealth != null)
            {
                targetHealth.photonView.RPC("SyncDamage", RpcTarget.All, explosionDamage);
            }
        }

        // 3. Delete the grenade from the server
        PhotonNetwork.Destroy(gameObject);
    }
}
