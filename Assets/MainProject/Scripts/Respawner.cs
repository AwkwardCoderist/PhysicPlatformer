using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Respawner : MonoBehaviour
{
    [SerializeField] private Transform teleportPoint;
    [SerializeField] private float duration = 0.3f;

    private Tweener tweener;

    private Transform objToTeleport;
    private PhysicObject physObj;


    private void Start()
    {
        tweener = teleportPoint.DOScale(1, duration).SetAutoKill(false).Pause();
        tweener.onComplete += EndOfTeleport;
    }

    public void TeleportObject(Transform obj)
    {
        objToTeleport = obj;
        if (objToTeleport.TryGetComponent(out physObj))
        {
            physObj.Freezed = true;
            physObj.ResetSize();
        }
        else
            physObj = null;

        if (objToTeleport.TryGetComponent(out Rigidbody2D rb))
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = 0;
        }
        objToTeleport.SetParent(teleportPoint);
        teleportPoint.localScale = Vector3.zero;
        objToTeleport.localPosition = Vector3.zero;
        objToTeleport.localEulerAngles = Vector3.zero;

        if (objToTeleport.TryGetComponent(out physObj))
        {
            physObj.ResetSize();
        }

        tweener.Restart();

    }

    private void EndOfTeleport()
    {
        if (physObj)
        {
            physObj.Freezed = false;
        }

        objToTeleport.SetParent(null);

    }

}
