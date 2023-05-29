using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Trigger : MonoBehaviour
{
    [SerializeField] private LayerMask reactLayers;
    [SerializeField] private bool invokeOnce = true;
    private bool invoked;
    public bool Invoked { get =>  invoked; set { invoked = value; } }
    [SerializeField] private float timeToActivate = 0;
    private float timeInTrigger = 0;
    [SerializeField] private List<Transform> reactObjects = new List<Transform>();
    [SerializeField] private UnityEvent enterEvent;
    [SerializeField] private UnityEvent exitEvent;
    private List<Transform> enteredObjects = new List<Transform>();


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (reactLayers == (reactLayers | (1 << collision.gameObject.layer)))
        {
            if (reactObjects.Count > 0)
                if (!reactObjects.Contains(collision.transform)) return;

            enteredObjects.Add(collision.transform);
        }
    }

    private void Update()
    {
        if (enteredObjects.Count > 0) //enter
        {
            if (!invoked)
            {
                if (timeInTrigger >= timeToActivate)
                {
                    enterEvent.Invoke();
                    if (invokeOnce) invoked = true;
                    timeInTrigger = 0;
                }

                timeInTrigger += Time.deltaTime;
            }            
        }
        else //exit
        {
            exitEvent.Invoke();
            invoked = false;
            timeInTrigger = 0;
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (reactLayers == (reactLayers | (1 << collision.gameObject.layer)))
        {
            if (enteredObjects.Count > 0)
                if (!enteredObjects.Contains(collision.transform)) return;

            enteredObjects.Remove(collision.transform);
        }
    }

    public void ClearListOfObjects()
    {
        enteredObjects.Clear();
    }
}
