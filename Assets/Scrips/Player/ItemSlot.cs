using UnityEngine.UI;

namespace Scrips.Player
{
    public class ItemSlot
    {
        private Item currentItem;
        private bool canChangeItemID;

        public ItemSlot(bool canChangeItemID)
        {
            this.canChangeItemID = canChangeItemID;
        }

        public byte GetItemID()
        {
            return currentItem.ItemID;
        }

        public byte GetStackSize()
        {
            return currentItem.StackSize;
        }

        public bool GetCanChange()
        {
            return canChangeItemID;
        }

        public void SetItem(Item newItem)
        {
            currentItem = newItem;
        }

        public void SetItemID(byte newItemID)
        {
            currentItem.ItemID = newItemID;
        }

        public void SetStackSize(byte newStackSize)
        {
            currentItem.StackSize = newStackSize;
        }

        public void SetCanChange(bool newCanChange)
        {
            canChangeItemID = newCanChange;
        }
    }
}
