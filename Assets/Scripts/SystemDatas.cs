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

    public static List<string> GetBattlersPaths() {
        List<string> battlersTextures = new List<string>();
        foreach (string s in GetDirectoryFiles(Application.dataPath.Replace("Assets", "") + Config.GetResourcePath(Monster.IMAGE_FOLDER)))
            battlersTextures.Add(Path.GetFileName(s));
        return battlersTextures;
    }
    public static List<string> GetBattleAnimationsFolders() {
        List<string> folders = new List<string>(Directory.GetDirectories(Application.dataPath.Replace("Assets", "") + Config.GetResourcePath(BattleAnimation.IMAGE_FOLDER)));
        for (int i = 0; i < folders.Count; i++)
            folders[i] = Path.GetFileName(folders[i]);
        return folders;
    }
    public static List<string> GetMapsPatterns() {
        List<string> mapsPatterns = new List<string>();
        foreach (string s in GetDirectoryFiles(Application.dataPath.Replace("Assets", "") + Config.GetResourcePath(Map.IMAGE_FOLDER)))
            mapsPatterns.Add(Path.GetFileName(s));
        return mapsPatterns;
    }
    

    public static List<BattleAnimation> GetBattleAnimations() {
        List<BattleAnimation> animations = new List<BattleAnimation>();
        foreach (string s in GetDirectoryFiles(Config.GetConfigPath("BattleAnims")))
            animations.Add(new BattleAnimation(int.Parse(Path.GetFileNameWithoutExtension(s))));
        return animations;
    }
    public static void SetBattleAnimations(List<BattleAnimation> _elements) {
        foreach (string file in Directory.GetFiles(Config.GetConfigPath("BattleAnims"))) {
            File.Delete(file);
        }
        foreach (BattleAnimation ba in _elements)
            ba.Export();
    }

    public static List<Map> GetMaps() {
        List<Map> maps = new List<Map>();
        foreach (string s in GetDirectoryFiles(Config.GetConfigPath("Maps")))
            maps.Add(new Map(int.Parse(Path.GetFileNameWithoutExtension(s))));
        return maps;
    }
    public static void SetMaps(List<Map> _elements) {
        foreach (string file in Directory.GetFiles(Config.GetConfigPath("Maps"))) {
            File.Delete(file);
        }
        foreach (Map m in _elements)
            m.Export();
    }

    /* Write all DataBase infos;
     */ 
    public static void Save() {
        // Write Player position, infos and current map


        // Write monster collection


        // Write Event list and variables values
    }
}
