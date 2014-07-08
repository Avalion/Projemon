﻿using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/**
 * This class provides some useful functions
 */
public class EditorUtility {
    public static void DrawEmptyRect(Rect _r, Color _color, int _size) {
        EditorGUI.DrawRect(new Rect(_r.x, _r.y, _r.width, _size), _color);
        EditorGUI.DrawRect(new Rect(_r.x, _r.y, _size, _r.height), _color);
        EditorGUI.DrawRect(new Rect(_r.x + _r.width - _size, _r.y, _size, _r.height), _color);
        EditorGUI.DrawRect(new Rect(_r.x, _r.y + _r.height - _size, _r.width, _size), _color);
    }

    public static int DisplayList<T>(int _selected, List<T> _elements, ref Vector2 _scrollPos, params GUILayoutOption[] _options) {
        int value = -1;
        _scrollPos = GUILayout.BeginScrollView(_scrollPos, _options);
        for (int i = 0; i < _elements.Count; i++) {
            if (GUILayout.Button(InterfaceUtility.IntString(i + 1, 3) + ": " + _elements[i].ToString(), i == _selected ? InterfaceUtility.SelectedStyle : InterfaceUtility.EmptyStyle))
                value = i;
        };
        GUILayout.EndScrollView();
        if (value == -1)
            return _selected;
        else
            return value;
    }


    [MenuItem("Creation/Clear All Caches &r")]
    public static void Clear() {
        Debug.Log("Cleared " + InterfaceUtility.ClearAllCache() + " objects !");
    }
}
