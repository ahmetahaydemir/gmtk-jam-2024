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
    public TransitionManager transitionManager;
    //
    private float timer;
    private bool started;
    private bool over;
    private int phase;
    //
    public static GameManager Instance;
    void Awake()
    {
        Instance = this;
    }
    //
    public void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        timer = 0f;
        cameraManager.Initialize(playerManager.playerTransform, playerManager.playerCamera);
        transitionManager.TransitionOut();
        for (int i = 0; i < enemyManager.initialSpawnCount; i++)
        {
            enemyManager.SpawnEnemyObject(playerManager.playerTransform.position, EnemyBehaviour.Neutral, waterManager.waterBase.transform.position.y);
        }
    }
    //
    public void Update()
    {
        if (started && !over)
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
                enemyManager.SpawnEnemyObject(playerManager.playerTransform.position, EnemyBehaviour.Chase, waterManager.waterBase.transform.position.y);
            }
        }
        //
        if (started)
        {
            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                transitionManager.TransitionIn();
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
    public void OnEnemyHit(int enemyIndex)
    {
        enemyManager.KillEnemy(enemyIndex);
    }
    public void OnPlayerHit(EnemyData enemyData, float totalMass)
    {
        playerManager.GetHitReaction(enemyData);
        if (totalMass <= 0.01f)
        {
            over = true;
            cameraManager.DeathReaction(waterManager.waterBase.transform.position.y);
            playerManager.DeathReaction(waterManager.waterBase.transform.position.y);
            canvasManager.DeathReaction();
        }
    }
    public void OnPlayClicked()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        waterManager.DislodgeSnakeRocks();
        cameraManager.ShakeCamera(false);
        cameraManager.CameraTransform.DORotate(Vector3.up * 145f, 0.75f).SetEase(Ease.InOutSine);
        //
        playerManager.playerTransform.DOMoveY(playerManager.playerTransform.position.y + 0.4f, 1f).SetEase(Ease.InOutSine).OnComplete(() =>
        {
            started = true;
            canvasManager.ShowInGameHud();
            phase = 0;
            canvasManager.ShowPhasePopup(phase);
        });
    }
}