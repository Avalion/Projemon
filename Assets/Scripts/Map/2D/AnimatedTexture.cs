using UnityEngine;
using System.Collections.Generic;

/** 
 * @class : AnimatedTexture
 * @author : J.Fremaux
 * 
 * @type : SYSTEM
 * @description : switch between a bunch of material Texture to animate it
**/
[System.Serializable]
public class AnimatedTexture : MonoBehaviour {
    public float delay = 0.2f;
    public List<Texture2D> imgs = new List<Texture2D>();

    private int selectedImg = 0;

    private float Counter;

    public void Start() {
        if (renderer == null) {
            DestroyImmediate(this);
            return;
        }
    }

    public void Update() {
        if (imgs != null && imgs.Count != 0) {
            if (delay != 0) {
                Counter += Time.deltaTime;
                while (Counter > delay) {
                    selectedImg = (selectedImg + 1) % (imgs.Count);
                    Counter -= delay;
                }
            } else {
                selectedImg = (selectedImg + 1) % (imgs.Count);
            }
            renderer.material.mainTexture = imgs[selectedImg];
        }
    }
}
