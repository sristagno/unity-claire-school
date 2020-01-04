using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    private Image _bar;
    private Text _text;
    private float _percentage;
    private Color _startColor;

    public float AlertPercentage = 25;
    public Color AlertColor = Color.red;

    public float Percentage
    {
        get { return _percentage; }
        set
        {
            _percentage = value;
            _percentage = Mathf.Clamp(_percentage, 0, 100);
            UpdatePercentage();
        }
    }

    private void UpdatePercentage()
    {
        _text.text = (int) Percentage + " %";
        _bar.fillAmount = _percentage / 100;
        if (_percentage <= AlertPercentage)
        {
            _bar.color = AlertColor;
        }
        else
        {
            _bar.color = _startColor;
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        _bar = transform.Find("Bar").GetComponent<Image>();
        _text = _bar.transform.Find("Text").GetComponent<Text>();
        _startColor = _bar.color;

        Percentage = 100;
    }

    
}
