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
    [SerializeField] private Vector2 _size;
    private Vector2 _defaultSize;
    [SerializeField] private float _bounciness = 0;
    [SerializeField] private float _friction = 0.4f;
    [SerializeField] private float _timeScale;
    [SerializeField] private float _timePosition;
    [SerializeField] private float recordInterval;
    [SerializeField] private int maxPosRots;
    private List<PosRot> posRots = new List<PosRot>();
    private float lastRecord = 0;
    private bool doRecords = true;
    public bool DoRecords
    {
        get => doRecords;
        set
        {
            doRecords = value;
            if(!doRecords)
                CreatePosRotPoint();

        }
    }

    public float stopTime = 0;
    public float timeline = 0;
    private float rbTime = 0;
    private int posRotRight;
    private int posRotLeft;
    private float lerpVal;
    private Vector2 lerpVel;
    private float lerpRot;
    private PhysicsMaterial2D physicsMaterial2D;
    private List<Joint2D> stickedObjects = new List<Joint2D>();

    private bool freezed;
    public bool Freezed
    {
        get => freezed;
        set
        {
            freezed = value;
            if (freezed)
            {
                rb.velocity = Vector2.zero;
                rb.angularVelocity = 0;
            }
            else
            {

            }
        }
    }

    public float mass
    {
        get
        {
            return _mass;
        }
        set
        {
            _mass = value;
            if (rb.useAutoMass)
                density = _mass / volume;
            else
                rb.mass = Mathf.Abs(value);
            rb.gravityScale = value;
        }
    }
    public float density
    {
        get
        {
            return col.density;
        }
        set
        {
            col.density = value;
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
            if (rb.useAutoMass)
                density = mass / _volume;
            transform.localScale = _size * value;
            
        }
    }
    public float width
    {
        get
        {
            return _size.x;
        }
        set
        {
            _size.x = value;
            transform.localScale = _size * _volume;
        }
    }
    public float height
    {
        get
        {
            return _size.y;
        }
        set
        {
            _size.y = value;
            transform.localScale = _size * _volume;
        }
    }
    public float bounciness
    {
        get
        {
            return physicsMaterial2D.bounciness;
        }
        set
        {
            _bounciness = value;
        }
    }
    public float friction
    {
        get
        {
            return physicsMaterial2D.friction;
        }
        set
        {
            _friction = value;
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
        if (rb) _mass = rb.mass;
        _volume = 1;
        _size = transform.localScale;
        _defaultSize = _size;
        timeScale = 1;

        physicsMaterial2D = new PhysicsMaterial2D();

    }

    private void FixedUpdate()
    {
        if (!freezed)
        {
            UpdatePhysic();
            UpdateRecording();
        }
    }

    private void UpdatePhysic()
    {
        if (timeScale >= 0)
        {
            if(rb) rb.AddForce(Physics2D.gravity * timeScale, ForceMode2D.Force);
        }

        physicsMaterial2D.bounciness = _bounciness;
        physicsMaterial2D.friction = _friction;

        if(rb) rb.sharedMaterial = physicsMaterial2D;
        if(col) col.sharedMaterial = physicsMaterial2D;

    }

    private void UpdateRecording()
    {
        if (rb)
        {
            if (timeScale > 0)
            {
                rb.isKinematic = false;

                if (lerpVel != Vector2.zero && lerpRot != 0)
                {
                    rb.velocity = lerpVel;
                    rb.angularVelocity = lerpRot;

                    lerpVel = Vector2.zero;
                    lerpRot = 0;
                }


                if (rbTime > lastRecord + recordInterval)
                {
                    CreatePosRotPoint();

                    lastRecord = rbTime;
                }

                if (posRots.Count > 2)
                {
                    posRotRight = posRots.Count - 1;
                    posRotLeft = posRots.Count - 2;
                }

                stopTime = rbTime;
            }
            else
            {
                rb.isKinematic = true;

                timeline = stopTime + timeScale;

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

                rb.velocity = Vector2.zero;
                rb.angularVelocity = 0;

                lerpVel = Vector2.Lerp(posRots[posRotRight].velocity, posRots[posRotLeft].velocity, lerpVal);
                lerpRot = Mathf.Lerp(posRots[posRotRight].torque, posRots[posRotLeft].torque, lerpVal);

            }

            if (rb.velocity.magnitude > 0.01f)
                rbTime += Time.deltaTime * timeScale;

        }
    }

    private void CreatePosRotPoint()
    {
        posRots.Add(new PosRot(transform.position, transform.rotation, rbTime, rb.velocity, rb.angularVelocity));
        if (posRots.Count > maxPosRots) posRots.RemoveAt(0);
    }

    public void ResetSize()
    {
        width = _defaultSize.x;
        height = _defaultSize.y;
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
