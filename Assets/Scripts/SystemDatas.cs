﻿using UnityEngine;
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

    private static string ATTACKS_FILE = Config.GetConfigPath("Datas") + "Attacks.txt";
    public static List<Attack> GetAttacks() {
        List<Attack> list = new List<Attack>();
        if (!File.Exists(ATTACKS_FILE))
            return list;

        StreamReader sr = new StreamReader(ATTACKS_FILE);

        string line = sr.ReadLine();
        while (line != "" && line != null) {
            Attack a = new Attack();

            string[] values = line.Split('#');
            a.name = values[0];
            a.type = (Monster.Type)int.Parse(values[1]);
            a.power = int.Parse(values[2]);
            a.precision = int.Parse(values[3]);
            a.battleAnimationID = int.Parse(values[4]);

            list.Add(a);

            line = sr.ReadLine();
        }

        sr.Dispose();
        return list;
    }
    public static void SetAttacks(List<Attack> _elements) {
        if (_elements.Count == 0) {
            if (File.Exists(ATTACKS_FILE))
                File.Delete(ATTACKS_FILE);
            return;
        }

        StreamWriter sw = new StreamWriter(ATTACKS_FILE);
        foreach (Attack a in _elements)
            sw.WriteLine(a.name + "#" + (int)a.type + "#" + a.power + "#" + a.precision + "#" + a.battleAnimationID);
        sw.Dispose();
    }


}
