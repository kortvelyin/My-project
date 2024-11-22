using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpNoteRotation : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        this.gameObject.transform.LookAt(Camera.main.transform.position);
    }
}
