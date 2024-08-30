using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class AB_Extension : MonoBehaviour
{
    [HideInInspector] public TextMeshProUGUI childText = null;
    public bool         selected    = false;
    public GameObject   pointer     = null;

    public UnityEvent OnSelect   = null;
    public UnityEvent OnDeselect = null;

    private void Awake()
    {
        childText = GetComponentInChildren<TextMeshProUGUI>();
    }
}