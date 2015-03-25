using System.Collections.Generic;
using UnityEngine;

public abstract class Menu : IDisplayable {
    // Current is the menu with the focus
    private static Menu Current = null;


    // References
    public Menu parent = null;
    private Vector2 selected;

    // Display
    public bool show;

    public List<List<MenuElement>> elements = new List<List<MenuElement>>();

    public Rect position;

    // Private
    private Rect cursorPosition;



    // Constructors
    public void Open(bool focus = true) {
        show = true;

        selected = GetFirstElementIndex();

        if (focus) 
            Current = this;
    }
    public void Close() {
        show = false;

        if (Current == this && parent != null)
            Current = parent;
    }

    // Elements gestion
    public void Add(MenuElement.Type _type, object _value, MenuElement.MenuAction _action, int _column, int _row) {
        Add(new MenuElement(_type, _value, _action), _column, _row);
    }
    public void Add(MenuElement element, int _row, int _column) {
        while (elements.Count < _row)
            elements.Add(new List<MenuElement>());
        while (elements[_row].Count < _column)
            elements[_row].Add(null);

        elements[_row][_column] = element;
    }

    public Vector2 GetFirstElementIndex() {
        for (int row = 0; row < elements.Count; ++row) {
            for (int column = 0; column < elements[column].Count; ++column) {
                if (elements[row][column] == null)
                    continue;
                return new Vector2(row, column);
            }
        }
        return new Vector2(-1,-1);
    }
    public MenuElement GetElement(int _row, int _column) {
        if (_row < 0 || _column < 0 || elements.Count < _row || elements[_row].Count < _column)
            return null;

        return elements[_row][_column];
    }

    public Vector2 FindIndex(MenuElement _element) {
        if (_element == null)
            return new Vector2(-1, -1);

        for (int row = 0; row < elements.Count; ++row) {
            for (int column = 0; column < elements[column].Count; ++column) {
                if (elements[row][column] == _element)
                    return new Vector2(row, column);
            }
        }
        return new Vector2(-1, -1);
    }

    // Display
    public override void Display() {
        if (!show)
            return;

        GUILayout.BeginArea(position);
        for (int row = 0; row < elements.Count; row++) {
            if (elements[row].Count == 0)
                continue;

            GUILayout.BeginHorizontal();
            for (int column = 0; column < elements[row].Count; column++) {
                if (elements[row][column] == null)
                    continue;

                elements[row][column].Display(selected == new Vector2(row, column));
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndArea();
    }

    // MonoBehaviour
    public void Update() {
        if (InputManager.Current.GetKey(KeyCode.LeftArrow)) {
            int index = (int)selected.y - 1;
            while (index > 0 && GetElement((int)selected.x, index) == null)
                --index;
            if (index != -1)
                selected.y = index;
        } else if (InputManager.Current.GetKey(KeyCode.RightArrow)) {
            int index = (int)selected.y + 1;
            while (index < elements[(int)selected.x].Count && GetElement((int)selected.x, index) == null)
                --index;
            if (index != -1)
                selected.y = index;
        } else if (InputManager.Current.GetKey(KeyCode.UpArrow)) {
            int index = (int)selected.x - 1;
            while (index > 0 && (elements[index].Count == 0 || elements[index].Find(P => P != null) == null))
                --index;
            if (index != -1) {
                selected.x = index;
                if (GetElement((int)selected.x, (int)selected.y) == null) {
                    int indexmoins = (int)selected.x - 1;
                    int indexplus = (int)selected.x + 1;
                    while (indexmoins > 0 && indexplus < elements[(int)selected.x].Count) {
                        if (indexmoins > 0 && GetElement((int)selected.x, indexmoins) != null) {
                            selected.y = indexmoins;
                            break;
                        }
                        --indexmoins;
                        if (indexplus < elements[(int)selected.x].Count && GetElement((int)selected.x, indexplus) != null) {
                            selected.y = indexplus;
                            break;
                        }
                        ++indexplus;
                    }
                }
            }
        } else if (InputManager.Current.GetKey(KeyCode.DownArrow)) {
            int index = (int)selected.x + 1;
            while (index > 0 && (elements[index].Count == 0 || elements[index].Find(P => P != null) == null))
                ++index;
            if (index != -1) {
                selected.x = index;
                if (GetElement((int)selected.x, (int)selected.y) == null) {
                    int indexmoins = (int)selected.x - 1;
                    int indexplus = (int)selected.x + 1;
                    while (indexmoins > 0 && indexplus < elements[(int)selected.x].Count) {
                        if (indexmoins > 0 && GetElement((int)selected.x, indexmoins) != null) {
                            selected.y = indexmoins;
                            break;
                        }
                        --indexmoins;
                        if (indexplus < elements[(int)selected.x].Count && GetElement((int)selected.x, indexplus) != null) {
                            selected.y = indexplus;
                            break;
                        }
                        ++indexplus;
                    }
                }
            }
        }
    }

    // Others
    public Rect calcCursorPos(Rect _itemRect) {
        if (World.Current.InterfaceResources._cursor)
            Debug.LogWarning("no cursor defined");

        Vector2 cursorSize = new Vector2(World.Current.InterfaceResources._cursor.width, World.Current.InterfaceResources._cursor.height);

        cursorPosition = new Rect(
                _itemRect.x + _itemRect.width - cursorSize.x * 0.4f,
                _itemRect.y + _itemRect.height - cursorSize.y * 0.6f,
                cursorSize.x,
                cursorSize.y
            );
        return cursorPosition;
    }
}
