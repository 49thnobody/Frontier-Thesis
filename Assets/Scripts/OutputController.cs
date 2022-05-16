using TMPro;
using UnityEngine;

public class OutputController : MonoBehaviour
{
    protected TextMeshProUGUI _text;
    public int Value { get; private set; }

    private void Awake()
    {
        _text = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void UpdateValue(int value)
    {
        Value = value;
        _text.text = Value.ToString();
    }
}