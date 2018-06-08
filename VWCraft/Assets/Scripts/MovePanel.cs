using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MovePanel : MonoBehaviour, IDragHandler, IPointerDownHandler, IEndDragHandler
{

    private Vector2 offset;
    private RectTransform titleRect;
    private RectTransform panelRect;
    private RectTransform parentRect;

    public void Start()
    {
        panelRect = (RectTransform)this.transform;
        titleRect = (RectTransform)this.transform.Find("Title").transform;
        parentRect = (RectTransform)this.transform.parent.transform;
    }

    public void OnDrag(PointerEventData eventData)
    {
        this.transform.position = eventData.position - offset;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        offset = eventData.position - (Vector2)this.transform.position;
        this.transform.position = eventData.position - offset;

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Vector3 position = panelRect.localPosition;

        Vector3 minPosition = parentRect.rect.min - panelRect.rect.min;
        Vector3 maxPosition = parentRect.rect.max - panelRect.rect.max;
        maxPosition.y -= titleRect.rect.yMax;

        position.x = Mathf.Clamp(panelRect.localPosition.x, minPosition.x, maxPosition.x);
        position.y = Mathf.Clamp(panelRect.localPosition.y, minPosition.y, maxPosition.y);

        panelRect.localPosition = position;
    }
}
