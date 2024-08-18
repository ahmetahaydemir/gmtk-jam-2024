using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
public class GameManager : MonoBehaviour
{
    public PlayerManager playerManager;
    public InputManager inputManager;
    public CameraManager cameraManager;
    public EnemyManager enemyManager;
    public WaterManager waterManager;
    public CanvasManager canvasManager;
    //
    private float timer;
    private bool started;
    //
    public void Start()
    {
        CanvasManager.OnPlayClick += OnPlayClicked;
        timer = 0f;
        cameraManager.Initialize(playerManager.playerTransform, playerManager.playerCamera);
        // Cursor.lockState = CursorLockMode.Locked;
        // Cursor.visible = false;
        for (int i = 0; i < enemyManager.initialSpawnCount; i++)
        {
            enemyManager.SpawnEnemyObject(playerManager.playerTransform.position, EnemyBehaviour.Neutral);
        }
    }
    //
    public void Update()
    {
        if (started)
        {
            timer += Time.deltaTime;
            //
            waterManager.UpdateWaterProgress(timer);
            playerManager.UpdatePlayerAction(inputManager.InputCache, waterManager.waterBase.transform.position.y, enemyManager.GetTotalMass());
            enemyManager.UpdateEnemyAction(playerManager.playerTransform.position, waterManager.waterBase.transform.position.y);
            canvasManager.UpdateInterface(waterManager.waterBase.transform.position.y, enemyManager.GetTotalMass());
            //
            if (Keyboard.current.tKey.wasPressedThisFrame || Keyboard.current.yKey.wasPressedThisFrame)
            {
                enemyManager.SpawnEnemyObject(playerManager.playerTransform.position, EnemyBehaviour.Hostile);
            }
        }
    }
    //
    public void LateUpdate()
    {
        if (started)
        {
            cameraManager.UpdateCameraAction(inputManager.InputCache, waterManager.waterBase.transform.position.y, enemyManager.GetTotalMass());
        }
    }
    //
    public void OnPlayClicked()
    {
        waterManager.DislodgeSnakeRocks();
        cameraManager.ShakeCamera();
        //
        playerManager.playerTransform.DOMoveY(playerManager.playerTransform.position.y + 0.4f, 1f).SetEase(Ease.InOutSine).OnComplete(() =>
        {
            started = true;
            canvasManager.ShowInGameHud();
        });
    }
}