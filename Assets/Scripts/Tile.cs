using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    private TileSO _prop;
    public TileSO prop
    {
        get { return _prop; }
        set { _prop = value;DrawTile(); }
    }
    private TextMeshProUGUI ValueText;

    public void DrawTile()
    {
        if (prop == null)
            Destroy(gameObject);
        else
        {     
            ValueText = GetComponentInChildren<TextMeshProUGUI>();
            gameObject.GetComponent<Image>().color = prop.BackgroundColor;
            ValueText.text = prop.Value.ToString();
            ValueText.color = prop.ForeColor;
        }       
    }
}
