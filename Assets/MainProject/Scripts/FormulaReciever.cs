using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormulaReciever : MonoBehaviour
{
    public void RecieveFormula(Formula formula)
    {
        Debug.Log(gameObject.name + " recieve");
    }
}
