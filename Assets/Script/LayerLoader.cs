using System.Collections;
using System.Collections.Generic;
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

    //url is a different thing, its an assetbundle, and a url instead of LayerItem

    public void LayerJsonToLayerBegin(string layerName, string layer)
    {
        var parentObject = new GameObject(layerName);
        if (layer.Contains("LayerItem"))
        {
            LayerItem layerItem = JsonUtility.FromJson<LayerItem>(layer);
            LayerInfoToLayer(layerItem, parentObject);
        }
        else
        {
            StartCoroutine(GetAssetBundle(parentObject));
        }
    }

    public void LayerInfoToLayer(LayerItem layerItem, GameObject parentObject)
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
}
