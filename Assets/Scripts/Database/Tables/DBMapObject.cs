using Mono.Data.Sqlite;
using UnityEngine;

/*
 * this table manage mapObjects
 */
public class DBMapObject : SQLTable {
    public int mapId;
    public Vector2 mapCoords;

    public string name = "MapObject";
    
    public string sprite = "";

    public MapObject.MovementSpeed speed = MapObject.MovementSpeed.Normal;
    public MapObject.Orientation orientation = MapObject.Orientation.Down;
    public MapObject.Layer layer = MapObject.Layer.Middle;
    public MapObject.ExecutionCondition execCondition = MapObject.ExecutionCondition.Action;

    public bool allowPassThrough = true;


    public override void FromRow(SqliteDataReader reader) {
        int pos = 0;
        ID = reader.GetInt32(pos++);

        mapId = reader.GetInt32(pos++);
        mapCoords = new Vector2(reader.GetInt32(pos++), reader.GetInt32(pos++));

        name = reader.GetString(pos++);

        sprite = reader.GetString(pos++);
        
        speed = (MapObject.MovementSpeed)reader.GetInt32(pos++);
        orientation = (MapObject.Orientation)reader.GetInt32(pos++); 
        layer = (MapObject.Layer)reader.GetInt32(pos++); 
        execCondition = (MapObject.ExecutionCondition)reader.GetInt32(pos++);

        allowPassThrough = reader.GetBoolean(pos++);
    }
    public override string Fields() {
        return "mapID, mapCoordX, mapCoordY, name, sprite, speed, orientation, layer, execCondition, allowPassThrough";
    }
    public override string TypedFields() {
        return "mapId integer, " + 
               "mapCoordX int DEFAULT 0, " +
               "mapCoordY integer DEFAULT 0, " +
               "name text, " + 
               "sprite text, " +
               "speed integer, " +
               "orientation integer, " +
               "layer integer, " +
               "execCondition integer, " +
               "allowPassThrough boolean";
    }
    public override string TableName() {
        return "T_MapObject";
    }
    public override string ToRow() {
        return
            Stringize(mapId) + ", " + 
            Stringize((int)mapCoords.x) + ", " + 
            Stringize((int)mapCoords.y) + ", " +
            Stringize(name) + ", " + 
            Stringize(sprite) + ", " +
            Stringize((int)speed) + ", " +
            Stringize((int)orientation) + ", " +
            Stringize((int)layer) + ", " +
            Stringize((int)execCondition) + ", " +
            Stringize(allowPassThrough);
    }
    public override void Delete() {
        DataBase.DeleteByID<DBMapObject>(ID);

        DataBase.Delete<DBMapObjectAction>("mapObjectID = " + ID);
    }

    public static DBMapObject ConvertFrom(Map _map, MapObject _source) {
        DBMapObject m = new DBMapObject();
        m.mapId = _map.ID;
        m.mapCoords = _source.mapCoords;

        m.name = _source.name;

        m.sprite = _source.spritePath;
        
        m.speed = _source.speed;
        m.orientation = _source.orientation;
        m.layer = _source.layer;
        m.execCondition = _source.execCondition;
        
        m.allowPassThrough = _source.allowPassThrough;

        return m;
    }
}
