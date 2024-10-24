using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    public static InventoryController Instance;
    
    [SerializeField] private ItemGrid selectedItemGrid = null;

    public GroundItemGrid groundGrid    = null;
    public ItemGrid       inventoryGrid = null;

    public ItemGrid SelectedItemGrid { get => selectedItemGrid; 
        set
        {
            selectedItemGrid = value;
            _highlighter?.SetParent(value);
        }     
    }

    private InputManager _inputManager;

    Vector2Int? prevSelectedItemPos;
    [SerializeField] InventoryItem    selectedItem;
    [SerializeField] private ItemGrid lastSelectedItemGrid = null;
    InventoryItem overlapItem;

    RectTransform selectedItemRect;

    [SerializeField] private List<ItemData> items;
    [SerializeField] private InventoryHighlight _highlighter;

    public GameObject       itemPrefab;
    public Transform        canvasTrans;
    private InventoryItem   itemToHighlight;

    Vector2Int oldPosition;

    private void Awake()
    {
        if (Instance != null) 
            Destroy(Instance.gameObject);
        Instance        = this;
        _highlighter    = GetComponent<InventoryHighlight>();
    }

    private void Start()
    {
        _inputManager = GameSceneManager.Instance?.inputManager;
    }

    private void Update()
    {
        ItemDrag();

        if (_inputManager.F_Action.Action.WasPressedThisFrame()) 
            CreateRandomItem();

        if (_inputManager.R_Action.Action.WasPressedThisFrame()) 
            RotateItem();

        if (_inputManager.Z_Action.Action.WasPressedThisFrame()) 
            InsertRandomItem();

        if (selectedItemGrid == null || !GameSceneManager.Instance.inventoryIsOpen)
        {
            _highlighter?.SetState(false);
            return;
        }

        HandleHighlight();

        if (_inputManager.mouseLeftAction.Action.WasPressedThisFrame())
        {
            ItemGet();
            InspectionView.Instance.ChangeState(false);
        }

        if (_inputManager.mouseRightAction.Action.WasPressedThisFrame())
        {
            InspectionView.Instance.ChangeState(false);

            InventoryItem item = GetItemReference();
            if (item.originGrid is GroundItemGrid || item == null)
                return;
            else InspectionView.Instance?.InspectItem(item);
        }
    }

    public void ChangeInventoryState(bool state)
    {
        if (!state)
        {
            if (selectedItem != null && prevSelectedItemPos != null)
            {
                ItemPlace((Vector2Int)prevSelectedItemPos);
                selectedItem            = null;
                prevSelectedItemPos     = null;
                lastSelectedItemGrid    = null;
            }
        }
    }

    private void RotateItem() => selectedItem?.Rotate();

    private void InsertRandomItem()
    {
        if (selectedItemGrid == null) 
            return;

        CreateRandomItem();

        InventoryItem itemToInsert = selectedItem;
        selectedItem = null;
        InsertItem(itemToInsert);
    }

    private void InsertItem(InventoryItem itemToInsert)
    {
        Vector2Int? posOnGrid = selectedItemGrid?.FindSpaceForItem(itemToInsert);

        if (posOnGrid == null)
            return;

        selectedItemGrid?.PlaceItem(itemToInsert, posOnGrid.Value.x, posOnGrid.Value.y);
    }

    private void CreateRandomItem()
    {
        InventoryItem inventoryItem = Instantiate(itemPrefab).GetComponent<InventoryItem>();
        selectedItem = inventoryItem;

        selectedItemRect = inventoryItem?.GetComponent<RectTransform>();
        selectedItemRect?.SetParent(canvasTrans);
        selectedItemRect?.SetAsLastSibling();

        int selectedItemID = Random.Range(0, items.Count);
        inventoryItem?.Set(items[selectedItemID], SelectedItemGrid);
    }

    private void HandleHighlight()
    {
        Vector2Int positonOnGrid = GetTileGridPosition();

        if (oldPosition == positonOnGrid) 
            return;

        oldPosition = positonOnGrid;

        if (selectedItem == null && selectedItemGrid != null)
        {
            itemToHighlight = selectedItemGrid?.GetItem(positonOnGrid.x, positonOnGrid.y);

            if (itemToHighlight != null)
            {
                _highlighter?.SetState(true);
                _highlighter?.SetSize(itemToHighlight);
                _highlighter?.SetPosition(selectedItemGrid, itemToHighlight);
            }
            else _highlighter?.SetState(false);
        }
        else
        {
            _highlighter?.SetState(selectedItemGrid.BoundryCheck(positonOnGrid.x, positonOnGrid.y, selectedItem));
            _highlighter?.SetSize(selectedItem);
            _highlighter?.SetPosition(selectedItemGrid, selectedItem, positonOnGrid.x, positonOnGrid.y);
        }
    }

    private void ItemGet()
    {
        Vector2Int? tileGridPos = prevSelectedItemPos = GetTileGridPosition();

        if (selectedItem == null && tileGridPos != null)
            PickUpItem((Vector2Int)tileGridPos);
        else 
            ItemPlace((Vector2Int)tileGridPos);
    }

    public void DropItem()
    {
        if (groundGrid != null)
        {
            InventoryItem item = InspectionView.Instance.SelectedItem;

            if (item == null || inventoryGrid == null)
                return;

            Vector2Int? pos = groundGrid.FindSpaceForItem(item);

            if (pos == null) 
                return;
            else
            {
                inventoryGrid.CleanItem(item);
                ItemPlace((Vector2Int)pos, groundGrid, InspectionView.Instance.SelectedItem);
            }
        }

        InspectionView.Instance.ChangeState(false);
    }

    private InventoryItem GetItemReference()
    {
        Vector2Int tileGridPos  = GetTileGridPosition();
        return selectedItemGrid?.GetItem(tileGridPos.x, tileGridPos.y);
    }

    private Vector2Int GetTileGridPosition()
    {
        Vector2 mousePos = Input.mousePosition;

        if (selectedItem != null)
        {
            mousePos.x -= (selectedItem.WIDTH - 1) * ItemGrid.tileSizeWidth / 2;
            mousePos.y += (selectedItem.HEIGHT - 1) * ItemGrid.tileSizeHeight / 2;
        }

        return selectedItemGrid.GetTileGridPosition(mousePos);
    }

    private void ItemPlace(Vector2Int tileGridPos)
    {
        if (selectedItemGrid.PlaceItem(selectedItem, tileGridPos.x, tileGridPos.y, ref overlapItem))
        {
            selectedItem.originGrid = selectedItemGrid;
            selectedItem = null;
            if (overlapItem != null)
            {
                selectedItem        = overlapItem;
                overlapItem         = null;
                selectedItemRect    = selectedItem?.GetComponent<RectTransform>();
                selectedItemRect?.SetAsLastSibling();
            }
        }

        lastSelectedItemGrid = null;
    }

    private bool ItemPlace(Vector2Int tileGridPos, ItemGrid grid, InventoryItem item)
    {
        if (item != null)
            selectedItem = item;

        bool result = grid.PlaceItem(selectedItem, tileGridPos.x, tileGridPos.y, ref overlapItem);
        if (result)
        {
            selectedItem.originGrid = grid;

            selectedItem = null;
            if (overlapItem != null)
            {
                selectedItem = overlapItem;
                overlapItem = null;
                selectedItemRect = selectedItem?.GetComponent<RectTransform>();
                selectedItemRect?.SetAsLastSibling();
            }
        }

        return result;
    }

    private void PickUpItem(Vector2Int tileGridPosition)
    {
        selectedItem = selectedItemGrid?.PickUpItem(tileGridPosition.x, tileGridPosition.y);
        if (selectedItem != null)
            selectedItemRect = selectedItem?.GetComponent<RectTransform>();
        lastSelectedItemGrid = selectedItemGrid;
    }

    private void ItemDrag()
    {
        if (selectedItem != null)
        {
            selectedItemRect.position = Input.mousePosition;
            selectedItemRect?.SetParent(canvasTrans);
        }
    }
}