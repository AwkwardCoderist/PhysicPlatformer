using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum Formula
{
    Force
}

public class DraggableFormula : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler, IDropHandler
{
    [SerializeField] private Transform dragContainer;
    [SerializeField] private Formula formula;

    private RectTransform obj;
    private Transform defaultParent;
    private Vector2 defaultPos;

    private void Start()
    {
        obj = GetComponent<RectTransform>();
        defaultPos = obj.anchoredPosition;
        defaultParent = obj.parent;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        obj.SetParent(dragContainer);
    }

    public void OnDrag(PointerEventData eventData)
    {
        obj.position += (Vector3)eventData.delta;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        obj.SetParent(defaultParent);
        obj.anchoredPosition = defaultPos;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag.TryGetComponent(out FormulaReciever reciever))
        {
            reciever.RecieveFormula(formula);
        }
    }
}
