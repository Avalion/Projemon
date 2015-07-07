using UnityEngine;

public class ActionSave : MapObjectAction {
    public override void Execute() {
        GameData.Save("Saves/Save0.sql");
        Terminate();
    }
    
    public override string InLine() {
        return "Save";
    }
    
    public override string Serialize() {
        return GetType().ToString() + "|";
    }
    public override void Deserialize(string s) {
        //string[] values = s.Split('|');
        //if (values.Length != 1)
        //    throw new System.Exception("SerializationError : elements count doesn't match... " + s);

    }
}

