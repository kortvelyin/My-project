using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
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
    public TMP_Text changeText;
    public Color loadedButton;

    public XRRayInteractor interactor;
    public Button listItem;
    LayerLoader loaderSc;
    public XRInteractionManager xRManager;
    NotesManager notesManager;
    ContactService contactService;
    authManager authMSc;
    RaycastHit intHit;
    public int openModelCount = 0;

    [HideInInspector]
    public GameObject selectedGo=null;
    private GameObject hoverGo=null;


    /// <summary>
    /// Handling all controller button press
    /// Spawning building blocks
    /// Changing color
    /// Choosing GameObject
    /// 
    /// Handling Layers UI
    /// Calling layer list
    /// Clearing LayerList
    /// Calling Loading Layer
    /// GetProjectNameByID
    /// IDtoName
    /// </summary>
    void Start()
    {
        loaderSc = GameObject.Find("Building").GetComponent<LayerLoader>();
        contactService = GameObject.Find("AuthManager").GetComponent<ContactService>();
        authMSc = GameObject.Find("AuthManager").GetComponent<authManager>();
        notesManager = GameObject.Find("NotesUIDocker").GetComponent<NotesManager>();

        //selectedGo = new GameObject();
        //hoverGo = new GameObject();
        
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
                }
                if (!isPressed)
                {
                    singlePress = true;
                }
            }
        }
        if (interactor.TryGetCurrent3DRaycastHit(out intHit))
        {
            //Debug.Log("intHit taken");
            if (!interactor.isSelectActive && selectedGo != intHit.transform.gameObject)
            {
                // Debug.Log("raycast hit in hover I");
                if (hoverGo != intHit.transform.gameObject && intHit.transform.gameObject.GetComponentInChildren<MeshRenderer>())
                {
                    // Debug.Log("raycast hit in hover II");
                    if (hoverGo!=null &&hoverGo.GetComponentInChildren<MeshRenderer>()!=null)
                    {
                        hoverGo.transform.gameObject.GetComponentInChildren<MeshRenderer>().material.EnableKeyword("_EMISSION");
                        hoverGo.transform.gameObject.GetComponentInChildren<MeshRenderer>().material.SetColor("_EmissionColor", Color.white * 0.0f);
                    }

                    hoverGo = intHit.transform.gameObject;
                    hoverGo.transform.gameObject.GetComponentInChildren<MeshRenderer>().material.EnableKeyword("_EMISSION");
                    hoverGo.transform.gameObject.GetComponentInChildren<MeshRenderer>().material.SetColor("_EmissionColor", Color.white * 0.3f);
                }
            }
            if (interactor.isSelectActive)
            {
                Debug.Log("interactor active");
                if (selectedGo!=null&&selectedGo.GetComponentInChildren<MeshRenderer>())
                {
                    selectedGo.transform.gameObject.GetComponentInChildren<MeshRenderer>().material.EnableKeyword("_EMISSION");
                    selectedGo.transform.gameObject.GetComponentInChildren<MeshRenderer>().material.SetColor("_EmissionColor", Color.white * 0.0f);
                }
                selectedGo = intHit.transform.gameObject;
                if (selectedGo.transform.gameObject.GetComponentInChildren<MeshRenderer>())
                {
                    selectedGo.transform.gameObject.GetComponentInChildren<MeshRenderer>().material.EnableKeyword("_EMISSION");
                    selectedGo.transform.gameObject.GetComponentInChildren<MeshRenderer>().material.SetColor("_EmissionColor", Color.cyan * 0.4f);
                }
                if (loaderSc.isInColorMode)
                {
                    if(selectedGo.GetComponentInChildren<XRGrabInteractable>())
                    {
                        selectedGo.GetComponentInChildren<XRGrabInteractable>().enabled = false;
                    }

                    if (selectedGo.GetComponentInChildren<Renderer>())
                    {
                        if (selectedGo.GetComponentInChildren<Changes>())
                        {
                            selectedGo.GetComponentInChildren<Changes>().ChangeColor();
                        }
                    }
                        
                }
                if (notesManager.gOname)
                    notesManager.gOname.GetComponentInChildren<TMP_Text>().text = selectedGo.transform.gameObject.name;
                notesManager.gOpos.GetComponentInChildren<TMP_Text>().text = selectedGo.transform.position.ToString();
            }
            if (selectedGo!=null&&selectedGo.GetComponentInChildren<XRGrabInteractable>())
            {
                selectedGo.GetComponentInChildren<XRGrabInteractable>().enabled = true;
            }
        }
        


    }
    
    public void OnLayerListDis()
    {
        changeText.text = "Model List";
        for (var i = savedContent.transform.childCount - 1; i >= 0; i--)
        {
            if (savedContent.transform.GetChild(i).gameObject.GetComponent<Image>().color != Color.green)
                Destroy(savedContent.transform.GetChild(i).gameObject);
        }
    }

    public void OnLayerEn()
    {
        changeText.text = "Open Model";
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
                var nN = Instantiate(listItem, savedContent.transform);
                nN.transform.SetSiblingIndex(openModelCount);
                nN.transform.GetComponentInChildren<TMP_Text>().text = layer.layername + " " + layer.start;
                nN.gameObject.name = layer.model;
                nN.gameObject.AddComponent<LoadLayer>().data = layer.model;
                nN.gameObject.GetComponent<LoadLayer>().data2 = layer.layername;
                nN.gameObject.GetComponent<LoadLayer>().btn = nN;
                nN.onClick.AddListener(() => nN.gameObject.GetComponent<LoadLayer>().Loading());
                for(int i = 0; i<openModelCount; i++)
                {
                    if(savedContent.transform.GetChild(i).gameObject.GetComponentInChildren<TMP_Text>().text== nN.transform.GetComponentInChildren<TMP_Text>().text)
                    {
                        nN.transform.GetComponentInChildren<Image>().color = Color.gray;
                    }
                }
            }
        }
    }


    public void LoadingLayer(string layerName, string layerModel, Button button)
    {
        if (button.transform.GetComponent<Image>().color == Color.white || button.transform.GetComponent<Image>().color == loadedButton)
        {
            button.transform.GetComponent<Image>().color = Color.green;
            if (DoesTagExist(layerName))
            {
                var parentObject = loaderSc.LayerJsonToLayerBegin(layerName, layerModel);
                openModelCount++;
                button.GetComponent<LoadLayer>().loadedParent = parentObject;
                //button.onClick.AddListener(() => { UnloadLayer(loadedObjects); });
            }
        }
        else if(button.transform.GetComponent<Image>().color == Color.green)
        {
            button.transform.GetComponent<Image>().color = loadedButton;
            openModelCount--;
            Debug.Log("UnLoading was called");
            if (button.GetComponent<LoadLayer>().loadedParent)
            {
                foreach (var child in button.GetComponent<LoadLayer>().loadedParent.transform.GetComponentsInChildren<Transform>())
                {
                    Destroy(child.gameObject);
                }
                Destroy(button.GetComponent<LoadLayer>().loadedParent);
            }
        }
    }

    /*public void UnloadLayer(List<GameObject> objects)
    {
        openModelCount--;
        foreach (var obj in objects)
        {
            Destroy(obj.gameObject);
        }
    }*/

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
                cube.AddComponent<Changes>();
                cube.GetComponentInChildren<Changes>().gotColor = buildingBlock.GetComponentInChildren<MeshRenderer>().sharedMaterial.color;
                cube.GetComponentInChildren<Changes>().StartChanges();
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
