using System.Collections.Generic;
using UnityEngine;

public abstract class Item {
    public bool onWildMonster = false;

    public abstract void Effect(List<Monster> caster, List<Monster> target);

}
