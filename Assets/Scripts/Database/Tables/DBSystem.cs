using Mono.Data.Sqlite;

/*
 * This table manage all general informations
 */
public class DBSystem : SQLTable {
    public int dbversion = 0;

    public override void FromRow(SqliteDataReader reader) {
        int pos = 0;
        ID = reader.GetInt32(pos++);
        dbversion = reader.GetInt32(pos++);
    }
    public override string Fields() {
        return "dbversion";
    }
    public override string TypedFields() {
        return "dbversion integer DEFAULT 0";
    }
    public override string TableName() {
        return "T_System";
    }
    public override string ToRow() {
        return
            Stringize(dbversion);
    }
    public override void Delete() {
        DataBase.DeleteByID<DBSystem>(ID);
    }
}
