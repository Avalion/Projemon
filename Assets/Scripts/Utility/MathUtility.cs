using UnityEngine;

/**
 * This class provides some useful math relative functions
 */
public class MathUtility {
	/** Rect operators
	*/
	public static Rect AddRect(Rect _in, float _x, float _y, float _width, float _height) {
        return new Rect(_in.x + _x, _in.y + _y, _in.width + _width, _in.height + _height);
    }
    public static Rect ExtendRect(Rect _in, float _size) {
        return new Rect(_in.x - _size, _in.y - _size, _in.width + 2 * _size, _in.height + 2 * _size);
    }

	public static Rect MultRect(Rect _in, float _value) {
        return new Rect((int)(_in.x * _value), (int)(_in.y * _value), (int)(_in.width * _value), (int)(_in.height * _value));
    }
	public static Rect MultRectSize(Rect _in, float _value) {
        return new Rect(_in.x, _in.y, (int)(_in.width * _value), (int)(_in.height * _value));
    }
	
    public static RectOffset AddRectOffset(RectOffset _a, RectOffset _b) {
        return new RectOffset(_a.left + _b.left, _a.right + _b.right, _a.top + _b.top, _a.bottom + _b.bottom);
    }
	public static RectOffset MultRectOffset(RectOffset _in, float _value) {
        return new RectOffset((int)(_in.left * _value), (int)(_in.right * _value), (int)(_in.top * _value), (int)(_in.bottom * _value));
    }

    public static bool IsPair(int _value) { 
        return _value % 2 == 0; 
    }
}