using DG.Tweening;
using StylizedWater2;
using UnityEngine;
public class WaterManager : MonoBehaviour
{
    public WaterObject water;
    public Transform waterContainer;
    public Transform waterBase;
    public Transform waterCover;
    //
    public Rigidbody[] initialSnakeRocks;
    //
    private Vector3 scaleCache;
    //
    public void UpdateWaterProgress(float time)
    {
        scaleCache = waterContainer.localScale;
        scaleCache.y = 1f + time * 0.05f;
        waterContainer.localScale = scaleCache;
    }
    //
    public void DislodgeSnakeRocks()
    {
        Vector3 centerPos = Vector3.zero;
        for (int i = 0; i < initialSnakeRocks.Length; i++)
        {
            centerPos += initialSnakeRocks[i].transform.position;
        }
        centerPos *= 0.33f;
        for (int i = 0; i < initialSnakeRocks.Length; i++)
        {
            initialSnakeRocks[i].AddExplosionForce(800f, centerPos + Vector3.up * 0.1f, 6f, 8f);
            initialSnakeRocks[i].transform.DOShakeRotation(1.5f, 30, 6);
            initialSnakeRocks[i].transform.DOScale(0f, 4f).SetEase(Ease.InQuad);
        }
    }
}