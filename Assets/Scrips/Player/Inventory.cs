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

    [SerializeField] private GameObject verticalParentPrefab;
    [SerializeField] private GameObject horiontalParentPrefab;

    [SerializeField] private int itemBoxSize = 30;
    [SerializeField] private int toolbarDividerSize = 15;

    private World world;
    private Canvas myCanvas;
    private void Awake()
    {
        world  = GameObject.Find("World").GetComponent<World>();
        myCanvas = transform.parent.GetComponent<Canvas>();
    }
    

    private void Update()
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(myCanvas.transform as RectTransform, Input.mousePosition, myCanvas.worldCamera, out pos);                   /////////                                                              ///////////
        //cursorItem.transform.position = myCanvas.transform.TransformPoint(pos);
    }

    /*public void GenerateInventorySlots()
    {
        int xStart = inventorySizeX / 2;
        int yStart = inventorySizeY / 2;

        byte itemIndex = (byte)(world.blocktypes.Length - 1);
        int stackSize = 64;
        
        for (int y = -yStart; y <= yStart; y++)
        {
            for (int x = -xStart; x <= xStart; x++)
            {
                if (itemIndex > 0)
                    itemIndex--;


                
                itemSlots[x + xStart, y + yStart] = new ItemSlot(itemIndex, stackSize, true);
                
                itemButtons[x + xStart, y + yStart] = Instantiate(itemButton,transform);
                itemButtons[x + xStart, y + yStart].transform.position = new Vector3((x * itemSlotDistance) + (resX * offsetPercent.x),(-y * itemSlotDistance) + (resY * offsetPercent.y) , 0);

                
                SetItemInSlot(x + xStart, y + yStart, itemIndex, stackSize);
                
            }
        }

        cursorSlot = new ItemSlot(0, 0, true);
        cursorItem = Instantiate(cursorItemPrefab, transform);
        cursorItem.transform.GetChild(0).GetComponent<Image>().sprite = world.blocktypes[0].icon;
        cursorItem.transform.GetChild(1).GetComponent<Text>().text = " ";
    }*/
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
        int dividerSizes = toolbarDividerSize;

        currentInventorySlots = new ItemSlot[verticalSlotAmount, horizontalSlotAmount];
        itemButtons = new GameObject[verticalSlotAmount, horizontalSlotAmount];
        
        GameObject verticalParent = Instantiate(verticalParentPrefab, transform);

        if(externalInventory != null)
        {
            //ExternalInventory
            for (int x = 0; x < externalInventoryVerticalLength; x++)
            {
                GameObject currentHorizontalParent = Instantiate(horiontalParentPrefab, verticalParent.transform);
                currentHorizontalParent.GetComponent<RectTransform>().sizeDelta = new Vector2(itemBoxSize * horizontalSlotAmount, itemBoxSize);
                for (int z = 0; z < externalInventoryHorizontalLength; z++)
                {
                    int rowIndex = x + playerInventoryVerticalLength;
                    Item currentItem = externalInventory[x,z];
                    currentInventorySlots[rowIndex, z] = new ItemSlot(true);
                    currentInventorySlots[rowIndex,z].SetItem(currentItem);
                
                    itemButtons[rowIndex, z] = Instantiate(itemButtonPrefab,currentHorizontalParent.transform);
                
                    SetItemInSlot(rowIndex, z, currentItem.ItemID, currentItem.StackSize);
                
                }
            }

            //Divider Between ExternalInventory and Inventory
            GameObject ExternalInventoryDivider = Instantiate(horiontalParentPrefab, verticalParent.transform);
            ExternalInventoryDivider.GetComponent<RectTransform>().sizeDelta = new Vector2(itemBoxSize * horizontalSlotAmount, toolbarDividerSize);
            dividerSizes *= 2;
        }
        else
        {
            
        }
        
        //Inventory
        for (int x = 1; x < playerInventoryVerticalLength; x++)
        {
            GameObject currentHorizontalParent = Instantiate(horiontalParentPrefab, verticalParent.transform);
            currentHorizontalParent.GetComponent<RectTransform>().sizeDelta = new Vector2(itemBoxSize * horizontalSlotAmount, itemBoxSize);
            for (int z = 0; z < playerInventoryHorizontalLength; z++)
            {
                Item currentItem = playerInventory[x,z];
                
                currentInventorySlots[x, z] = new ItemSlot(false);
                currentInventorySlots[x,z].SetItem(currentItem);
                
                itemButtons[x, z] = Instantiate(itemButtonPrefab,currentHorizontalParent.transform);
                
                SetItemInSlot(x, z, currentItem.ItemID, currentItem.StackSize);
                
            }
        }

        //Divider Between Inventory and Toolbarclone
        GameObject toolbarDivider = Instantiate(horiontalParentPrefab, verticalParent.transform);
        toolbarDivider.GetComponent<RectTransform>().sizeDelta = new Vector2(itemBoxSize * horizontalSlotAmount, toolbarDividerSize);
        
        
        //ToolbarClone-Row
        GameObject toolbarClone = Instantiate(horiontalParentPrefab, verticalParent.transform);
        toolbarClone.GetComponent<RectTransform>().sizeDelta = new Vector2(itemBoxSize * horizontalSlotAmount, itemBoxSize);
        for (int z = 0; z < playerInventoryHorizontalLength; z++)
        {
            Item currentItem = playerInventory[0,z];
                
            currentInventorySlots[0,z] = new ItemSlot(true);
            currentInventorySlots[0,z].SetItem(currentItem);
                
            itemButtons[0,z] = Instantiate(itemButtonPrefab,toolbarClone.transform);
                
            SetItemInSlot(0, z, currentItem.ItemID, currentItem.StackSize);
        }
        
        //Positioning and Scaling of InventoryBox
        verticalParent.GetComponent<RectTransform>().sizeDelta = new Vector2(itemBoxSize * horizontalSlotAmount, itemBoxSize * verticalSlotAmount + dividerSizes);
        verticalParent.GetComponent<RectTransform>().position += new Vector3(0, -10, 0);
        

        //CursorSlot
        cursorSlot = new ItemSlot(true);
        cursorSlot.SetItem(new Item(0,0));
        cursorItem = Instantiate(cursorItemPrefab, transform);
        cursorItem.transform.GetChild(0).GetComponent<Image>().sprite = world.blocktypes[0].icon;
        cursorItem.transform.GetChild(1).GetComponent<Text>().text = " ";
    }

    private void SetItemInSlot(int posX, int posY , byte id, int stacksize)
    {
        GameObject newButton = itemButtons[posX, posY];
        UpdateItem(newButton, id, stacksize);
        
        newButton.GetComponent<Button>().onClick.AddListener(delegate { SwitchItems(posX, posY);});
    }

    public void SwitchItems(int posX, int posY)
    {
        ItemSlot clickedButton = currentInventorySlots[posX, posY];
        byte clickedButtonItem = clickedButton.GetItemID();
        byte clickedButtonStackSize = clickedButton.GetStackSize();
        
        byte cursorItemID = cursorSlot.GetItemID();
        byte cursorStackSize = cursorSlot.GetStackSize();

        if (clickedButton.GetCanChange())
        {
            clickedButton.SetItemID(cursorItemID);
            clickedButton.SetStackSize(cursorStackSize);
            UpdateItem(itemButtons[posX, posY],cursorItemID,cursorStackSize);
        }
        if (cursorSlot.GetCanChange())
        {
            cursorSlot.SetItemID(clickedButtonItem);
            cursorSlot.SetStackSize(clickedButtonStackSize);
            UpdateItem(cursorItem,clickedButtonItem,clickedButtonStackSize);
        }

    }

    private void UpdateItem(GameObject itemboxToUpdate, byte newItemId, int newStackSize)
    {
        itemboxToUpdate.transform.GetChild(0).GetComponent<Image>().sprite = world.blocktypes[newItemId].icon;
        itemboxToUpdate.transform.GetChild(1).GetComponent<Text>().text = (newStackSize == 0 || newItemId == 0) ? "" : newStackSize.ToString();
    }
}
