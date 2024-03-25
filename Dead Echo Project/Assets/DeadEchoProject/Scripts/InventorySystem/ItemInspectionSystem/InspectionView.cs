using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InspectionView : MonoBehaviour
{
    #region - Singleton Pattern -
    public static InspectionView Instance;
    private void Awake()
    {
        if (Instance != null) 
            Destroy(Instance.gameObject); 
        Instance = this;
    }
    #endregion

    [Header("Item Inspection")]
    [SerializeField] private TextMeshProUGUI    _itemName          = null;
    [SerializeField] private TextMeshProUGUI    _itemDescription   = null;
    [SerializeField] private Image              _itemImage         = null;

    [Header("Dependencies")]
    [SerializeField] private Button             _inpsectItem        = null;
    [SerializeField] private ObjectInspector    _objectInspector    = null;
    [SerializeField] private TranscriptionView  _transcriptionView  = null;

    [SerializeField] private ItemData _selectedItem = null;

    private void Start()
    {
        _inpsectItem?.onClick.AddListener(delegate { _objectInspector?.Inspect(_selectedItem); });
    }

    public void InspectItem(InventoryItem item)
    {
        if (_itemImage      == null) return;
        if (_itemName       == null) return;
        if (_inpsectItem    == null) return;

        ItemData selectedItem = item.data;

        _selectedItem       = selectedItem;
        _itemImage.sprite   = selectedItem.Icon;
        _itemName.text      = selectedItem.Name;
    }
}