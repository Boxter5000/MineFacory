using UnityEngine.UI;

namespace Scrips.Player
{
    public class ItemSlot
    {
        public byte itemID;
        private int stackSize;
        private bool canChangeItemID;                                                   ///////////
        public Image itemImg;
        //public Text sizeText;

        public ItemSlot(byte itemID, int stackSize, bool canChangeItemID)               ///////////
        {
            this.stackSize = stackSize;
            this.itemID = itemID;
            this.canChangeItemID = canChangeItemID;                                         ///////////
        }

        public byte GetItemID()                                                     ///////////
        {
            return itemID;
        }

        public int GetStackSize()                                               ///////////
        {
            return stackSize;
        }

        public bool GetCanChange()                                               ///////////
        {
            return canChangeItemID;
        }

        public void SetItemID(byte newItemID)                                               ///////////
        {
            itemID = newItemID;
        }

        public void SetStackSize(int newStackSize)                                               ///////////
        {
            stackSize = newStackSize;
        }

        public void SetCanChange(bool newCanChange)                                               ///////////
        {
            canChangeItemID = newCanChange;
        }
    }
}
