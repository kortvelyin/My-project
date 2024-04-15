using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Networking;


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
        if (layer.Contains("LayerItem"))
        {
            LayerItem layerItem = JsonUtility.FromJson<LayerItem>(layer);
            LayerInfoToLayer(layerItem, parentObject,layerName);
        }
        else if(layer.Contains("htt"))
        {
            StartCoroutine(GetAssetBundle(parentObject));
        }
        else
        {
            //do nothing, change button color?
        }
                
                }

    public void LayerInfoToLayer(LayerItem layerItem, GameObject parentObject, string layerName)
    {
        GameObject item=null;
        if(layerItem.objectType == "Cube")
        {
            item= GameObject.CreatePrimitive(PrimitiveType.Cube);
            item.name=layerItem.name;
        }
        else if(prefabs.ToString().Contains(layerItem.objectType))
        {
            for(int i = 0; i < prefabs.Count; i++)
            {
                if (prefabs[i].name == layerItem.name)
                {
                    item = Instantiate(prefabs[i]);
                    break;
                }
            }
        }
        else
        {
            item = GameObject.CreatePrimitive(PrimitiveType.Cube);
            item.GetComponent<Renderer>().material.color=Color.red;
           
        }
        item.gameObject.tag= layerName;
        item.name = layerItem.name;
        item.transform.parent = parentObject.transform;
        item.transform.position = layerItem.transform.position;
        item.transform.rotation = layerItem.transform.rotation;
        item.transform.localScale = layerItem.transform.localScale;
        item.GetComponent<Renderer>().material.color = layerItem.color;
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

            foreach(var asset in loadAsset)
            {
                Instantiate(asset,parent.transform);
            }
            
        }
    }

    //set tag to parents tag
    //get objects by tag put them into sstruct
    //update changes to assettbundle upload


   
    public List<LayerItem> SaveBlocks(string layerName="none")
    {
        layerName = authMSc.userData.name;
       GameObject[] blocks= GameObject.FindGameObjectsWithTag(layerName);
        List<LayerItem> upBlocks = new List<LayerItem>(new LayerItem[blocks.Length]);//new LayerItem[blocks.Length];
        int i = 0;
        Debug.Log("Number of cubes: "+blocks.Length);
        foreach (var block in upBlocks)//(int i=0;i<blocks.Length;i++)
        {

            block.objectType = "Cube"; //Object reference is null                                                                                 //right now it's always a cube, but it will be set with a button
            block.name= layerName; //tag?
            block.transform= blocks[i].transform;
            block.color = blocks[i].GetComponentInChildren<Renderer>().material.color;
            Destroy(blocks[i].gameObject);
            i++;
        }

        return upBlocks;
    }

    public void LayerToServer(string layerName="demo")
    {
        layerName = authMSc.userData.name;
        List<LayerItem> doneModelArray = SaveBlocks(layerName);
        var jlayer = JsonUtility.ToJson(doneModelArray);
        var upProjectItem = new Project
        {
            name = "Test",
            start= "now",
            finish= "2024.02.24.",
            layername= layerName,
            model= jlayer
        };
        
        var jProjectItem = JsonUtility.ToJson(upProjectItem);
        Debug.Log("Posting ProjectItem+ " + jProjectItem);
        StartCoroutine(contactService.PostData_Coroutine(jProjectItem, "http://localhost:3000/projects"));
    }
}
