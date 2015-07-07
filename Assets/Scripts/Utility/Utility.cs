using UnityEngine;
using System.Collections.Generic;
using System.IO;

/**
 * This class provides some useful functions
 */
public class Utility {
    public static GameObject FindGameObject(string name) {
        if (name == "")
            return null;
        GameObject o = GameObject.Find(name);
        if (o == null)
            o = new GameObject(name);
        return o;
    }

    /** Log
     */
    public static void Log(object o) {
        Debug.Log(o);

        StreamWriter sw = new StreamWriter("log.txt", true);
        sw.WriteLine(o.ToString());
        sw.Close();
    }

    /** Color as Strings
     */
    public static string ConvertColorToHexa(Color32 _color) {
        //We use Color32 because ToString("x") is not working with Color.
        string colorS = _color.ToString("x");
        colorS = colorS.Replace("RGBA(", " ");
        colorS = colorS.Replace(")", " ");
        colorS = colorS.Replace(" ", "");
        string[] colorRGBA = colorS.Split(',');
        for (int i = 0; i < colorRGBA.Length; ++i) {
            if (colorRGBA[i].Length == 1) {
                colorRGBA[i] = "0" + colorRGBA[i];
            }

        }
        if (colorRGBA.Length >= 4)
            return "#" + colorRGBA[0] + colorRGBA[1] + colorRGBA[2] + colorRGBA[3];
        else
            return "#" + colorRGBA[0] + colorRGBA[1] + colorRGBA[2];
    }
    public static string ConvertTextureToHexa(Texture2D _texture2D) {
        try {
            Color32 _color = _texture2D.GetPixel(1, 1);
            //We use Color32 because ToString("x") is not working with Color.
            string colorS = _color.ToString("x");
            colorS = colorS.Replace("RGBA(", " ");
            colorS = colorS.Replace(")", " ");
            colorS = colorS.Replace(" ", "");
            string[] colorRGBA = colorS.Split(',');
            for (int i = 0; i < colorRGBA.Length; ++i) {
                if (colorRGBA[i].Length == 1)
                    colorRGBA[i] = "0" + colorRGBA[i];
            }

            if (colorRGBA.Length >= 4)
                return "#" + colorRGBA[0] + colorRGBA[1] + colorRGBA[2] + colorRGBA[3];
            else
                return "#" + colorRGBA[0] + colorRGBA[1] + colorRGBA[2];
        } catch (System.Exception) {
            Debug.LogWarning("An error occured importing a texture. Texture changed to White color");
            return "#FFFFFFFF";
        }
    }
    public static Color ConvertHexaToColor(string _value) {
        Color c = new Color();

        // parse string color
        if (!_value.StartsWith("#") || (_value.Length != 7 && _value.Length != 9))
            throw new System.ArgumentException("Hexa color not start with # or not have good length : " + c);

        // remove # start
        string color = _value.Substring(1);
        c.r = System.Int32.Parse(color.Substring(0, 2), System.Globalization.NumberStyles.HexNumber) / 255f;
        c.g = System.Int32.Parse(color.Substring(2, 2), System.Globalization.NumberStyles.HexNumber) / 255f;
        c.b = System.Int32.Parse(color.Substring(4, 2), System.Globalization.NumberStyles.HexNumber) / 255f;
        c.a = (_value.Length == 9) ? System.Int32.Parse(color.Substring(6, 2), System.Globalization.NumberStyles.HexNumber) / 255f : 1;
        return c;
    }
    public static Color ConvertTextureToColor(Texture2D _texture2D) {
        try {
            return _texture2D.GetPixel(1, 1);
        } catch (System.Exception) {
            Debug.LogWarning("An error occured importing a texture. Texture changed to White color");
            return Color.white;
        }
    }
    public static Texture2D ConvertHexaToTexture(string _value, int _x = 1, int _y = 1) {
        return ConvertColorToTexture(ConvertHexaToColor(_value), _x, _y);
    }
    public static Texture2D ConvertColorToTexture(Color _color, int _x = 1, int _y = 1) {
        // make texture2d with parsed color
        Texture2D retour = new Texture2D(_x, _y);

        Color[] colors = new Color[_x * _y];

        for (int i = 0; i < _x; i++)
            for (int j = 0; j < _y; j++)
                colors[j * _x + i] = _color;

        retour.SetPixels(colors);
        retour.Apply();

        // return it
        return retour;
    }

    /** verifiaction macro
     */
    public static bool IsBetween(float value, float min, float max) {
        return value <= max && value >= min;
    }

    /** Types
     */
    public static Color GetColorFromType(Monster.Type _type) {
        switch (_type) {
            case Monster.Type.Water: return new Color(0.6f, 0.6f, 1f, 0.7f);
            case Monster.Type.Fire: return new Color(1f, 0.2f, 0.2f, 0.7f);
            case Monster.Type.Earth: return new Color(0.7f, 0.3f, 0.1f, 0.7f);
            case Monster.Type.Air: return new Color(0.8f, 0.8f, 0.8f, 0.7f);
            case Monster.Type.Elec: return new Color(1f, 1f, 0.6f, 0.7f);
            case Monster.Type.Plant: return new Color(0.6f, 1f, 0.6f, 0.7f);
            case Monster.Type.Shadow: return new Color(0.5f, 0.2f, 0.4f, 0.7f);
            case Monster.Type.Life: return new Color(1f, 0.9f, 0.8f, 0.7f);
            case Monster.Type.Death: return new Color(0.2f, 0.2f, 0.2f, 0.7f);
            case Monster.Type.Stone: return new Color(0.4f, 0.4f, 0.4f, 0.7f);
            case Monster.Type.Ice: return new Color(0.7f, 0.8f, 1f, 0.7f);
            case Monster.Type.Metal: return new Color(0.6f, 0.6f, 0.6f, 0.7f);
            default: return Color.black;
        }
    }

    /** GameObjects
    */
    public static void SetLayerRecursively(Transform _parent, LayerMask _layer) {
        _parent.gameObject.layer = _layer;
        foreach (Transform child in _parent)
            SetLayerRecursively(child, _layer);
    }
    
    /** Shaders
    */
    static string[] commonPropertyFloatNames = new string[] { "_AO", "_BaseLight", "_BumpAmt", "_BumpReflectionStr", "_ChromaticDispersion", "_Cutoff", "_EmissionLM", "_InvFade", "_Occlusion", "_Parallax", "_ReflDistort", "_ReflectStrength", "_ReflToRefrExponent", "_Refraction", "_RefrDistort", "_ShadowStrength", "_Shininess", "_SquashAmount", "_TranslucencyViewDependency", "_WaveScale" };
    static string[] commonPropertyColorNames = new string[] { "_Color", "_EmisColor", "_Emission", "_GlowColor", "_HorizonColor", "_ReflectColor", "_RefrColor", "_RefrColorDepth", "_SpecColor", "_SpecularColor", "_Tint", "_TintColor", "_UnderwaterColor" };
    static string[] commonPropertyTextureNames = new string[] { "_BackTex", "_BumpMap", "_BumpSpecMap", "_Control", "_Cube", "_DecalTex", "_Detail", "_DisplacementHeightMap", "_DownTex", "_Fresnel", "_FalloffTex", "_FrontTex", "_GlossMap", "_Illum", "_LeftTex", "_LightMap", "_LightTextureB0", "_MainTex", "_ParallaxMap", "_ReflectionTex","_ReflectiveColor", "_ReflectiveColorCube", "_RefractionTex", "_RightTex", "_SecondDisplacementHeightMap", "_ShadowOffset", "_ShadowTex", "_ShoreTex", "_Splat0", "_Splat1", "_Splat2", "_Splat3", "_Tex", "_TranslucencyMap", "_UpTex" };
    static string[] commonPropertyVectorNames = new string[] { "WaveSpeed", "_DisplacementXz", "_Displacement", "_ShoreTiling", "_Scale" };

    public static string[] GetMaterialColorProperties(Material _m) {
        if (_m == null)
            return new string[] { };
        List<string> list = new List<string>();

        foreach (string s in commonPropertyColorNames) {
            if (_m.HasProperty(s))
                list.Add(s);
        }

        return list.ToArray();
    }
    public static string[] GetMaterialTextureProperties(Material _m) {
        if (_m == null)
            return new string[] { };
        List<string> list = new List<string>();
        foreach (string s in commonPropertyTextureNames) {
            if (_m.HasProperty(s))
                list.Add(s);
        }

        return list.ToArray();
    }
    public static string[] GetMaterialFloatProperties(Material _m) {
        if (_m == null)
            return new string[] { };
        List<string> list = new List<string>();

        foreach (string s in commonPropertyFloatNames) {
            if (_m.HasProperty(s))
                list.Add(s);
        }

        return list.ToArray();
    }
    public static string[] GetMaterialVectorProperties(Material _m) {
        if (_m == null)
            return new string[] { };
        List<string> list = new List<string>();

        foreach (string s in commonPropertyVectorNames) {
            if (_m.HasProperty(s))
                list.Add(s);
        }

        return list.ToArray();
    }
}