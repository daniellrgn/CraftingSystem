using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour, IDropHandler {
    public int slotNum;
    private Inventory inv;
    private ItemData itemData;

    // Use this for initialization
    void Start()
    {
        inv = transform.parent.parent.parent.GetComponent<Inventory>();
        if (inv == null)
        {
            transform.parent.parent.parent.GetComponent<LootInventory>();
        }
    }

    public ItemData GetItemData()
    {
        if (transform.childCount > 1)
        {
            Transform existingItemObject = transform.GetChild(1);
            ItemData existingData = existingItemObject.GetComponent<ItemData>();
            return existingData;
        }
        return null;
    }

    public void OnDrop(PointerEventData eventData)
    {
        Transform droppedItem = eventData.pointerDrag.transform;
        ItemData droppedData = droppedItem.GetComponent<ItemData>();

        if (!inv.isOutput && droppedData != null)
        {
            if (inv.items[slotNum].ID == -1) // The slot is empty, move the dragged item.
            {
                if (!droppedData.CheckSplitStack())
                {
                    droppedData.inv.items[droppedData.slot] = new Item();
                }
                inv.items[slotNum] = droppedData.item;
                droppedData.slot = slotNum;
                droppedData.inv = inv;
            }
            else if (droppedData.slot != slotNum || droppedData.inv != inv)
            {   // The slot is occupied, decide what to do with the existing item.
                Transform existingItem = transform.GetChild(1);
                itemData = existingItem.GetComponent<ItemData>();
                bool split = droppedData.CheckSplitStack();
                if (droppedData.item.Stackable && (droppedData.item.ID == itemData.item.ID || split))
                {
                    if (droppedData.item.ID == itemData.item.ID)
                    {   // Combine the item stacks.
                        if (!split)
                        {
                            droppedData.inv.items[droppedData.slot] = new Item();
                        }
                        itemData.amount += droppedData.amount;
                        itemData.UpdateAmountDisplay();
                        Destroy(droppedItem.gameObject);
                    }
                    else if (split)
                    {   // Replace the item stack.
                        ItemData residualData = droppedData.GetResidualData();
                        residualData.amount += droppedData.amount;
                        residualData.UpdateAmountDisplay();
                        Destroy(droppedItem.gameObject);
                    }
                }
                else if (!droppedData.inv.isOutput)
                { // Items need to be swapped.
                  // Change swapped item's slot, inv, and position references to the dropped item's, and add it to the dropped item's original inventory.
                    itemData.slot = droppedData.slot;
                    itemData.inv = droppedData.inv;
                    itemData.inv.items[droppedData.slot] = itemData.item;
                    existingItem.transform.SetParent(itemData.inv.slots[droppedData.slot].transform);
                    existingItem.transform.position = itemData.inv.slots[droppedData.slot].transform.position;

                    // Change dropped item's slot, inv, and position references, and add it to this slot's inventory.
                    droppedData.slot = slotNum;
                    droppedData.inv = inv;
                    droppedData.inv.items[droppedData.slot] = droppedData.item;
                    droppedData.transform.SetParent(transform);
                    droppedData.transform.position = transform.position;

                    itemData = droppedData;
                }

            }
        }
    }

    public void RemoveItem()
    {
        inv.items[slotNum] = new Item();
    }
}
