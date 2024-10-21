using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class MenuObject : MonoBehaviour
{
    private CanvasGroup _cg => GetComponent<CanvasGroup>();

    public MenuType type;

    //Public Data
    public bool isActive = false;
    public string menuName = "Menu_";

    public void OpenMenu()
    {
        isActive = true; 
        gameObject.SetActive(true);
    }

    public void CloseMenu()
    {
        isActive = false; 
        gameObject.SetActive(false);
    }
}