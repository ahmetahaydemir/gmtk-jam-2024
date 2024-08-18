using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class GenericButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    [Header("Generic")]
    public Button buttonReference;
    public TextMeshProUGUI textReference;
    public GameObject hoverReference;
    public AudioSource audioSourceHover;
    public AudioSource audioSourceClick;
    //
    private float textFontSizeCache;
    private Tween hoverTween;
    private Tween clickTween;
    //
    public virtual void Start()
    {
        buttonReference.onClick.AddListener(OnClick);
        textFontSizeCache = textReference.fontSize;
    }
    //
    public virtual void OnClick()
    {
        if (audioSourceClick != null) { audioSourceClick.Play(); }
        //
        if (clickTween != null)
        {
            clickTween.Restart();
        }
        else
        {
            clickTween = transform.DOPunchPosition(Vector3.down * 4f, 0.25f).SetEase(Ease.InOutSine);
            clickTween.SetAutoKill(false);
        }
    }
    //
    public virtual void OnSelect(BaseEventData eventData)
    {
        if (audioSourceHover != null) { audioSourceHover.Play(); }
        if (hoverReference != null) { hoverReference.SetActive(true); }
        if (textReference != null) textReference.fontSize = textFontSizeCache * 1.1f;
        //
        if (hoverTween != null)
        {
            hoverTween.Restart();
        }
        else
        {
            hoverTween = transform.DOPunchPosition(Vector3.right * 4f, 0.25f).SetEase(Ease.InOutSine);
            hoverTween.SetAutoKill(false);
        }
    }
    //
    public virtual void OnDeselect(BaseEventData eventData)
    {
        if (hoverReference != null) { hoverReference.SetActive(false); }
        if (textReference != null) textReference.fontSize = textFontSizeCache;
    }
    //
    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        EventSystem.current.SetSelectedGameObject(gameObject);
    }
    //
    public virtual void OnPointerExit(PointerEventData eventData)
    {
        EventSystem.current.SetSelectedGameObject(null);
    }
}