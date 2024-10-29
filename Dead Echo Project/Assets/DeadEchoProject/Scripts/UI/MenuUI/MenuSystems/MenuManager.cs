using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum MenuType { Additive, Override}
public class MenuManager : MonoBehaviour
{
    [SerializeField] private List<MenuData> menuItems = new List<MenuData>();

    [SerializeField] private MenuData _selectedMenu = null;
    [SerializeField] private MenuData _lastMenu     = null;
    public void OpenMenu(string menuName)
    {
        MenuData menu = menuItems.Find(e => e.tag == menuName);

        if (menu == null)
            return;

        _lastMenu = _selectedMenu;
        _lastMenu.Deactivate();

        menu.Activate();

        _selectedMenu = menu;
    }

    public void OpenMenu(string menuName, Action actionOnOpen)
    {
        MenuData menu = menuItems.Find(e => e.tag == menuName);

        if (menu == null)
            return;

        _lastMenu = _selectedMenu;
        _lastMenu.Deactivate();

        menu.Activate(actionOnOpen);

        _selectedMenu = menu;
    }

    public void CloseAllMenus(Action action)
    {
        if (_selectedMenu == null)
            return;
        else
            _selectedMenu.Deactivate(action);
    }
}

[Serializable]
public class MenuData
{
    public  string       tag                = string.Empty;
    public  GameObject   panel              = null;
    public  UnityEvent   OnActive           = null;
    public  UnityEvent   OnDeactive         = null;
    public  UnityEvent   OnDeactiveFadeEnd  = null;
    public  UnityEvent   OnActiveFadeEnd    = null;
    public  float        fadeTime           = 1f;
    private CanvasGroup  _cg                = null;

    private CanvasGroup CG
    {
        get
        {
            _cg = panel.GetComponent<CanvasGroup>();
            if (_cg == null)
                _cg = panel.AddComponent<CanvasGroup>();
            return _cg;
        }
    }

    public void Activate()
    {
        OnActive?.Invoke();

        CG.DOFade(1f, fadeTime).SetUpdate(true).onComplete += delegate { OnActiveFadeEnd.Invoke(); };
        CG.interactable     = true;
        CG.blocksRaycasts   = true;
    }

    public void Deactivate()
    {
        CG.DOFade(0f, fadeTime).SetUpdate(true).onComplete += delegate { OnDeactiveFadeEnd.Invoke(); };
        CG.interactable     = false;
        CG.blocksRaycasts   = false;
    }

    public void Activate(Action action)
    {
        OnActive?.Invoke();

        CG.DOFade(1f, fadeTime).SetUpdate(true).onComplete += delegate { action.Invoke(); OnActiveFadeEnd.Invoke(); };
        CG.interactable     = true;
        CG.blocksRaycasts   = true;
    }

    public void Deactivate(Action action)
    {
        CG.DOFade(0f, fadeTime).SetUpdate(true).onComplete += delegate { action.Invoke(); OnDeactiveFadeEnd.Invoke(); };
        CG.interactable     = false;
        CG.blocksRaycasts   = false;
    }
}