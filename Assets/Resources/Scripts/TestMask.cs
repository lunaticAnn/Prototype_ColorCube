using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMask : MonoBehaviour {
    bool locker =false;
    public Transform center_eye;
	// Use this for initialization
	void Start () {

    }

    public void start_wave() {
        transform.position =center_eye.position;      
        if (!locker){
            Debug.Log("start");
            StartCoroutine("Wave");
            locker = true;
        }    
    }

    // Update is called once per frame
    IEnumerator Wave () {
        int i = 0;
        while (i < 120){
            transform.localScale = new Vector3(10f * i, 10f * i, 500f);
            i++;
            yield return new WaitForEndOfFrame();
        }
        transform.localScale = new Vector3(0, 0, 500f);
        locker = false;
    }
}
