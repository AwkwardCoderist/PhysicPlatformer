using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum Formula
{
    Force,
    Density,
    Velocity,
    Elasticity,
    Area,
    Friction
}

public class DraggableFormula : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler, IDropHandler
{
    [SerializeField] private Transform dragContainer;
    [SerializeField] private Formula formula;
    [SerializeField] private Physics2DRaycaster physics2DRaycaster;

    private RectTransform obj;
    private Transform defaultParent;
    private Vector2 defaultPos;
    private List<RaycastResult> raycastResults = new List<RaycastResult>();
    private int defaultChildIndex;

    private void Start()
    {
        obj = GetComponent<RectTransform>();
        defaultPos = obj.anchoredPosition;
        defaultParent = obj.parent;
        defaultChildIndex = transform.GetSiblingIndex();
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
        obj.SetSiblingIndex(defaultChildIndex);
        obj.anchoredPosition = defaultPos;
    }

    public void OnDrop(PointerEventData eventData)
    {
        physics2DRaycaster.Raycast(eventData, raycastResults);
        foreach (RaycastResult raycastResult in raycastResults)
        {
            Debug.Log(raycastResult.gameObject);
            if(raycastResult.gameObject.TryGetComponent(out FormulaReciever reciever))
            {
                reciever.RecieveFormula(formula);
            }
        }
    }

}
