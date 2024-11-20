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
    [SerializeField] private TextMeshProUGUI    _itemType          = null;
    [SerializeField] private TextMeshProUGUI    _itemDescription   = null;
    [SerializeField] private Image              _itemImage         = null;

    [Header("Dependencies")]
    [SerializeField] private Button             _dropItem           = null;
    [SerializeField] private Button             _itemInteract           = null;
    [SerializeField] private ObjectInspector    _objectInspector    = null;

    [Header("Item")]
    [SerializeField] private InventoryItem  _selectedItem   = null;
    [SerializeField] private Vector2        _posOffset      = new Vector2();

    InputManager inptManager = null;

    public Button   DropButton   { get => _dropItem;     }
    public InventoryItem SelectedItem { get => _selectedItem; }

    private void Start()
    {
        inptManager         = GetComponent<InputManager>();
        _cg.blocksRaycasts  = false;
    }

    public void InspectItem(InventoryItem item)
    {
        if (_itemImage          == null 
            || _itemName        == null  
            || _cg              == null 
            || item             == null
            || item.originGrid  == null) return;

        _selectedItem = item;
        _itemInteract.gameObject.SetActive(true);

        _dropItem?.     onClick.RemoveAllListeners();
        _itemInteract?. onClick.RemoveAllListeners();

        if (item.originGrid != null)
        {
            string dropLabel = item.originGrid is GroundItemGrid ? "Get" : "Drop";

            _dropItem.GetComponent<TextMeshProUGUI>().text = dropLabel;

            if (item.originGrid is GroundItemGrid)
                _dropItem?.onClick.AddListener(() => InventoryController.Instance.SelectedItemGrid.PlaceItem(item));
            else if (item.originGrid is not GroundItemGrid)
                _dropItem?.onClick.AddListener(() => InventoryController.Instance.DropCurrentItem());

            _dropItem.onClick.AddListener(() => ChangeState(false));
        }

        TextMeshProUGUI interactText = _itemInteract.GetComponent<TextMeshProUGUI>();

        switch(item.data.Type)
        {
            case ItemType.Consumable:
            {
                interactText.text = "Use";
                _itemInteract.onClick.AddListener(() => InventoryController.Instance.UseItem());
            }  break;
            case ItemType.Inspection:
            {
                interactText.text = "Inspect";
                _itemInteract.onClick.AddListener(() => _objectInspector.Inspect(item.data));
            } break;
            default:
            {
                interactText.text = "Equip";
                _itemInteract.onClick.AddListener(() => InventoryController.Instance.EquipItem());
            } break;
        }

        _itemInteract.onClick.AddListener(() => ChangeState(false));

        ChangeState(true);

        if (_cg.interactable)
        {
            Vector2 position = Input.mousePosition;

            float pivotX = position.x / Screen.width;
            float pivotY = position.y / Screen.height;

            _rect.pivot = new Vector2(pivotX, pivotY);
            transform.position = _posOffset + position;
        }


        _itemImage.sprite       = _selectedItem.data.Icon;
        _itemName.text          = _selectedItem.data.Name;
        _itemDescription.text   = _selectedItem.data.Description;
        _itemType.text          = _selectedItem.data.Type.ToString();
    }

    public void ChangeState(bool state)
    {
        _cg.alpha           = state ? 1 : 0;
        _cg.interactable    = state;
        _cg.blocksRaycasts  = state;
    }
}