using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public class GenericSlider : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    [Header("Generic")]
    public Slider sliderReference;
    public GameObject hoverReference;
    public string audioPathHover;
    public string audioPathClick;
    //
    void Update()
    {
        if (hoverReference.activeSelf)
        {
            if (Mouse.current.scroll.value.y > 0)
            {
                // SetSliderValue(sliderReference.value + (sliderReference.maxValue * 0.05f));
                SetSliderValue(sliderReference.value + 1f);
            }
            else if (Mouse.current.scroll.value.y < 0)
            {
                // SetSliderValue(sliderReference.value - (sliderReference.maxValue * 0.05f));
                SetSliderValue(sliderReference.value - 1f);
            }
        }
    }
    //
    public void ListenEvent(UnityAction<float> method)
    {
        sliderReference.onValueChanged.AddListener(method);
    }
    //
    public void ForgetEvent(UnityAction<float> method)
    {
        sliderReference.onValueChanged.RemoveListener(method);
    }
    //
    public void SetSliderValue(float amount)
    {
        sliderReference.value = Mathf.Clamp(amount, sliderReference.minValue, sliderReference.maxValue);
    }
    //
    public void OnSelect(BaseEventData eventData)
    {
        if (hoverReference != null) { hoverReference.SetActive(true); }
    }
    //
    public void OnDeselect(BaseEventData eventData)
    {
        if (hoverReference != null) { hoverReference.SetActive(false); }
    }
    //
    public void OnPointerEnter(PointerEventData eventData)
    {
        EventSystem.current.SetSelectedGameObject(gameObject);
    }
    //
    public void OnPointerExit(PointerEventData eventData)
    {
        EventSystem.current.SetSelectedGameObject(null);
    }
}