using System.Collections.Generic;
using UnityEngine;

public class Menu {
    // Current is the menu with the focus
    public static Menu Current = null;


    // Display
    public Rect position;

    // References
    public Menu parent = null;

    private MenuDisplay display;

    // Contents
    public List<List<MenuElement>> elements = new List<List<MenuElement>>();


    
    // Constructors
    public void Open(bool focus = true) {
        display = new GameObject("Menu").AddComponent<MenuDisplay>();
        display.menu = this;

        display.selected = GetFirstElementIndex();

        Player.Lock();

        if (focus) 
            Current = this;
    }
    public void Close() {
        GameObject.Destroy(display.gameObject);

        Player.Unlock();

        if (Current == this && parent != null)
            Current = parent;
    }

    // Elements gestion 
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

    public void Clear() {
        foreach (List<MenuElement> list in elements)
            list.Clear();

        elements.Clear();
    }
    public void Clear(int _row) {
        elements[_row].Clear();
    }
}

public class MenuDisplay : IDisplayable {
    public Menu menu;

    public Vector2 selected;

    // Private
    private Rect cursorPosition;


    // Display
    public override void Display() {
        GUILayout.BeginArea(menu.position);
        for (int row = 0; row < menu.elements.Count; row++) {
            if (menu.elements[row].Count == 0)
                continue;

            GUILayout.BeginHorizontal();
            for (int column = 0; column < menu.elements[row].Count; column++) {
                if (menu.elements[row][column] == null)
                    continue;

                menu.elements[row][column].Display(selected == new Vector2(row, column));

                if (selected == new Vector2(row, column) && Menu.Current == menu && Event.current.type == EventType.Repaint)
                    CalcCursorPos(GUILayoutUtility.GetLastRect());
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndArea();
    }

    // MonoBehaviour
    public void Update() {
        if (InputManager.Current.GetKey(KeyCode.LeftArrow)) {
            int index = (int)selected.y - 1;
            while (index > 0 && menu.GetElement((int)selected.x, index) == null)
                --index;
            if (index != -1)
                selected.y = index;
        } else if (InputManager.Current.GetKey(KeyCode.RightArrow)) {
            int index = (int)selected.y + 1;
            while (index < menu.elements[(int)selected.x].Count && menu.GetElement((int)selected.x, index) == null)
                --index;
            if (index != -1)
                selected.y = index;
        } else if (InputManager.Current.GetKey(KeyCode.UpArrow)) {
            int index = (int)selected.x - 1;
            while (index > 0 && (menu.elements[index].Count == 0 || menu.elements[index].Find(P => P != null) == null))
                --index;
            if (index != -1) {
                selected.x = index;
                if (menu.GetElement((int)selected.x, (int)selected.y) == null) {
                    int indexmoins = (int)selected.x - 1;
                    int indexplus = (int)selected.x + 1;
                    while (indexmoins > 0 && indexplus < menu.elements[(int)selected.x].Count) {
                        if (indexmoins > 0 && menu.GetElement((int)selected.x, indexmoins) != null) {
                            selected.y = indexmoins;
                            break;
                        }
                        --indexmoins;
                        if (indexplus < menu.elements[(int)selected.x].Count && menu.GetElement((int)selected.x, indexplus) != null) {
                            selected.y = indexplus;
                            break;
                        }
                        ++indexplus;
                    }
                }
            }
        } else if (InputManager.Current.GetKey(KeyCode.DownArrow)) {
            int index = (int)selected.x + 1;
            while (index > 0 && (menu.elements[index].Count == 0 || menu.elements[index].Find(P => P != null) == null))
                ++index;
            if (index != -1) {
                selected.x = index;
                if (menu.GetElement((int)selected.x, (int)selected.y) == null) {
                    int indexmoins = (int)selected.x - 1;
                    int indexplus = (int)selected.x + 1;
                    while (indexmoins > 0 && indexplus < menu.elements[(int)selected.x].Count) {
                        if (indexmoins > 0 && menu.GetElement((int)selected.x, indexmoins) != null) {
                            selected.y = indexmoins;
                            break;
                        }
                        --indexmoins;
                        if (indexplus < menu.elements[(int)selected.x].Count && menu.GetElement((int)selected.x, indexplus) != null) {
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
    public Rect CalcCursorPos(Rect _itemRect) {
        if (World.Current.InterfaceResources._cursor)
            Debug.LogWarning("no cursor defined");

        Vector2 cursorSize = new Vector2(World.Current.InterfaceResources._cursor.width, World.Current.InterfaceResources._cursor.height);

        cursorPosition = new Rect(
                _itemRect.x + _itemRect.width - cursorSize.x * (0.4f + 0.1f * MathUtility.Lerp2),
                _itemRect.y + _itemRect.height - cursorSize.y * (0.6f + 0.1f * MathUtility.Lerp2),
                cursorSize.x,
                cursorSize.y
            );
        return cursorPosition;
    }
}
