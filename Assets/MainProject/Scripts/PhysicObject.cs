using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicObject : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float _mass;
    public float mass
    {
        get
        {
            return _mass;
        }
        set
        {
            _mass = value;
            rb.mass = Mathf.Abs(value);
            rb.gravityScale = value;
        }
    }
    public float gravity = 9.8f;


    private void Update()
    {
        UpdatePhysic();
    }

    private void UpdatePhysic()
    {
        //rb.AddForce(Vector3.down * gravity * mass * Time.deltaTime, ForceMode2D.Force);
    }

}
