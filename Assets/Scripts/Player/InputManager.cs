using UnityEngine;
using System.Collections.Generic;

/**
 * This class manages inputs
 */
public class InputManager : MonoBehaviour {
    private static InputManager ms_current;
    public static InputManager Current {
        get {
            if (ms_current == null) {
                ms_current = GameObject.FindObjectOfType<InputManager>();
                if (ms_current == null)
                    ms_current = Utility.FindGameObject("InputManager").AddComponent<InputManager>();
            }
            return ms_current;
        }
    }

    /**
     * constructor : initialize devise
     */
    void Start() {
    }


    /**
     * Mouse Movement
     */
    public float XAxisPixels {
        get { return Input.GetAxis("Mouse X") * 4; }
    }
    public float XAxisScreen01 {
        get {
            return XAxisPixels / Screen.width;
        }
    }
    public float YAxisPixels {
        get { return Input.GetAxis("Mouse Y") * 4; }
    }
    public float YAxisScreen01 {
        get {
            return YAxisPixels / Screen.height;
        }
    }

    public float ZoomAxis {
        get { return Input.GetAxis("Mouse ScrollWheel"); }
    }
    
    /**
     * Mouse Buttons
     */
    public bool ButtonPressed() {
        return (Input.GetMouseButton(0) || Input.GetMouseButton(1) || Input.GetMouseButton(2));
    }
    public bool ButtonPressed(int _b) {
        return Input.GetMouseButton(_b);
    }
    
    public bool ButtonPressedUp() {
        return (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1) || Input.GetMouseButtonUp(2));
    }
    public bool ButtonPressedUp(int _b) {
        return Input.GetMouseButtonUp(_b);
    }
    public bool ButtonPressedDown() {
        return (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2));
    }
    public bool ButtonPressedDown(int _b) {
        return Input.GetMouseButtonDown(_b);
    }

    /**
     * get input coords
     */
    public Vector2 GetControlPos() {
        return Input.mousePosition;
    }

    /**
     * Keyboard
     */
    private List<KeyCode> GetKeysToCheck(KeyCode key) {
        List<KeyCode> keysToInspect = new List<KeyCode>() { key };

        switch (key) {
            case KeyCode.Return:
                keysToInspect.Add(KeyCode.KeypadEnter); break;
            case KeyCode.UpArrow:
                keysToInspect.Add(KeyCode.Z); break;
            case KeyCode.LeftArrow:
                keysToInspect.Add(KeyCode.Q); break;
            case KeyCode.RightArrow:
                keysToInspect.Add(KeyCode.D); break;
            case KeyCode.DownArrow:
                keysToInspect.Add(KeyCode.S); break;
            case KeyCode.LeftShift:
                keysToInspect.Add(KeyCode.RightShift); break;
            case KeyCode.RightShift:
                keysToInspect.Add(KeyCode.LeftShift); break;
        }

        return keysToInspect;
    }
    
    public bool GetKey(KeyCode key) {
        List<KeyCode> keysToInspect = GetKeysToCheck(key);

        bool valid = false;
        foreach (KeyCode k in keysToInspect)
            valid = valid || Input.GetKey(k);

        return valid;
    }
    public bool GetKeyDown(KeyCode key) {
        List<KeyCode> keysToInspect = GetKeysToCheck(key);

        bool valid = false;
        foreach (KeyCode k in keysToInspect)
            valid = valid || Input.GetKeyDown(k);

        return valid;
    }
    public bool GetKeyUp(KeyCode key) {
        List<KeyCode> keysToInspect = GetKeysToCheck(key);

        bool valid = false;
        foreach (KeyCode k in keysToInspect)
            valid = valid || Input.GetKeyUp(k);

        return valid;
    }

}

