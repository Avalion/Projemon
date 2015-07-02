using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/**
 * This class provides some useful functions
 */
public class UtilityEditor {
    /**
     *  GUI
     */
    public static void DrawEmptyRect(Rect _r, int _size, Color _color) {
        EditorGUI.DrawRect(new Rect(_r.x, _r.y, _r.width, _size), _color);
        EditorGUI.DrawRect(new Rect(_r.x, _r.y, _size, _r.height), _color);
        EditorGUI.DrawRect(new Rect(_r.x + _r.width - _size, _r.y, _size, _r.height), _color);
        EditorGUI.DrawRect(new Rect(_r.x, _r.y + _r.height - _size, _r.width, _size), _color);
    }

    /**
     *  Special Editor
     */
    public static int DisplayList<T>(int _selected, List<T> _elements, ref Vector2 _scrollPos, params GUILayoutOption[] _options) {
        int value = -1;
        _scrollPos = GUILayout.BeginScrollView(_scrollPos, _options);
        for (int i = 0; i < _elements.Count; i++) {
            if (GUILayout.Button(InterfaceUtility.IntString(i + 1, 3) + ": " + _elements[i].ToString(), i == _selected ? InterfaceUtility.SelectedStyle : InterfaceUtility.LabelStyle))
                value = i;
        };
        GUILayout.EndScrollView();
        if (value == -1)
            return _selected;
        else
            return value;
    }
    
    public static int DisplayTableIDPopup<T>(int _selected, List<T> _elements, params GUILayoutOption[] _options) where T : SQLTable {
        List<string> names = new List<string>();
        for (int i = 0; i < _elements.Count; ++i)
            if (_elements[i] != null)
                names.Add(InterfaceUtility.IntString(i + 1, 3) + ": " + _elements[i].ToString());
        
        int index = _elements.FindIndex(D => D.ID == _selected);
        
        index = EditorGUILayout.Popup(index, names.ToArray());

        return _elements[index].ID;
    }


    public static int MapField(string label, int _mapId, params GUILayoutOption[] _options) {
        List<int> ids = new List<int>();
        List<string> names = new List<string>();

        foreach (Map mo in SystemDatas.GetMaps()) {
            ids.Add(mo.ID);
            names.Add(InterfaceUtility.IntString(mo.ID + 1, 3) + ": " + mo.name);
        }

        return EditorGUILayout.IntPopup(label, _mapId, names.ToArray(), ids.ToArray(), _options);
    }
    public static int MapObjectField(string label, int _mapObjectId, bool includePlayer, params GUILayoutOption[] _options) {
        List<int> ids = new List<int>();
        List<string> names = new List<string>();

        if (World.Current.currentMap == null) {
            GUILayout.Label("Exception : No map selected...", InterfaceUtility.ErrorStyle);
            return -2;
        }

        foreach (MapObject mo in World.Current.currentMap.mapObjects) {
            ids.Add(mo.mapObjectId);
            names.Add(mo.mapObjectId + ": " + mo.name);
        }

        if (includePlayer) {
            ids.Insert(0, -1);
            names.Insert(0, "Player");
        }

        return EditorGUILayout.IntPopup(label, _mapObjectId, names.ToArray(), ids.ToArray(), _options);
    }
    public static int MonsterPatternField(string label, int _varId, params GUILayoutOption[] _options) {
        List<int> ids = new List<int>();
        List<string> names = new List<string>();

        foreach (DBMonsterPattern monster in DataBase.Select<DBMonsterPattern>()) {
            ids.Add(monster.ID);
            names.Add(InterfaceUtility.IntString(monster.ID + 1, 3) + ": " + monster.name);
        }

        if (label == "")
            return EditorGUILayout.IntPopup(_varId, names.ToArray(), ids.ToArray(), _options);
        else
            return EditorGUILayout.IntPopup(label, _varId, names.ToArray(), ids.ToArray(), _options);
    }
    public static int StateField(string label, int _stateId, params GUILayoutOption[] _options) {
        List<int> ids = new List<int>();
        List<string> names = new List<string>();

        foreach (DBState state in DataBase.Select<DBState>()) {
            ids.Add(state.ID);
            names.Add(InterfaceUtility.IntString(state.ID + 1, 3) + ": " + state.name);
        }

        if (label == "")
            return EditorGUILayout.IntPopup(_stateId, names.ToArray(), ids.ToArray(), _options);
        else
            return EditorGUILayout.IntPopup(label, _stateId, names.ToArray(), ids.ToArray(), _options);
    }
    public static int VariableField(string label, int _varId, params GUILayoutOption[] _options) {
        List<int> ids = new List<int>();
        List<string> names = new List<string>();

        foreach (DBVariable var in DataBase.Select<DBVariable>()) {
            ids.Add(var.ID);
            names.Add(InterfaceUtility.IntString(var.ID + 1, 3) + ": " + var.name);
        }

        if (label == null)
            return EditorGUILayout.IntPopup(label, _varId, names.ToArray(), ids.ToArray(), _options);
        else
            return EditorGUILayout.IntPopup(label, _varId, names.ToArray(), ids.ToArray(), _options);
    }


    public static List<string> ToStringList<T>(List<T> list) {
        List<string> stringList = new List<string>();
        foreach (T t in list) {
            stringList.Add(t.ToString());
        }
        return stringList;
    }

    
    /**
    *  Generic implementation of AssetDatabase
    */
    public static T LoadAssetAtPath<T>(string _path) where T : Object {
        if (_path.Contains(Application.dataPath.Replace("/", "\\")) || _path.Contains(Application.dataPath))
            _path = "Assets" + _path.Remove(0, Application.dataPath.Length);

        T obj = AssetDatabase.LoadAssetAtPath(_path.Replace("\\", "/"), typeof(T)) as T;

        if (obj == null)
            Debug.LogWarning("The " + typeof(T).ToString() + " " + _path + " wasn't found");

        return obj;
    }

    /**
     *  Cache gestion
     */
    [MenuItem("Creation/Clear All Caches &r")]
    public static void Clear() {
        Debug.Log("Cleared " + InterfaceUtility.ClearAllCache() + " objects !");
    }

}
