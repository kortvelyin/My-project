using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Networking;
using static UnityEditor.Experimental.GraphView.GraphView;


[Serializable]
public class LayerItem
{
    public string objectType="";
    public string name="";
    public Transform transform=null;
    public Color color=new Color();
}
public class LayerLoader : MonoBehaviour
{


    public List<GameObject> prefabs;
    public GameObject userParentObject;
    ContactService contactService;
    authManager authMSc;
    //url is a different thing, its an assetbundle, and a url instead of LayerItem

    private void Start()
    {
        userParentObject = new GameObject();
        userParentObject.name = "userID";
        contactService = GameObject.Find("AuthManager").GetComponent<ContactService>();
        authMSc = GameObject.Find("AuthManager").GetComponent<authManager>();
        
    }

    ////listprojects, string on gO, klick and turn that to blocks
    ///list projects
    //klicks projects, get item, call LayerJsonToLayerBegin(string layerName, string layer)


    public void LayerJsonToLayerBegin(string layerName, string layer)
    {
        var parentObject = new GameObject(layerName);
        parentObject.transform.parent = userParentObject.transform;
        if (layer.Contains("layeritem"))
        {
            Project getLayer = JsonUtility.FromJson<Project>(layer);
            
            LayerInfoToLayer(getLayer, parentObject, layerName);
        }
        else if (layer.Contains("htt"))
        {
            StartCoroutine(GetAssetBundle(parentObject));
        }
        else
        {
            //do nothing, change button color?
        }

    }

    public void LayerInfoToLayer(Project getLayer, GameObject parentObject, string layerName)
    {
        List<string> layerItemList = JsonUtility.FromJson<List<string>>(getLayer.model);
        GameObject item = null;
        foreach (var layerItem in layerItemList)
        {
            LayerItem lItem = JsonUtility.FromJson<LayerItem>(layerItem);
            if (lItem.objectType == "Cube")
            {
                item = GameObject.CreatePrimitive(PrimitiveType.Cube);
                item.name = lItem.name;
            }
            else if (prefabs.ToString().Contains(lItem.objectType))
            {
                for (int i = 0; i < prefabs.Count; i++)
                {
                    if (prefabs[i].name == lItem.name)
                    {
                        item = Instantiate(prefabs[i]);
                        break;
                    }
                }
            }
            else
            {
                item = GameObject.CreatePrimitive(PrimitiveType.Cube);
                item.GetComponent<Renderer>().material.color = Color.red;

            }
            item.gameObject.tag = layerName;
            item.name = lItem.name;
            item.transform.parent = parentObject.transform;
            item.transform.position = lItem.transform.position;
            item.transform.rotation = lItem.transform.rotation;
            item.transform.localScale = lItem.transform.localScale;
            item.GetComponentInChildren<Renderer>().material.color = lItem.color;

        }
       
    }

    IEnumerator GetAssetBundle(GameObject parent)
    {
        UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle("https://www.my-server.com/myData.unity3d");
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(www);
            var loadAsset = bundle.LoadAllAssets();
            yield return loadAsset;

            foreach (var asset in loadAsset)
            {
                Instantiate(asset, parent.transform);
            }

        }
    }

    //set tag to parents tag
    //get objects by tag put them into sstruct
    //update changes to assettbundle upload



    public List<string> SaveBlocks(string layerName = "none")
    {
        layerName = authMSc.userData.name;
        GameObject[] blocks = GameObject.FindGameObjectsWithTag(layerName);
        List<string> upBlocks = new List<string>(new string[blocks.Length]);//new LayerItem[blocks.Length];
        //int i = 0;
        Debug.Log("Number of cubes: " + blocks.Length);
        for (int i = 0; i < blocks.Length; i++)
        {
            var postLayerI = new LayerItem();

            postLayerI.name = layerName; //tag?
            postLayerI.objectType = "Cube";
            postLayerI.transform = blocks[i].transform;
            postLayerI.color = blocks[i].GetComponentInChildren<Renderer>().material.color;

            Destroy(blocks[i].gameObject);
            upBlocks[i]= JsonUtility.ToJson(postLayerI);
            Debug.Log("layeriteminpost: " + upBlocks[i]);
        }

        return upBlocks;
    }

    public void LayerToServer(string layerName = "demo")
    {
        layerName = authMSc.userData.name;
        List<string> doneModelArray = SaveBlocks(layerName);
        var jlayer = JsonUtility.ToJson(doneModelArray);
        var upProjectItem = new Project
        {
            name = "Demo",
            start = "2024",
            finish = "2024.07.04.",
            layername = layerName,
            model = jlayer
        };

        var jProjectItem = JsonUtility.ToJson(upProjectItem);
        Debug.Log("Posting ProjectItem+ " + jProjectItem);
        StartCoroutine(contactService.PostData_Coroutine(jProjectItem, "http://localhost:3000/projects"));
    }

}

   
