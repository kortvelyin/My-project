using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Build : MonoBehaviour
{
    public Transform origin;
    public Transform buildingBlock;
    public bool isInBuildMode = false; 
    public InputDevice inputDevice;
    bool triggerValue;
    public GameObject savedContent;
    public Button listItem;
    LayerLoader loaderSc;

    ContactService contactService;
    authManager authMSc;
    public TMP_Text projectNameInput;
    // Start is called before the first frame update
    void Start()
    {
        loaderSc = GameObject.Find("Building").GetComponent<LayerLoader>();
        contactService = GameObject.Find("AuthManager").GetComponent<ContactService>();
        authMSc = GameObject.Find("AuthManager").GetComponent<authManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(isInBuildMode)
        {
            var leftHandDevices = new List<UnityEngine.XR.InputDevice>();
            UnityEngine.XR.InputDevices.GetDevicesAtXRNode(UnityEngine.XR.XRNode.LeftHand , leftHandDevices);
            if (leftHandDevices.Count > 0)
            {
                UnityEngine.XR.InputDevice device = leftHandDevices[0];
                
                if (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.gripButton, out triggerValue) && triggerValue)
                {
                    if(!device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.gripButton, out triggerValue) && !triggerValue)
                    {
                    BuildBlocks();
                    Debug.Log("Trigger button is released.");
                    }
                    
                }
            }
        }
        
    }

    public void OnLayerListDis()
    {
        for (var i = savedContent.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(savedContent.transform.GetChild(i).gameObject);
        }
    }

    public void OnLayerEn()
    {
        GetLayerListByName();
    }

    public void GetLayerListByName()
    {
        StartCoroutine(contactService.GetRequest("http://localhost:3000/projects/byname/Test"));///+projectNameInput.text));

    }

    public void ToConsole(List<Project> layers)
    {
        
        foreach (var layer in layers)
        {
            Debug.Log("Layers: " + layer.name + " : " + layer.layername);
            var nN = Instantiate(listItem, savedContent.transform);
           
            nN.transform.GetComponentInChildren<TMP_Text>().text = layer.name+" : "+layer.layername;
            nN.gameObject.name = layer.model;
            //nN.gameObject.AddComponent<Button>();
            nN.onClick.AddListener(() => loaderSc.LayerJsonToLayerBegin(layer.layername, layer.model));
            //ToConsole(note.ToString());
        }
    }

    
    public void BuildModeToggle()
    {
        isInBuildMode = !isInBuildMode; 
    }
    public void BuildBlocks()
    {if(isInBuildMode)
        {
            RaycastHit hit;
            if (Physics.Raycast(origin.position, origin.forward, out hit))
            {
               var cube= Instantiate(buildingBlock, new Vector3(hit.point.x,hit.point.y+0.5f,hit.point.z), Quaternion.identity,loaderSc.userParentObject.transform);
                cube.tag=authMSc.userData.name;
            }
        }
      
    }
}
