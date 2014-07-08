using UnityEngine;
using System.Collections.Generic;

/** 
 * @class : AnimatedGif
 * @author : J.Fremaux
 * 
 * @type : SYSTEM
 * @description : switch between a bunch of GUI Texture to animate it
**/
[System.Serializable]
public class AnimatedGif {
    public float delay = 0.2f;
    public List<Texture2D> imgs = new List<Texture2D>();
    
    private int selectedImg = 0;

    private float Counter;
    private float time;

    [HideInInspector]
    public Texture2D image { get {
        if (imgs != null && imgs.Count != 0) {
            if (delay != 0) {
                Counter += Time.realtimeSinceStartup - time;
                time = Time.realtimeSinceStartup;
                while (Counter > delay) {
                    selectedImg = (selectedImg + 1) % (imgs.Count);
                    Counter -= delay;
                }
            } else {
                selectedImg = (selectedImg + 1) % (imgs.Count);
            }
            return imgs[selectedImg];
        }
        return null;
    } }
}
