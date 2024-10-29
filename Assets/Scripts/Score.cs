using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public sealed class Score : MonoBehaviour
{
    public static Score Instance { get; private set; }

    public int _value;
    public int score
    {
        get => _value;

        set
        { 
            if (_value == value) return;
            _value = value;

            score_text.SetText($"Score: {_value}");
        }
    }

    [SerializeField] private TextMeshProUGUI score_text;

    private void Awake() => Instance = this;

    
}
