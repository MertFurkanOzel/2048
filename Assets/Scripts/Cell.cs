using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public int CoordX;
    public int CoordY;
    public Tile tile
    {
        get { return gameObject.GetComponentInChildren<Tile>(); }
        private set { }
    }
    public bool isEmpty=> tile == null;
    public bool isLocked=false;
}
