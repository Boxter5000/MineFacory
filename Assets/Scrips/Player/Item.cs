public class Item
{
    public const byte MaxStackSize = 64;
    public byte ItemID { set; get; }

    public byte StackSize
    {
        set => _stackSize = value > MaxStackSize ? MaxStackSize : value;
        get => _stackSize;
    }
    private byte _stackSize;
    public Item(byte newItemID, byte newStackSize)
    {
        ItemID = newItemID;
        _stackSize = newStackSize;
    }
}