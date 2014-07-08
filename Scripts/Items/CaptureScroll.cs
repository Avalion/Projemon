using UnityEngine;
using System.Collections.Generic;

public class CaptureScroll : Item {

    public enum Quality { Normal, Superior, Hyperior, Master };
    public Quality quality;

    public int price = 100;

    public float CalculCaptureRate(List<Monster> caster, List<Monster> target) {
        float captureRate = 0;
        float stateFactor = 1;
        float qualityFactor = 1;



        if (quality == Quality.Master)
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

            if ((quality == Quality.Normal && target[0].lvl <= 15) || (quality == Quality.Superior && target[0].lvl <= 30) || (quality == Quality.Hyperior && target[0].lvl <= 45))
                qualityFactor = 1;
            else if ((quality == Quality.Normal && target[0].lvl <= 25) || (quality == Quality.Superior && target[0].lvl <= 40) || (quality == Quality.Hyperior && target[0].lvl <= 55))
                qualityFactor = 0.5f;
            else
                qualityFactor = 0.25f;

            captureRate = Mathf.Min((1 - (target[0].life / (float)target[0].maxLife) + caster[0].stat_luck * 0.01f - target[0].stat_luck * 0.1f + 0.2f * caster[0].lvl / (float)target[0].lvl + Random.Range(0, 0.1f)) * target[0].capture_rate * stateFactor * qualityFactor, 95);
        }
        return captureRate;
    }

    public override void Effect(List<Monster> caster, List<Monster> target) {
        float captureRate = CalculCaptureRate(caster, target);
        float random = Random.Range(0, 1f);
        Debug.Log(captureRate +" # "+ random);
        if (random <= captureRate)
            Battle.Message = target[0].monsterName + " is captured !";
        else
            Battle.Message = "Capture has failed. Noob !";
    }
}
