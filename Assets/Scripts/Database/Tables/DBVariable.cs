using Mono.Data.Sqlite;
using UnityEngine;

/*
 * this table manage attack patterns
 */
public class DBVariable : SQLTable {
    public string name = "";
    public int value = 0;

    public override void FromRow(SqliteDataReader reader) {
        int pos = 0;
        ID = reader.GetInt32(pos++);

        name = reader.GetString(pos++);
        value = reader.GetInt32(pos++);
    }
    public override string Fields() {
        return "name, value";
    }
    public override string TypedFields() {
        return "name text NOT NULL, " +
               "value integer DEFAULT 0";
    }
    public override string TableName() {
        return "T_Variable";
    }
    public override string ToRow() {
        return
            Stringize(name) + ", " + 
            Stringize(value);
    }
    public override void Delete() {
        DataBase.DeleteByID<DBVariable>(ID);
    }
}
