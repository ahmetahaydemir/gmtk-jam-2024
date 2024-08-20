using System;
using System.Text;
using System.Threading;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class CanvasManager : MonoBehaviour
{
    public GenericButton playButton;
    public GenericButton optionsButton;
    public GenericButton creditsButton;
    public GenericButton exitButton;
    //
    public CanvasGroup mainMenuGroup;
    public CanvasGroup inGameGroup;
    public CanvasGroup popupPhaseGroup;
    public CanvasGroup completedPanelGroup;
    public GameObject creditsPanel;
    public GameObject optionsPanel;
    //
    public TextMeshProUGUI gameLoseText;
    public TextMeshProUGUI gameWinText;
    public GenericSlider musicVolumeSlider;
    public GenericSlider sfxVolumeSlider;
    public TextMeshProUGUI depthText;
    public Image depthBg;
    public TextMeshProUGUI sizeText;
    public Image sizeBg;
    public RectTransform sizeFillRect;
    public RectTransform sizeBgRect;
    public TextMeshProUGUI phaseTitleText;
    public TextMeshProUGUI phaseDescText;
    public TextMeshProUGUI phaseDepthText;
    public AudioSource phaseChangeSFX;
    //
    [Header("Boss")]
    public CanvasGroup bossHealthGroup;
    public Image healthBackLine;
    public Image healthFillLine;
    public Image healthTrailLine;
    //
    private Tween creditsShowTween;
    private Tween optionsShowTween;
    //
    void Awake()
    {
        playButton.buttonReference.onClick.AddListener(OnPlayButtonClick);
        optionsButton.buttonReference.onClick.AddListener(OnOptionsButtonClick);
        creditsButton.buttonReference.onClick.AddListener(OnCreditsButtonClick);
        exitButton.buttonReference.onClick.AddListener(OnExitButtonClick);
        musicVolumeSlider.ListenEvent(OnMusicVolumeSlider);
        sfxVolumeSlider.ListenEvent(OnSFXVolumeSlider);
    }
    //
    public void OnGameCompleted(float waterBaseLevel, float size, float time)
    {
        bossHealthGroup.transform.DOPunchPosition(Vector3.down * 4f, 2f).OnComplete(() =>
        {
            bossHealthGroup.DOFade(0f, 0.75f).SetEase(Ease.InOutSine).OnComplete(() =>
            {
                completedPanelGroup.gameObject.SetActive(true);
                completedPanelGroup.DOFade(1f, 0.75f).SetEase(Ease.InOutSine);
                gameWinText.gameObject.SetActive(true);
                gameWinText.DOFade(1f, 1.4f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
                //
                int seconds = Mathf.RoundToInt(time) % 60;
                int minutes = Mathf.RoundToInt(time / 60f);
                string timeFormat = string.Format("{0}:{1}", minutes, seconds);
                //
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine("! You Win !");
                stringBuilder.Append("Depth : <color=#0088A8>");
                stringBuilder.Append(string.Format("{0:F1} M", waterBaseLevel));
                stringBuilder.Append("</color> - Size : <color=#629651>");
                stringBuilder.Append(string.Format("{0:F1} KG", size));
                stringBuilder.Append("</color> - Time : <color=#8C5555>");
                stringBuilder.Append(timeFormat);
                stringBuilder.AppendLine("</color>");
                stringBuilder.Append("Press <color=#505050>ESC</color> To Scale Down And Rise Again");
                gameWinText.text = stringBuilder.ToString();
            });
        });
    }
    //
    private float healthLengthTarget;
    private float healthLengthCache;
    private float healthLengthFull;
    private Tween healthPulse;
    private Color healthPulseColor;
    private float healthTrailSpeed;
    public void ShowBossHealthBar()
    {
        bossHealthGroup.DOFade(1f, 0.6f).SetEase(Ease.InOutSine);
        healthLengthFull = 1f;
        healthLengthTarget = healthLengthFull;
        healthLengthCache = healthLengthFull;
    }
    public void UpdateBossHealthBar(float healthRatio)
    {
        healthLengthTarget = healthLengthFull * healthRatio;
        healthLengthCache = Mathf.Lerp(healthLengthCache, healthLengthTarget, Time.deltaTime * 5f);
        //
        healthFillLine.transform.localScale = Vector3.right * (healthLengthTarget) + Vector3.up + Vector3.forward;
        healthTrailLine.transform.localScale = Vector3.right * (healthLengthCache) + Vector3.up + Vector3.forward;
    }
    public void PulseBossHealth()
    {
        if (healthPulse != null)
        {
            healthPulse.Restart();
        }
        else
        {
            healthPulseColor = healthFillLine.color;
            healthPulse = DOVirtual.Float(0f, 0.5f, 0.5f, time =>
                    {
                        healthTrailSpeed = 0f;
                        healthFillLine.color = Color.Lerp(healthPulseColor * 1.5f, healthPulseColor, time * 4f);
                    }).OnComplete(() => { healthTrailSpeed = 5f * 0.6f; });
            healthPulse.SetAutoKill(false);
        }
    }
    //
    public void OnPlayButtonClick()
    {
        optionsPanel.SetActive(false);
        creditsPanel.SetActive(false);
        playButton.buttonReference.interactable = false;
        mainMenuGroup.DOFade(0f, 0.75f).OnComplete(() =>
        {
            mainMenuGroup.interactable = false;
            mainMenuGroup.blocksRaycasts = false;
        });
        GameManager.Instance.OnPlayClicked();
    }
    public void OnOptionsButtonClick()
    {
        creditsPanel.SetActive(false);
        optionsPanel.SetActive(!optionsPanel.activeSelf);
        musicVolumeSlider.SetSliderValue(GameManager.musicVolume);
        sfxVolumeSlider.SetSliderValue(GameManager.sfxVolume);
        if (optionsShowTween != null)
        {
            optionsShowTween.Restart();
        }
        else
        {
            optionsShowTween = optionsPanel.transform.DOPunchScale(Vector3.one * 0.075f, 0.25f).SetEase(Ease.InOutSine);
            optionsShowTween.SetAutoKill(false);
        }
    }
    public void OnCreditsButtonClick()
    {
        optionsPanel.SetActive(false);
        creditsPanel.SetActive(!creditsPanel.activeSelf);
        if (creditsShowTween != null)
        {
            creditsShowTween.Restart();
        }
        else
        {
            creditsShowTween = creditsPanel.transform.DOPunchScale(Vector3.one * 0.075f, 0.25f).SetEase(Ease.InOutSine);
            creditsShowTween.SetAutoKill(false);
        }
    }
    public void OnExitButtonClick()
    {
        Application.Quit();
    }
    public void OnMusicVolumeSlider(float vol)
    {
        PlayerPrefs.SetFloat("musicVolume", vol);
        GameManager.Instance.SetVolumeLevel();
    }
    public void OnSFXVolumeSlider(float vol)
    {
        PlayerPrefs.SetFloat("sfxVolume", vol);
        GameManager.Instance.SetVolumeLevel();
    }
    //
    public void ShowInGameHud()
    {
        inGameGroup.DOFade(1f, 0.75f);
    }
    //
    private Tween colorTween;
    private Tween sizeTween;
    public void HitSizeTweenEffect()
    {
        if (sizeTween != null)
        {
            sizeTween.Restart();
        }
        else
        {
            sizeTween = sizeBg.transform.parent.DOPunchScale(Vector3.one * 0.1f, 0.5f);
            sizeTween.SetAutoKill(false);
        }
        if (colorTween != null)
        {
            colorTween.Restart();
        }
        else
        {
            colorTween = sizeBg.DOColor((Color.red * 0.4f + Color.white * 0.6f), 0.3f).SetLoops(2, LoopType.Yoyo);
            colorTween.SetAutoKill(false);
        }
    }
    public void UpdateInterface(float waterDepth, float playerSize)
    {
        depthText.text = string.Format("{0:F1} M", waterDepth);
        sizeText.text = string.Format("{0:F1} KG", playerSize);
    }
    public void DeathReaction(float waterBaseLevel)
    {
        colorTween.Kill();
        sizeText.text = string.Format("{0:F1} KG", 0);
        sizeBg.color = (Color.red * 0.4f + Color.white * 0.6f);
        sizeBg.transform.parent.DOPunchScale(Vector3.one * 0.5f, 1f);
        gameLoseText.gameObject.SetActive(true);
        gameLoseText.DOFade(1f, 1f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        bossHealthGroup.gameObject.SetActive(false);
    }
    private Tween phaseColorTween;
    private Tween phaseSizeTween;
    public void ShowPhasePopup(int index)
    {
        if (phaseSizeTween != null)
        {
            phaseSizeTween.Restart();
        }
        else
        {
            phaseSizeTween = depthBg.transform.parent.DOPunchScale(Vector3.one * 0.1f, 0.5f);
            phaseSizeTween.SetAutoKill(false);
        }
        if (phaseColorTween != null)
        {
            phaseColorTween.Restart();
        }
        else
        {
            phaseColorTween = depthBg.DOColor((Color.red * 0.4f + Color.white * 0.6f), 0.3f).SetLoops(2, LoopType.Yoyo);
            phaseColorTween.SetAutoKill(false);
        }
        //
        switch (index)
        {
            case 0:
                phaseTitleText.text = "Shallow Coast";
                phaseDescText.text = "Enjoy the breeze and feast";
                phaseDepthText.text = "Depth Level < 15";
                break;
            case 1:
                phaseTitleText.text = "Uninvited Current";
                phaseDescText.text = "Everyone wants a piece of it";
                phaseDepthText.text = "Depth Level < 30";
                break;
            case 2:
                phaseTitleText.text = "Change of Scales";
                phaseDescText.text = "Climb the food chain";
                phaseDepthText.text = "Depth Level < 60";
                break;
            case 3:
                phaseTitleText.text = "Far From Surface";
                phaseDescText.text = "Where the light abandons us";
                phaseDepthText.text = "Depth Level < 100";
                break;
            case 4:
                phaseTitleText.text = "Deep End";
                phaseDescText.text = "It is coming for you...";
                phaseDepthText.text = "Depth Level < ?????";
                //
                phaseColorTween.Kill();
                depthBg.color = (Color.red * 0.4f + Color.white * 0.6f);
                break;
        }
        //
        popupPhaseGroup.transform.DOPunchPosition(Vector3.right * 4f, 0.5f).OnComplete(() =>
            {
                phaseChangeSFX.Play();
                popupPhaseGroup.DOFade(1f, 1f).SetEase(Ease.InOutSine).OnComplete(() =>
                {
                    popupPhaseGroup.transform.DOPunchPosition(Vector3.down * 5f, 1f).OnComplete(() =>
                    {
                        popupPhaseGroup.DOFade(0f, 1f).SetEase(Ease.InOutSine);
                    });
                });
            });
    }
    public void OpenURL(string url)
    {
        Application.OpenURL(url);
    }
}