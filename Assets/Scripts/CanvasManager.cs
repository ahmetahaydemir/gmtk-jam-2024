using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    public TextMeshProUGUI depthText;
    public TextMeshProUGUI sizeText;
    public RectTransform sizeFillRect;
    public RectTransform sizeBgRect;
    //
    public void UpdateInterface(float waterDepth, float playerSize)
    {
        depthText.text = string.Format("{0:F1} M", waterDepth);
        sizeText.text = string.Format("{0:F1} KG", playerSize);
    }
}