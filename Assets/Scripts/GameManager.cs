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
    private float interval;
    //
    public static float musicVolume;
    public static float sfxVolume;
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
        if (PlayerPrefs.HasKey("musicVolume"))
        {
            musicVolume = PlayerPrefs.GetFloat("musicVolume");
        }
        else
        {
            musicVolume = 1f;
        }
        if (PlayerPrefs.HasKey("sfxVolume"))
        {
            sfxVolume = PlayerPrefs.GetFloat("sfxVolume");
        }
        else
        {
            sfxVolume = 1f;
        }
        SetVolumeLevel();
    }
    //
    public void Update()
    {
        if (started && !over)
        {
            timer += Time.deltaTime;
            //
            waterManager.UpdateWaterProgress(timer, phase);
            playerManager.UpdatePlayerAction(inputManager.InputCache, waterManager.waterBase.transform.position.y, enemyManager.GetTotalMass(), phase);
            enemyManager.UpdateEnemyAction(playerManager.playerTransform.position, waterManager.waterBase.transform.position.y);
            canvasManager.UpdateInterface(waterManager.waterBase.transform.position.y, enemyManager.GetTotalMass());
            //
            if (Keyboard.current.tKey.wasPressedThisFrame || Keyboard.current.yKey.wasPressedThisFrame)
            {
                enemyManager.SpawnEnemyObject(playerManager.playerTransform.position, waterManager.waterBase.transform.position.y);
            }
            //
            switch (phase)
            {
                case 0:
                    if (timer > interval)
                    {
                        interval += 0.5f;
                        enemyManager.SpawnEnemyObject(playerManager.playerTransform.position, waterManager.waterBase.transform.position.y);
                    }
                    if (enemyManager.phaseOneSpawnDepth > waterManager.waterBase.transform.position.y)
                    {
                        phase = 1;
                        waterManager.WaterDeepVisual(phase);
                        canvasManager.ShowPhasePopup(phase);
                    }
                    break;
                case 1:
                    if (timer > interval)
                    {
                        interval += 0.75f;
                        enemyManager.SpawnEnemyObject(playerManager.playerTransform.position, waterManager.waterBase.transform.position.y);
                    }
                    if (enemyManager.phaseTwoSpawnDepth > waterManager.waterBase.transform.position.y)
                    {
                        phase = 2;
                        waterManager.WaterDeepVisual(phase);
                        canvasManager.ShowPhasePopup(phase);
                    }
                    break;
                case 2:
                    if (timer > interval)
                    {
                        interval += 0.85f;
                        enemyManager.SpawnEnemyObject(playerManager.playerTransform.position, waterManager.waterBase.transform.position.y);
                    }
                    if (enemyManager.phaseThreeSpawnDepth > waterManager.waterBase.transform.position.y)
                    {
                        phase = 3;
                        waterManager.WaterDeepVisual(phase);
                        canvasManager.ShowPhasePopup(phase);
                    }
                    break;
                case 3:
                    if (enemyManager.phaseFourSpawnDepth > waterManager.waterBase.transform.position.y)
                    {
                        phase = 4;
                        waterManager.WaterDeepVisual(phase);
                        canvasManager.ShowPhasePopup(phase);
                        SpawnBoss();
                    }
                    else
                    {
                        if (timer > interval)
                        {
                            interval += 1.5f;
                            enemyManager.SpawnEnemyObject(playerManager.playerTransform.position, waterManager.waterBase.transform.position.y);
                        }
                    }
                    break;
                case 4:
                    if (enemyManager.bossEnemy != null)
                    {
                        canvasManager.UpdateBossHealthBar(enemyManager.bossEnemy.GetHealthRatio());
                    }
                    break;
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
    public void OnEnemyHit(int enemyIndex, float charge)
    {
        enemyManager.OnEnemyHit(enemyIndex, charge);
    }
    public void UpdateBossState()
    {

    }
    public void OnGameCompleted()
    {
        waterManager.OnGameCompleted(playerManager.playerTransform);
        canvasManager.OnGameCompleted();
    }
    public void OnPlayerGrow(float growAmount)
    {
        playerManager.GrowReaction(growAmount);
    }
    public void OnPlayerHit(EnemyData enemyData, float totalMass)
    {
        playerManager.GetHitReaction(enemyData);
        canvasManager.HitSizeTweenEffect();
        if (totalMass <= 0.01f)
        {
            over = true;
            cameraManager.DeathReaction(waterManager.waterBase.transform.position.y);
            playerManager.DeathReaction(waterManager.waterBase.transform.position.y);
            canvasManager.DeathReaction(waterManager.waterBase.transform.position.y);
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
    public void SpawnBoss()
    {
        DOVirtual.Float(0f, 1f, 3f, (float x) =>
        {

        }).OnComplete(() =>
        {
            cameraManager.ShakeCamera();
            enemyManager.SpawnBossObject(playerManager.playerTransform.position, waterManager.waterBase.transform.position.y);
            canvasManager.ShowBossHealthBar();
        });
    }
    //
    public void SetVolumeLevel()
    {
        if (PlayerPrefs.HasKey("musicVolume"))
        {
            musicVolume = PlayerPrefs.GetFloat("musicVolume");
        }
        if (PlayerPrefs.HasKey("sfxVolume"))
        {
            sfxVolume = PlayerPrefs.GetFloat("sfxVolume");
        }
        waterManager.musicSource.volume = musicVolume;
    }
}