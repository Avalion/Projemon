using UnityEngine;

/**
 * This class displays a AnimationBattle
 */
public class DisplayBattleAnimation : MonoBehaviour {
    public BattleAnimation battleAnimation = null;

    public int m_currentframe = 0;

    public void Update() {
        foreach (BattleAnimation.ImageInstance i in battleAnimation.instances.FindAll(I => I.frame == m_currentframe)) {
            GUI.DrawTexture(i.position, i.image);
        }

        m_currentframe++;
        if (m_currentframe == battleAnimation.nbFrames)
            DestroyImmediate(gameObject);
    }
}
