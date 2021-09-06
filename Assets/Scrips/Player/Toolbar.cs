using System;
using System.Collections;
using System.Collections.Generic;
using Scrips.Player;
using UnityEngine;
using UnityEngine.UI;

public class Toolbar : MonoBehaviour
{
    private Inventory inventory;
    private World world;
    public Player player;

    public RectTransform highlight;
    public ItemSlot[] itemSlots;

    private int slotIndex = 0;

    private void Start()
    {
        inventory = FindObjectOfType<Inventory>();
        world = GameObject.Find("World").GetComponent<World>();
        //player.SelectedBlockIndex = itemSlots[slotIndex].GetItemID();
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

            if (slotIndex > itemSlots.Length - 1)
                slotIndex = 0;
            if (slotIndex < 0)
                slotIndex = itemSlots.Length - 1;
        }
    }

    public void ChangeItem(int index, byte newItemID)
    {

    }
}