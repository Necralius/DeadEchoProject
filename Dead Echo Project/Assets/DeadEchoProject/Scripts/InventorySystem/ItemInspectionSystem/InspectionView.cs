using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
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

    private CanvasGroup _cg     => GetComponent<CanvasGroup>();
    RectTransform       _rect   => GetComponent<RectTransform>();

    [Header("Item Inspection")]
    [SerializeField] private TextMeshProUGUI    _itemName          = null;
    [SerializeField] private TextMeshProUGUI    _itemDescription   = null;
    [SerializeField] private Image              _itemImage         = null;

    [Header("Dependencies")]
    [SerializeField] private Button             _inpsectItem        = null;
    [SerializeField] private Button             _dropItem           = null;
    [SerializeField] private Button             _equipItem          = null;
    [SerializeField] private ObjectInspector    _objectInspector    = null;

    [Header("Item")]
    //[SerializeField] private ItemData           _selectedItem       = null;
    [SerializeField] private InventoryItem _selectedItem = null;
    
    InputManager inptManager = null;

    public Button   DropButton   { get => _dropItem;     }
    public InventoryItem SelectedItem { get => _selectedItem; }

    private void Start()
    {
        inptManager         = GetComponent<InputManager>();
        _cg.blocksRaycasts  = false;
        _inpsectItem?.onClick.AddListener(delegate { _objectInspector?.Inspect(_selectedItem.data); });
        _dropItem?.onClick.AddListener(delegate { InventoryController.Instance.DropItem(); });
    }

    public void InspectItem(InventoryItem item)
    {
        if (_itemImage      == null) return;
        if (_itemName       == null) return;
        if (_inpsectItem    == null) return;
        if (_cg             == null) return;
        if (item            == null) return;

        _dropItem.gameObject.SetActive(item.originGrid is not GroundItemGrid);

        ChangeState(true);
        _selectedItem       = item;

        if (_cg.interactable)
        {
            Vector2 position = Input.mousePosition;

            float pivotX = position.x / Screen.width;
            float pivotY = position.y / Screen.height;

            _rect.pivot = new Vector2(pivotX, pivotY);
            transform.position = position;
        }

        if (item.data is NodeItem) 
            _inpsectItem.gameObject.SetActive(true);
        else _inpsectItem.gameObject.SetActive(false);


        _itemImage.sprite   = _selectedItem.data.Icon;
        _itemName.text      = _selectedItem.data.Name;
    }

    public void ChangeState(bool state)
    {
        _cg.alpha           = state ? 1 : 0;
        _cg.interactable    = state;
        _cg.blocksRaycasts  = state;
    }
}