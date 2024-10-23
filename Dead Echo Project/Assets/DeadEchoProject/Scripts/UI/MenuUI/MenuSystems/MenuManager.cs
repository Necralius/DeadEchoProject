using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum MenuType { Additive, Override}
public class MenuManager : MonoBehaviour
{
    public List<MenuObject> menuObjects = new List<MenuObject>();

    [SerializeField] private List<MenuData> menuItems = new List<MenuData>();

    [SerializeField] private MenuData _selectedMenu = null;
    [SerializeField] private MenuData _lastMenu     = null;

    public void ActivateMenu(string menuName)
    {
        MenuData menu = menuItems.Find(e => e.tag == menuName);

        if (menu == null)
            return;
        
        if (_lastMenu != null)
            _lastMenu.Deactivate();

        _selectedMenu = menu;

        _selectedMenu.Activate();
    }

    public void OpenMenu(string menuName)
    {
        MenuData menu = menuItems.Find(e => e.tag == menuName);

        if (menu == null)
            return;
        if (_lastMenu != null)
            _lastMenu.Deactivate();

        menu.Activate();

        _lastMenu = menu;
    }
    public void OpenMenu(MenuObject menu)
    {
 
        if (menuObjects.Contains(menu))
        {
            bool Overridable = menu.type == MenuType.Override;
            foreach (var obj in menuObjects)
            {
                if (obj == menu) obj.OpenMenu();
                else if (Overridable) obj.CloseMenu();
            }
        }
        else Debug.LogWarning("This object is not in the object list");
    }
}

[Serializable]
public class MenuData
{
    public  string       tag         = string.Empty;
    public  GameObject   panel       = null;
    public  UnityEvent   OnActive    = null;
    public  float        fadeTime    = 1f;
    private CanvasGroup  _cg         = null;

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

        CG.DOFade(1f, fadeTime);
        CG.interactable     = true;
        CG.blocksRaycasts   = true;
    }

    public void Deactivate()
    {
        CG.DOFade(0f, fadeTime);
        CG.interactable     = false;
        CG.blocksRaycasts   = false;
    }
}