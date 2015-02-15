using UnityEngine;
using UnityEditor;
using System.IO;

public class TextureCutter : EditorWindow {
    public Texture2D sprite;
    
    public string outputFolder;

    public int resolutionX = 192;
    public int resolutionY = 192;

    [MenuItem("Tools/TextureCutter")]
    public static void Init() {
        TextureCutter window = EditorWindow.GetWindow<TextureCutter>();
        window.minSize = new Vector2(300, 100);
        window.maxSize = new Vector2(300, 101);
        window.Show();

        InterfaceUtility.ClearAllCache();
    }

    public void OnGUI() {
        sprite = EditorGUILayout.ObjectField("Texture", sprite, typeof(Texture2D), false) as Texture2D;

        resolutionX = EditorGUILayout.IntField("X", resolutionX);
        resolutionY = EditorGUILayout.IntField("X", resolutionY);

        outputFolder = EditorGUILayout.TextField("Out Folder", outputFolder);

        if (GUILayout.Button("Exec") && sprite != null && resolutionX > 0 && resolutionY > 0) {
            Cut();
        }
    }

    public void Cut() {
        Vector2 currentPattern = new Vector2(sprite.width / resolutionX, sprite.height / resolutionY);

        string directory = Application.dataPath.Replace("Assets", "") + Config.GetResourcePath(BattleAnimation.IMAGE_FOLDER) + outputFolder + "/";
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        for (int y = 0; y < currentPattern.y; y++) {
            for (int x = 0; x < currentPattern.x; x++) {
                InterfaceUtility.ClearAllCache();
                
                Texture2D t = InterfaceUtility.SeparateTexture(sprite, x, y, resolutionX, resolutionY);
                t.alphaIsTransparency = true;

                File.WriteAllBytes(directory + sprite.name + "_" + y + "_" + x + ".png", t.EncodeToPNG());
                
                InterfaceUtility.ClearAllCache();
            }
        }

        AssetDatabase.Refresh();
    }
}
