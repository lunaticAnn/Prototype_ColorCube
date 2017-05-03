using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {
    /*==============================
            3D sound management
     ===============================*/
     //time for cleaning up useless 
    const float Refresh_Time = 1f;

    public static SoundManager instance = null;

    public AudioClip Flame;
    public AudioClip RingAppear;
    public AudioClip Select;
    public AudioClip success;

    void Awake() {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Start() {
        Invoke("Reap", Refresh_Time);
    }

    public AudioSource PlaySfx(AudioClip clip,  Vector3 Position = default(Vector3), bool loop = false) {
        GameObject sound_obj = new GameObject("AudioSource");
        sound_obj.transform.SetParent(transform);
        sound_obj.transform.position = Position;
        AudioSource ad = sound_obj.AddComponent<AudioSource>();
        ad.loop = loop;
        ad.clip = clip;
        ad.Play();
        return ad;
	}

    public bool stop_play(AudioSource src)
    {
        if (src == null) return false;
        if (src.isPlaying)
        {
            src.Stop();
            return true;
        }
        return false;
    }

    void Reap() {
        AudioSource[] ads = GetComponentsInChildren<AudioSource>();
        int cnt = ads.GetLength(0);
        int i;
        for (i = cnt; i > 0; i--) {
            if (!ads[i].isPlaying) {
                Debug.Log("reap useless audio sources.");
                Destroy(ads[i].gameObject);
            }
        }
        Invoke("Reap", Refresh_Time);
    }

}
