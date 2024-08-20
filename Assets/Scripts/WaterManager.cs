using DG.Tweening;
using StylizedWater2;
using StylizedWater2.UnderwaterRendering;
using UnityEngine;
public class WaterManager : MonoBehaviour
{
    public WaterObject water;
    public UnderwaterRenderer underwater;
    public Light sun;
    public AudioSource musicSource;
    public Transform waterContainer;
    public Transform waterBase;
    public Transform waterCover;
    public GameObject bubbleExpObject;
    //
    public Rigidbody[] initialSnakeRocks;
    //
    private Vector3 scaleCache;
    //
    public void UpdateWaterProgress(float time, int phase)
    {
        if (waterBase.position.y > -150f)
        {
            scaleCache = waterContainer.localScale;
            scaleCache.y = 1f + time * 0.1f;
            waterContainer.localScale = scaleCache;
        }
    }
    //
    public void WaterDeepVisual(int phase)
    {
        DOVirtual.Float(1.5f - (phase * 0.13f), 1.5f - ((phase + 1f) * 0.13f), 6f, (float x) =>
        {
            sun.intensity = x;
        });
        DOVirtual.Float(1f - (phase * 0.065f), 1f - ((phase + 1f) * 0.065f), 6f, (float y) =>
        {
            underwater.fogBrightness = y;
        });
        DOVirtual.Float(1f - (phase * 0.02f), 1f - ((phase + 1f) * 0.02f), 6f, (float z) =>
        {
            musicSource.pitch = z;
        });
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
        bubbleExpObject.SetActive(true);
    }
}