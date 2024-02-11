using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Tile_",menuName ="Create Tile")]
public class TileSO : ScriptableObject
{
    public Color32 BackgroundColor;
    public Color32 ForeColor;
    public int Value;
    public int id;
}
