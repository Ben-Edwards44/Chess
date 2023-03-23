using UnityEngine;
using UnityEngine.UI;

public class sliderValue : MonoBehaviour
{
    public Text valueText;

    public void updateValueText(float value)
    {
        valueText.text = value.ToString("0.0") + "s";
    }
}
