using UnityEngine;
using System.Collections.Generic;
using System.IO;

/**
 * This class designs animation in battles as list of images linked to frames
 * 
 * WARNING ! When you modify this class, you should modify associated functions in SystemDatas !
 */
public class BattleAnimation {
    // Constantes
    public const string IMAGE_FOLDER = "BattleAnims";
    public const float TIME_BETWEEN_FRAMES = 0.05f;

    public int ID;

    public string name;
    public int nbFrames = 20;

    public class ImageInstance {
        public string imageFolder;
        public Texture2D image;
        public Rect position;
        public int frame;

        public float alpha;

        // Utils
        public Rect GetRelativeRect(Rect parent, Vector2 position) {
            Vector2 parentRealSize = new Vector2(parent.width - image.width, parent.height - image.height);

            Rect pos = new Rect(position.x - parent.x - image.width / 2f, 
                                position.y - parent.y - image.height / 2f, 
                                image.width, 
                                image.height);

            return new Rect(pos.x / parentRealSize.x, pos.y / parentRealSize.y, pos.width / parent.width, pos.height / parent.height);
        }
        public Rect GetPixelRect(Rect parent) {
            Vector2 realSize = new Vector2(parent.width * position.width, parent.height * position.height);
            Vector2 parentRealSize = new Vector2(parent.width - realSize.x, parent.height - realSize.y);

            return new Rect(parent.x + parentRealSize.x * position.x,
                            parent.y + parentRealSize.y * position.y,
                            realSize.x,
                            realSize.y);
        }

        public void Display(Rect rect) {
            Display(rect, alpha);
        }
        public void Display(Rect rect, float _alpha) {
            Color c = GUI.color;
            GUI.color = new Color(c.r, c.g, c.b, _alpha);
            GUI.Label(rect, image, InterfaceUtility.LabelStyle);
            GUI.color = c;
        }
    }
    public List<ImageInstance> instances = new List<ImageInstance>();

    public BattleAnimation(int ID) {
        this.ID = ID;
        Import();
    }

    public void Display(Rect effectZone) {
        DisplayBattleAnimation anim = new GameObject("BattleAnim").AddComponent<DisplayBattleAnimation>();
        anim.battleAnimation = this;
        anim.displayZone = effectZone;
    }

    public override string ToString() {
        return name;
    }

    // Serialization
    public void Export() {
        string filePath = Config.GetConfigPath(IMAGE_FOLDER) + ID + ".txt";
        if (instances.Count == 0) {
            if (File.Exists(filePath))
                File.Delete(filePath);
            return;
        }

        StreamWriter sw = new StreamWriter(filePath);

        sw.WriteLine("------------");
        sw.WriteLine("- Anim " + InterfaceUtility.IntString(ID, 3) + " -");
        sw.WriteLine("------------");
        sw.WriteLine(name);
        sw.WriteLine(nbFrames);
        sw.WriteLine();

        List<ImageInstance> toExport = instances.FindAll(II => II.frame < nbFrames);
        sw.WriteLine(toExport.Count);
        foreach (ImageInstance i in toExport) {
            sw.WriteLine(i.frame + "#" + Config.RectToString(i.position) + "#" + i.imageFolder + "#" + i.image.name + "#" + i.alpha);
        }
        sw.WriteLine();

        sw.Close();
        sw.Dispose();
    }
    public void Import() {
        string filePath = Config.GetConfigPath(IMAGE_FOLDER) + ID + ".txt";
        if (!File.Exists(filePath))
            return;

        StreamReader sr = new StreamReader(filePath);
        
        sr.ReadLine();
        sr.ReadLine();
        sr.ReadLine();
        name = sr.ReadLine();
        nbFrames = int.Parse(sr.ReadLine());
        sr.ReadLine();

        instances = new List<ImageInstance>();
        int count = int.Parse(sr.ReadLine());
        for (int i = 0; i < count; i++) {
            string line = sr.ReadLine();
            string[] values = line.Split('#');
            ImageInstance im = new ImageInstance();
            im.frame = int.Parse(values[0]);
            im.position = Config.StringToRect(values[1]);
            im.imageFolder = values[2];
            
            string localpath = Config.GetResourcePath(IMAGE_FOLDER) + im.imageFolder + "/" + values[3] + ".png";
            im.image = Resources.LoadAssetAtPath(localpath, typeof(Texture2D)) as Texture2D;

            im.alpha = float.Parse(values[4]);
            instances.Add(im);
        }


        sr.Close();
        sr.Dispose();
    }
}
