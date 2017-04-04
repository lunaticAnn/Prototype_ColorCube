using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour {

    public int num_frames;
    public Vector3 pos, velocity;

    Vector3[] vecs;
    float[] times;
    int p;
    float sum_time;

    bool enough;

    private void Awake() {
        p = 0;
        sum_time = 0;
        vecs = new Vector3[num_frames];
        times = new float[num_frames];
        enough = false;
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        // pos
        pos = transform.position;

        // new info
        if (p == num_frames - 1) {
            enough = true;
        }
        vecs[p] = transform.position;
        times[p] = Time.deltaTime;

        // count velocity
        int tmp_p = p;
        p = (p + 1) % num_frames;
        if (!enough) {
            return;
        }
        velocity = (vecs[p] - vecs[tmp_p]) / (times[p] - times[tmp_p]);
	}
}
