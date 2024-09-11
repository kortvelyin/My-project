using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;


public class Build : MonoBehaviour
{
    public Transform origin;
    public Transform buildingBlock;
    public bool isInBuildMode = false; 
    public InputDevice inputDevice;
    bool singlePress = true;
    public GameObject savedContent;
    
    public GameObject savedUI;
    public GameObject listUI;
    
    public XRRayInteractor interactor;
    public Button listItem;
    LayerLoader loaderSc;
    public XRInteractionManager xRManager;
    NotesManager notesManager;
    ContactService contactService;
    authManager authMSc;
    RaycastHit intHit;

    [HideInInspector]
    public GameObject selectedGo;
    private GameObject hoverGo;
    

/// <summary>
/// Handling all controller button press
/// Spawning building blocks
/// Changing color
/// Choosing GameObject
/// 
/// Handling Layers UI
/// Calling layer list
/// Calling Loading Layer
/// </summary>
    void Start()
    {
        loaderSc = GameObject.Find("Building").GetComponent<LayerLoader>();
        contactService = GameObject.Find("AuthManager").GetComponent<ContactService>();
        //xRManager= GetComponent<XRInteractionManager>();
        authMSc = GameObject.Find("AuthManager").GetComponent<authManager>();
        
        notesManager = GameObject.Find("NotesUIDocker").GetComponent<NotesManager>();

        selectedGo = new GameObject();
        hoverGo = new GameObject();
        
    }

    // Update is called once per frame
    void Update()
    {



        if (isInBuildMode)
        {
            var leftHandDevices = new List<UnityEngine.XR.InputDevice>();
            UnityEngine.XR.InputDevices.GetDevicesAtXRNode(UnityEngine.XR.XRNode.LeftHand, leftHandDevices);
            if (leftHandDevices.Count > 0)
            {
                //Debug.Log("I found a left hand device.");
                UnityEngine.XR.InputDevice device = leftHandDevices[0];

                InputHelpers.IsPressed(device, InputHelpers.Button.TriggerButton, out bool isPressed, -1f);
                if (isPressed && singlePress)
                {
                    BuildBlocks();
                    Debug.Log("triggeris pressed");
                    singlePress = false;
                    // BuildBlocks();


                }
                if (!isPressed)
                {
                    singlePress = true;
                }
            }
        }
        else if (interactor.TryGetCurrent3DRaycastHit(out intHit))
        {
            if (!interactor.isSelectActive&&selectedGo != intHit.transform.gameObject)
            {
                // Debug.Log("raycast hit in hover I");
                if (hoverGo != intHit.transform.gameObject && intHit.transform.gameObject.GetComponent<MeshRenderer>())
                {
                    // Debug.Log("raycast hit in hover II");
                    if (hoverGo.GetComponent<MeshRenderer>())
                    {
                        hoverGo.transform.gameObject.GetComponent<MeshRenderer>().material.EnableKeyword("_EMISSION");
                        hoverGo.transform.gameObject.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", Color.white * 0.0f);
                    }

                    hoverGo = intHit.transform.gameObject;
                    hoverGo.transform.gameObject.GetComponent<MeshRenderer>().material.EnableKeyword("_EMISSION");
                    hoverGo.transform.gameObject.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", Color.white * 0.3f);
                }
            }
            if (interactor.isSelectActive)
            {
                if (selectedGo != null || selectedGo != intHit.transform.gameObject)
                {
                    if (selectedGo.GetComponent<MeshRenderer>())
                    {
                        selectedGo.transform.gameObject.GetComponent<MeshRenderer>().material.EnableKeyword("_EMISSION");
                        selectedGo.transform.gameObject.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", Color.white * 0.0f);
                    }
                    selectedGo = intHit.transform.gameObject;
                    if (selectedGo.transform.gameObject.GetComponent<MeshRenderer>())
                    {
                        selectedGo.transform.gameObject.GetComponent<MeshRenderer>().material.EnableKeyword("_EMISSION");
                        selectedGo.transform.gameObject.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", Color.cyan * 0.4f);
                    }
                    if (loaderSc.isInColorMode)
                    {

                        if ((selectedGo.GetComponent<Renderer>()))
                        {
                            if (selectedGo.GetComponent<Changes>())
                            {
                                selectedGo.GetComponent<Changes>().ChangeColor();
                            }
                            else
                            {
                                selectedGo.AddComponent<Changes>().ChangeColor();
                                selectedGo.GetComponentInChildren<Changes>().ChangeColor();
                            }
                        }
                        else
                        {
                            if ((selectedGo.GetComponentInChildren<Renderer>()))
                            {
                                if (selectedGo.GetComponentInChildren<Changes>())
                                    selectedGo.GetComponentInChildren<Changes>().ChangeColor();
                                else
                                {
                                    selectedGo.transform.GetChild(1).AddComponent<Changes>().ChangeColor();
                                }
                            }

                        }
                    }

                    if (notesManager.gOname)
                        notesManager.gOname.GetComponentInChildren<TMP_Text>().text = selectedGo.transform.gameObject.name;
                    notesManager.gOpos.GetComponentInChildren<TMP_Text>().text = selectedGo.transform.position.ToString();
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
        StartCoroutine(contactService.GetRequest("http://"+authMSc.ipAddress+":3000/projects/byname/Demo"));///+projectNameInput.text));
        
    }

    public void ToConsole(List<Project> layers)
    {
        
        foreach (var layer in layers)
        {
            if (layer.model.Contains("objectType"))
            {
                Debug.Log("Layersmodel: "+layer.model);
                var nN = Instantiate(listItem, savedContent.transform);
                nN.transform.SetSiblingIndex(0);
                nN.transform.GetComponentInChildren<TMP_Text>().text = layer.layername + " " + layer.start;
                nN.gameObject.name = layer.model;
                nN.gameObject.AddComponent<LoadLayer>().data = layer.model;
                nN.gameObject.GetComponent<LoadLayer>().data2 = layer.layername;
                nN.gameObject.GetComponent<LoadLayer>().btn = nN;
                nN.onClick.AddListener(() => nN.gameObject.GetComponent<LoadLayer>().Loading());
            }
        }
    }


    public void LoadingLayer(string layerName, string layerModel, Button button)
    {
            Debug.Log("LayerName: "+layerName+" Model: "+layerModel+" Button: "+button.name);
        if (button.transform.GetComponent<Image>().color==Color.white)
        {
        button.transform.GetComponent<Image>().color = Color.green;
            if (DoesTagExist(layerName))
            {
                loaderSc.LayerJsonToLayerBegin(layerName, layerModel);
            }
        }
        else
        {
            button.transform.GetComponent<Image>().color = Color.white;
            if (DoesTagExist(layerName))
            {
                GameObject[] blocks = GameObject.FindGameObjectsWithTag(layerName);
                foreach (var block in blocks)
                {
                    Destroy(block.gameObject);
                }
            }
        }
    }

    public void Clear()
    {
        string layerName = loaderSc.layerTitleText.text;
        if (DoesTagExist(layerName))
        {
            GameObject[] blocks = GameObject.FindGameObjectsWithTag(layerName);
            foreach (var block in blocks)
            {
                Destroy(block.gameObject);
            }
        }
    }

    public static bool DoesTagExist(string aTag)
    {
        try
        {
            GameObject.FindGameObjectsWithTag(aTag);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public void BuildModeToggle(Button button)
    {
        isInBuildMode = !isInBuildMode;
        if (isInBuildMode)
        {
            button.GetComponent<Image>().color = new Color(0.3f, 0.6f, 0.6f);
        }
        else
            button.GetComponent<Image>().color = Color.white;

    }
    public void BuildBlocks()
    {
        if (isInBuildMode)
        {
            if (interactor.TryGetCurrent3DRaycastHit(out intHit))
            {
                Debug.Log("Cube position: " + intHit.point.ToString());
               var cube= Instantiate(buildingBlock, intHit.point, Quaternion.identity,loaderSc.userParentObject.transform);
                cube.tag="Cube";
            }
        }
    }

  

    public string GetProjectNameByID(string id)
    {
        StartCoroutine(contactService.GetRequest("http://"+authMSc.ipAddress+":3000/projects/:" + id));///+projectNameInput.text));


        return id;
    }

    public string IDtoName(string json)
    { 
        var project = JsonConvert.DeserializeObject<Project>(json);
        return project.name; 
    }

}
