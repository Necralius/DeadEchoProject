using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionSelector : MonoBehaviour
{
    [SerializeField] private List<ActionButton> _buttons;

    public void SelectButton(ActionButton button)
    {
        foreach(var btn  in _buttons)
        {
            if (btn == button) btn.selected = true;
            else btn.selected = false;
        }
    }
}