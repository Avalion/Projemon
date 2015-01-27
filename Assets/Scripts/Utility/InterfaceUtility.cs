using UnityEngine;
using System.Collections.Generic;

/**
 * This class provides some useful GUI relative functions
 */
public class InterfaceUtility {
    /** Default designs
     */
    private static GUIStyle emptyStyle = null;
    public static GUIStyle EmptyStyle {
        get {
            if (emptyStyle == null)
                emptyStyle = new GUIStyle();
            return emptyStyle;
        }
    }
    
    private static GUIStyle labelStyle = null;
    public static GUIStyle LabelStyle {
        get {
            if (labelStyle == null)
                labelStyle = new GUIStyle(GUI.skin.label);
            return labelStyle;
        }
    }

    private static GUIStyle warningStyle = null;
    public static GUIStyle WarningStyle {
        get {
            if (warningStyle == null) {
                warningStyle = new GUIStyle(GUI.skin.label);
                warningStyle.normal.textColor = Color.yellow;
            }
            return labelStyle;
        }
    }

    private static GUIStyle errorStyle = null;
    public static GUIStyle ErroStyle {
        get {
            if (errorStyle == null) {
                errorStyle = new GUIStyle(GUI.skin.label);
                errorStyle.normal.textColor = Color.red;
            }
            return labelStyle;
        }
    }

    private static GUIStyle selectedStyle = null;
    public static GUIStyle SelectedStyle {
        get {
            if (selectedStyle == null) {
                selectedStyle = new GUIStyle(GUI.skin.label);
                selectedStyle.normal.background = HexaToTexture("#6644FF66");
            }
            return selectedStyle;
        }
    }

    private static GUIStyle titleStyle = null;
    public static GUIStyle TitleStyle {
        get {
            if (titleStyle == null) {
                titleStyle = new GUIStyle(GUI.skin.label);
                titleStyle.fontStyle = FontStyle.Bold;
                titleStyle.fontSize = Mathf.RoundToInt(titleStyle.fontSize * 1.5f);
                titleStyle.padding.left = 20;
            }
            return titleStyle;
        }
    }

    private static GUIStyle centeredStyle = null;
    public static GUIStyle CenteredStyle {
        get {
            if (centeredStyle == null) {
                centeredStyle = new GUIStyle(GUI.skin.label);
                centeredStyle.alignment = TextAnchor.MiddleCenter;
            }
            return centeredStyle;
        }
    }


    /** String Gestion
     */
    public static string VerticalText(string _input) {
        string s = "";
        foreach (char c in _input)
            s += c + "\n";
        return s.Substring(0, s.Length - 1);
    }
    public static string IntString(int _input, int _nbchar) {
        string s = _input.ToString();
        while (s.Length < _nbchar) {
            s = "0" + s;
        }
        return s;
    }

    /** Generate textures
    */
    public static Color HexaToColor(string _value) {
        Color c = new Color();

        // parse string color
        if (!_value.StartsWith("#") || (_value.Length != 7 && _value.Length != 9))
            throw new System.ArgumentException("Hexa color don't start with # or haven't valid length : " + c);

        // remove # start
        string color = _value.Substring(1);
        c.r = System.Int32.Parse(color.Substring(0, 2), System.Globalization.NumberStyles.HexNumber) / 255f;
        c.g = System.Int32.Parse(color.Substring(2, 2), System.Globalization.NumberStyles.HexNumber) / 255f;
        c.b = System.Int32.Parse(color.Substring(4, 2), System.Globalization.NumberStyles.HexNumber) / 255f;
        c.a = (_value.Length == 9) ? System.Int32.Parse(color.Substring(6, 2), System.Globalization.NumberStyles.HexNumber) / 255f : 1;
        return c;
    }
    public static string ColorToHexa(Color32 _color) {
        string color = _color.ToString("x");
        color = color.Replace("RGBA(", " ");
        color = color.Replace(")", " ");
        color = color.Replace(" ", "");
        string[] colorRGBA = color.Split(',');
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

    public static Texture2D HexaToTexture(string _value) {
        return HexaToTexture(_value, 1, 1);
    }
    public static Texture2D HexaToTexture(string _value, int _x, int _y) {
        Color col = HexaToColor(_value);
        return ColorToTexture(col, _x, _y);
    }
    
    public static Texture2D ColorToTexture(Color _color) {
        return ColorToTexture(_color, 1, 1);
    }
    public static Texture2D ColorToTexture(Color _color, int _x, int _y) {
        string serializedId = _color.r + ":" + _color.g + ":" + _color.b + ":" + _color.a;
        
        if (colored.ContainsKey(serializedId))
            return colored[serializedId];
        
        Texture2D tex = new Texture2D(_x, _y);
        
        Color[] pixels = new Color[_x * _y];

        for (int i = 0; i < _x * _y; i++)
            pixels[i] = _color;

        tex.SetPixels(pixels);
        tex.Apply();

        colored.Add(serializedId, tex);

        return tex;
    }

    /** ProgressBar
     */
    private static GUIStyle ProgressBarBackground = new GUIStyle() { margin = new RectOffset(6,6,4,4) };
    public static void ProgressBar(float _width, float _height, int _value, int _max, Texture2D _color, Texture2D _background, params GUILayoutOption[] _options) {
        _value = Mathf.Clamp(_value, 0, _max);
        GUILayout.BeginHorizontal(_options);
        ProgressBarBackground.fixedHeight = _height;
        ProgressBarBackground.normal.background = _background;
        GUILayout.Label("", ProgressBarBackground, GUILayout.Width(_width));
        GUILayout.Space(-_width - ProgressBarBackground.margin.left);
        ProgressBarBackground.normal.background = _color;
        GUILayout.Label("", ProgressBarBackground, GUILayout.Width(Mathf.CeilToInt(_width * _value / (float)_max)));
        GUILayout.Space(_width - Mathf.CeilToInt(_width * _value / (float)_max));
        GUILayout.EndHorizontal();
    }

    /** Rect
     */
    public static Rect GetScreenRelativeRect(Rect r, bool sizeRelative = false) {
        float width = sizeRelative ? r.width * Screen.width : r.width;
        float height = sizeRelative ? r.height * Screen.height : r.height;
        return new Rect(r.x * (Screen.width - width), r.y * (Screen.height - height), width, height);
    }

    /** Tooltip
     */
    private static GUIStyle ms_tooltipStyle;
    public static GUIStyle TooltipStyle {
        get {
            if (ms_tooltipStyle == null) {
                ms_tooltipStyle = new GUIStyle();
                ms_tooltipStyle.normal.background = ColorToTexture(Color.white);
                ms_tooltipStyle.padding = new RectOffset(5, 5, 5, 5);
            }
            return ms_tooltipStyle;
        }
    }
    public static void DisplayToolTip() {
        if (GUI.tooltip != "") {
            Vector2 pos = InputManager.Current.GetControlPos() + new Vector2(15, 10);
            Vector2 size = TooltipStyle.CalcSize(new GUIContent(GUI.tooltip));

            GUI.Label(new Rect(Mathf.Min(pos.x, Screen.width - size.x), pos.y, size.x, size.y), GUI.tooltip, TooltipStyle);
        }
    }

    /** Box 
     */
    private static List<GUIStyle> ms_borders = null;
    public static GUIStyle GetBorder(string _border) {
        if (ms_borders == null) {
            ms_borders = new List<GUIStyle>();
            string[] bgs = new string[9] { "border_top_left", "border_top", "border_top_right", "border_left", "background", "border_right", "border_bottom_left", "border_bottom", "border_bottom_right" };
            foreach (string text in bgs) {
                GUIStyle g = new GUIStyle();

                g.normal.background = GetTexture(Config.GetResourcePath("System/Box") + text + ".png");
                
                g.wordWrap = true;

                if (text.Contains("top") || text.Contains("bottom"))
                    g.fixedHeight = 10;
                if (text.Contains("right") || text.Contains("left"))
                    g.fixedWidth = 10;

                ms_borders.Add(g);
            }
        }
        
        switch (_border) {
            case "top_left":
                return ms_borders[0];
            case "top":
                return ms_borders[1];
            case "top_right":
                return ms_borders[2];
            case "left":
                return ms_borders[3];
            case "background":
                return ms_borders[4];
            case "right":
                return ms_borders[5];
            case "bottom_left":
                return ms_borders[6];
            case "bottom":
                return ms_borders[7];
            case "bottom_right":
                return ms_borders[8];
        }
        return null;
    }

    public static void BeginBox(params GUILayoutOption[] _options) {
        GUILayout.BeginVertical(_options);
        GUILayout.BeginHorizontal();
        GUILayout.Label("", GetBorder("top_left"));
        GUILayout.Label("", GetBorder("top"));
        GUILayout.Label("", GetBorder("top_right"));
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("", GetBorder("left"), GUILayout.MaxHeight(Screen.height));
        GUILayout.BeginHorizontal(GetBorder("background"));
        GUILayout.Space(5);
        GUILayout.BeginVertical(GUILayout.MaxHeight(Screen.height));
        GUILayout.Space(5);
    }
    public static void EndBox() {
        GUILayout.Space(5);
        GUILayout.EndVertical();
        GUILayout.Space(5);
        GUILayout.EndHorizontal();
        GUILayout.Label("", GetBorder("right"), GUILayout.MaxHeight(Screen.height));
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("", GetBorder("bottom_left"));
        GUILayout.Label("", GetBorder("bottom"));
        GUILayout.Label("", GetBorder("bottom_right"));
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
    }
    public static int DisplayMenu(List<GUIContent> _contents, params GUILayoutOption[] _options) {
        BeginBox(_options);
        for (int i = 0; i < _contents.Count; i++) {
            if (GUILayout.Button(_contents[i])) {
                EndBox();
                return i;
            }
        }
        EndBox();
        return -1;
    }

    /** AudioClip
     */
    public static AudioClip GetAudio(string _relativePath) {
        if (loaded.ContainsKey(_relativePath))
            return loaded[_relativePath] as AudioClip;
        
        try {
            AudioClip clip = Resources.LoadAssetAtPath(_relativePath, typeof(AudioClip)) as AudioClip;
            loaded.Add(_relativePath, clip);
            return clip;
        } catch { Debug.LogError("File not found at " + _relativePath); }
        return null;
    }

    /** Textures
     */
    public static Texture2D GetTexture(string _relativePath) {
        if (loaded.ContainsKey(_relativePath))
            return loaded[_relativePath] as Texture2D;

        try {
            Texture2D tex = Resources.LoadAssetAtPath(_relativePath, typeof(Texture2D)) as Texture2D;
            loaded.Add(_relativePath, tex);
            return tex;
        } catch { Debug.LogError("File not found at " + _relativePath); }
        return null;
    }
    public static Texture2D InvertTexture(Texture2D _texture) {
        string serializedId = _texture.name;
        
        if (inverted.ContainsKey(serializedId))
            return inverted[serializedId];
        
        Texture2D tex = new Texture2D(_texture.width, _texture.height);

        Color[] pixels = _texture.GetPixels();
        Color[] newPixels = _texture.GetPixels();

        for (int i = 0; i < _texture.height; i++) {
            for (int j = 0; j < _texture.width; j++) {
                int index = i * _texture.width + j;
                int newIndex = i * _texture.width + (_texture.width - 1 - j);
                newPixels[newIndex] = pixels[index];
            }
        }

        tex.SetPixels(newPixels);
        tex.Apply();
        inverted.Add(serializedId, tex);

        return tex;
    }
    public static Texture2D SeparateTexture(Texture2D _texture, int x, int y, int _resolutionX, int _resolutionY) {
        if (_texture == null)
            return null;

        string serializedId = _texture.name + " (" + x + "," + y + "," + _resolutionX + "," + _resolutionY+")";

        if (separated.ContainsKey(serializedId))
            return separated[serializedId];
        
        Texture2D tex = new Texture2D(_resolutionX, _resolutionY);
        tex.name = serializedId;
        
        Color[] _pixels = _texture.GetPixels(x * _resolutionX, _texture.height - (y + 1) * _resolutionY, _resolutionX, _resolutionY);
        
        tex.SetPixels(_pixels);
        tex.Apply();

        separated.Add(serializedId, tex);

        return tex;
    }

    /** Cache
     */
    private static Dictionary<string, object> loaded = new Dictionary<string, object>();
    private static Dictionary<string, Texture2D> inverted = new Dictionary<string, Texture2D>();
    private static Dictionary<string, Texture2D> colored = new Dictionary<string, Texture2D>();
    private static Dictionary<string, Texture2D> separated = new Dictionary<string, Texture2D>();
    public static int ClearAllCache() {
        int nb = loaded.Count + inverted.Count + colored.Count + separated.Count;
        
        loaded.Clear();
        inverted.Clear();
        colored.Clear();
        separated.Clear();

        Resources.UnloadUnusedAssets();
        System.GC.Collect();

        return nb;
    }
}