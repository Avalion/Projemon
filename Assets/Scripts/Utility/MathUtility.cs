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

    public static float Alea(float[] _float) {
        if (_float.Length == 0)
            throw new System.Exception("MathException : Trying to get values from an empty table.");
        return _float[Random.Range(0, _float.Length - 1)];
    }

    public static bool IsPair(int _value) { 
        return _value % 2 == 0; 
    }

    public static bool TestProbability(float value) {
        if (value < 0 || value > 1)
            throw new System.Exception("MathException : Invalid value to calc probabilities !");
        if (value == 1)
            return true;

        float random = Random.Range(0, 1f);

        return (random <= value);
    }
    public static bool TestProbability100(float value) {
        return TestProbability(value / 100f);
    }



    public static float Lerp {
        get { return Mathf.Cos(Time.time) / 2f + 0.5f; }
    }
    public static float Lerp2 {
        get { return Mathf.Cos(Time.time); }
    }
}