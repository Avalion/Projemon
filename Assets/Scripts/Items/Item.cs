using System.Collections.Generic;
using UnityEngine;

public abstract class Item {
    public enum Usability { None, Once, All }
    public Usability usability;

    public enum PossibleTarget { All, Ally, Enemy, WildMonster };
    public PossibleTarget possibleTarget;

    public int price = 0;

    public string name;

    /**
     * Vérifications des règles d'utilisation des objets.
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

        if (usability == Usability.Once) {
           // TODO : remove from inventory
        }
    }

    protected abstract void Effect(Battler caster, List<Monster> target);

}
