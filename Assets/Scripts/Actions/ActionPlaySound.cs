using UnityEngine;

/**
 * This action will play a BGS or a BGM
 */
public class ActionPlaySound : MapObjectAction {
    public const string IMAGE_FOLDER = "Musics";
    public string soundPath;
    
    public bool bgm = false;

    public ActionPlaySound() {}
    public ActionPlaySound(string _soundPath, bool _bgm = false, bool _waitForEnd = false) {
        bgm = _bgm;
        waitForEnd = _waitForEnd;
    }

    public override void Execute() {
        if (bgm) {
            if (soundPath == "")
                World.Current.BGM = null;
            else
                World.Current.BGM = InterfaceUtility.GetAudio(soundPath);
            Terminate();
        } else {
            ActionPlaySoundDisplay display = new GameObject("action_Sound").AddComponent<ActionPlaySoundDisplay>();
            display.action = this;
            display.SendMessage("Start");
        }
    }

    public override string InLine() {
        return "Play " + (bgm ? "BGM : " : "BGS : ") + soundPath + ".";
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
    [HideInInspector]
    public ActionPlaySound action;

    private AudioSource source;

    public void Start() {
        source = new AudioSource();
        source.clip = InterfaceUtility.GetAudio(action.soundPath);
        source.Play();
    }

    public void Update() {
        transform.position = Camera.main.transform.position;

        if (!source.isPlaying)
            action.Terminate();
    }
}