using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Formula Limits", menuName = "ScriptableObjects/Formula Limits")]
public class FormulaLimits : ScriptableObject
{
    public Vector2 massLimit;
    public Vector2 volumeLimit;
    public Vector2 timeScaleLimit;
    public Vector2 elastityKoefLimit;
    public Vector2 heightLimit;
    public Vector2 widthtLimit;
    public Vector2 frictionKoefLimit;
}
