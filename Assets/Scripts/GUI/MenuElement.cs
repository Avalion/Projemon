using UnityEngine;

public class MenuElement {
    public enum Type { Space, Label, Button }
    public Type type = Type.Label;

    public object value;

    public delegate void MenuAction();
    public MenuAction action = null;

    // Macros
    public static MenuElement FlexibleSpace() {
        return new MenuElement();
    }
    public static MenuElement Space(int value) {
        return new MenuElement(Type.Space, value);
    }
    public static MenuElement Label(string value) {
        return new MenuElement(Type.Label, value);
    }
    public static MenuElement Button(GUIContent value, MenuAction _action) {
        return new MenuElement(Type.Button, value, _action);
    }
    


    private MenuElement(MenuElement.Type _type = Type.Space, object _value = null, MenuElement.MenuAction _action = null) {
        type = _type;
        value = _value;
        action = _action;
    }

    public object Display(bool active = false) {
        switch (type) {
            case Type.Space:
                if (value == null || (int)value == 0)
                    GUILayout.FlexibleSpace();
                else
                    GUILayout.Space((int)value);
                break;
            case Type.Label:
                GUILayout.Label((string)value, active ? World.Current.InterfaceResources.menuElementStyle : World.Current.InterfaceResources.menuElementActiveStyle);
                break;
            case Type.Button:
                if (GUILayout.Button((GUIContent)value, active ? World.Current.InterfaceResources.menuElementStyle : World.Current.InterfaceResources.menuElementActiveStyle) && action != null)
                    action();
                break;
        }

        return null;
    }

}