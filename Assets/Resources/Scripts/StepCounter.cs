using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StepCounter : MonoBehaviour {

    public static StepCounter instance;
    int step;
    Text text;

    private void Awake() {
        if (instance == null)
        {
            instance = this;
        }
        else {
            Destroy(gameObject);
        }

        step = 0;
        text = GetComponent<Text>();
    }
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        text.text = "Steps: " + step;
	}

    public void inc() {
        step += 1;
    }
}
