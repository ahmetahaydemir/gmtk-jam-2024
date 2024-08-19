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
    //
    public TextMeshProUGUI depthText;
    public TextMeshProUGUI sizeText;
    public Image sizeBg;
    public RectTransform sizeFillRect;
    public RectTransform sizeBgRect;
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
}