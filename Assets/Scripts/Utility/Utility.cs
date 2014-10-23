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