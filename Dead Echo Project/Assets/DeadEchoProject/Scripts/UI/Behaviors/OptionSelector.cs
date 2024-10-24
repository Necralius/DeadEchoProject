using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionSelector : MonoBehaviour
{
    [SerializeField] private List<ActionButton> _buttons;

    public List<ActionButton> Buttons
    {
        get
        {
            _buttons.RemoveAll(e => e == null);
            return _buttons;
        }
    }

    public void SelectButton(ActionButton button)
    {
        foreach(var btn  in Buttons)
        {
            if (btn == button) 
                btn._extension.selected = true;
            else btn._extension.selected = false;

            btn.UpdateState();
        }
    }

    public void DeselectAll()
    {
        foreach(var btn in Buttons)
        {
            btn._extension.selected = false;
            btn.UpdateState();
        }
    }
}