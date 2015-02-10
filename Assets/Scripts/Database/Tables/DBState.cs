using Mono.Data.Sqlite;
using UnityEngine;

/*
 * this table manage attack patterns
 */
public class DBState : SQLTable {
    public string name;
    public bool value;

    public override void FromRow(SqliteDataReader reader) {
        int pos = 0;
        ID = reader.GetInt32(pos++);

        name = reader.GetString(pos++);
        value = reader.GetBoolean(pos++);
    }
    public override string Fields() {
        return "name, value";
    }
    public override string TypedFields() {
        return "name text NOT NULL, " +
               "value boolean DEFAULT 0";
    }
    public override string TableName() {
        return "T_State";
    }
    public override string ToRow() {
        return
            Stringize(name) + ", " + 
            Stringize(value);
    }
    public override void Delete() {
        DataBase.DeleteByID<DBState>(ID);
    }
}
