using Mono.Data.Sqlite;
using System.Collections.Generic;
using System.IO;

public class DataBase {
    private static SqliteConnection m_dbConnection = null;
    private static string m_connectionString;

    public static bool IsConnected { get { return m_dbConnection != null; } }

    // Constructors
    public static void Connect(string _filename) {
        if (IsConnected)
            Close();

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

        if (system.dbversion < 1) V0toV1();

        Update<DBSystem>("dbversion", CURRENT_DB_VERSION, "id=" + system.ID);
    }

    private static void V0toV1() {
        ExecCommand(new DBMonster().Create());
        ExecCommand(new DBMonsterPattern().Create());
        ExecCommand(new DBAttack().Create());
        ExecCommand(new DBMapObject().Create());
        ExecCommand(new DBMapObjectAction().Create());
        ExecCommand(new DBState().Create());
        ExecCommand(new DBVariable().Create());
    }

    // Destructors
    public static void Close() {
        if (IsConnected) {
            m_dbConnection.Close();
        }

        m_dbConnection = null;
    }

    // Commands
    public static int ExecCommand(string _command) {
        if (m_dbConnection == null) throw new System.Exception("DataBaseException : Database is not open.");
        try {
            SqliteCommand cmd = m_dbConnection.CreateCommand();
            cmd.CommandText = _command;
            int r = cmd.ExecuteNonQuery();
            cmd = null;
            return r;
        }
        catch (System.Exception e) { throw new System.Exception("SQL error\nCommand: " + _command, e); }
    }

    // SQL
    public static List<T> Select<T>(string where = "", int limit = -1, int limitCount = 0, string orderBy = "") where T : SQLTable, new() {
        if (m_dbConnection == null) throw new System.Exception("DataBaseException : Database is not open.");

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
        cmd = null;

        return r;
    }
    public static T SelectUnique<T>(string where = "") where T : SQLTable, new() {
        List<T> r = Select<T>(where, 2);		// 2 is intended here (to find if more that one)
        if (r.Count == 0) throw new System.Exception("DataBaseException : SelectUnique found no entry in " + typeof(T) + " with " + where + ".");
        if (r.Count > 1) throw new System.Exception("DataBaseException : SelectUnique found more than one entry in " + typeof(T) + " with " + where + ".");
        return r[0];
    }
    public static T SelectById<T>(int id) where T : SQLTable, new() {
        return SelectUnique<T>("id=" + id);
    }

    public static int Count<T>(string where = "", int limit = -1) where T : SQLTable, new() {
        if (m_dbConnection == null) throw new System.Exception("DataBaseException : Database is not open.");

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
        cmd = null;

        return count;
    }

    public static void Insert<T>(T data) where T : SQLTable, new() {
        ExecCommand(data.InsertInto());
    }
    public static void Replace<T>(T newdata) where T : SQLTable, new() {
        ExecCommand(newdata.Replace());
    }

    public static void Update<T>(string column, object value, string where = "") where T : SQLTable, new() {
        if (m_dbConnection == null) throw new System.Exception("DataBaseException : Database is not open.");

        if (!new T().Fields().Contains(column))
            return;

        string cmd = "UPDATE " + new T().TableName() + " SET `" + column + "`=" + SQLTable.Stringize(value);
        if (where.Length > 0)
            cmd += " WHERE " + where;
        
        ExecCommand(cmd);
    }
    
    public static void DeleteByID<T>(int ID) where T : SQLTable, new() {
        Delete<T>("ID = " + ID);
    }
    public static void Delete<T>(string where = "") where T : SQLTable, new() {
        if (m_dbConnection == null) throw new System.Exception("DataBaseException : Database is not open.");

        string cmd = "DELETE FROM " + new T().TableName();
        if (where.Length > 0)
            cmd += " WHERE " + where;
        
        ExecCommand(cmd);
    }

    // Utils
    public static int GetLastInsertId() {
        if (m_dbConnection == null) throw new System.Exception("DataBaseException : Database is not open.");
        try {
            SqliteCommand cmd = m_dbConnection.CreateCommand();
            cmd.CommandText = "SELECT last_insert_rowid();";
            SqliteDataReader reader = cmd.ExecuteReader();
            int r = reader.Read() ? reader.GetInt32(0) : -1;
            cmd = null;
            return r;
        } catch (System.Exception e) { throw new System.Exception("SQL error with InsertLastId.", e); }
    }

    public static void Vacuum() {
        ExecCommand("VACUUM");
    }

    // MapObject
    public static List<DBMapObject> GetMapObjects(int mapId) {
        return DataBase.Select<DBMapObject>("mapId = " + mapId);
    }

    public static List<DBMapObjectAction> GetMapObjectActions(int mapObjectId) {
        List<DBMapObjectAction> result = DataBase.Select<DBMapObjectAction>("mapObjectId = " + mapObjectId);
        result.Sort(delegate(DBMapObjectAction a, DBMapObjectAction b) { return a.executeOrder.CompareTo(b.executeOrder); });
        return result;
    }

    // Variables And States
    public static int GetVariable(int _id) { return SelectById<DBVariable>(_id).value; }
    public static int GetVariable(string _name) { return SelectUnique<DBVariable>("name=" + _name).value; }
    public static void SetVariable(int _id, int _value) { Update<DBVariable>("value", _value, "id=" + _id); }

    public static bool GetState(int _id) { return SelectById<DBState>(_id).value; }
    public static bool GetState(string _name) { return SelectUnique<DBState>("name=" + _name).value; }
    public static void SetState(int _id, bool _value) { Update<DBState>("value", _value, "id=" + _id); }
}