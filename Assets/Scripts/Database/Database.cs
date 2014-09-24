using Mono.Data.Sqlite;
using System.Collections.Generic;
using System.IO;

public class DataBase {
    private static SqliteConnection m_dbConnection = null;
    private static string m_connectionString;

    // Constructors
    public static void Connect(string _filename) {
        m_connectionString = "Version=3; Journal Mode=Off; Synchronous=OFF; temp store=3; Cache Size=10000; Page Size=4096; Data Source=" + _filename;

        try {
            bool haveToBeInitialized = !File.Exists(_filename);
            
            if (m_dbConnection == null) m_dbConnection = new SqliteConnection(m_connectionString);
            if (m_dbConnection.State == 0) m_dbConnection.Open();

            if (haveToBeInitialized)
                Initialize();

            CheckVersion();
        }
        catch (System.Exception e) { throw new System.Exception("SQL error with connectionString=" + m_connectionString, e); }
    }

    public static void Initialize() {
        DBSystem system = new DBSystem();
        ExecCommand(system.Create());

        ExecCommand(system.InsertInto());
    }

    private static int CURRENT_DB_VERSION = 1;
    private static void CheckVersion() {
        DBSystem system = SelectUnique<DBSystem>();

        if (system.dbversion < 1)
            V0toV1();

        //Update<DBSystem>("dbversion", CURRENT_DB_VERSION, "id=" + system.ID);
    }

    private static void V0toV1() {
    
    }

    // Destructors
    public static void Close() {
        m_dbConnection.Close();
        m_dbConnection.Dispose();
        m_dbConnection = null;
    }

    // Commands
    public static int ExecCommand(string _command, SqliteParameter[] param = null) {
        if (m_dbConnection == null) throw new System.Exception("Database is not open.");
        try {
            SqliteCommand cmd = m_dbConnection.CreateCommand();
            cmd.CommandText = _command;
            if (param != null) {
                cmd.Parameters.AddRange(param);
            }
            int r = cmd.ExecuteNonQuery();
            cmd.Dispose();
            cmd = null;
            return r;
        }
        catch (System.Exception e) { throw new System.Exception("SQL error\nCommand: " + _command + "\nParams: " + param, e); }
    }

    // SQL
    public static List<T> Select<T>(string where = "", int limit = -1, int limitCount = 0, string orderBy = "") where T : SQLTable, new() {
        if (m_dbConnection == null) throw new System.Exception("Database is not open.");

        string q = "SELECT * FROM " + new T().TableName();
        if (where.Length > 0) q += " WHERE " + where;
        if (orderBy.Length > 0) q += " ORDER BY " + orderBy;
        if (limit >= 0) {
            q += " LIMIT " + limit;
            if (limitCount > 0)
                q += " OFFSET " + limitCount;
        }

        List<T> r = new List<T>();
        SqliteCommand cmd = m_dbConnection.CreateCommand();
        cmd.CommandText = q;
        SqliteDataReader reader = cmd.ExecuteReader();
        for (; reader.Read(); ) {
            T t = new T();
            t.FromRow(reader);
            r.Add(t);
        }
        // clean up
        reader.Close();
        reader = null;
        cmd.Dispose();
        cmd = null;

        return r;
    }
    public static T SelectUnique<T>(string where = "") where T : SQLTable, new() {
        List<T> r = Select<T>(where, 2);		// 2 is intended here (to find if more that one)
        if (r.Count == 0) throw new System.Exception("SelectUnique<>() found no entry.");
        if (r.Count > 1) throw new System.Exception("SelectUnique<>() found more than one entry.");
        return r[0];
    }

    public static int Count<T>(string where = "", int limit = -1) where T : SQLTable, new() {
        if (m_dbConnection == null)
            throw new System.Exception("Database is not open.");

        string q = "SELECT COUNT(*) FROM (SELECT 1 FROM " + new T().TableName();
        if (where.Length > 0) q += " WHERE " + where;
        if (limit >= 0) {
            q += " LIMIT " + limit;
        }
        q += ")";
        SqliteCommand cmd = m_dbConnection.CreateCommand();
        cmd.CommandText = q;
        SqliteDataReader reader = cmd.ExecuteReader();
        reader.Read();
        int count = reader.GetInt32(0);

        // clean up
        reader.Close();
        reader = null;
        cmd.Dispose();
        cmd = null;

        return count;
    }

    public static void Insert<T>(T data) where T : SQLTable, new() {
        ExecCommand(data.InsertInto());
    }

    public static void Update<T>(string column, object value, string where = "") where T : SQLTable, new() {
        if (m_dbConnection == null)
            throw new System.Exception("Database is not open.");

        if (!new T().Fields().Contains(column))
            return;

        string cmd = "UPDATE " + new T().TableName() + " SET `" + column + "`=" + SQLTable.Stringize(value);
        if (where.Length > 0)
            cmd += " WHERE " + where;
        
        ExecCommand(cmd);
    }

    // Utils
    private int GetLastInsertId() {
        if (m_dbConnection == null) throw new System.Exception("Database is not open.");
        try {
            SqliteCommand cmd = m_dbConnection.CreateCommand();
            cmd.CommandText = "SELECT last_insert_rowid();";
            SqliteDataReader reader = cmd.ExecuteReader();
            int r = reader.Read() ? reader.GetInt32(0) : -1;
            cmd.Dispose();
            cmd = null;
            return r;
        }
        catch (System.Exception e) { throw new System.Exception("SQL error with InsertLastId.", e); }
    }
}