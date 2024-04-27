using Newtonsoft.Json;
//using SerializableCallback;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
//using static UnityEditor.Experimental.GraphView.GraphView;

public class Build : MonoBehaviour
{
    public Transform origin;
    public Transform buildingBlock;
    public bool isInBuildMode = false; 
    public InputDevice inputDevice;
    bool triggerValue;
    public GameObject savedContent;
    public GameObject oneContent;
    public GameObject savedUI;
    public GameObject listUI;
    public TMP_Text layerTitleText;
    public XRRayInteractor interactor;
    public Button listItem;
    LayerLoader loaderSc;
    public XRInteractionManager xRManager;
    NotesManager notesManager;
    ContactService contactService;
    authManager authMSc;
    IXRSelectInteractable interActeble;
    RaycastHit intHit;

    [HideInInspector]
    public GameObject selectedGo;
    private GameObject hoverGo;



    // Start is called before the first frame update
    void Start()
    {
        loaderSc = GameObject.Find("Building").GetComponent<LayerLoader>();
        contactService = GameObject.Find("AuthManager").GetComponent<ContactService>();
        //xRManager= GetComponent<XRInteractionManager>();
        authMSc = GameObject.Find("AuthManager").GetComponent<authManager>();
        
        notesManager = GameObject.Find("NotesUIDocker").GetComponent<NotesManager>();


    }

    // Update is called once per frame
    void Update()
    {

        
        
        if (isInBuildMode)
        {
            var leftHandDevices = new List<UnityEngine.XR.InputDevice>();
            UnityEngine.XR.InputDevices.GetDevicesAtXRNode(UnityEngine.XR.XRNode.LeftHand , leftHandDevices);
            if (leftHandDevices.Count > 0)
            {
                Debug.Log("I found a left hand device.");
                UnityEngine.XR.InputDevice device = leftHandDevices[0];

                if (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.gripButton, out triggerValue) && triggerValue)
                {
                    if (!device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.gripButton, out triggerValue) && !triggerValue)
                    {
                        BuildBlocks();
                        Debug.Log("Trigger button is released.");
                    }

                }
            }
        } 
        else if (!isInBuildMode && interactor.TryGetCurrent3DRaycastHit(out intHit))
        {
            if (selectedGo != intHit.transform.gameObject)
            {
                if (hoverGo != intHit.transform.gameObject)
                {
                    if(hoverGo != null)
                    hoverGo.transform.gameObject.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", Color.white * 0.0f);

                    hoverGo = intHit.transform.gameObject;
                    hoverGo.GetComponent<MeshRenderer>().material.EnableKeyword("_EMISSION");
                    hoverGo.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", Color.white * 0.3f);
                }
            }
            
            if (interactor.isSelectActive)
            {
                if(selectedGo!= intHit.transform.gameObject)
                {
                   if( selectedGo != null)
                    { selectedGo.transform.gameObject.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", Color.white * 0.0f); }
                    selectedGo = intHit.transform.gameObject;
                    selectedGo.transform.gameObject.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", Color.cyan * 0.4f);
                    if(notesManager.gOname)
                    notesManager.gOname.GetComponentInChildren<TMP_Text>().text= selectedGo.transform.gameObject.name;
                  
                    //pos
                    //put le notes in nmanasger
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
        StartCoroutine(contactService.GetRequest("http://localhost:3000/projects/byname/Demo"));///+projectNameInput.text));
        
    }

    public void ToConsole(List<Project> layers)
    {
        
        foreach (var layer in layers)
        {
            Debug.Log("Layers: "+layer.layername);
            var nN = Instantiate(listItem, savedContent.transform);
           
            nN.transform.GetComponentInChildren<TMP_Text>().text = layer.layername;
            nN.gameObject.name = layer.model;
            //nN.gameObject.AddComponent<Button>();
            nN.onClick.AddListener(() => LoadingLayer(layer.layername, layer.model, nN));
            //ToConsole(note.ToString());
        }
    }


    public void LoadingLayer(string layerName, string layerModel, Button button)
    {

        if(button.transform.GetComponent<Image>().color==Color.white)
        {
            button.transform.GetComponent<Image>().color = Color.green;
            //create tag
            //or own dictionary?
            //create basic tags
            if (DoesTagExist(layerName))
            {
                loaderSc.LayerJsonToLayerBegin(layerName, layerModel);
                
            }
        }
        else
        {
            button.transform.GetComponent<Image>().color = Color.white;
            //check for tag if tag doesnt exist, dont bother
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
                
               var cube= Instantiate(buildingBlock, new Vector3(intHit.point.x, intHit.point.y+0.5f,intHit.point.z), Quaternion.identity,loaderSc.userParentObject.transform);
                cube.tag=authMSc.userData.name;
            }
        }
      
    }

    /// blockUIOnInteractableSelection
    //Enabling this option will block UI interaction when selecting interactables.
    /// currentNearestValidTarget
    /// hitClosestOnly
    /// isSelectActive
    /// rayEndPoint /rayEndTransform
    /// GetValidTargets(List<IXRInteractable>)
    /// OnSelectEntering(SelectEnterEventArgs)
    /// TryGetCurrent3DRaycastHit(out RaycastHit)
    /// TryGetHitInfo(out Vector3, out Vector3, out int, out bool)
    /// Interface IXRSelectInteractor.hasSelection
    /// OnSelectEntered(SelectEnterEventArgs)
    /// selectActionTrigger
    ///  QueryTriggerInteraction.collide
    ///  
    /// IXRSelectInteractable.isSelected
    /// IsSelectableBy(IXRSelectInteractor)
    /// 
    /// 
    /// 
    /// <returns></returns>

    public string GetProjectNameByID(string id)
    {
        StartCoroutine(contactService.GetRequest("http://localhost:3000/projects/:"+id));///+projectNameInput.text));


        return id;
    }

    public string IDtoName(string json)
    { 
        var project = JsonConvert.DeserializeObject<Project>(json);
        return project.name; 
    }

}
