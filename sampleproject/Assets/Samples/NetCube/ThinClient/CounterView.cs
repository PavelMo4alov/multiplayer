using System;
using UnityEngine.UI;
using UnityEngine;

public class CounterView : MonoBehaviour
{
    private Text _text;

    private void Start()
    {
        _text = GetComponent<Text>();
    }

    private void Update()
    {
        _text.text = $"Count: {Counter.Count}";
    }
}
