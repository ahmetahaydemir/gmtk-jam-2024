using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    public TextMeshProUGUI depthText;
    //
    public void UpdateInterface(float waterDepth)
    {
        depthText.text = string.Format("{0:F1} M", waterDepth);
    }
}
