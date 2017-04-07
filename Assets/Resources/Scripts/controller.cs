﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class controller : MonoBehaviour {

    public static controller instance;

    public float delta;
    Color target, everage;

    private void Awake() {
        if (instance == null) instance = this;
        else DestroyImmediate(gameObject);

        levelUp();
    }
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        updateColor();
        checkLevel();
    }

    void levelUp() {
        // set level
        target = new Color(Random.Range(0, 255) / 255.0f, Random.Range(0, 255) / 255.0f, Random.Range(0, 255) / 255.0f);
    }

    void updateColor() {
        float r = (CubeController.instance.corner000.r + CubeController.instance.corner111.r) * 0.5f;
        float g = (CubeController.instance.corner000.g + CubeController.instance.corner111.g) * 0.5f;
        float b = (CubeController.instance.corner000.b + CubeController.instance.corner111.b) * 0.5f;
        everage = new Color(r, g, b);
    }

    void checkLevel() {
        float r = Mathf.Abs(everage.r - target.r);
        float g = Mathf.Abs(everage.g - target.g);
        float b = Mathf.Abs(everage.b - target.b);
        if (r + g + b < delta) {
            // celebrate
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public Color getTarget() {
        return target;
    }

    public Color getEverage() {
        return everage;
    }
}
