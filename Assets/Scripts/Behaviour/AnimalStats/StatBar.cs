using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatBar : MonoBehaviour
{
	public Slider slider;
	public Gradient gradient;
	public Image fill;
    public TextMeshProUGUI barName;

    public void SetText(string text)
    {
        barName.text = text;
    }

	public void SetMaxValue(float value)
	{
		slider.maxValue = value;
		slider.value = value;

		fill.color = gradient.Evaluate(1f);
	}

    public void SetValue(float value)
	{
		slider.value = value;
		fill.color = gradient.Evaluate(slider.normalizedValue);
	}
}