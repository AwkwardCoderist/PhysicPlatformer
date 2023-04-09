using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GravityUI : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    [SerializeField] private Canvas parentCanvas;
    private RectTransform obj;
    public Vector3 dir;
    public float xLimit;
    public float yLimit;
    public float forcePerUnit = 1;

    private Vector2 xLimits;
    private Vector2 yLimits;

    private Vector2 defaulGravity;

    private void Start()
    {
        obj = GetComponent<RectTransform>();
        defaulGravity = Physics2D.gravity;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
    }

    public void OnDrag(PointerEventData eventData)
    {
        //RectTransformUtility.ScreenPointToLocalPointInRectangle(parentCanvas.transform as RectTransform, eventData.position, Camera.main, out Vector2 localPoint);

        obj.position = eventData.position;

        obj.anchoredPosition = new Vector2(Mathf.Clamp(obj.anchoredPosition.x, -xLimit, xLimit), Mathf.Clamp(obj.anchoredPosition.y, -yLimit, yLimit));

        dir = CalculateDir();

        Physics2D.gravity = dir * forcePerUnit;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
    }

    private Vector3 CalculateDir()
    {
        return (obj.position - obj.parent.position);
    }

    public void ResetKnobPos()
    {
        obj.anchoredPosition = new Vector2(defaulGravity.x / forcePerUnit, defaulGravity.y / forcePerUnit);
        Physics2D.gravity = defaulGravity;
    }

}
