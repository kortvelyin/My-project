using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Changes : MonoBehaviour
{
    //Original data
    private Color ogColor;
    private Material ogMaterial;
    private Transform ogTransform;

    // Start is called before the first frame update
    void Start()
    {
        ogColor=this.gameObject.GetComponent<Material>().color;
        ogMaterial = this.gameObject.GetComponent<Material>();
        ogTransform = this.gameObject.transform;
    }

   public void ChangeColor()
    {
        if(gameObject.GetComponent<Material>().color==ogColor)
        {
            gameObject.GetComponent<Material>().color = Color.green;
        }
        else if (gameObject.GetComponent<Material>().color == Color.green)
        {
            gameObject.GetComponent<Material>().color = Color.yellow;
        }
        else if (gameObject.GetComponent<Material>().color == Color.yellow)
        {
            gameObject.GetComponent<Material>().color = Color.red;
        }
        else if (gameObject.GetComponent<Material>().color == Color.red)
        {
            gameObject.GetComponent<Material>().color = Color.clear;
        }
        else if (gameObject.GetComponent<Material>().color == Color.clear)
        {
            gameObject.GetComponent<Material>().color =ogColor;
        }
    }
}
