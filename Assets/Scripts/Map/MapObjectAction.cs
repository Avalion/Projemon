/**
 * This abstract class patterns the differents Actions a MapObject can launch.
 */
[System.Serializable]
public abstract class MapObjectAction {
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
    
    public static MapObjectAction Generate(string s) {
        string type = s.Substring(0, s.IndexOf('|'));
        MapObjectAction action = ((MapObjectAction)System.Activator.CreateInstance(System.Type.GetType(type)));
        action.Deserialize(s);

        return action;
    }
    public static MapObjectAction Generate(DBMapObjectAction _source) {
        return Generate(_source.serialized);
    }
}
