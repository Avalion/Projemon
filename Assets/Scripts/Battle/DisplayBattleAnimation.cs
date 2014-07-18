using UnityEngine;

/**
 * This class displays a AnimationBattle
 */
public class DisplayBattleAnimation : MonoBehaviour {
    public BattleAnimation battleAnimation = null;

    public Rect displayZone;

    public int m_currentframe = 0;

    public float time = 0;

    private GUIStyle style = new GUIStyle();

    public void Start() {
        style.normal.background = InterfaceUtility.HexaToTexture("#000000");
    }

    public void OnGUI() {
        GUI.depth = -1;
        foreach (BattleAnimation.ImageInstance i in battleAnimation.instances.FindAll(I => I.frame == m_currentframe)) {
            //style.normal.background = i.image;
            i.Display(i.GetPixelRect(displayZone));
        }
    }

    public void Update() {
        time += Time.deltaTime;

        if (time > BattleAnimation.TIME_BETWEEN_FRAMES) {
            time = 0;
            m_currentframe++;
            if (m_currentframe == battleAnimation.nbFrames)
                DestroyImmediate(gameObject);
        }
    }
}
