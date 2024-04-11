using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Networking;

public class LayerLoader : MonoBehaviour
{
    [System.Serializable]
    public  struct LayerItem
    {
        public string objectType;
        public string name;
        public Transform transform;
        public Color color;
    }

    
    public GameObject[] prefabs;
    public GameObject userParentObject = new GameObject();
    ContactService contactService;

    //url is a different thing, its an assetbundle, and a url instead of LayerItem

    private void Start()
    {
        userParentObject.name = "userID";
        contactService = new ContactService();
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
        else
        {
            StartCoroutine(GetAssetBundle(parentObject));
        }
    }

    public void LayerInfoToLayer(LayerItem layerItem, GameObject parentObject, string layerName)
    {
        GameObject item=null;
        if(layerItem.objectType == "cube")
        {
            item= GameObject.CreatePrimitive(PrimitiveType.Cube);
            item.name=layerItem.name;
        }
        else if(prefabs.ToString().Contains(layerItem.objectType))
        {
            for(int i = 0; i < prefabs.Length; i++)
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

    public LayerItem[] SaveBlocks(string layerName)
    {
       GameObject[] blocks= GameObject.FindGameObjectsWithTag(layerName);
        LayerItem[] upBlocks=new LayerItem[blocks.Length];
        for(int i=0;i<blocks.Length;i++)
        {
            upBlocks[i].objectType = "cube";//right now it's always a cube, but it could be set with a button
            upBlocks[i].name= layerName; //tag?
            upBlocks[i].transform= blocks[i].transform;
            upBlocks[i].color = blocks[i].GetComponent<Renderer>().material.color;
        }

        return upBlocks;
    }

    public void LayerToServer(string layerName)
    {
        LayerItem[] doneModelArray = SaveBlocks(layerName);
        var jnote = JsonUtility.ToJson(doneModelArray);
        StartCoroutine(contactService.PostData_Coroutine(jnote, "http://localhost:3000/projects"));
    }
}
