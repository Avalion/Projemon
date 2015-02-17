using UnityEngine;

/**
 * This action will launch target mapObject actions
 */
public class ActionExecuteMapObjectActions : MapObjectAction {
    public int mapObjectId = -1;

    public ActionExecuteMapObjectActions() {}

    public override void Execute() {
        MapObject mo = World.Current.GetMapObjectById(mapObjectId);
        if (mo == null || mo.isRunning) {
            Terminate();
            return;
        }
        // TODO : Add a boolean to lock player while actions
        World.Current.ExecuteActions(mo, false, delegate() { Terminate(); });
    }

    public override string InLine() {
        DBMapObject moa = DataBase.SelectById<DBMapObject>(mapObjectId);
        return "Execute " + mapObjectId + ":" + (moa != null ? moa.name : "") + " actions.";
    }

    public override string Serialize() {
        return GetType().ToString() + "|" + mapObjectId;
    }
    public override void Deserialize(string s) {
        string[] values = s.Split('|');
        if (values.Length != 2)
            throw new System.Exception("SerializationError : elements count doesn't match... " + s);
        
        mapObjectId = int.Parse(values[1]);
    }
}
