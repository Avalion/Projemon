/**
 * This abstract class patterns the differents Actions a MapObject can launch.
 */
public abstract class MapObjectAction {
    private bool valid = false;

    public bool waitForEnd = true;

    public bool Done() { return valid || !waitForEnd; }
    public void Init() { valid = false; }

    public abstract void Execute();
    public virtual void Terminate() {
        valid = true;
    }
}
