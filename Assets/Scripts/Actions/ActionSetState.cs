using UnityEngine;

public class ActionSetState : MapObjectAction {
    public int stateId = -1;
    
    public bool value = false;


    public ActionSetState() { }
    
    public override void Execute() {
        if (stateId < 0)
            return;

        DataBase.SetState(stateId, value);
    }

    public override string InLine() {
        DBState state = DataBase.SelectById<DBState>(stateId);
        return "Set state " + (state != null ? stateId + ":" + state.name : "[TO DEFINE]") + " to " + value.ToString();
    }

    public override string Serialize() {
        return GetType().ToString() + "|" + stateId + "|" + value;
    }
    public override void Deserialize(string s) {
        string[] values = s.Split('|');
        if (values.Length != 3)
            throw new System.Exception("SerializationError : elements count doesn't match... " + s);

        int.TryParse(values[1], out stateId);
        bool.TryParse(values[2], out value);
    }
}
