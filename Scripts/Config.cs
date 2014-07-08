using UnityEngine;
using System.IO;

/**
 * This class provides some useful functions to get right folders and to Normalize some elements
 */
public class Config {
    /** Paths
     */
    private static string ConfigPath { get { return Application.dataPath + "/System/"; } }
    public static string GetConfigPath(string _folder) {
        if (!Directory.Exists(ConfigPath + _folder))
            Directory.CreateDirectory(ConfigPath + _folder);
        return ConfigPath + _folder + "/";
    }

    private static string ResourcePath { get { return "Assets/Resources/"; } }
    public static string GetResourcePath(string _folder) {
        if (!Directory.Exists(ResourcePath + _folder))
            Directory.CreateDirectory(ResourcePath + _folder);
        return ResourcePath + _folder + "/";
    }

    /** Normalize string
     */
    public static Rect StringToRect(string _input) {
        string[] values = _input.Replace("(", "").Replace(")", "").Split(',');

        try {
            return new Rect(float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2]), float.Parse(values[3]));
        }
        catch { return new Rect(); }
    }
    public static string RectToString(Rect _input) {
        return "(" + _input.x + "," + _input.y + "," + _input.width + "," + _input.height + ")";
    }
}
