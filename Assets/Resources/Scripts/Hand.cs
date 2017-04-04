using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour {

    public Vector3 pos, velocity;
    Vector3 tmp = Vector3.zero;

    private void Awake() {
        pos = transform.position; 
    }
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        // pos
        tmp = transform.position;
        velocity = (pos - tmp) / Time.deltaTime;
        pos = tmp;
	}
}
