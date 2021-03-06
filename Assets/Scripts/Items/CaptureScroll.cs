﻿using UnityEngine;
using System.Collections.Generic;

/**
 * This item capture a wild monster
 */
public class CaptureItem : Item {
    public bool sureToCatch;
    
    public int middleLvlLimit;
    public int maxLvlLimit;

    public float CalculCaptureRate(List<Monster> caster, List<Monster> target) {
        float captureRate = 0;
        float stateFactor = 1;
        float qualityFactor = 1;



        if (sureToCatch)
            captureRate = 1;
        else {
            switch (target[0].state) {
                case Monster.State.Insane:
                case Monster.State.Poisoned:
                    stateFactor = 1.5f;
                    break;
                case Monster.State.Paralized:
                case Monster.State.Sleepy:
                    stateFactor = 2;
                    break;
                default:
                    stateFactor = 1;
                    break;
            }

            if (target[0].lvl <= middleLvlLimit)
                qualityFactor = 1;
            else if (target[0].lvl <= maxLvlLimit)
                qualityFactor = 0.5f;
            else
                qualityFactor = 0.25f;

            captureRate = Mathf.Min((1 - (target[0].life / (float)target[0].maxLife) + caster[0].stat_luck * 0.01f - target[0].stat_luck * 0.1f + 0.2f * caster[0].lvl / (float)target[0].lvl + Random.Range(0, 0.1f)) * target[0].monsterPattern.capture_rate * stateFactor * qualityFactor, 95);
        }
        return captureRate;
    }

    protected override void Effect(Battler caster, List<Monster> target) {
        System.Diagnostics.Debug.Assert(target.Count > 0);

        float captureRate = CalculCaptureRate(caster.monsters, target);
        
        if (MathUtility.TestProbability(captureRate)) {
            // SUCCESS
            Battle.Current.Message = target[0].monsterName + " est capturé !";

            MonsterCollection.AddToCollection(target[0]);

            Battle.Current.Win();
        } else {
            // FAIL
            Battle.Current.Message = "Mince, ca n'a pas réussi !";
        }
    }
}
