using UnityEngine;

/**
 * This action will remove an item to a battler
 */
public class ActionRemoveItem : MapObjectAction {
    public Item item;


    public ActionRemoveItem() {}
    public ActionRemoveItem(Item _item) {
        item = _item;       
    }

    public override void Execute() {
        if (Player.Current.inventory.ContainsKey(item.name)) {
            Player.Current.inventory[item.name] -= 1;
            if (Player.Current.inventory[item.name] == 0)
                Player.Current.inventory.Remove(item.name);
        }
        Terminate();
    }

    public override string InLine() {
        return "Remove Item " + (item != null ? item.name : "[TO DEFINE]") + " from Player.";
    }
    public override string Serialize() {
        // TODO : Serialize Items class
        return GetType().ToString() + "|";
    }
    public override void Deserialize(string s) {
        string[] values = s.Split('|');
        if (values.Length != 1)
            throw new System.Exception("SerializationError : elements count doesn't match... " + s);
        // TODO : Deserialize Item class
    }
}