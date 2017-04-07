using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class magic : MonoBehaviour {

    public GameObject reference;
    public float dis, deltatime, delaytime;

    float lasttime, sumtime;
    bool flag;

    public float a, b;
    private void Awake() {
        sumtime = 0;
        lasttime = 0;
        reference.transform.parent = transform;
        flag = false;
    }
    // Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        sumtime += Time.deltaTime;
        if (flag) {
            return;
        }

        float tmp = (reference.transform.position - transform.position).y;
        a = tmp;
        b = sumtime - lasttime;
        if (tmp < -dis)
        {
            lasttime = sumtime;
        }
        else if (tmp > dis && sumtime - lasttime <= delaytime) {
            trigger();
        }
	}

    protected virtual void trigger() {

    }

    protected IEnumerator waiting(GameObject obj) {
        flag = true;
        yield return new WaitForSeconds(delaytime);
        Destroy(obj);
        flag = false;
    }
}
