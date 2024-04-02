using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TranscriptionView : MonoBehaviour
{
    [SerializeField] private Image           _img;
    [SerializeField] private TextMeshProUGUI _transcriptionText;
    [SerializeField] private Button          _transcriptionButton;
    [SerializeField] private GameObject      _transcriptionArea;

    public void OpenView(NodeItem item)
    {
        if (item               == null) return;
        if (_transcriptionArea == null) return;
        if (_transcriptionText == null) return;
        if (_transcriptionArea == null) return;

        _transcriptionArea.SetActive(true);

        _transcriptionText.text = item.Transcription;

        if (item.Font       != null) _transcriptionText.font    = item.Font;
        if (item.TextColor  != null) _transcriptionText.color   = item.TextColor;
        else _transcriptionText.color = Color.white;
    }
}