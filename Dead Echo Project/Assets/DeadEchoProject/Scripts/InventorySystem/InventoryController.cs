using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    public static InventoryController Instance;
    
    [SerializeField] private ItemGrid selectedItemGrid = null;
    [SerializeField] private ItemGrid defaultItemGrid  = null;

    public GroundItemGrid groundGrid    = null;

    public ItemGrid SelectedItemGrid { get => selectedItemGrid == null ? defaultItemGrid : selectedItemGrid; 
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
        if (CharacterManager.Instance.isDead || !GameSceneManager.Instance.inventoryIsOpen)
        {
            _highlighter?.SetState(!(selectedItemGrid == null));
            return;
        }

        ItemDrag();
        HandleHighlight();

        if (_inputManager.R_Action.Action.WasPressedThisFrame()) 
            RotateItem();

        if (_inputManager.mouseLeftAction.Action.WasPressedThisFrame()) 
            ItemGet();

        if (_inputManager.mouseRightAction.Action.WasPressedThisFrame())
            InspectionView.Instance?.InspectItem(GetItemReference());

        if (_inputManager.F_Action.Action.WasPressedThisFrame())
            CreateRandomItem();
        if (_inputManager.Z_Action.Action.WasPressedThisFrame())
            InsertRandomItem();
    }

    private void InsertRandomItem()
    {
        if (selectedItemGrid == null)
            return;

        CreateRandomItem();

        InventoryItem itemToInsert = selectedItem;
        selectedItem = null;
        InsertItem(itemToInsert);
    }

    private void CreateRandomItem()
    {
        InventoryItem randomItem = Instantiate(itemPrefab).GetComponent<InventoryItem>();
        selectedItem = randomItem;

        selectedItemRect = randomItem?.GetComponent<RectTransform>();
        selectedItemRect?.SetParent(canvasTrans);
        selectedItemRect?.SetAsLastSibling();

        int selectedItemID = Random.Range(0, items.Count);
        randomItem?.Set(items[selectedItemID], groundGrid);
    }

    public void ChangeInventoryState(bool state)
    {
        CanvasGroup cg = canvasTrans.GetComponent<CanvasGroup>();

        cg.alpha            = state ? 1 : 0;
        cg.interactable     = state;
        cg.blocksRaycasts   = state;

        if (!state)
        {
            if (selectedItem != null && prevSelectedItemPos != null)
            {
                ItemPlace((Vector2Int)prevSelectedItemPos, lastSelectedItemGrid, selectedItem);
                selectedItem            = null;
                prevSelectedItemPos     = null;
                lastSelectedItemGrid    = null;
            }
        }
    }

    public void UseItem()
    {
        Debug.Log("Using Item!");

    }

    public void EquipItem()
    {
        Debug.Log("Equiping Item!");


    }

    private void RotateItem() => selectedItem?.Rotate();

    private void InsertItem(InventoryItem itemToInsert)
    {
        Vector2Int? posOnGrid = selectedItemGrid?.FindSpaceForItem(itemToInsert);

        if (posOnGrid == null)
            return;

        selectedItemGrid?.PlaceItem(itemToInsert, posOnGrid.Value.x, posOnGrid.Value.y);
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
            if (selectedItem == null)
                return;

            _highlighter?.SetState(SelectedItemGrid.BoundryCheck(positonOnGrid.x, positonOnGrid.y, selectedItem));
            _highlighter?.SetSize(selectedItem);
            _highlighter?.SetPosition(SelectedItemGrid, selectedItem, positonOnGrid.x, positonOnGrid.y);
        }
    }

    private void ItemGet()
    {
        Vector2Int? tileGridPos = prevSelectedItemPos = GetTileGridPosition();

        if (selectedItem == null && tileGridPos != null)
            PickUpItem((Vector2Int)tileGridPos);
        else 
            ItemPlace((Vector2Int)tileGridPos, SelectedItemGrid, selectedItem);
    }

    public void DropCurrentItem()
    {
        if (groundGrid != null)
        {
            InventoryItem item = InspectionView.Instance.SelectedItem;

            if (item == null)
                return;
            SelectedItemGrid.PickUpItem(item.onGridPosX, item.onGridPosY);

            groundGrid.PlaceItem(item);
        }

        InspectionView.Instance.ChangeState(false);
    }

    private InventoryItem GetItemReference()
    {
        Vector2Int tileGridPos  = GetTileGridPosition();
        return SelectedItemGrid?.GetItem(tileGridPos.x, tileGridPos.y);
    }

    private Vector2Int GetTileGridPosition(bool interactionFromGround = false)
    {
        Vector2 mousePos = Input.mousePosition;

        if (selectedItem != null)
        {
            mousePos.x -= (selectedItem.WIDTH - 1) * ItemGrid.tileSizeWidth / 2;
            mousePos.y += (selectedItem.HEIGHT - 1) * ItemGrid.tileSizeHeight / 2;
        }

        return SelectedItemGrid.GetTileGridPosition(mousePos);
    }

    private void ItemPlace(Vector2Int tileGridPos, ItemGrid grid, InventoryItem item)
    {
        ItemGrid currentGrid = grid == null ? SelectedItemGrid : grid;

        bool result = currentGrid.PlaceItem(item, tileGridPos.x, tileGridPos.y, ref overlapItem);

        if (result)
        {
            item.originGrid = currentGrid;
            selectedItem    = null;

            if (overlapItem != null)
            {
                selectedItem        = overlapItem;
                overlapItem         = null;
                selectedItemRect    = selectedItem?.GetComponent<RectTransform>();
                selectedItemRect?.SetAsLastSibling();
            }
            InspectionView.Instance.ChangeState(false);
        }

        lastSelectedItemGrid = null;
    }

    private void PickUpItem(Vector2Int tileGridPosition)
    {
        selectedItem = selectedItemGrid?.PickUpItem(tileGridPosition.x, tileGridPosition.y);
        if (selectedItem != null)
        {
            selectedItemRect = selectedItem?.GetComponent<RectTransform>();
            InspectionView.Instance.ChangeState(false);
        }
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