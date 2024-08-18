using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class GenericToggle : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    [Header("Generic")]
    public Toggle toggleReference;
    public GameObject hoverReference;
    public string audioPathHover;
    public string audioPathClick;
    //
    public void ListenEvent(UnityAction<bool> method)
    {
        toggleReference.onValueChanged.AddListener(method);
    }
    //
    public void ForgetEvent(UnityAction<bool> method)
    {
        toggleReference.onValueChanged.RemoveListener(method);
    }
    //
    public void OnSelect(BaseEventData eventData)
    {
        Debug.Log("SELECT " + eventData.selectedObject.name);
        if (hoverReference != null) { hoverReference.SetActive(true); }
    }
    //
    public void OnDeselect(BaseEventData eventData)
    {
        Debug.Log("DESELECT " + eventData.selectedObject.name);
        if (hoverReference != null) { hoverReference.SetActive(false); }
    }
    //
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverReference != null) { hoverReference.SetActive(true); }
        Debug.Log("HOVER IN " + eventData.pointerEnter.name);
    }
    //
    public void OnPointerExit(PointerEventData eventData)
    {
        if (hoverReference != null) { hoverReference.SetActive(false); }
        Debug.Log("HOVER OUT " + eventData.pointerEnter.name);
    }
}