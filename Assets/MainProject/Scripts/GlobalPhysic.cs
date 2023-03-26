using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalPhysic : MonoBehaviour
{
    public static GlobalPhysic instance;

    [SerializeField] private Vector2 _gravity = new Vector2(0, -9.8f);
    public Vector2 gravity
    {
        get
        {
            return _gravity;
        }
        set
        {
            _gravity = value;
            Physics2D.gravity = _gravity;
        }
    }
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    public void OnChangeX(float value) 
    { 
        _gravity.x = value;
        gravity = _gravity;
    }
    public void OnChangeY(float value)
    {
        _gravity.y = value;
        gravity = _gravity;
    }

}
