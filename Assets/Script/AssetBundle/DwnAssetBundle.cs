using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class DwnAssetBundle : MonoBehaviour
{

    public Transform spawnPoint;
    void Start()
    {
        /*var myLoadedAssetBundle
            = AssetBundle.LoadFromFile("X:\\unitypro\\My project\\Assets\\lampab");
        if (myLoadedAssetBundle == null)
        {
            Debug.Log("Failed to load AssetBundle!");
            return;
        }
        var prefab = myLoadedAssetBundle.LoadAsset<GameObject>("lampab");
        var g=Instantiate(prefab);
        g.transform.position = spawnPoint.position;*/




        //StartCoroutine(GetAssetBundle());
    }

    public IEnumerator GetAssetBundle()
    {
        GameObject item = null;
        string driveurl = "https://drive.usercontent.google.com/u/0/uc?id=1ppwQDTDkjt07RuKWVlmF1NT0gPgxoioC&export=download";



       // http://152.66.245.135:3000/download/lampab

        using (UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle("http://152.66.245.135:3000/download/lampab", 0))
        {
                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log("assetbundle problem"+www.error);
                }
                else
                {
                    AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(www);
                
                    foreach(var name in bundle.GetAllAssetNames())
                    {
                        Debug.Log("ASSETNAME: " + name.ToString());
                    }
                    GameObject go = bundle.LoadAsset(bundle.GetAllAssetNames()[0]) as GameObject;


                Debug.Log("loadedAsset:" + go.name);
                    item =Instantiate(go);
                    item.transform.position=spawnPoint.position;

                    //bundle.Unload(false);
                    yield return new WaitForEndOfFrame();
                }
                //www.Dispose();

        }
        
        
        
         
    }



    private void InstantiateGameObjectFromAssetBundle(GameObject go)
    {
        if(go!=null)
        {
            GameObject instanceGo = Instantiate(go);
            instanceGo.transform.position=spawnPoint.position;
            Debug.Log("Opened");
        }
        else
        {
            Debug.LogWarning("asset bundle go is null");
        }
    }
}
