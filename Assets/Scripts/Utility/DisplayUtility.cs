using UnityEngine;

public class DisplayUtility {
    public static bool IsBattle() {
        return Battle.Current != null;
    }
    
    
    
#region LevelUp
    private class LevelUpWindow : IDisplayable {
        private const int WINDOW_WIDTH = 200; 
        private const int WINDOW_HEIGHT = 200; 

        public Monster m;
        public int lifeUp, staminaUp, mightUp, resUp, luckUp, speedUp;

        private static Rect Rect {
            get {
                if (IsBattle())
                    return new Rect();
                else
                    return new Rect(Screen.width - WINDOW_WIDTH, Screen.height - ActionMessage.MESSAGE_HEIGHT - WINDOW_HEIGHT + 30, WINDOW_WIDTH, WINDOW_HEIGHT);
            }
        }

        public override void Display() {
            GUILayout.BeginArea(Rect);
            InterfaceUtility.BeginBox(GUILayout.Width(Rect.width), GUILayout.Height(Rect.height));

            GUILayout.BeginHorizontal();
            GUILayout.Label("PV : " + m.maxLife, GUILayout.Width(WINDOW_WIDTH / 3));
            GUILayout.Label("+ " + lifeUp, GUILayout.Width(WINDOW_WIDTH / 4));
            GUILayout.Label("→ " + (m.maxLife + lifeUp), GUILayout.Width(WINDOW_WIDTH / 3));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("END: " + m.maxLife, GUILayout.Width(WINDOW_WIDTH / 3));
            GUILayout.Label("+ " + lifeUp, GUILayout.Width(WINDOW_WIDTH / 4));
            GUILayout.Label("→ " + (m.maxLife + lifeUp), GUILayout.Width(WINDOW_WIDTH / 3));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("FOR: " + m.stat_might, GUILayout.Width(WINDOW_WIDTH / 3));
            GUILayout.Label("+ " + mightUp, GUILayout.Width(WINDOW_WIDTH / 4));
            GUILayout.Label("→ " + (m.stat_might + mightUp), GUILayout.Width(WINDOW_WIDTH / 3));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("RES: " + m.stat_resistance, GUILayout.Width(WINDOW_WIDTH / 3));
            GUILayout.Label("+ " + resUp, GUILayout.Width(WINDOW_WIDTH / 4));
            GUILayout.Label("→ " + (m.stat_resistance + resUp), GUILayout.Width(WINDOW_WIDTH / 3));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("VIT: " + m.stat_speed, GUILayout.Width(WINDOW_WIDTH / 3));
            GUILayout.Label("+ " + speedUp, GUILayout.Width(WINDOW_WIDTH / 4));
            GUILayout.Label("→ " + (m.stat_speed + speedUp), GUILayout.Width(WINDOW_WIDTH / 3));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("CHA: " + m.stat_luck, GUILayout.Width(WINDOW_WIDTH / 3));
            GUILayout.Label("+ " + luckUp, GUILayout.Width(WINDOW_WIDTH / 4));
            GUILayout.Label("→ " + (m.stat_luck + luckUp), GUILayout.Width(WINDOW_WIDTH / 3));
            GUILayout.EndHorizontal();

            InterfaceUtility.EndBox();
            GUILayout.EndArea();
        }
    }
    public static void DisplayLevelUpWindow(Monster m, int lifeUp, int staminaUp, int mightUp, int resUp, int luckUp, int speedUp) {
        LevelUpWindow window = new GameObject("Interface-LevelUp").AddComponent<LevelUpWindow>();
        window.m = m;
        window.lifeUp = lifeUp;
        window.staminaUp = staminaUp;
        window.mightUp = mightUp;
        window.resUp = resUp;
        window.luckUp = luckUp;
        window.speedUp = speedUp;
    }
#endregion
}
