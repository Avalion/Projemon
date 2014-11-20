using UnityEngine;

/**
 * This action will play a BGS or a BGM
 */
[System.Serializable]
public class ActionPlaySound : MapObjectAction {
    public AudioClip sound;

    // Todo : store only a relative Path to songs
    public string soundPath;

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

    public override string InLine() {
        return "Play " + (bgm ? "BGM : " : "BGS : ") + sound.name+".";
    }

    public override string Serialize() {
        return GetType().ToString() + "|" + soundPath + "|" + bgm;
    }
    public override void Deserialize(string s) {
        string[] values = s.Split('|');
        if (values.Length != 3)
            throw new System.Exception("SerializationError : elements count doesn't match... " + s);

        soundPath = values[1];
        bgm = bool.Parse(values[2]);
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