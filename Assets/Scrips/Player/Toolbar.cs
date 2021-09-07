using System;
using System.Collections;
using System.Collections.Generic;
using Scrips.Player;
using Scrips.World;
using UnityEngine;
using UnityEngine.UI;

public class Toolbar : MonoBehaviour
{
    public World world;
    public Player player;


    private int slotIndex = 0;
    private RectTransform highlight;
    private GameObject[] toolbarSlots = new GameObject[9]; 
    private Item[] toolbarItems = new Item[9];

    [SerializeField] private GameObject toolbarSlotPrefab;
    [SerializeField] private GameObject highlightPrefab;
    [SerializeField] private GameObject horizontalParentPrefab;

    private void Awake()
    {
        world = GameObject.Find("World").GetComponent<World>();
<<<<<<< Updated upstream
        player.SelectedBlockIndex = itemSlots[slotIndex].GetItemID();
=======
    }

    public void SetToolbar(Item[] newToolbarItems)
    {
        GameObject currentHorizontalParent = Instantiate(horizontalParentPrefab, transform);
        currentHorizontalParent.GetComponent<RectTransform>().sizeDelta = new Vector2(Inventory.ItemBoxSize * toolbarSlots.Length, Inventory.ItemBoxSize);
        
        for (int i = 0; i < toolbarSlots.Length; i++)
        {
            toolbarSlots[i] = Instantiate(toolbarSlotPrefab, currentHorizontalParent.transform);
        }

        highlight = Instantiate(highlightPrefab, transform).GetComponent<RectTransform>();
        highlight.position = toolbarSlots[slotIndex].GetComponent<RectTransform>().position;
        
        for (int i = 0; i < toolbarItems.Length; i++)
        {
            ChangeItem(i, newToolbarItems[i]);
        }
        GivePlayerHighlightedItem();
>>>>>>> Stashed changes
    }

    private void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0)
        {
            if (scroll > 0)
                slotIndex--;
            else
                slotIndex++;

            if (slotIndex > toolbarItems.Length - 1)
                slotIndex = 0;
            if (slotIndex < 0)
                slotIndex = toolbarItems.Length - 1;
            highlight.position = toolbarSlots[slotIndex].GetComponent<RectTransform>().position;
            GivePlayerHighlightedItem();
        }
    }

    public void ChangeItem(int index, Item newItem)
    {
        toolbarItems[index] = newItem;
        if(index == slotIndex)
            GivePlayerHighlightedItem();
        UpdateItemBox(toolbarSlots[index], newItem.ItemID, newItem.StackSize);
    }

    private void GivePlayerHighlightedItem()
    {
        player.SelectedBlockIndex = toolbarItems[slotIndex].ItemID;
    }

    private void UpdateItemBox(GameObject itemBoxToUpdate, byte newItemId, int newStackSize)
    {
        itemBoxToUpdate.transform.GetChild(0).GetComponent<Image>().sprite = world.blocktypes[newItemId].icon;
        itemBoxToUpdate.transform.GetChild(1).GetComponent<Text>().text = (newStackSize == 0 || newItemId == 0) ? "" : newStackSize.ToString();
    }
}