using UnityEngine;

/**
 * This action will add an item to a battler
 */
public class ActionAddItem : MapObjectAction {
    public Item item = null;
    
    public ActionAddItem() { }
    public ActionAddItem(Item _item) {
        item = _item;
    }

    public override void Execute() {
        if (item == null) {
            Terminate();
            throw new System.Exception("Unexpected Behaviour : Trying to add a inexisting Item type to the team...");
        }

        if (Player.Current.inventory.ContainsKey(item.name)) {
            Player.Current.inventory[item.name] += 1;
        } else {
            Player.Current.inventory.Add(item.name, 1);
        }
        Terminate();
    }

    public override string InLine() {
        return "Add Item " + (item != null ? item.name : "[TO DEFINE]") + " to Player.";
    }

    public override string Serialize() {
        // TODO : Serialize Item class

        return GetType().ToString();
    }
    public override void Deserialize(string s) {
        string[] values = s.Split('|');
        if (values.Length != 1)
            throw new System.Exception("SerializationError : elements count doesn't match... " + s);

        // TODO : Serialize Item class

    }
}