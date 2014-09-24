using UnityEngine;
using System.Collections.Generic;

/**
 * This class defines battlers
 */
public class Battler : MapObject {
    public List<Monster> monsters = new List<Monster>();
    public int activeMonster;

    public Dictionary<string,int> inventory = new Dictionary<string,int>();

    public int goldCount;
}