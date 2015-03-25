using UnityEditor;
using UnityEngine;
using System.IO;

class TextureGenerator : EditorWindow {
    private Color m_color = Color.black;
    
    [MenuItem("Tools/Texture Generator", priority = 215)]
    public static void Init() {
        TextureGenerator window = GetWindow<TextureGenerator>();
        window.minSize = new Vector2(200, 50);
        window.maxSize = new Vector2(201, 51);
        window.title = "Texture Generator";
        window.Show();
    }

    public void OnGUI() {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Color");
        m_color = EditorGUILayout.ColorField(m_color);
        GUILayout.EndHorizontal();
        
        GUILayout.BeginHorizontal();
        GUILayout.Label(Utility.ConvertColorToHexa(m_color));
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Generate"))
            Generate();
        GUILayout.EndHorizontal();
    }


    /**
     * Randomly place the GameObject selection on selected axes.  
     */
    private void Generate() {
        Texture2D t = Utility.ConvertColorToTexture(m_color, 2, 2);

        
        string assetPath = "Images/" + Utility.ConvertColorToHexa(m_color).Substring(1) + ".png";

        if (!Directory.Exists(Application.dataPath + "/Images"))
            Directory.CreateDirectory(Application.dataPath + "/Images");

        File.WriteAllBytes(Application.dataPath + "/" + assetPath, t.EncodeToPNG());

        AssetDatabase.Refresh();
        
        t = EditorUtility.LoadAssetAtPath<Texture2D>("Assets/" + assetPath);
        EditorGUIUtility.PingObject(t);
    }
}