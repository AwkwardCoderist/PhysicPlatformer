using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Trigger : MonoBehaviour
{
    [SerializeField] private LayerMask reactLayers;
    [SerializeField] private List<Transform> reactObjects = new List<Transform>();
    [SerializeField] private UnityEvent unityEvent;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (reactLayers == (reactLayers | (1 << collision.gameObject.layer)))
        {
            if (reactObjects.Count > 0)
                if (!reactObjects.Contains(collision.transform)) return;

            unityEvent.Invoke();
        }
    }
}
