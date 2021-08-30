using Scrips.Player;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Inventory : MonoBehaviour
{
    private ItemSlot[,] itemSlots;
    private GameObject[,] itemButtons;
    private ItemSlot cursorSlot;
    private GameObject cursorItem;
    public GameObject cursorItemPrefab;
    public GameObject itemButton;

    public int inventorySizeX = 9;
    public int inventorySizeY = 5;

    [SerializeField] private GameObject verticalParentPrefab;
    [SerializeField] private GameObject horiontalParentPrefab;

    [SerializeField] private int itemBoxSize = 30;
    [SerializeField] private int toolbarDividerSize = 15;
    
    [HideInInspector]public ItemSlot[] toolbarSlots;
    private GameObject[] toolbarItems;

    private World world;
    private Canvas myCanvas;
    private void Awake()
    {
        itemSlots = new ItemSlot[inventorySizeX, inventorySizeY];
        itemButtons = new GameObject[inventorySizeX, inventorySizeY];
        toolbarSlots = new ItemSlot[inventorySizeX];
        toolbarItems = new GameObject[inventorySizeX];
        world  = GameObject.Find("World").GetComponent<World>();
        myCanvas = transform.parent.GetComponent<Canvas>();
        GenerateInventorySlots();
    }
    

    private void Update()
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(myCanvas.transform as RectTransform, Input.mousePosition, myCanvas.worldCamera, out pos);                   /////////                                                              ///////////
        cursorItem.transform.position = myCanvas.transform.TransformPoint(pos);
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
    public void GenerateInventorySlots()
    {
        byte itemIndex = (byte)(world.blocktypes.Length - 1);
        int stackSize = 64;

        GameObject verticalParent = Instantiate(verticalParentPrefab, transform);
        verticalParent.GetComponent<RectTransform>().sizeDelta = new Vector2(itemBoxSize * inventorySizeX, itemBoxSize * inventorySizeY + itemBoxSize + toolbarDividerSize);
        verticalParent.GetComponent<RectTransform>().position += new Vector3(0, -10, 0);

        for (int y = 0; y < inventorySizeY; y++)
        {
            GameObject currentHorizontalParent = Instantiate(horiontalParentPrefab, verticalParent.transform);
            currentHorizontalParent.GetComponent<RectTransform>().sizeDelta = new Vector2(itemBoxSize * inventorySizeX, itemBoxSize);
            for (int x = 0; x < inventorySizeX; x++)
            {
                if (itemIndex > 0)
                    itemIndex--;


                
                itemSlots[x, y] = new ItemSlot(itemIndex, stackSize, true);
                
                itemButtons[x, y] = Instantiate(itemButton,currentHorizontalParent.transform);
                
                SetItemInSlot(x, y, itemIndex, stackSize);
                
            }
        }

        GameObject toolbarDivider = Instantiate(horiontalParentPrefab, verticalParent.transform);
        toolbarDivider.GetComponent<RectTransform>().sizeDelta = new Vector2(itemBoxSize * inventorySizeX, toolbarDividerSize);
        
        GameObject toolbarClone = Instantiate(horiontalParentPrefab, verticalParent.transform);
        toolbarClone.GetComponent<RectTransform>().sizeDelta = new Vector2(itemBoxSize * inventorySizeX, itemBoxSize);
        for (int x = 0; x < inventorySizeX; x++)
        {
                
            toolbarSlots[x] = new ItemSlot(0, 0, true);
                
            toolbarItems[x] = Instantiate(itemButton,toolbarClone.transform);
                
            UpdateItem(toolbarItems[x],0,0);
        }
        

        cursorSlot = new ItemSlot(0, 0, true);
        cursorItem = Instantiate(cursorItemPrefab, transform);
        cursorItem.transform.GetChild(0).GetComponent<Image>().sprite = world.blocktypes[0].icon;
        cursorItem.transform.GetChild(1).GetComponent<Text>().text = " ";
    }

    public void SetItemInSlot(int posX, int posY , byte id, int stacksize)
    {
        GameObject newButton = itemButtons[posX, posY];
        UpdateItem(newButton, id, stacksize);
        
        newButton.GetComponent<Button>().onClick.AddListener(delegate { SwitchItems(posX, posY);});
    }

    public void SwitchItems(int posX, int posY)
    {
        ItemSlot clickedButton = itemSlots[posX, posY];
        byte clickedButtonItem = clickedButton.GetItemID();
        int clickedButtonStackSize = clickedButton.GetStackSize();
        
        byte cursorItemID = cursorSlot.GetItemID();
        int cursorStackSize = cursorSlot.GetStackSize();

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
