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
    //
    public void Start()
    {
        timer = 0f;
        cameraManager.Initialize(playerManager.playerTransform, playerManager.playerCamera);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        for (int i = 0; i < enemyManager.initialSpawnCount; i++)
        {
            enemyManager.SpawnEnemyObject(playerManager.playerTransform.position, EnemyBehaviour.Neutral);
        }
    }
    //
    public void Update()
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
    //
    public void LateUpdate()
    {
        cameraManager.UpdateCameraAction(inputManager.InputCache, waterManager.waterBase.transform.position.y, enemyManager.GetTotalMass());
    }
}