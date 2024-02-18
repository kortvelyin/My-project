using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
//using UnityEditor.PackageManager;
using UnityEngine;

public class PlayerPr : MonoBehaviour
{

    public GameObject spawnedPlayerObject;
    // Start is called before the first frame update
    void Start()
    {
        if(NetworkManager.Singleton.IsClient)
        {
        Debug.Log("we spawned an object:D");
        var instance = Instantiate(spawnedPlayerObject);
        instance.transform.parent = instance.transform;
        var instanceNetworkObject = instance.GetComponent<NetworkObject>();
        instanceNetworkObject.SpawnWithOwnership(NetworkManager.Singleton.LocalClientId);
        }
        
    }

    
}
