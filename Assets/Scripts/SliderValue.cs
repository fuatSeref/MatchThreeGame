using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SliderValue : MonoBehaviour
{
    [SerializeField] private string propertyName;

  [SerializeField]  private TextMeshProUGUI propertyText;

    public void SetText(float value)
    {
        propertyText.text = propertyName + " " + (int)value;
    }

}
