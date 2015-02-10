using UnityEngine;
using System.Collections.Generic;

public class Interface : MonoBehaviour {
	public class ToDisplay {
        public Rect zone;

        public delegate void Display();
    }

    public Dictionary<string, ToDisplay> toDisplay = new Dictionary<string, ToDisplay>();


    public void OnGUI() {
        foreach (var pair in toDisplay) {
            GUILayout.BeginArea(pair.Value.zone);

            GUILayout.EndArea();
        }
    }
}
