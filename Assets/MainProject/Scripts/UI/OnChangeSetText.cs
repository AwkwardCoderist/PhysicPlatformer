using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OnChangeSetText : MonoBehaviour
{
    private TMP_Text TMP_Text;

    private void Awake()
    {
        TMP_Text = GetComponent<TMP_Text>();
    }

    public void SetText(float value)
    {
        TMP_Text.text = value.ToString();
    }
}
