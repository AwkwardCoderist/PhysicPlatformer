using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PhysicObject : MonoBehaviour
{

    [System.Serializable]
    public struct PosRot
    {
        public Vector2 position;
        public Quaternion rotation;
        public Vector2 velocity;
        public float torque;

        public float timeMark;

        public PosRot(Vector2 pos, Quaternion rot, float time, Vector2 vel, float torq)
        {
            position = pos;
            rotation = rot;
            timeMark = time;
            velocity = vel;
            torque = torq;
        }
    }

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Collider2D col;
    [SerializeField] private float _mass;
    [SerializeField] private float _volume;
    [SerializeField] private float _timeScale;
    [SerializeField] private float _timePosition;
    [SerializeField] private float recordInterval;
    [SerializeField] private int maxPosRots;
    private List<PosRot> posRots = new List<PosRot>();
    private float lastRecord = 0;
    public bool doRecords = true;
    public float stopTime = 0;
    public float timeline = 0;
    private int posRotRight;
    private int posRotLeft;
    private float lerpVal;
    private Vector2 lerpVelocity;
    private float lerpAngularVelocity;

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

    public float volume
    {
        get
        {
            return _volume;
        }
        set
        {
            _volume = value;
            transform.localScale = Vector2.one * value;
            
        }
    }

    public float timeScale
    {
        get
        {
            return _timeScale;
        }
        set
        {
            _timeScale = value;
        }
    }

    public float timePosition
    {
        get
        {
            return _timePosition;
        }
        set
        {
            _timePosition = value;
        }
    }
    private void Start()
    {
        _mass = rb.mass;
        _volume = transform.localScale.x;
        timeScale = 1;
    }

    private void Update()
    {
        UpdatePhysic();
        UpdateRecording();
    }

    private void UpdatePhysic()
    {
        if (timeScale >= 0)
        {
            rb.AddForce(Physics2D.gravity * timeScale, ForceMode2D.Force);
        }
    }

    private void UpdateRecording()
    {
        if (doRecords)
        {

            if (lerpVelocity != Vector2.zero && lerpAngularVelocity != 0)
            {
                rb.velocity = lerpVelocity;
                rb.angularVelocity = lerpAngularVelocity;
                rb.velocity = Vector2.zero;
                rb.angularVelocity = 0;
            }

            rb.isKinematic = false;

            if (Time.time > lastRecord + recordInterval)
            {

                posRots.Add(new PosRot(transform.position, transform.rotation, Time.time, rb.velocity, rb.angularVelocity));
                if (posRots.Count > maxPosRots) posRots.RemoveAt(0);
                lastRecord = Time.time;
            }

            if (posRots.Count > 2)
            {
                posRotRight = posRots.Count - 1;
                posRotLeft = posRots.Count - 2;
            }

            stopTime = Time.time;
        }
        else
        {
            rb.isKinematic = true;
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0;

            timeline = stopTime + timePosition;

            if (timeline > posRots[posRotRight].timeMark)
            {
                if (posRotRight < posRots.Count - 1)
                {
                    posRotRight++;
                    posRotLeft++;
                }
            }

            if (timeline < posRots[posRotLeft].timeMark)
            {
                if (posRotLeft > 0)
                {
                    posRotRight--;
                    posRotLeft--;
                }
            }

            lerpVal = 1 - (timeline - posRots[posRotLeft].timeMark) / (posRots[posRotRight].timeMark - posRots[posRotLeft].timeMark);

            transform.position = Vector2.Lerp(posRots[posRotRight].position, posRots[posRotLeft].position, lerpVal);
            transform.rotation = Quaternion.Lerp(posRots[posRotRight].rotation, posRots[posRotLeft].rotation, lerpVal);
            
            lerpVelocity = Vector2.Lerp(posRots[posRotRight].velocity, posRots[posRotLeft].velocity, lerpVal);
            lerpAngularVelocity = Mathf.Lerp(posRots[posRotRight].torque, posRots[posRotLeft].torque, lerpVal);
        }
    }


    private void OnDrawGizmosSelected()
    {
        foreach (PosRot posRot in posRots)
        {
            Gizmos.DrawWireSphere(posRot.position, 0.1f);
            Gizmos.DrawLine(posRot.position, posRot.position + (Vector2)(posRot.rotation * Vector2.up));
        }
    }

}
