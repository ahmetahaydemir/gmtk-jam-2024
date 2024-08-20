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
    public GameObject gameWinEffect;
    public AudioSource gameWinSFX;
    //
    public Rigidbody[] initialSnakeRocks;
    //
    private Vector3 scaleCache;
    private bool gameOver;
    //
    public void UpdateWaterProgress(float time, int phase)
    {
        if (waterBase.position.y > -150f && !gameOver)
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
    public void OnGameCompleted(Transform playerTrans)
    {
        DOVirtual.Float(0f, 1f, 10f, time =>
                    {
                        gameWinEffect.transform.position = playerTrans.position + Vector3.up * (2f + time);
                    });
        gameWinEffect.SetActive(true);
        gameWinSFX.Play();
        gameOver = true;
        DOVirtual.Float(sun.intensity, 1.5f, 6f, (float x) =>
        {
            sun.intensity = x;
        });
        DOVirtual.Float(underwater.fogBrightness, 1.1f, 6f, (float y) =>
        {
            underwater.fogBrightness = y;
        });
        DOVirtual.Float(musicSource.pitch, 1f, 10f, (float z) =>
        {
            musicSource.pitch = z;
        });
    }
}