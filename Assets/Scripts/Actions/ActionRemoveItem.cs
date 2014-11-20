using UnityEngine;

/**
 * This action will remove an item to a battler
 */
[System.Serializable]
public class ActionRemoveItem : MapObjectAction {

    Item item;
    Battler target;

    public ActionRemoveItem(Item _item, Battler _target) {
        item = _item;
        target = _target;       
    }

    public override void Execute() {
        if (target.inventory.ContainsKey(item.name)) {
            target.inventory[item.name] -= 1;
            if (target.inventory[item.name] == 0)
                target.inventory.Remove(item.name);
        }
        Terminate();
    }

    public override string InLine() {
        return "Remove Item " + item.name + " to " + target.name+".";
    }
    public override string Serialize() {
        // TODO : Serialize Items class
        // TODO : Add MapObjectID when MapObject are into DB
        return GetType().ToString();
    }
    public override void Deserialize(string s) {
        string[] values = s.Split('|');
        if (values.Length != 1)
            throw new System.Exception("SerializationError : elements count doesn't match... " + s);
        // TODO : Deserialize Item class
        // TODO : Read and find MapObjectID when MapObject are into DB
    }
}