using UnityEngine;
using System.Collections.Generic;

public class GameData {
    private static string currentFilePath = null;

    public static DBSystem dbSystem;

    public static List<DBAttack> dbAttacks;
    public static List<DBState> dbStates;
    public static List<DBVariable> dbVariables;
    public static List<DBMonsterPattern> dbMonsterPatterns;


    public static void Save(string filePath) {
        if (filePath == "" || filePath == "database.sql") throw new System.Exception("Cannot edit the main database !!!");
        if (DataBase.IsConnected) {
            Debug.LogError("DataBaseException : Impossible to connect while a connection is still opened... : " + currentFilePath);
            return;
        }
        DataBase.Connect(Application.dataPath + "/" + filePath);
        currentFilePath = filePath;

        // Attacks don't have to be modified

        
        // State & Variables
        DataBase.Delete<DBState>();
        foreach (DBState state in dbStates) {
            DataBase.Insert<DBState>(state);
        }
        DataBase.Delete<DBVariable>();
        foreach (DBVariable variable in dbVariables) {
            DataBase.Insert<DBVariable>(variable);
        }
        
        // MonsterPattern only differs on encountered
        foreach (int patternId in MonsterCollection.encounteredMonsters)
            DataBase.Update<DBMonsterPattern>("encountered", true, "id=" + patternId);

        // Monsters captured
        DataBase.Delete<DBMonster>();
        foreach (Monster monster in MonsterCollection.capturedMonsters) {
            DataBase.Insert<DBMonster>(DBMonster.ConvertFrom(monster));
        }

        // TODO : Save MapObjects when the have local states !

        // Player infos
        DataBase.Update<DBSystem>("playerName", Player.Current.name);
        DataBase.Update<DBSystem>("playerMapID", World.Current.currentMap.ID);
        DataBase.Update<DBSystem>("playerCoordsX", (int)Player.Current.mapCoords.x);
        DataBase.Update<DBSystem>("playerCoordsY", (int)Player.Current.mapCoords.y);
        DataBase.Update<DBSystem>("playerGold", Player.Current.goldCount);


        DataBase.Close();
    }
    public static void Load(string filePath) {
        if (filePath == "") filePath = "database.sql";
        if (DataBase.IsConnected) {
            Debug.LogError("DataBaseException : Impossible to connect while a connection is still opened... : " + currentFilePath);
            return;
        }
        DataBase.Connect(Application.dataPath + "/" + filePath);
        currentFilePath = filePath;

        dbSystem = DataBase.SelectUnique<DBSystem>();
        dbAttacks = DataBase.Select<DBAttack>();
        dbStates = DataBase.Select<DBState>();
        dbVariables = DataBase.Select<DBVariable>();
        dbMonsterPatterns = DataBase.Select<DBMonsterPattern>();

        MonsterCollection.encounteredMonsters = new List<int>();
        MonsterCollection.capturedMonsters = new List<Monster>();
        Player.Current.monsters = new List<Monster>();

        List<DBMonsterPattern> patterns = DataBase.Select<DBMonsterPattern>("encountered=1");
        foreach (DBMonsterPattern m in patterns) {
            MonsterCollection.Encounter(m);
        }

        List<DBMonster> collection = DataBase.Select<DBMonster>();
        foreach (DBMonster m in collection) {
            Monster monster = Monster.Generate(m);
            MonsterCollection.AddToCollection(monster);

            if (m.inTeam) {
                if (Player.Current.monsters.Count < Player.MAX_TEAM_NUMBER) {
                    Player.Current.monsters.Add(monster);
                }
            }
        }

        DataBase.Close();
    }

    public static void Dispose() {
        if (dbAttacks != null) dbAttacks.Clear();
        if (dbStates != null) dbStates.Clear();
        if (dbVariables != null) dbVariables.Clear();
        if (dbMonsterPatterns != null) dbMonsterPatterns.Clear();

        dbSystem = null;
        dbAttacks = null;
        dbStates = null;
        dbVariables = null;
        dbMonsterPatterns = null;

        Resources.UnloadUnusedAssets();
    }



    // Getters
    public static List<DBMapObject> LoadMapObjects(int _mapId) {
        if (DataBase.IsConnected) {
            Debug.LogError("DataBaseException : Unable to connect while a connection is still opened... : " + currentFilePath);
            return null;
        }
        if (currentFilePath == null) {
            Debug.LogError("DataBaseException : No database to connect... You have to Load a database before using it.");
            return null;
        }
        
        DataBase.Connect(Application.dataPath + "/" + currentFilePath);
        
        List<DBMapObject> list = DataBase.Select<DBMapObject>("mapId = " + _mapId);
        
        DataBase.Close();

        return list;
    }

    public static List<DBMapObjectAction> LoadMapObjectActions(int _mapObjectId) {
        if (DataBase.IsConnected) {
            Debug.LogError("DataBaseException : Unable to connect while a connection is still opened... : " + currentFilePath);
            return null;
        }
        if (currentFilePath == null) {
            Debug.LogError("DataBaseException : No database to connect... You have to Load a database before using it.");
            return null;
        }

        DataBase.Connect(Application.dataPath + "/" + currentFilePath);

        List<DBMapObjectAction> result = DataBase.Select<DBMapObjectAction>("mapObjectId = " + _mapObjectId);
        result.Sort(delegate(DBMapObjectAction a, DBMapObjectAction b) { return a.executeOrder.CompareTo(b.executeOrder); });
        
        DataBase.Close();

        return result;
    }


    public static DBMonsterPattern GetPattern(int _id) { return dbMonsterPatterns.Find(V => V.ID == _id); }
    public static DBAttack GetAttack(int _id) { return dbAttacks.Find(V => V.ID == _id); }
    
    // Variables And States
    public static int GetVariable(int _id) { return dbVariables.Find(V => V.ID == _id).value; }
    public static int GetVariable(string _name) { return dbVariables.Find(V => V.name == _name).value; }
    public static void SetVariable(int _id, int _value) { dbVariables.Find(V => V.ID == _id).value = _value; }
    public static void SetVariable(int _id, string _name) { dbVariables.Find(V => V.ID == _id).name = _name; }

    public static bool GetState(int _id) { return dbStates.Find(V => V.ID == _id).value; }
    public static bool GetState(string _name) { return dbStates.Find(V => V.name == _name).value; }
    public static void SetState(int _id, bool _value) { dbStates.Find(V => V.ID == _id).value = _value; }
    public static void SetState(int _id, string _name) { dbStates.Find(V => V.ID == _id).name = _name; }

}
