/**
 * This abstract class patterns the differents Actions a MapObject can launch.
 */
[System.Serializable]
public class MapObjectAction {
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
}
