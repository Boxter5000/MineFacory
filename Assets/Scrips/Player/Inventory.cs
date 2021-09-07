using Scrips.Player;
using Scrips.World;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Inventory : MonoBehaviour
{
    private ItemSlot[,] currentInventorySlots;
    private GameObject[,] itemButtons;
    private ItemSlot cursorSlot;
    private GameObject cursorItem;
    public GameObject itemButtonPrefab;
    public GameObject cursorItemPrefab;

    [SerializeField] private RectTransform inventoryContainerBox;
    [SerializeField] private GameObject verticalParentPrefab;
    [SerializeField] private GameObject horizontalParentPrefab;

    public const int ItemBoxSize = 30;
    public const int ToolbarDividerSize = 15;

    private World world;
    private Canvas myCanvas;
    public Toolbar _toolbar;
    private void Awake()
    {
        world  = GameObject.Find("World").GetComponent<World>();
        myCanvas = transform.parent.GetComponent<Canvas>();
    }
    

    private void Update()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(myCanvas.transform as RectTransform, Input.mousePosition, myCanvas.worldCamera, out var pos);                   /////////                                                              ///////////
        cursorItem.transform.position = myCanvas.transform.TransformPoint(pos);
    }
    public void SetItemSlots(Item[,] playerInventory, Item[,] externalInventory)
    {
        int playerInventoryHorizontalLength = playerInventory.GetLength(1);
        int playerInventoryVerticalLength = playerInventory.GetLength(0);
        int externalInventoryHorizontalLength = externalInventory?.GetLength(1) ?? 0;
        int externalInventoryVerticalLength = externalInventory?.GetLength(0) ?? 0;
        int horizontalSlotAmount = (playerInventoryHorizontalLength > externalInventoryHorizontalLength)
            ? playerInventoryHorizontalLength
            : externalInventoryHorizontalLength;
        int verticalSlotAmount = playerInventoryVerticalLength + externalInventoryVerticalLength;
        int dividerSizes = ToolbarDividerSize;

        currentInventorySlots = new ItemSlot[verticalSlotAmount, horizontalSlotAmount];
        itemButtons = new GameObject[verticalSlotAmount, horizontalSlotAmount];
        
        GameObject verticalParent = Instantiate(verticalParentPrefab, transform);

        if(externalInventory != null)
        {
            //ExternalInventory
            for (int x = 0; x < externalInventoryVerticalLength; x++)
            {
                GameObject currentHorizontalParent = Instantiate(horizontalParentPrefab, verticalParent.transform);
                currentHorizontalParent.GetComponent<RectTransform>().sizeDelta = new Vector2(ItemBoxSize * horizontalSlotAmount, ItemBoxSize);
                for (int z = 0; z < externalInventoryHorizontalLength; z++)
                {
                    int rowIndex = x + playerInventoryVerticalLength;
                    Item currentItem = externalInventory[x,z];
                    currentInventorySlots[rowIndex, z] = new ItemSlot(true);
                    currentInventorySlots[rowIndex,z].SetItem(currentItem);
                
                    itemButtons[rowIndex, z] = Instantiate(itemButtonPrefab,currentHorizontalParent.transform);
                
                    SetItemInSlot(rowIndex, z, currentItem);
                
                }
            }

            //Divider Between ExternalInventory and Inventory
            GameObject externalInventoryDivider = Instantiate(horizontalParentPrefab, verticalParent.transform);
            externalInventoryDivider.GetComponent<RectTransform>().sizeDelta = new Vector2(ItemBoxSize * horizontalSlotAmount, ToolbarDividerSize);
            dividerSizes *= 2;
        }
        
        //Inventory
        for (int x = 1; x < playerInventoryVerticalLength; x++)
        {
            GameObject currentHorizontalParent = Instantiate(horizontalParentPrefab, verticalParent.transform);
            currentHorizontalParent.GetComponent<RectTransform>().sizeDelta = new Vector2(ItemBoxSize * horizontalSlotAmount, ItemBoxSize);
            for (int z = 0; z < playerInventoryHorizontalLength; z++)
            {
                Item currentItem = playerInventory[x,z];
                
                currentInventorySlots[x, z] = new ItemSlot(false);
                currentInventorySlots[x,z].SetItem(currentItem);
                
                itemButtons[x, z] = Instantiate(itemButtonPrefab,currentHorizontalParent.transform);
                
                SetItemInSlot(x, z, currentItem);
                
            }
        }

        //Divider Between Inventory and ToolbarClone
        GameObject toolbarDivider = Instantiate(horizontalParentPrefab, verticalParent.transform);
        toolbarDivider.GetComponent<RectTransform>().sizeDelta = new Vector2(ItemBoxSize * horizontalSlotAmount, ToolbarDividerSize);
        
        
        //ToolbarClone-Row
        GameObject toolbarClone = Instantiate(horizontalParentPrefab, verticalParent.transform);
        toolbarClone.GetComponent<RectTransform>().sizeDelta = new Vector2(ItemBoxSize * horizontalSlotAmount, ItemBoxSize);
        for (int z = 0; z < playerInventoryHorizontalLength; z++)
        {
            Item currentItem = playerInventory[0,z];
                
            currentInventorySlots[0,z] = new ItemSlot(true);
            currentInventorySlots[0,z].SetItem(currentItem);
                
            itemButtons[0,z] = Instantiate(itemButtonPrefab,toolbarClone.transform);
                
            SetItemInSlot(0, z, currentItem);
        }
        
        //Positioning and Scaling of InventoryBox
        verticalParent.GetComponent<RectTransform>().sizeDelta = new Vector2(ItemBoxSize * horizontalSlotAmount, ItemBoxSize * verticalSlotAmount + dividerSizes);
        verticalParent.GetComponent<RectTransform>().position += new Vector3(0, -10, 0);
        inventoryContainerBox.sizeDelta = new Vector2(ItemBoxSize * (horizontalSlotAmount + 1), ItemBoxSize * (verticalSlotAmount + 1.5f) + dividerSizes);
        inventoryContainerBox.position += new Vector3(0, 10, 0);
        

        //CursorSlot
        cursorSlot = new ItemSlot(true);
        cursorSlot.SetItem(new Item(0,0));
        cursorItem = Instantiate(cursorItemPrefab, transform);
        cursorItem.transform.GetChild(0).GetComponent<Image>().sprite = world.blocktypes[0].icon;
        cursorItem.transform.GetChild(1).GetComponent<Text>().text = " ";
    }

    public void CloseInventory()
    {
        Destroy(cursorItem);
    }

    private void SetItemInSlot(int posX, int posY , Item newItem)
    {
        GameObject newButton = itemButtons[posX, posY];
        UpdateItem(newButton, newItem);
        
        newButton.GetComponent<Button>().onClick.AddListener(delegate { SwitchItems(posX, posY);});
    }

    private void SwitchItems(int posX, int posY)
    {
        ItemSlot clickedButton = currentInventorySlots[posX, posY];
        Item clickedButtonItem = clickedButton.GetItem();
        byte clickedButtonItemID = clickedButtonItem.ItemID;
        byte clickedButtonStackSize = clickedButtonItem.StackSize;

        Item cursorCurrentItem = cursorSlot.GetItem();
        byte cursorItemID = cursorCurrentItem.ItemID;
        byte cursorStackSize = cursorCurrentItem.StackSize;

        if (clickedButton.GetCanChange())
        {
            clickedButtonItem.ItemID = cursorItemID;
            clickedButtonItem.StackSize = cursorStackSize;
            if(posX == 0)
                _toolbar.ChangeItem(posY,clickedButtonItem);
            clickedButton.SetItem(clickedButtonItem);
            UpdateItem(itemButtons[posX, posY],clickedButtonItem);
        }
        if (cursorSlot.GetCanChange())
        {
            cursorCurrentItem.ItemID = clickedButtonItemID;
            cursorCurrentItem.StackSize = clickedButtonStackSize;
            cursorSlot.SetItem(cursorCurrentItem);
            UpdateItem(cursorItem,cursorCurrentItem);
        }

    }

    private void UpdateItem(GameObject itemBoxToUpdate, Item newItem)
    {
        itemBoxToUpdate.transform.GetChild(0).GetComponent<Image>().sprite = world.blocktypes[newItem.ItemID].icon;
        itemBoxToUpdate.transform.GetChild(1).GetComponent<Text>().text = (newItem.StackSize == 0 || newItem.ItemID == 0) ? "" : newItem.StackSize.ToString();
    }
}
