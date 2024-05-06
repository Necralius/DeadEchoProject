using TMPro;
using UnityEngine;

public class AB_Extension : MonoBehaviour
{
    [HideInInspector] public TextMeshProUGUI childText = null;
    public bool         selected    = false;
    public GameObject   pointer     = null;

    private void Awake()
    {
        childText = GetComponentInChildren<TextMeshProUGUI>();
    }
}