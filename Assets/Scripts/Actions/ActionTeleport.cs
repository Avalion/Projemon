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
		if (World.Current.currentMap != null)
        	mapID = World.Current.currentMap.ID;
    }
    public ActionTeleport(MapObject _target, MapObject.Orientation _orientation, int _mapID, Vector2 _arrival) {
        mapObjectId = _target.mapObjectId;
        orientation = _orientation;
        arrival = _arrival;
        mapID = _mapID;
    }

    public override void Execute() {
        MapObject target = World.Current.GetMapObjectById(mapObjectId);

        if (World.Current.currentMap.ID != mapID) {
            if (target != Player.Current) {
                Debug.LogError("InternalException : A mapObject cannot be teleported to another map. Please use Interrupteurs instead.");
                return;
            }
            World.Current.LoadMap(mapID, arrival);
        }
        
        target.mapCoords = arrival;
        target.orientation = orientation;
        
        Terminate();
    }

    public override string InLine() {
        // In line is an Editor feature. Database is available
        DBMapObject moa = DataBase.SelectById<DBMapObject>(mapObjectId);
        return "Teleport " + (mapObjectId == -1 ? "Player" : (moa != null ? mapObjectId + ":" + moa.name : "[TO DEFINE]")) + " to : " + arrival + ":" + orientation;
    }

    public override string Serialize() {
        return GetType().ToString() + "|" + mapObjectId + "|" + (int)orientation + "|" + mapID + "|" + arrival.x + "|" + arrival.y; 
    }
    public override void Deserialize(string s) {
        string[] values = s.Split('|');
        if (values.Length != 6)
            throw new System.Exception("SerializationError : elements count doesn't match... " + s);

        mapObjectId = int.Parse(values[1]);
        orientation = (MapObject.Orientation)int.Parse(values[2]);
        mapID = int.Parse(values[3]);
        arrival = new Vector2(float.Parse(values[4]), float.Parse(values[5]));
    }
}
