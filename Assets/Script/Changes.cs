using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Changes on the Building parts, right now only the colors can be changed
/// Called: LayerLoader.cs in LayerInfoToLayer()
/// </summary>
public class Changes : MonoBehaviour
{
    //Original data
    [HideInInspector]
    public Color ogColor;
    [HideInInspector]
    public Material ogMaterial;
    [HideInInspector]
    public Transform ogTransform;

    // Start is called before the first frame update
    void Start()
    {
        ogColor=gameObject.GetComponent<Renderer>().material.color;
        ogMaterial = this.gameObject.GetComponent<Material>();
        ogTransform = this.gameObject.transform;
    }

   public void ChangeColor()
    {
        if(gameObject.GetComponent<Renderer>().material.color == ogColor)
        {
            gameObject.GetComponent<Renderer>().material.color = Color.green;
        }
        else if (gameObject.GetComponent<Renderer>().material.color == Color.green)
        {
            gameObject.GetComponent<Renderer>().material.color = Color.yellow;
        }
        else if (gameObject.GetComponent<Renderer>().material.color == Color.yellow)
        {
            gameObject.GetComponent<Renderer>().material.color = Color.red;
        }
        else if (gameObject.GetComponent<Renderer>().material.color == Color.red)
        {
            gameObject.GetComponent<Renderer>().material.color = Color.white;
            Color changeA = gameObject.GetComponent<Renderer>().material.color;
            changeA.a = 0.4f; //this is how you can change the alpha and then set it
            gameObject.GetComponent<Renderer>().material.color = changeA;

        }
        else if (gameObject.GetComponent<Renderer>().material.color == Color.white)
        {
            gameObject.GetComponent<Renderer>().material.color = ogColor;
        }
    }
}
