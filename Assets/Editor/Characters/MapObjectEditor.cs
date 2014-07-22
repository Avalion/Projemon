using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/**
 * This class design MapObject Inspector
 */
[CustomEditor(typeof(MapObject))]
public class MapObjectEditor : Editor {
    MapObject mapObject = null;

    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        if (mapObject == null) {
            mapObject = target as MapObject;
        }

        foreach(MapObjectAction a in mapObject.actions){
            GUILayout.BeginHorizontal();
            GUILayout.Label(a.GetType().ToString());
            GUILayout.EndHorizontal();
        }
    }
}

[CustomEditor(typeof(PNJBattler))]
public class PNJBattlerEditor : MapObjectEditor { }
