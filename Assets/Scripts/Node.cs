using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuildingType 
{
    Free,
    House,
    Shop,
    Wall,
    Entrance

}



public class Node : MonoBehaviour
{
    private int x;
    private int y;

    public float val;
    public int active;

    public BuildingType buildingType;

    public Node[] neighbors = new Node[4];

    private void Start()
    {

    }

    public void SetMat()
    {
        if (active == 1)
        {
            gameObject.GetComponent<Renderer>().material.color = Color.black;

        }
        else
        {
            gameObject.GetComponent<Renderer>().material.color = Color.white;

        }
    }

    public void SetMat(Color color)
    {
        gameObject.GetComponent<Renderer>().material.color = color;

    }
}
