using System;
using System.Collections;
using System.Collections.Generic;
using Scrips.Player;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Inventory : MonoBehaviour
{
    public ItemSlot[,] itemSlots;
    public Button[,] itemButtons;
    public ItemSlot curserSlot;
    public Button itemButton;

    public int inventorySizeX = 9;
    public int inventorySizeY = 5;
    public Vector3 offsetPercent;
    
    int resX = Screen.width;
    int resY = Screen.height;

    private World world;


    [SerializeField] private float itemSlotDistance = 30f;
    private void Awake()
    {
        itemSlots = new ItemSlot[inventorySizeX, inventorySizeY];
        itemButtons = new Button[inventorySizeX, inventorySizeY];
        world  = GameObject.Find("World").GetComponent<World>();
        GenerateInventorySlots();
    }
    

    private void Update()
    {
        resX = Screen.width;
        resY = Screen.height;
    }

    public void GenerateInventorySlots()
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

                stackSize = Random.Range(10,100);
                
                itemSlots[x + xStart, y + yStart] = new ItemSlot(itemIndex, stackSize);
                
                itemButtons[x + xStart, y + yStart] = Instantiate(itemButton,transform);
                itemButtons[x + xStart, y + yStart].transform.position = new Vector3((x * itemSlotDistance) + (resX * offsetPercent.x),(-y * itemSlotDistance) + (resY * offsetPercent.y) , 0);

                
                SetItemInSlot(x + xStart, y + yStart, itemIndex, stackSize);
                
            }
        }
    }

    public void SetItemInSlot(int posx, int posy, byte id, int stacksize)
    {
        itemButtons[posx, posy].transform.GetChild(0).GetComponent<Image>().sprite = world.blocktypes[id].itemImage;

        if (id == 0)
        {
            itemButtons[posx, posy].transform.GetChild(1).GetComponent<Text>().text = " ";
            itemButtons[posx,posy].onClick.AddListener(Testok);
        }
        else
        {
            itemButtons[posx, posy].transform.GetChild(1).GetComponent<Text>().text = stacksize.ToString();
        }

        
    }

    public void Testok()
    {
        Debug.Log(name);
    }
    
    private void OnDrawGizmos()
    {
        for (int x = 0; x < 9; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                Gizmos.DrawCube(new Vector3((x * itemSlotDistance) + (resX * offsetPercent.x),(-y * itemSlotDistance) + (resY * offsetPercent.y) , 0),new Vector3(30,30,1));
                
            }
        }
    }
}
