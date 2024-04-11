using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Build : MonoBehaviour
{
    public Transform origin;
    public Transform buildingBlock;
    public bool isInBuildMode = false; 
    public InputDevice inputDevice;
    bool triggerValue;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isInBuildMode)
        {
            var leftHandDevices = new List<UnityEngine.XR.InputDevice>();
            UnityEngine.XR.InputDevices.GetDevicesAtXRNode(UnityEngine.XR.XRNode.LeftHand, leftHandDevices);
            if (leftHandDevices.Count > 0)
            {
                UnityEngine.XR.InputDevice device = leftHandDevices[0];
                
                if (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out triggerValue) && triggerValue)
                {
                    BuildBlocks();
                    Debug.Log("Trigger button is pressed.");
                }
            }
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
                Instantiate(buildingBlock, new Vector3(hit.point.x,hit.point.y+buildingBlock.localScale.y,hit.point.z), Quaternion.identity);
            }
        }
      
    }
}
