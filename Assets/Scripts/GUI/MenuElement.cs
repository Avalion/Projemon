using UnityEngine;

public class MenuElement {
    public enum Type { Space, Label, Button }
    public Type type = Type.Label;

    public object value;

    public delegate void MenuAction();
    public MenuAction action = null;

    public MenuElement(MenuElement.Type _type = Type.Space, object _value = null, MenuElement.MenuAction _action = null) {
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
                if (GUILayout.Button((string)value, active ? World.Current.InterfaceResources.menuElementStyle : World.Current.InterfaceResources.menuElementActiveStyle) && action != null)
                    action();
                break;
        }

        return null;
    }

}