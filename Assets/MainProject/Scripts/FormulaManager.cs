using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FormulaManager : MonoBehaviour
{
    public static FormulaManager instance;

    [SerializeField] private GameObject forceFormulaUI;

    private GameObject activeObject;
    private Formula activeFormula;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this);
    }

    public void RecieveObject(FormulaReciever sender, Formula formula)
    {
        activeObject = sender.gameObject;
        activeFormula = formula;



    }

}
