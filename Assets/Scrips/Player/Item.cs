using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    public const byte MaxStackSize = 64;
    public byte ItemID
    {
        set => itemID = value;
        get => itemID;
    }

    private byte itemID;

    public byte StackSize
    {
        set => stackSize = value > 64 ? (byte) 64 : value;
        get => stackSize;
    }
    private byte stackSize;
    public Item(byte newItemID, byte newStackSize)
    {
        itemID = newItemID;
        stackSize = newStackSize;
    }
}
