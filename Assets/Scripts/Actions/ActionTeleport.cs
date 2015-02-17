using UnityEngine;

/**
 * This action will teleport a MapObject to a destination
 */
public class ActionTeleport : MapObjectAction {
    public int mapObjectId = -1;
    public MapObject.Orientation orientation;
    public int mapID;
    public Vector2 arrival;

    public ActionTeleport() {
        mapObjectId = Player.Current.mapObjectId;
    }
    public ActionTeleport(MapObject _target, MapObject.Orientation _orientation, int _mapID, Vector2 _arrival) {
        mapObjectId = _target.mapObjectId;
        orientation = _orientation;
        arrival = _arrival;
        mapID = _mapID;
    }

    public override void Execute() {
        MapObject target = World.Current.GetMapObjectById(mapObjectId);
        if(World.Current.currentMap.ID != mapID) World.Current.LoadMap(mapID);

        target.mapCoords = arrival;
        target.orientation = orientation;
        
        Terminate();
    }

    public override string InLine() {
        DBMapObject moa = null;
        try {
            moa = DataBase.SelectById<DBMapObject>(mapObjectId);
        } catch { }
        return "Teleport " + (moa != null ? moa.name : "") + " to : " + arrival + ":" + orientation;
    }

    public override string Serialize() {
        // TODO : Add MapObjectID when MapObject are into DB
        return GetType().ToString() + "|" + (int)orientation + "|" + mapID + "|" + arrival.x + "|" + arrival.y; 
    }
    public override void Deserialize(string s) {
        string[] values = s.Split('|');
        if (values.Length != 5)
            throw new System.Exception("SerializationError : elements count doesn't match... " + s);

        // TODO : Read and find MapObjectID when MapObject are into DB
        orientation = (MapObject.Orientation)int.Parse(values[1]);
        mapID = int.Parse(values[2]);
        arrival = new Vector2(float.Parse(values[3]), float.Parse(values[4]));
    }
}
