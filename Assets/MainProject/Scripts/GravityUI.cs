using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GravityUI : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    [SerializeField] private RectTransform parentObj;
    private RectTransform obj;
    public Vector3 dir;
    public float xLimit;
    public float yLimit;

    private Vector2 xLimits;
    private Vector2 yLimits;

    private float width;

    private void Start()
    {
        obj = GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 newPos = eventData.position;
        newPos.x = Mathf.Clamp(newPos.x, parentObj.position.x + parentObj.rect.xMin, parentObj.position.x + parentObj.rect.xMax);
        newPos.y = Mathf.Clamp(newPos.y, parentObj.position.y + parentObj.rect.yMin, parentObj.position.y + parentObj.rect.yMax);
        
        obj.position = newPos;

        dir = CalculateDir();

        Physics2D.gravity = dir;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
    }

    private Vector3 CalculateDir()
    {
        return (obj.position - parentObj.position);
    }

}
