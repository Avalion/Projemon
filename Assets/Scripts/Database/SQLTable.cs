using Mono.Data.Sqlite;

public abstract class SQLTable {
    public int ID;

    public abstract string TableName();
    public abstract string Fields();
    public abstract string TypedFields();
    public abstract void FromRow(SqliteDataReader reader);
    public abstract string ToRow();

    public string InsertInto() { return "INSERT INTO " + TableName() + " (" + Fields() + ") VALUES (" + ToRow() + ")"; }
    public string Replace() { return "INSERT OR REPLACE INTO " + TableName() + " (id, " + Fields() + ") VALUES (" + ID + ", " + ToRow() + ")"; }
    public string Create() { return "CREATE TABLE IF NOT EXISTS '" + TableName() + "' (id integer NOT NULL PRIMARY KEY AUTOINCREMENT, " + TypedFields() + ")"; }

    private static string SQLEscape(string _string) {
        return _string
            .Replace("'", "''")
            .Replace("`", "``")
            .Replace("\n", "\\n")
        ;
    }
    public static string Stringize(object s) {
        // String
        if (s.GetType() == typeof(string))
            return "'" + SQLEscape((string)s) + "'";
        // Bool
        if (s.GetType() == typeof(bool))
            return (bool)s ? "1" : "0"; 

        return s.ToString();
    }
}
