using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalPhysic : MonoBehaviour
{
    public static GlobalPhysic instance;

    [SerializeField] private float _gravity = -9.8f;
    public float gravity
    {
        get
        {
            return _gravity;
        }
        set
        {
            _gravity = value;
            Physics2D.gravity = new Vector3(0, _gravity);
        }
    }
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }



}
