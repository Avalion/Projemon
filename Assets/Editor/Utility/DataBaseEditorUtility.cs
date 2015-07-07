using System.Collections.Generic;
using UnityEngine;

public class DataBaseEditorUtility {

    // Getters
    public static List<MapObject> GetMapObjects(int _mapId) {
        List<MapObject> mapObjects = new List<MapObject>();

        foreach (DBMapObject mo in DataBase.Select<DBMapObject>("mapId = " + _mapId)) {
            // Generate element
            MapObject m = new MapObject();
            m.mapObjectId = mo.ID;
            m.mapCoords = mo.mapCoords;

            m.name = mo.name;
            m.spritePath = mo.sprite;
            m.sprite = InterfaceUtility.GetTexture(Config.GetResourcePath(MapObject.IMAGE_FOLDER) + mo.sprite);

            m.speed = mo.speed;
            m.orientation = mo.orientation;
            m.layer = mo.layer;
            m.execCondition = mo.execCondition;

            m.allowPassThrough = mo.allowPassThrough;

            // Generate actions
            List<DBMapObjectAction> actions = DataBase.Select<DBMapObjectAction>("mapObjectId = " + mo.ID);
            actions.Sort(delegate(DBMapObjectAction a, DBMapObjectAction b) { return a.executeOrder.CompareTo(b.executeOrder); });
            
            foreach (DBMapObjectAction action in actions) {
                MapObjectAction moa = MapObjectAction.Generate(action);
                if (moa == null) {
                    Debug.LogError("===> Skipping this action.");
                    continue;
                }

                m.actions.Add(moa);
            }

            mapObjects.Add(m);
        }

        return mapObjects;
    }

    // Variables And States
    public static int GetVariable(int _id) { return DataBase.SelectById<DBVariable>(_id).value; }
    public static int GetVariable(string _name) { return DataBase.SelectUnique<DBVariable>("name=" + _name).value; }
    public static void SetVariable(int _id, int _value) { DataBase.Update<DBVariable>("value", _value, "id=" + _id); }
    public static void SetVariable(int _id, string _name) { DataBase.Update<DBVariable>("name", _name, "id=" + _id); }

    public static bool GetState(int _id) { return DataBase.SelectById<DBState>(_id).value; }
    public static bool GetState(string _name) { return DataBase.SelectUnique<DBState>("name=" + _name).value; }
    public static void SetState(int _id, bool _value) { DataBase.Update<DBState>("value", _value, "id=" + _id); }
    public static void SetState(int _id, string _name) { DataBase.Update<DBState>("name", _name, "id=" + _id); }


    // Utility
    public static string GetUniqueMapObjectName(string pattern) {
        int n = 0;
        while (true) {
            string name = pattern + (n > 0 ? " " + n : "");
            if (DataBase.Select<DBMapObject>("name=" + name).Count == 0)
                return name;
            n++;
        }
    }
}
