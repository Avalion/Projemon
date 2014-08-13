using System.Collections.Generic;
using UnityEngine;

public abstract class Item {
    public enum PossibleTarget { All, Ally, Enemy, WildMonster };
    public PossibleTarget possibleTarget;

    public int cost = 0;

    /**
     * On suppose que tous les monstres de target appartiennent au même Battler
     */
    public virtual void Use(Battler caster, List<Monster> target) {
        switch(possibleTarget){
            case PossibleTarget.All :
                break;
            case PossibleTarget.Ally :
                foreach (Monster m in target) {
                    if (m.battler == caster)
                        return;
                }
                break;
            case PossibleTarget.Enemy :
                foreach (Monster m in target) {
                    if (m.battler != caster)
                        return;
                }
                break;
            case PossibleTarget.WildMonster :
                foreach (Monster m in target) {
                    if (m.battler == null)
                        return;
                }
                break;
        }
        
        Effect(caster, target);
    }

    protected abstract void Effect(Battler caster, List<Monster> target);

}
