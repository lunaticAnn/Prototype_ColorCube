using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class magicright : magic {

    public GameObject pref;

    protected override void trigger()
    {
        Color c = controller.instance.getEverage();
        GameObject obj = Instantiate(pref) as GameObject;
        ParticleSystem.MainModule settings = obj.GetComponent<ParticleSystem>().main;
        settings.startColor = new ParticleSystem.MinMaxGradient(c);
        StartCoroutine(base.waiting(obj));
    }
}
