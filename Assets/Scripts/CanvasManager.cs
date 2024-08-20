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
    public GameObject creditsPanel;
    public GameObject optionsPanel;
    //
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
    public void DeathReaction()
    {
        colorTween.Kill();
        sizeText.text = string.Format("{0:F1} KG", 0);
        sizeBg.color = (Color.red * 0.4f + Color.white * 0.6f);
        sizeBg.transform.parent.DOPunchScale(Vector3.one * 0.5f, 1f);
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
                sizeBg.color = (Color.red * 0.4f + Color.white * 0.6f);
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
}