using UnityEngine;

/**
 * This action will add an item to a battler
 */
[System.Serializable]
public class ActionAddItem : MapObjectAction {

    Item item;
    Battler target;

    public ActionAddItem(Item _item, Battler _target) {
        item = _item;
        target = _target;
    }

    public override void Execute() {
        if (target.inventory.ContainsKey(item.name)) { 
            target.inventory[item.name] += 1;
        } else {
            target.inventory.Add(item.name, 1);
        }
        Terminate();
    }

    public override string InLine() {
        return "Add Item " + item.name + " to " + target.name+".";
    }
}