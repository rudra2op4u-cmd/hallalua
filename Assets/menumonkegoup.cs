using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;
using UnityEngine.UI;

public class menumonkegoup : MonoBehaviour
{
    [SerializeField]
    public float timeInterval = 5f;
    [SerializeField]
    public float nexttime = 0;
    public GameObject ragdoll ;   
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time >= nexttime)
        {
            Vector3 playerposition = new Vector3(Random.Range(0f, 797f), 385f,Random.Range(-187, 824f));
            Instantiate(ragdoll,playerposition,Random.rotation);
            nexttime = Time.time + timeInterval;
        }
            
    }
    
}   
