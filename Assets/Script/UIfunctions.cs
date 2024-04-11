using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIfunctions : MonoBehaviour
{
    public GameObject gObject;

    public void ToggleGO(GameObject go)
    {
        go.SetActive(!go.activeSelf);
    }
}
