using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;

    [SerializeField] private Vector2 force;

    private void Update()
    {
        rb.AddForce(force * Time.deltaTime);
    }
}
