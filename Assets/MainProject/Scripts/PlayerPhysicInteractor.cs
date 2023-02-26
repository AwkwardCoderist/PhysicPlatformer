using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerPhysicInteractor : MonoBehaviour
{

    [SerializeField] private GameObject formulasWindow;

    public void OnFormulasWindow(InputValue value)
    {
        formulasWindow.SetActive(!formulasWindow.activeSelf);
    }
}
