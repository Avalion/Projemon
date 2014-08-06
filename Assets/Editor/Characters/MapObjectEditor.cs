using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/**
 * This class design MapObject Inspector
 */
public class MapObjectEditor : Editor {
    MapObject mapObject = null;

    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        if (mapObject == null) {
            mapObject = target as MapObject;
        }

        if (GUILayout.Button("Edit Actions")) {
            ObjectActionList list = EditorWindow.GetWindow<ObjectActionList>();
            list.minSize = new Vector2(1200, 400);
            list.mapObject = mapObject;
            list.Show();
        }
    }
}

[CustomEditor(typeof(Player))]
public class PlayerEditor : MapObjectEditor { }

[CustomEditor(typeof(PNJBattler))]
public class PNJBattlerEditor : MapObjectEditor { }
