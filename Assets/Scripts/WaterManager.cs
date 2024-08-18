using StylizedWater2;
using UnityEngine;
public class WaterManager : MonoBehaviour
{
    public WaterObject water;
    public Transform waterContainer;
    public Transform waterBase;
    public Transform waterCover;
    //
    private Vector3 scaleCache;
    //
    public void UpdateWaterProgress(float time)
    {
        scaleCache = waterContainer.localScale;
        scaleCache.y = 1f + time * 0.05f;
        waterContainer.localScale = scaleCache;
    }
}