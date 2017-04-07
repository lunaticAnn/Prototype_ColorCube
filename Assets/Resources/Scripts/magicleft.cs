using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class magicleft : magic {

    public GameObject pref;

    protected override void trigger()     {
        Color c = controller.instance.getTarget();
        GameObject obj = Instantiate(pref) as GameObject;
        obj.transform.SetParent(transform);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.Euler(180f, -90f, 0);
        ParticleSystem.MainModule settings = obj.GetComponent<ParticleSystem>().main;
        settings.startColor = new ParticleSystem.MinMaxGradient(c);
        StartCoroutine(base.waiting(obj));
    }
}
