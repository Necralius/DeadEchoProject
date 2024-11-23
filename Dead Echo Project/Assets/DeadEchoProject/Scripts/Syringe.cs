using UnityEngine;

public class Syringe : MonoBehaviour
{
    [HideInInspector] public CureItem item = null;

    public void Deactive()
    {
        BodyController.Instance._curing = false;
        gameObject.SetActive(false);
        BodyController.Instance._equippedGun.DrawGun();
    }

    public void Use()
    {
        if (item != null)
            CharacterManager.Instance.CurrentHealth += item.healthToCure;
    }
}