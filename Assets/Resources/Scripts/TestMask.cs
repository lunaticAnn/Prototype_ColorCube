using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMask : MonoBehaviour {
    bool locker =false;
	// Use this for initialization
	void Start () {

    }

    void Update() {
        if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger) || Input.GetKeyDown(KeyCode.A)) {
            if (!locker){
                Debug.Log("start");
                StartCoroutine("Wave");
                locker = true;
            }
        }
    }

    // Update is called once per frame
    IEnumerator Wave () {
        int i = 0;
        while (i < 120)
        {
            transform.localScale = new Vector3(10f * i, 10f * i, 200f);
            i++;
            yield return new WaitForEndOfFrame();
        }
        transform.localScale = new Vector3(0, 0, 200f);
        locker = false;
    }
}
