using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Unity.VisualScripting;
using UnityEngine.XR.Interaction.Toolkit;
using System.Linq;

/// <summary>
/// Main functions to the Build.cs
/// Functions to convert layer json data to models
/// and back
/// LayerItem class
/// LayerJsonToLayerBegin(string layerName, string layer)
/// LayerInfoToLayer(string model, GameObject parentObject, string layerName)
/// TransformStringFromData(Transform transform)
/// GetAssetBundle(GameObject parent)
///  SaveBlocks(string layerName = "Cube")
///  LayerToServer(string layerName = "demo")
///  ObjectColorModeToggle(Button button)
/// </summary>


[Serializable]
public class LayerItem
{
    public string objectType="";
    public string name="";
    public string transform=null;
    public Color color=new Color();
}
public class LayerLoader : MonoBehaviour
{


    public List<GameObject> prefabs;
    List<GameObject> loadedObjects = new List<GameObject>();
    public GameObject userParentObject;
    ContactService contactService;
    authManager authMSc;
    DwnAssetBundle assetB;
    Build buildSc;
    public bool isInColorMode;
    public TMP_Text layerTitleText;



    private void Start()
    {
        buildSc = GameObject.Find("Building").GetComponent<Build>();
        
        userParentObject = new GameObject();
        userParentObject.name = "userID";
        contactService = GameObject.Find("AuthManager").GetComponent<ContactService>();
        authMSc = GameObject.Find("AuthManager").GetComponent<authManager>();
        assetB= GameObject.Find("AuthManager").GetComponent<DwnAssetBundle>();
    }



    public GameObject LayerJsonToLayerBegin(string layerName, string layer)
    {
        var parentObject = new GameObject(layerName);
        
        parentObject.transform.parent = userParentObject.transform;
        if (layer.Contains("layeritem"))
        {
            Debug.Log("it is indeed full of layeritems");
            
            
            LayerInfoToLayer(layer, parentObject, layerName);
        }
        else if (layer.Contains("htt"))
        {
            StartCoroutine(GetAssetBundle(layer, parentObject, layerName));
        }
        else if(layer.Contains("Item"))//with jsonHelper
        {
            Debug.Log("it is indeed full of items: "+ layer);
            
            if(layer.Contains("objectType"))
                loadedObjects = LayerInfoToLayer(layer, parentObject, layerName);
        }
        else
        {
            Debug.Log("Couldn't find layer type");
        }
        return parentObject;

    }

    public List<GameObject> LayerInfoToLayer(string model, GameObject parentObject, string layerName)
    {
        List<GameObject> loadedObjects = new List<GameObject>();
        var layerItemList = JsonHelper.FromJson<string>(model);
       
        GameObject item = null;
        foreach (var layerItem in layerItemList)
        {
            LayerItem lItem = JsonUtility.FromJson<LayerItem>(layerItem);

           
            for (int i = 0; i < prefabs.Count; i++)
            {
                if (lItem.objectType.Contains(prefabs[i].name))
                {
                    item = Instantiate(prefabs[i]);
                }
            }
            loadedObjects.Add(item);
            item.tag = layerName;
            parentObject.tag = layerName; //so that this gets destroyed too
            item.name = lItem.objectType;
            item.transform.parent = parentObject.transform;
            var transfromArray= JsonHelper.FromJson<String>(lItem.transform);

            item.transform.position = JsonUtility.FromJson<Vector3>(transfromArray[0]);
            item.transform.rotation = JsonUtility.FromJson<Quaternion>(transfromArray[1]);
            item.transform.localScale = JsonUtility.FromJson<Vector3>(transfromArray[2]);
            if (item.GetComponentInChildren<MeshRenderer>())
            {
                if (item.GetComponentInChildren<MeshRenderer>())
                    item.AddComponent<Changes>();//.ogMaterial = item.GetComponent<Renderer>().material;
                        
                item.GetComponentInChildren<Changes>().gotColor = lItem.color;
                item.GetComponentInChildren<Changes>().StartChanges();

            }
                    
                   
                
            
            
        }
                if(item == null) 
                {
                     contactService.commCube.GetComponent<Renderer>().material.color = Color.red;
                     Debug.Log("couldnt find match in layer recreation ");
                }
        return loadedObjects;
    }


    public string TransformStringFromData(Transform transform)
    {
        string[] transformArray =new string[3];
        transformArray[0]= JsonUtility.ToJson(transform.position);
        transformArray[1] = JsonUtility.ToJson(transform.rotation);
        transformArray[2] = JsonUtility.ToJson(transform.lossyScale);

        return JsonHelper.ToJson(transformArray);
    }
    public IEnumerator GetAssetBundle(string model, GameObject parentObject, string layerName)
    {
        loadedObjects = new List<GameObject>();
        var layerItemList = JsonHelper.FromJson<string>(model);
        GameObject item = null;
        string driveurl = "https://drive.usercontent.google.com/u/0/uc?id=1IZxxjHIlJIcHO2p35wjQbT8iMRGPr9rP&export=download";



        foreach (var layerItem in layerItemList)
        {
            LayerItem lItem = JsonUtility.FromJson<LayerItem>(layerItem);
            lItem.objectType=lItem.objectType.Replace("_", "/");
            Debug.Log("URL: " + lItem.objectType);
            using (UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(lItem.objectType, 0))
            {
                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log("assetbundle problem" + www.error);
                }
                else
                {
                    AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(www);

                    
                    GameObject go = bundle.LoadAsset(bundle.GetAllAssetNames()[0]) as GameObject;
                    item = Instantiate(go);
                    loadedObjects.Add(item);

                    item.tag = layerName;
                    parentObject.tag = layerName; //so that this gets destroyed too
                    item.name = lItem.objectType;
                    item.transform.parent = parentObject.transform;
                    var transfromArray = JsonHelper.FromJson<String>(lItem.transform);
                    item.transform.position = JsonUtility.FromJson<Vector3>(transfromArray[0]);
                    item.transform.rotation = JsonUtility.FromJson<Quaternion>(transfromArray[1]);
                    item.transform.localScale = JsonUtility.FromJson<Vector3>(transfromArray[2]);
                    for (int i = 0; i<item.transform.childCount; i++)
                    {
                        if(item.transform.GetChild(i).GetComponent<Renderer>())
                        {
                            var materials = item.transform.GetChild(i).GetComponent<Renderer>().materials;
                            foreach(var material in materials)
                            {
                                material.shader= Shader.Find("Universal Render Pipeline/Lit");
                            }
                            item.transform.GetChild(i).AddComponent<BoxCollider>().isTrigger = true;
                            item.transform.GetChild(i).AddComponent<Changes>();//.ogMaterial = item.GetComponent<Renderer>().material;

                            item.transform.GetChild(i).GetComponent<Changes>().gotColor = lItem.color;
                            item.transform.GetChild(i).GetComponent<Changes>().StartChanges();
                        }
                    }
                    


                    //item.GetComponentInChildren<Renderer>().material.shader = Shader.Find("Universal Render Pipeline/Lit");
                    //item.transform.GetChild(0).AddComponent<BoxCollider>().isTrigger = true;
                    
                    
                    bundle.Unload(false);
                    yield return new WaitForEndOfFrame();
                }
                www.Dispose();

            }
        }

        

    }





    public string SaveBlocks(string layerName = "Cube")
    {
        GameObject[] blocks = GameObject.FindGameObjectsWithTag(layerName);
        if (blocks.Length == 0)
            return 0.ToString();
        string[] upBlocks = new string[blocks.Length];
        
        for (int i = 0; i < blocks.Length; i++)
        {
            var postLayerI = new LayerItem();
            postLayerI.name = layerName;
            postLayerI.objectType = blocks[i].name.Replace('/', '_');
            postLayerI.transform = TransformStringFromData(blocks[i].transform);
            postLayerI.color = blocks[i].GetComponentInChildren<Renderer>().material.color;
            Destroy(blocks[i].gameObject);
            upBlocks[i]= JsonUtility.ToJson(postLayerI);
        }
        
        var postStrinsArr=JsonHelper.ToJson(upBlocks);
            return postStrinsArr;
    }

    public void LayerToServer(string layerName = "demo")
    {
        layerName = layerTitleText.text;
        var doneModelArray = SaveBlocks(layerName);
        if (doneModelArray == 0.ToString())
            return;
        var upProjectItem = new Project
        {
            name = "Demo",
            start = System.DateTime.Now.ToString(),
            finish = "2024.07.04.",
            layername = layerName,
            model = doneModelArray
        };

        var jProjectItem = JsonUtility.ToJson(upProjectItem);
        StartCoroutine(contactService.PostData_Coroutine(jProjectItem, "http://"+authMSc.ipAddress+":3000/projects"));
    }


    public void ObjectColorModeToggle(Button button)
    {
        isInColorMode = !isInColorMode;
        if (isInColorMode)
        {
            button.GetComponent<Image>().color = new Color(0.3f, 0.6f, 0.6f);
        }
        else
            button.GetComponent<Image>().color = Color.white;

    }

}

   
