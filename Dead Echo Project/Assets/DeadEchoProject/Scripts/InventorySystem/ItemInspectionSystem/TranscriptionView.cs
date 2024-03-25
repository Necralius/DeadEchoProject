using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TranscriptionView : MonoBehaviour
{
    [SerializeField] private Image           _img;
    [SerializeField] private TextMeshProUGUI _transcriptionText;
    [SerializeField] private GameObject      _transcriptionArea;

    public void OpenView(NodeItem item)
    {
        _transcriptionArea.SetActive(true);


    }


}