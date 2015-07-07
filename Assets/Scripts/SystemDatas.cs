using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

/**
 *  This class provides the functions to get All Datas (Attack, Battlers icons, BattleAnimations, ...).
 */
public class SystemDatas {
    public static string[] GetDirectoryFiles(string _directory) {
        List<string> files = new List<string>();
        foreach (string s in Directory.GetFiles(_directory)) {
            if (Path.GetExtension(s) == ".meta")
                continue;
            if (Path.GetFileName(s) == "Thumbs.db")
                continue;
            files.Add(s);
        }
        return files.ToArray();
    }

    public static List<string> GetMapObjectsPaths() {
        List<string> textures = new List<string>();
        foreach (string s in GetDirectoryFiles(Application.dataPath.Replace("Assets", "") + Config.GetResourcePath(MapObject.IMAGE_FOLDER)))
            textures.Add(Path.GetFileName(s));
        return textures;
    }
    public static List<string> GetMonstersPaths() {
        List<string> textures = new List<string>();
        foreach (string s in GetDirectoryFiles(Application.dataPath.Replace("Assets", "") + Config.GetResourcePath(Monster.IMAGE_FOLDER)))
            textures.Add(Path.GetFileName(s));
        return textures;
    }
    public static List<string> GetBattleAnimationsFolders() {
        List<string> folders = new List<string>(Directory.GetDirectories(Application.dataPath.Replace("Assets", "") + Config.GetResourcePath(BattleAnimation.IMAGE_FOLDER)));
        for (int i = 0; i < folders.Count; i++)
            folders[i] = Path.GetFileName(folders[i]);
        return folders;
    }
    public static List<string> GetMapsPatterns() {
        List<string> patterns = new List<string>();
        foreach (string s in GetDirectoryFiles(Application.dataPath.Replace("Assets", "") + Config.GetResourcePath(Map.IMAGE_FOLDER)))
            patterns.Add(Path.GetFileName(s));
        return patterns;
    }
    public static List<string> GetFaces() {
        List<string> faces = new List<string>();
        foreach (string s in GetDirectoryFiles(Application.dataPath.Replace("Assets", "") + Config.GetResourcePath(ActionMessage.IMAGE_FOLDER)))
            faces.Add(Path.GetFileName(s));
        return faces;
    }
    public static List<string> GetMusics() {
        List<string> patterns = new List<string>();
        foreach (string s in GetDirectoryFiles(Application.dataPath.Replace("Assets", "") + Config.GetResourcePath(ActionPlaySound.IMAGE_FOLDER)))
            patterns.Add(Path.GetFileName(s));
        return patterns;
    }
    

    public static List<BattleAnimation> GetBattleAnimations() {
        List<BattleAnimation> animations = new List<BattleAnimation>();
        foreach (string s in GetDirectoryFiles(Config.GetConfigPath(BattleAnimation.IMAGE_FOLDER)))
            animations.Add(new BattleAnimation(int.Parse(Path.GetFileNameWithoutExtension(s))));
        return animations;
    }
    public static void SetBattleAnimations(List<BattleAnimation> _elements) {
        foreach (string file in Directory.GetFiles(Config.GetConfigPath(BattleAnimation.IMAGE_FOLDER))) {
            File.Delete(file);
        }
        foreach (BattleAnimation ba in _elements)
            ba.Export();
    }

    public static List<Map> GetMaps() {
        List<Map> maps = new List<Map>();
        foreach (string s in GetDirectoryFiles(Config.GetConfigPath(Map.IMAGE_FOLDER)))
            maps.Add(new Map(int.Parse(Path.GetFileNameWithoutExtension(s))));
        return maps;
    }
    public static void SetMaps(List<Map> _elements) {
        // maps;
        foreach (string file in Directory.GetFiles(Config.GetConfigPath(Map.IMAGE_FOLDER))) {
            File.Delete(file);
        }
        foreach (Map m in _elements) {
            m.Export();

            // mapObjects
            foreach (MapObject mo in m.mapObjects) {
                DataBase.Replace<DBMapObject>(DBMapObject.ConvertFrom(m, mo));

                // Renew MOActions
                DataBase.Delete<DBMapObjectAction>("mapObjectID = " + mo.mapObjectId);

                foreach (MapObjectAction action in mo.actions)
                    DataBase.Insert<DBMapObjectAction>(DBMapObjectAction.ConvertFrom(mo, action));
            }
        }
    }
}
