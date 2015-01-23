using Mono.Data.Sqlite;
using UnityEngine;

/*
 * this table manage mapObjects
 */
public class DBMapObjectAction : SQLTable {
    public int mapObjectId;
    
    public string serialized;
    
    public override void FromRow(SqliteDataReader reader) {
        int pos = 0;
        ID = reader.GetInt32(pos++);

        mapObjectId = reader.GetInt32(pos++);
        serialized = reader.GetString(pos++);
    }
    public override string Fields() {
        return "mapObjectId, serialized";
    }
    public override string TypedFields() {
        return "mapObjectId integer, " + 
               "serialized text";
    }
    public override string TableName() {
        return "T_MapObjectAction";
    }
    public override string ToRow() {
        return
            Stringize(mapObjectId) + ", " + 
            Stringize(serialized);
    }
    public override void Delete() {
         DataBase.DeleteByID<DBMapObjectAction>(ID);
    }

    public static DBMapObjectAction ConvertFrom(MapObject _object, MapObjectAction _source) {
        DBMapObjectAction m = new DBMapObjectAction();
        m.mapObjectId = _object.mapObjectId;
        m.serialized = _source.Serialize();
        return m;
    }

}
