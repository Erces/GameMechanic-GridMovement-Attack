using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IPointerEnterHandler, IDropHandler, IPointerExitHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        eventData.pointerDrag.GetComponent<CardHolder>().DropEntity();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        eventData.pointerDrag.GetComponent<CardHolder>().SetCardOff();

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        
    }
}
