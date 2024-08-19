using System;
using System.Collections;
using System.Collections.Generic;
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
    //
    public TextMeshProUGUI depthText;
    public TextMeshProUGUI sizeText;
    public Image sizeBg;
    public RectTransform sizeFillRect;
    public RectTransform sizeBgRect;
    public TextMeshProUGUI phaseTitleText;
    public TextMeshProUGUI phaseDescText;
    public TextMeshProUGUI phaseDepthText;
    public AudioSource phaseChangeSFX;
    //
    void Awake()
    {
        playButton.buttonReference.onClick.AddListener(OnPlayButtonClick);
    }
    //
    public void OnPlayButtonClick()
    {
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

    }
    public void OnCreditsButtonClick()
    {

    }
    public void OnExitButtonClick()
    {
        Application.Quit();
    }
    //
    public void ShowInGameHud()
    {
        inGameGroup.DOFade(1f, 0.75f);
    }
    //
    public void UpdateInterface(float waterDepth, float playerSize)
    {
        depthText.text = string.Format("{0:F1} M", waterDepth);
        sizeText.text = string.Format("{0:F1} KG", playerSize);
    }
    public void DeathReaction()
    {
        sizeText.text = string.Format("{0:F1} KG", 0);
        sizeBg.color = (Color.red * 0.5f + Color.white * 0.5f);
        sizeBg.transform.parent.DOPunchScale(Vector3.one * 1.05f, 1f);
    }
    public void ShowPhasePopup(int index)
    {
        switch (index)
        {
            case 0:
                phaseTitleText.text = "Shallow Coast";
                phaseDescText.text = "Enjoy the breeze and feast";
                phaseDepthText.text = "Depth Level <= 10";
                break;
            case 1:
                phaseTitleText.text = "Uninvited Current";
                phaseDescText.text = "Everyone wants a piece of it";
                phaseDepthText.text = "Depth Level <= 30";
                break;
            case 2:
                phaseTitleText.text = "Change of Scales";
                phaseDescText.text = "Climb the food chain";
                phaseDepthText.text = "Depth Level <= 50";
                break;
            case 3:
                phaseTitleText.text = "Far From Surface";
                phaseDescText.text = "Where the light abandons us";
                phaseDepthText.text = "Depth Level <= 100";
                break;
            case 4:
                phaseTitleText.text = "Deep End";
                phaseDescText.text = "It is coming for you...";
                phaseDepthText.text = "Depth Level <= ?????";
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