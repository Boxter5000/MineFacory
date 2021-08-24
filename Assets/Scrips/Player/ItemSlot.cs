namespace Scrips.Player
{
    public class ItemSlot
    {
        private byte itemID;
        private int stackSize;
        //public Image itemImg;
        //public Text sizeText;

        public ItemSlot(byte itemID, int stackSize)
        {
            this.stackSize = stackSize;
            this.itemID = itemID;
        }
    }
}
