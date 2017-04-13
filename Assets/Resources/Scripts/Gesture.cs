using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gesture : MonoBehaviour {

    public Hand left, right;
    public GameObject wave;
    public float ratio;
    public float grow_time;

    bool status, waiting, split_delay;

    int dir;
    float dis;

    void Awake() {
        status = false;
        waiting = false;
        split_delay = false;
    }
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (waiting) {
            update_select();
        } else if (!split_delay) {
            update_status();
        }
	}

    bool get_trigger() {
        return OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger) && OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger);
    }

    void update_status() {
        bool new_status = get_trigger();

        if (status && !new_status) {
            split();
        }
        if (new_status) {
            set_wave();
        }
        status = new_status;
    }

    void set_wave() {
        set_info();
        if (dir == 0) {
            wave.transform.rotation = Quaternion.Euler(0, 90, 0);
        } else if (dir == 1) {
            wave.transform.rotation = Quaternion.Euler(90, 0, 0);
        } else {
            wave.transform.rotation = Quaternion.Euler(0, 0, 0);
        }

        wave.transform.localScale = new Vector3(dis * ratio, dis * ratio, 1);
    }

    void set_info() {
        Vector3 tmp = right.pos - left.pos;
        if (tmp.x < 0) tmp.x *= -1;
        if (tmp.y < 0) tmp.y *= -1;
        if (tmp.z < 0) tmp.z *= -1;

        if (tmp.x > tmp.z && tmp.y > tmp.z) dir = 2;
        if (tmp.x > tmp.y && tmp.z > tmp.y) dir = 1;
        if (tmp.y > tmp.x && tmp.z > tmp.x) dir = 0;

        if (tmp.x > tmp.y && tmp.x > tmp.z) dis = tmp.x;
        if (tmp.y > tmp.x && tmp.y > tmp.z) dis = tmp.y;
        if (tmp.z > tmp.y && tmp.z > tmp.x) dis = tmp.z;
    }

    void split() {
        // call split
        set_info();
        CubeController.instance.split_cube(dir);
        StartCoroutine(coGrow());
        StepCounter.instance.inc();
        waiting = true;
    }

    void update_select() {
        if (OVRInput.GetDown(OVRInput.Button.One)) {
            waiting = false;
            // right.gameObject.transform.position
        }
        if (OVRInput.GetDown(OVRInput.Button.Three))
        {
            waiting = false;
            // left.gameObject.transform.position
        }
    }

    IEnumerator coGrow() {
        split_delay = true;
        float now = wave.transform.localScale.x;
        float target = 400 - now;

        for (float res = grow_time; res > 0; res -= 0.025f) {
            float r = (1 - res / grow_time) * target + now;
            wave.transform.localScale = new Vector3(r, r, 1);
            yield return new WaitForSeconds(0.025f);
        }

        wave.transform.localScale = new Vector3(0, 0, 1);
        split_delay = false;
    }
}
