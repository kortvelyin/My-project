using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;
using Newtonsoft.Json;
using System;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using static UnityEditor.Experimental.GraphView.GraphView;

public class UIfunctions : MonoBehaviour
{
    public GameObject gObject;
    bool toggle=true;
    public void ToggleGO(GameObject go)
    {
        go.SetActive(!go.activeSelf);
    }

    public void ToggleColor(UnityEngine.UI.Button button)
    {
        if (toggle)
        {
            button.GetComponent<UnityEngine.UI.Image>().color = Color.white;
            toggle = !toggle;
        }
        else
        {
            button.GetComponent<UnityEngine.UI.Image>().color = Color.grey;
            toggle = !toggle;
        }
    }
}
