using UnityEngine;

/**
 * This action will play a BGS or a BGM
 */
[System.Serializable]
public class ActionPlaySound : MapObjectAction {
    public AudioClip sound;

    public bool bgm = false;

    public ActionPlaySound() {}
    public ActionPlaySound(AudioClip _sound, bool _bgm = false, bool _waitForEnd = false) {
        sound = _sound;
        bgm = _bgm;
        waitForEnd = _waitForEnd;
    }

    public override void Execute() {
        if (bgm) {
            World.Current.currentBGM.Stop();
            World.Current.currentBGM.clip = sound;
            if (sound != null)
                World.Current.currentBGM.Play();
            Terminate();
        } else {
            ActionPlaySoundDisplay display = new GameObject("action_Sound").AddComponent<ActionPlaySoundDisplay>();
            display.action = this;
        }
    }
}

public class ActionPlaySoundDisplay : MonoBehaviour {
    public ActionPlaySound action;

    public AudioSource source;

    public void Start() {
        source = new AudioSource();
        source.clip = action.sound;
        source.Play();
    }

    public void Update() {
        transform.position = Camera.main.transform.position;

        if (!source.isPlaying)
            action.Terminate();
    }
}