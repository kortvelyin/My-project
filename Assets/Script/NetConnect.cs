using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.VisualScripting;
using System.Net.Sockets;
using System.Net;

public class NetConnect : MonoBehaviour
{

  void Start()
    {

       // if (Application.isEditor)
        {
            Create();
            //StServer();
        }
       // else
        {
       //     Join();
        }

        GetLocalIPAddress();
    }


    public string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                Debug.Log( ip.ToString());
                return ip.ToString();
            }
        }
        throw new System.Exception("No network adapters with an IPv4 address in the system!");
    }

    public void Create()
    {
        Debug.Log("in create");
        NetworkManager.Singleton.StartHost();
    }

    public void Join()
    {
        NetworkManager.Singleton.StartClient();
    }

    public void StServer()
    {
        NetworkManager.Singleton.StartServer();
    }



}
