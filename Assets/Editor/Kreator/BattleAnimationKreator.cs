using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

/**
 * This class provides gestion of BattleAnimations
 */
public class BattleAnimationKreator : EditorWindow {
    public static List<BattleAnimation> elements = new List<BattleAnimation>();
    public int selectedElement = 0;
    BattleAnimation current {
        get { return elements[selectedElement]; }
    }

    // Images
    private static List<string> imagesFolders = new List<string>();
    private static string selectedFolder;

    private static List<Texture2D> images = new List<Texture2D>();
    private static Texture2D selectedImage;

    // Design
    private Vector2 scrollPosList = Vector2.zero;
    private Vector2 scrollpos = new Vector2();

    private static GUIStyle imageStyle = new GUIStyle();
    private static GUIStyle selectedImageStyle = new GUIStyle();

    private static GUIStyle canvasImageStyle;

    private float alpha = 1;

    // Canvas
    private int currentFrame = 0;

    public static BattleAnimation.ImageInstance selectedInstance = null;
    
    private Vector2 dragStartPosition = new Vector2();

    // Preview
    private bool displayPrecedent = true;
    
    public bool isPlaying;
    public float time;


    // Launch
    [MenuItem("Creation/Battle Animations &B")]
    public static void Init() {
        BattleAnimationKreator window = EditorWindow.GetWindow<BattleAnimationKreator>();
        window.minSize = new Vector2(1000, 500);
        window.maxSize = new Vector2(1000, 501);
        window.Show();

        InterfaceUtility.ClearAllCache();

        elements = SystemDatas.GetBattleAnimations();
        
        imagesFolders = SystemDatas.GetBattleAnimationsFolders();
        if (imagesFolders.Count > 0)
            selectedFolder = imagesFolders[0];

        InitStyles();

        if (selectedFolder != "")
            UpdateImages();
    }
    
    public static void InitStyles() {
        imageStyle = new GUIStyle();
        imageStyle.padding = new RectOffset(5, 5, 3, 3);
        imageStyle.normal.textColor = Color.white;
        
        selectedImageStyle = new GUIStyle(imageStyle);
        selectedImageStyle.normal.background = InterfaceUtility.HexaToTexture("#AAAAFF77");
    }
    
    // Display
    public void OnGUI() {
        if (imagesFolders.Count == 0) {
            GUILayout.Label("No directories in " + BattleAnimation.IMAGE_FOLDER);
            if (GUILayout.Button("Refresh")) {
                imagesFolders = SystemDatas.GetBattleAnimationsFolders();
                if (imagesFolders.Count > 0)
                    selectedFolder = imagesFolders[0];
            }
            return;
        }
        
        GUILayout.BeginHorizontal();
        // List
        GUILayout.BeginVertical(GUILayout.Width(150));
        int sel = EditorUtility.DisplayList<BattleAnimation>(selectedElement, elements, ref scrollPosList);
        if (sel != selectedElement) {
            selectedElement = sel;
            currentFrame = 0;
        }
        GUILayout.FlexibleSpace();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Add")) {
            elements.Add(new BattleAnimation(elements.Count));
            selectedElement = elements.Count - 1;
            current.instances.Clear();
            current.name = "";
            current.nbFrames = 20;
        }
        if (GUILayout.Button("Delete")) {
            if (selectedElement == elements.Count - 1 && selectedElement > 0) {
                elements.RemoveAt(selectedElement);
                selectedElement = elements.Count - 1;
            } else {
                current.instances.Clear();
                current.name = "";
                current.nbFrames = 20;
            }
        }
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();

        if (selectedElement < 0 || selectedElement >= elements.Count)
            return;

        // Canvas
        GUILayout.BeginVertical();
        DisplayCanvas();

        // FrameDisplay
        GUILayout.BeginHorizontal();
        int val = EditorGUILayout.IntSlider(currentFrame + 1, 1, current.nbFrames) - 1;
        if (val != currentFrame) {
            SetCurrentFrame(val);
        }
        GUILayout.Label("/", GUILayout.Width(15));
        current.nbFrames = Mathf.Max(2, EditorGUILayout.IntField(current.nbFrames, GUILayout.Width(40)));
        GUILayout.Label("(" + Mathf.RoundToInt(1 / BattleAnimation.TIME_BETWEEN_FRAMES) + " fps)", GUILayout.Width(52));
        if (GUILayout.Button(isPlaying? "Stop" : "Play", GUILayout.Width(60)) || (Event.current.type == EventType.keyDown && Event.current.keyCode == KeyCode.Space)) {
            if (currentFrame == current.nbFrames - 1)
                SetCurrentFrame(0);
            isPlaying = !isPlaying;
            time = Time.realtimeSinceStartup;
            selectedInstance = null;
        }
        
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();

        // Images
        GUILayout.BeginVertical();
        current.name = EditorGUILayout.TextField("Name", current.name);

        int value = EditorGUILayout.Popup(imagesFolders.IndexOf(selectedFolder), imagesFolders.ToArray());
        if (imagesFolders[value] != selectedFolder) {
            selectedFolder = imagesFolders[value];
            UpdateImages();
        }
        
        GUILayout.Space(5);

        GUILayout.BeginHorizontal();
        GUILayout.Label("Alpha", GUILayout.Width(40));
        float tmp = GUILayout.HorizontalSlider(alpha, 0, 1);
        tmp = EditorGUILayout.FloatField(tmp, GUILayout.Width(40));
        if (tmp != alpha) {
            alpha = tmp;
            if (selectedInstance != null)
                selectedInstance.alpha = alpha;
        }
        
        GUILayout.EndHorizontal();

        GUILayout.FlexibleSpace();
        scrollpos = GUILayout.BeginScrollView(scrollpos);
        foreach (Texture2D t in images) {
            if (selectedImage != t) {
                if (GUILayout.Button(new GUIContent(" " + t.name, t), imageStyle, GUILayout.Height(40)))
                    selectedImage = t;
            } else {
                GUILayout.Label(new GUIContent(" " + t.name, t), selectedImageStyle, GUILayout.Height(40));
            }
        }
        if (GUILayout.Button("", InterfaceUtility.LabelStyle, GUILayout.MaxHeight(Screen.height)))
            selectedImage = null;
        GUILayout.EndScrollView();
        
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("OK")) {
            SystemDatas.SetBattleAnimations(elements);
            Close();
        }
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();
        GUILayout.EndHorizontal();

        if (isPlaying) {
            if (currentFrame != current.nbFrames - 1 && Time.realtimeSinceStartup - time > BattleAnimation.TIME_BETWEEN_FRAMES) {
                currentFrame++;
                time = Time.realtimeSinceStartup;
            }
            if (currentFrame >= current.nbFrames - 1)
                isPlaying = false;
            Repaint();
        }
    }

    private void SetCurrentFrame(int _val) {
        currentFrame = _val;
        selectedInstance = null;
        Repaint();
    }

    public void DisplayCanvas() {
        Rect canvasRect = GUILayoutUtility.GetRect(Screen.width * 0.6f, Screen.height - 60);
        // Box
        canvasRect = MathUtility.ExtendRect(canvasRect, -4);
        EditorGUI.DrawRect(canvasRect, Color.black);
        canvasRect = MathUtility.ExtendRect(canvasRect, -2);
        EditorGUI.DrawRect(canvasRect, Color.grey);
        // Grid
        EditorGUI.DrawRect(new Rect(canvasRect.x + canvasRect.width / 2f, canvasRect.y, 2, canvasRect.height), Color.white);
        EditorGUI.DrawRect(new Rect(canvasRect.x, canvasRect.y + canvasRect.height / 2f, canvasRect.width, 2), Color.white);

        /** Display
         */
        GUI.BeginGroup(canvasRect);
        foreach (BattleAnimation.ImageInstance i in current.instances.FindAll(I => I.frame == currentFrame)) {
            if (!isPlaying) {
                if (i == selectedInstance)
                    EditorUtility.DrawEmptyRect(MathUtility.ExtendRect(i.GetPixelRect(new Rect(0, 0, canvasRect.width, canvasRect.height)), 2), InterfaceUtility.HexaToColor("#0000FFFF"), 2);
                else
                    EditorUtility.DrawEmptyRect(MathUtility.ExtendRect(i.GetPixelRect(new Rect(0, 0, canvasRect.width, canvasRect.height)), 1), InterfaceUtility.HexaToColor("#4444FFCC"), 1);
            }
            i.Display(i.GetPixelRect(new Rect(0, 0, canvasRect.width, canvasRect.height)));
        }
        if (!isPlaying && displayPrecedent) {
            foreach (BattleAnimation.ImageInstance i in current.instances.FindAll(I => I.frame == currentFrame - 1)) {
                EditorUtility.DrawEmptyRect(MathUtility.ExtendRect(i.GetPixelRect(new Rect(0, 0, canvasRect.width, canvasRect.height)), 1), InterfaceUtility.HexaToColor("#CCCCFF44"), 1);
                
                i.Display(i.GetPixelRect(new Rect(0, 0, canvasRect.width, canvasRect.height)), i.alpha * 0.3f);
            }
        }
        GUI.EndGroup();

        /** Events
         */
        if (!isPlaying) 
            CanvasEvents(canvasRect);
    }
    private void CanvasEvents(Rect _rect) {
        if (!_rect.Contains(Event.current.mousePosition))
            return;
        /** Mouse Events
         */
        if (Event.current.type == EventType.MouseDown && Event.current.button == 0) {
            // SELECT
            foreach (BattleAnimation.ImageInstance i in current.instances) {
                if (i.frame == currentFrame && MathUtility.AddRect(i.GetPixelRect(new Rect(0,0,_rect.width, _rect.height)), _rect.x, _rect.y, 0, 0).Contains(Event.current.mousePosition)) {
                    selectedInstance = i;
                    alpha = selectedInstance.alpha;
                    dragStartPosition = Event.current.mousePosition;
                    Repaint();
                    return;
                }
            }
            
            if (selectedImage == null)
                return;
            
            // CREATE
            selectedInstance = new BattleAnimation.ImageInstance() { image = selectedImage, frame = currentFrame, imageFolder = selectedFolder, alpha = this.alpha };
            selectedInstance.position = selectedInstance.GetRelativeRect(_rect, Event.current.mousePosition);

            current.instances.Add(selectedInstance);
            dragStartPosition = Event.current.mousePosition;
            Repaint();
        }
        if (selectedInstance != null && Event.current.type == EventType.MouseDrag && Event.current.button == 0) {
            // Replace
            int index = current.instances.IndexOf(selectedInstance);
            Vector2 move = Event.current.mousePosition - dragStartPosition;
            selectedInstance.position = MathUtility.AddRect(selectedInstance.position, move.x / (_rect.width - selectedInstance.image.width), move.y / (_rect.height - selectedInstance.image.height), 0, 0);
            current.instances[index] = selectedInstance;
            dragStartPosition = Event.current.mousePosition;

            Repaint();
        }
        
        /** Keyboard Events
         */
        if (Event.current.type == EventType.KeyDown) {
            if (Event.current.keyCode == KeyCode.H) {
                displayPrecedent = !displayPrecedent;
            }
            if (Event.current.keyCode == KeyCode.PageUp) {
                SetCurrentFrame(currentFrame + 1);
            }
            if (Event.current.keyCode == KeyCode.PageDown) {
                SetCurrentFrame(currentFrame - 1);
            }
        }
        
        if (selectedInstance != null) {
            if (Event.current.type == EventType.KeyDown) {
                if (Event.current.keyCode == KeyCode.Delete) {
                    current.instances.Remove(selectedInstance);
                    if (current.instances.Count > 0) {
                        selectedInstance = current.instances[current.instances.Count - 1];
                        alpha = selectedInstance.alpha;
                    } else
                        selectedInstance = null;
                } else if (Event.current.keyCode == KeyCode.Escape) {
                    selectedInstance = null;
                }
            } else if (Event.current.isKey) {
                int index = current.instances.IndexOf(selectedInstance);
                Vector2 move = Vector2.zero;
                if (Event.current.keyCode == KeyCode.UpArrow)
                    move.y -= 0.01f;
                if (Event.current.keyCode == KeyCode.DownArrow)
                    move.y += 0.01f;
                if (Event.current.keyCode == KeyCode.LeftArrow)
                    move.x -= 0.01f;
                if (Event.current.keyCode == KeyCode.RightArrow)
                    move.x += 0.01f;
                if (move != Vector2.zero) {
                    selectedInstance.position = MathUtility.AddRect(selectedInstance.position, move.x, move.y, 0, 0);
                    current.instances[index] = selectedInstance;
                }
            }
            
            Repaint();
        }
    }

    // Utils
    private static void UpdateImages() {
        string availableExtension = ".png";
        
        selectedImage = null;
        images.Clear();
        foreach (string file in Directory.GetFiles(Application.dataPath.Replace("Assets", "") + Config.GetResourcePath(BattleAnimation.IMAGE_FOLDER) + selectedFolder)) {
            if (!availableExtension.Contains(Path.GetExtension(file)))
                continue;
            try {
                string localpath = Config.GetResourcePath(BattleAnimation.IMAGE_FOLDER) + selectedFolder + "/" + Path.GetFileName(file);
                Texture2D t = Resources.LoadAssetAtPath(localpath, typeof(Texture2D)) as Texture2D;
                if (t != null) {
                    images.Add(t);
                    if (selectedImage == null)
                        selectedImage = t;
                }
                    
            } catch { }
        }
        
    }
}
