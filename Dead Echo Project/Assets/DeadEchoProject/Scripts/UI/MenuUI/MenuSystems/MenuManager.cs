using System;
using System.Collections.Generic;
using UnityEngine;

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

        if (menu != null)
        {
            if (_lastMenu != null)
                _lastMenu.Deactivate();

            _selectedMenu = menu;

            _selectedMenu.Activate();
        }       
    }

    public void OpenMenu(string menuName)
    {
        if (menuObjects.Find(e => e.menuName == menuName))
        {
            bool Overridable = menuObjects.Find(e => e.menuName == menuName).type == MenuType.Override;

            menuObjects.ForEach(e => { if (e.menuName == menuName) e.OpenMenu(); else if (Overridable) e.CloseMenu(); });
        }
        else Debug.LogWarning("This object is not in the object list");
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
    public string       tag     = string.Empty;
    public GameObject   panel   = null;
    public CanvasGroup  cg      = null;

    public void Activate()
    {
        if (cg == null)
            cg = panel.AddComponent<CanvasGroup>();

        cg.alpha            = 1.0f;
        cg.interactable     = true;
        cg.blocksRaycasts   = true;
    }

    public void Deactivate()
    {
        if (cg == null)
            cg = panel.AddComponent<CanvasGroup>();

        cg.alpha            = 0f;
        cg.interactable     = false;
        cg.blocksRaycasts   = false;
    }
}