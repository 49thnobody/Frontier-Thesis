using UnityEngine;
using UnityEngine.UI;

public class OutputController : MonoBehaviour
{
    protected Text _text;
    public int Value { get; private set; }

    private void Awake()
    {
        _text = GetComponentInChildren<Text>();
    }

    public void UpdateValue(int value)
    {
        Value = value;
        _text.text = Value.ToString();
    }
}