using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormulaReciever : MonoBehaviour
{
    [SerializeField] private FormulaLimits limits;

    public void RecieveFormula(Formula formula)
    {
        Debug.Log(gameObject.name + " recieve");
        FormulaManager.instance.RecieveObject(this, formula, limits);
    }
}
