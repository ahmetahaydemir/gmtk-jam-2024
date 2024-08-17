using UnityEngine;
public class GameManager : MonoBehaviour
{
    public PlayerManager playerManager;
    public InputManager inputManager;
    public CameraManager cameraManager;
    //
    public void Start()
    {
        cameraManager.Initialize(playerManager.playerTransform, playerManager.playerCamera);
    }
    //
    public void Update()
    {
        playerManager.UpdatePlayerAction(inputManager.InputCache);
        // enemyManager.UpdateEnemyAction(playerManager.playerTransform.position);
        //
        // if (Keyboard.current.tKey.wasPressedThisFrame || Keyboard.current.yKey.wasPressedThisFrame)
        // {
        //     enemyManager.SpawnEnemyObject(playerManager.playerTransform.position);
        // }
    }
    //
    public void LateUpdate()
    {
        cameraManager.UpdateCameraAction(inputManager.InputCache);
    }
}