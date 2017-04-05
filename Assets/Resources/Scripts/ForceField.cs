using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceField  {
    /*======================== Force Field =====================
        Vector3 F = sampler(position, forcefield.type)
        returns the force vector in force field of force type at the position in space 
    ============================================================*/
    public enum forcetype {ring, sphere, cubic, none};
    public forcetype t;

    float s;
    Vector3 c;
    Vector3 r;
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="t">Force field type : ring, sphere and cubic</param>
    /// <param name="c"> Center position </param>
    /// <param name="r"> Radius in 3-dimensions </param>
    public ForceField(forcetype type, float strength, Vector3 radius, Vector3 center= default(Vector3) ) {
        t = type;
        s = strength;
        r = radius;
        c = center;
        Debug.Log(center);       
    }

    public Vector3 sample_force(Vector3 position) {
        switch (t) {
            case forcetype.cubic:
                //6-direction only force
                return Vector3.zero;
            case forcetype.sphere:
                return sphere_force(position);
            case forcetype.ring:
                //special type
                return Vector3.zero;
            default:
                return Vector3.zero;
        }
    }

    public void set_strength(float _s) {
        s = _s;
    }

    public void set_center(Vector3 _c)
    {
        c = _c;
    }

    Vector3 sphere_force(Vector3 p) {
        //bilinear interpolation from center to surface
        Vector3 dir = p - c;
        if (r.x == 0 || r.y == 0 || r.z == 0) {
            Debug.Log("No space in force field, check radius.");
            return Vector3.zero;
        }
        if ((Mathf.Abs(dir.x) - r.x) > 1e-5 || (Mathf.Abs(dir.y) - r.y) > 1e-5 || (Mathf.Abs(dir.z) - r.z) > 1e-5) {
            //outside the force field
            return Vector3.zero;
        }
        float force_scalar = (dir.x / r.x) * (dir.x / r.x) + (dir.y / r.y) * (dir.y / r.y) + (dir.z / r.z) * (dir.z / r.z);
        return s * force_scalar * dir.normalized;    
    }

    

}
