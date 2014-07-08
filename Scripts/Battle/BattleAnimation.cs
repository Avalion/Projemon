using UnityEngine;
using System.Collections.Generic;
using System.IO;

/**
 * This class designs animation in battles as list of images linked to frames
 */
public class BattleAnimation {
    public const string IMAGE_FOLDER = "BattleAnimations";
    
    public int ID;

    public string name;
    public int nbFrames = 20;

    public class ImageInstance {
        public string imageFolder;
        public Texture2D image;
        public Rect position;
        public int frame;
    }
    public List<ImageInstance> instances = new List<ImageInstance>();

    public BattleAnimation(int ID) {
        this.ID = ID;
        Import();
    }

    public void Display() {
        new GameObject("BattleAnim").AddComponent<DisplayBattleAnimation>().battleAnimation = this;
    }

    public override string ToString() {
        return name;
    }

    // Serialization
    public void Export() {
        string filePath = Config.GetConfigPath("BattleAnims") + ID + ".txt";
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
            sw.WriteLine(i.frame + "#" + Config.RectToString(i.position) + "#" + i.imageFolder + "#" + i.image.name);
        }
        sw.WriteLine();

        sw.Close();
        sw.Dispose();
    }
    public void Import() {
        string filePath = Config.GetConfigPath("BattleAnims") + ID + ".txt";
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
            instances.Add(im);
        }


        sr.Close();
        sr.Dispose();
    }
}
