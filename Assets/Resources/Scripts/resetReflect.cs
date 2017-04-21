using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class resetReflect : MonoBehaviour {

    public GameObject target;
	// Use this for initialization
	void Start () {
        StartCoroutine(coWait());
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.R)) {
            resetY();
        }
	}

    void resetY() {
        Vector3 vec = transform.position;
        vec.y = target.transform.position.y;
        transform.position = vec;
    }
    IEnumerator coWait() {
        yield return new WaitForSeconds(0.5f);
        resetY();
    }
}
