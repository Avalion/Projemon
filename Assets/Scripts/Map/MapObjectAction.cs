using UnityEngine;
/**
 * This abstract class patterns the differents Actions a MapObject can launch.
 * 
 *  /!\ Actions have to be terminated
 */
public abstract class MapObjectAction {
    public int actionId;

    private bool valid = false;

    public bool waitForEnd = true;
     
    public bool Done() { return valid || !waitForEnd; }
    public void Init() { valid = false; }

    public virtual void Execute() {}
    public virtual void Terminate() {
        valid = true;
    }

    public virtual string InLine() {
        return "Unknown action";
    }

    public abstract string Serialize();
    public abstract void Deserialize(string s);
    
    private static MapObjectAction Generate(string s) {
        string type = s.Substring(0, s.IndexOf('|'));
        MapObjectAction action = ((MapObjectAction)System.Activator.CreateInstance(System.Type.GetType(type)));
        try {
            action.Deserialize(s);
        } catch (System.Exception e) {
            Debug.LogError("SerializationError : An error occured while trying to generate an " + type + " : " + e.Message);
            return null;
        }

        return action;
    }
    public static MapObjectAction Generate(DBMapObjectAction _source) {
        MapObjectAction action = Generate(_source.serialized);
        if (action == null)
            return null;
        action.actionId = _source.ID;
        return action;
    }
}
