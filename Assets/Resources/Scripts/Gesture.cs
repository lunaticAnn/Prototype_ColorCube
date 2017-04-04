using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gesture : MonoBehaviour {

    public Hand left, right;
    public float speed_threshold;
    public int sum_speed;

    Vector3 pos, delta;
    bool status;
    bool waiting;
    int s;

    void Awake() {
        pos = transform.position;
        s = 0;
        status = false;
        waiting = false;
    }
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        update_status();
        if (waiting) {
            update_select();
        } else {
            update_gesture();
        }
	}

    bool get_trigger() {
        return OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger) && OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger);
    }

    void update_status() {
        bool new_status = get_trigger();

        if (status && !new_status) {

            s = 0;
        } else if (!status && new_status) {

        }
        status = new_status;
    }

    void update_gesture() {
        if (!status) {
            pos = transform.position;
            return;
        }

        if (left.velocity.magnitude < speed_threshold || right.velocity.magnitude < speed_threshold) {
            s = 0;
            return;
        }

        s += 1;
        if (s == 1) {
            delta = right.pos - left.pos;
        }
        if (s == sum_speed) {
            Vector3 tmp = right.pos - left.pos - delta;
            if (tmp.x < 0) {
                tmp.x *= -1;
            }
            if (tmp.y < 0) {
                tmp.y *= -1;
            }
            if (tmp.z < 0) {
                tmp.z *= -1;
            }

            if (tmp.x >= tmp.z && tmp.y >= tmp.z) {
                // cut z
            } else if (tmp.x >= tmp.y && tmp.z >= tmp.y) {
                // cut y
            } else {
                // cut x
            }

            s = 0;
            waiting = true;
        }
        pos = transform.position;
    }

    void update_select() {
        if (OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger)) {
            waiting = false;
            // right.gameObject.transform.position
        }
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
        {
            waiting = false;
            // left.gameObject.transform.position
        }
    }
}
