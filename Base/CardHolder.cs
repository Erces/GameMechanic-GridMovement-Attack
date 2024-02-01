using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardHolder : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private Card card;
    [SerializeField]
    private GameObject entityPrefab;

    private Transform parent;

    public void OnBeginDrag(PointerEventData eventData)
    {
        parent = this.transform.parent;
        this.transform.SetParent(this.transform.parent.parent);
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        this.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        this.transform.SetParent(parent);
        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {

    }

    public void OnPointerExit(PointerEventData eventData)
    {

    }

    public void SetCardOff()
    {
        card.gameObject.SetActive(false);
        entityPrefab.gameObject.SetActive(true);
    }

    public void DropEntity()
    {
        entityPrefab.transform.SetParent(parent.parent.parent);
        entityPrefab.transform.position = Mouse3D.GetMouseWorldPosition();
    }
}
