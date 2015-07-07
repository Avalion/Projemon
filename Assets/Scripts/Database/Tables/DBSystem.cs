using Mono.Data.Sqlite;
using UnityEngine;

/*
 * This table manage all general informations
 */
public class DBSystem : SQLTable {
    public int dbversion = 0;

    public string playerName;
    public int playerGold;
    public int playerMapID;
    public Vector2 playerCoords;

    public override void FromRow(SqliteDataReader reader) {
        int pos = 0;
        ID = reader.GetInt32(pos++);

        dbversion = reader.GetInt32(pos++);
        playerName = reader.GetString(pos++);
        playerGold = reader.GetInt32(pos++);
        playerMapID = reader.GetInt32(pos++);
        playerCoords.x = reader.GetInt32(pos++);
        playerCoords.y = reader.GetInt32(pos++);
    }
    public override string Fields() {
        return "dbversion, playerName, playerGold, playerMapID, playerCoordsX, playerCoordsY";
    }
    public override string TypedFields() {
        return "dbversion integer DEFAULT 0, playerName text DEFAULT 'Player', playerGold integer DEFAULT 0, playerMapID integer, playerCoordsX integer, playerCoordsY integer";
    }
    public override string TableName() {
        return "T_System";
    }
    public override string ToRow() {
        return
            Stringize(dbversion) + ", " + 
            Stringize(playerName) + ", " + 
            Stringize(playerGold) + ", " + 
            Stringize(playerMapID) + ", " + 
            Stringize((int)playerCoords.x) + ", " + 
            Stringize((int)playerCoords.y);
    }
    public override void Delete() {
        DataBase.DeleteByID<DBSystem>(ID);
    }
}
