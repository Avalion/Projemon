using System.Collections.Generic;

public class ActionIf : MapObjectAction {
    public enum ConditionType { None, State, Variable, MapCoordsX, MapCoordsY }
    public ConditionType conditionType = ConditionType.None;
    
    public int source;
    public int value;
    
    public bool isActive;

    public ActionIf() {}
    
    public override void Execute() {
        isActive = false;

        switch (conditionType) {
            case ConditionType.State:
                if (GameData.GetState(source)) isActive = true;
                break;
            case ConditionType.Variable:
                if (GameData.GetVariable(source) == value) isActive = true;
                break;
            default:
                UnityEngine.Debug.Log("Not Handled For Now");
                isActive = false; 
                break;
        }

        Terminate();
    }


    public override string InLine() {
        return "If ([" + conditionType + "]" + source + " == " + value + ") {";
    }
    public override string Serialize() {
        return GetType().ToString() + "|" + (int)conditionType + "|" + source + "|" + value;
    }
    public override void Deserialize(string s) {
        string[] values = s.Split('|');
        if (values.Length != 4)
            throw new System.Exception("SerializationError : elements count doesn't match... " + s);
        conditionType = (ConditionType)int.Parse(values[1]);
        source = int.Parse(values[2]);
        value = int.Parse(values[3]);
    }
}
public class ConditionElse : MapObjectAction {
    public int parentId;
    
    public ConditionElse() { }
    public ConditionElse(ActionIf _parent) { parentId = _parent.actionId; }

    public override void Execute() {
        Terminate();
    }

    public override string InLine() {
        return "} Else {";
    }
    public override string Serialize() {
        return GetType().ToString() + "|" + parentId;
    }
    public override void Deserialize(string s) {
        string[] values = s.Split('|');
        if (values.Length != 2)
            throw new System.Exception("SerializationError : elements count doesn't match... " + s);
        parentId = int.Parse(values[1]);
    }
}
public class ConditionEnd : MapObjectAction {
    public int parentId;

    public ConditionEnd() { }
    public ConditionEnd(ActionIf _parent) { parentId = _parent.actionId; }

    public override void Execute() {
        Terminate();
    }

    public override string InLine() {
        return "}";
    }
    public override string Serialize() {
        return GetType().ToString() + "|" + parentId;
    }
    public override void Deserialize(string s) {
        string[] values = s.Split('|');
        if (values.Length != 2)
            throw new System.Exception("SerializationError : elements count doesn't match... " + s);
        parentId = int.Parse(values[1]);
    }
}


