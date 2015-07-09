﻿using System.Collections.Generic;

public class ActionIf : MapObjectAction {
    public enum ConditionType { None, MapCoordsX, MapCoordsY }
    public ConditionType conditionType = ConditionType.None;
    
    public int source;
    public int value;
    
    public bool isActive;

    public ActionIf() {}
    
    public override void Execute() {
        isActive = false;

        switch (conditionType) {
            
            default: isActive = false; break;
        }

        Terminate();
    }


    public override string InLine() {
        return "If (" + conditionType + " = " + value + ") {";
    }
    public override string Serialize() {
        return GetType().ToString() + "|" + (int)conditionType + "|" + value;
    }
    public override void Deserialize(string s) {
        string[] values = s.Split('|');
        if (values.Length != 3)
            throw new System.Exception("SerializationError : elements count doesn't match... " + s);
        conditionType = (ConditionType)int.Parse(values[1]);
        value = int.Parse(values[2]);
    }
}
public class ConditionElse : MapObjectAction {
    public ActionIf parent;
    
    public ConditionElse() { }
    public ConditionElse(ActionIf _parent) { parent = _parent; }

    public override void Execute() {
        Terminate();
    }

    public override string InLine() {
        return "} Else {";
    }
    public override string Serialize() {
        return GetType().ToString() + "|";
    }
    public override void Deserialize(string s) {}
}
public class ConditionEnd : MapObjectAction {
    public ActionIf parent;

    public ConditionEnd() { }
    public ConditionEnd(ActionIf _parent) { parent = _parent; }

    public override void Execute() {
        Terminate();
    }

    public override string InLine() {
        return "}";
    }
    public override string Serialize() {
        return GetType().ToString() + "|";
    }
    public override void Deserialize(string s) {}
}


