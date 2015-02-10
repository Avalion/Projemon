using UnityEngine;

/**
 * This action will launch target mapObject actions
 * 
 * TODO : Wait message to be able to Terminate this action !!!!
 */
public class ActionExecuteMapObjectActions : MapObjectAction {
    int mapObjectId = -1;

    public ActionExecuteMapObjectActions() {}

    public override void Execute() {
        MapObject mo = World.Current.GetMapObjectById(mapObjectId);
        if (mo != null && !mo.isRunning)
            // TODO : Add a boolean to lock player while actions
            World.Current.ExecuteActions(mo, false);
        Terminate();
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
