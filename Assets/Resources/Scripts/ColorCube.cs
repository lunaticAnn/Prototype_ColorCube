using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorCube : MonoBehaviour {
    //vertice number in 3 dimensions
    const float EDGE_LENGTH = 1f;
    const float DELTA_V = 0.1f;
    const float LINE_WIDTH = 0.05f;
    const float BREATH = 0.0003f;

    public int x, y, z;
    public Material mat;

    int vertice_cnt;
    int line_num;
    ParticleSystem _par;
    //index xyz to linear index
    int[,,] indexed_xyz;
    ParticleSystem.Particle[] pars;

    //positions for particles in color cube.
    Vector3[] pos;
    Color[] color;
    struct line_se
    {
        public int start_index, end_index;
    }

    List<line_se> lines= new List<line_se>();

    void Awake()
	{
        _par = GetComponent<ParticleSystem>();
        indexing();
        init_pos();
        var ma = _par.main;
        ma.maxParticles = vertice_cnt;
	}

    void Start() {
        //emit particles, and set it on the correct position
        _par.Emit(vertice_cnt);
        particle_init();
        StartCoroutine("init_colorcube");
    }

    void indexing() {
        int i, j, k;
        indexed_xyz = new int[x, y, z];
        for (i = 0; i < x; i++)
            for (j = 0; j < y; j++)
                for (k = 0; k < z; k++)
                    indexed_xyz[i, j, k] = (i * x + j) * y + k;
        Debug.Log("Indexing finished.");
    }
    

    void init_pos() {
        vertice_cnt = x * y * z;
        pos = new Vector3[vertice_cnt];
        color = new Color[vertice_cnt];
        Vector3 recenter_offset = 0.5f * EDGE_LENGTH * new Vector3(-x, -y, -z);
        int i, j, k;
        for (i = 0; i < x; i++)
            for (j = 0; j < y; j++)
                for (k = 0; k < z; k++){
                    pos[indexed_xyz[i, j, k]] = recenter_offset + new Vector3(i * EDGE_LENGTH, j * EDGE_LENGTH, k * EDGE_LENGTH);
                    color[indexed_xyz[i, j, k]] = new Color ((1f * i)/x, (1f * j) / y, (1f * k) / z);
                }

        }

        //index particles to their x,y,z
        void particle_init(){
        int i;
        pars = new ParticleSystem.Particle[vertice_cnt];
        int current_alive = _par.GetParticles(pars);
        if (current_alive != vertice_cnt) {
            Debug.LogError(current_alive+"Particle number does not match vertices number, check initialization order.");
            return;
        }
      
        for (i = 0; i < vertice_cnt; i++){
                pars[i].startLifetime = Mathf.Infinity;
                pars[i].position = Vector3.zero;
                pars[i].velocity = Vector3.zero;
                pars[i].startColor = color[i];
            }
        _par.SetParticles(pars, vertice_cnt);
        Debug.Log("Initialization succeeded.");
        
    }

    //particle movement functions
    float fly_to_dest(ref ParticleSystem.Particle p, Vector3 dest) {
        Vector3 delta = DELTA_V * (dest - p.position);
        p.position += delta;
        return Vector3.Magnitude(delta);
    }


    IEnumerator init_colorcube() {
        int i,j,k;
        float max_v = -1f ;
        do {
            max_v = -1f;
            int current_alive = _par.GetParticles(pars);
            if (current_alive != vertice_cnt){
                Debug.LogError(current_alive + "Particle number does not match vertices number, check initialization order.");
            }
            for (i = 0; i < x; i++)
                for (j = 0; j < y; j++)
                    for (k = 0; k < z; k++) {
                        float v = fly_to_dest(ref pars[indexed_xyz[i, j, k]], pos[indexed_xyz[i, j, k]]);
                        max_v = Mathf.Max(v, max_v);
                    }
            _par.SetParticles(pars, vertice_cnt);

            if(lines.Count == 0)
                init_cubelines(pars);
            else 
                update_pos_cubelines();
            yield return new WaitForEndOfFrame();
            
        } while (max_v > 1e-3);
        Debug.Log("Initialize finished.");

        //create lines here
        StartCoroutine("slightly_movement");
    }

    IEnumerator slightly_movement() {
        int i, j, k;
        while (true)
        {
            int current_alive = _par.GetParticles(pars);
            if (current_alive != vertice_cnt)
            {
                Debug.LogError(current_alive + "Particle number does not match vertices number, check initialization order.");
            }
            for (i = 0; i < x; i++)
                for (j = 0; j < y; j++)
                    for (k = 0; k < z; k++)
                        pars[indexed_xyz[i, j, k]].position += Mathf.Sin(Time.time +2 * Mathf.PI * i / x + Mathf.PI * j / y)* BREATH *Vector3.up;
                 
            _par.SetParticles(pars, vertice_cnt);   
             update_pos_cubelines();
            yield return new WaitForEndOfFrame();
        }

    }


    LineRenderer create_line() {
        GameObject short_line = new GameObject("Line");
        short_line.transform.SetParent(transform);
        LineRenderer lr = short_line.AddComponent<LineRenderer>();
        lr.material = mat;
        return lr;
    }

    void init_line(ParticleSystem.Particle p1, ParticleSystem.Particle p2) {
        LineRenderer lr = create_line();
        lr.startWidth = LINE_WIDTH;
        lr.endWidth = LINE_WIDTH;
        lr.startColor = p1.GetCurrentColor(_par);
        lr.endColor = p2.GetCurrentColor(_par);
        lr.numPositions = 2;
        lr.SetPositions(new Vector3[2] {p1.position, p2.position});
    }

    void update_pos_line(ParticleSystem.Particle p1, ParticleSystem.Particle p2, LineRenderer lr) {
        lr.SetPositions(new Vector3[2] { p1.position, p2.position });
    }
    //-----------------------------Highly Unsafe Area---------------------
    void init_cubelines(ParticleSystem.Particle[] particles) {
        int i, j, k;
        line_num = (x - 1) * y * z + (y - 1) * x * z + (z - 1) * x * y;
        
        for (i = 0; i < x; i++)
            for (j = 0; j < y; j++)
                for (k = 0; k < z; k++) {
                    if (i + 1 < x) {
                        //create line
                        init_line(particles[indexed_xyz[i, j, k]], particles[indexed_xyz[i + 1, j, k]]);
                        // update lines[child index] to line_se(index of start, index of end);
                        line_se l;
                        l.start_index = indexed_xyz[i, j, k];
                        l.end_index = indexed_xyz[i + 1, j, k];
                        lines.Add(l);
                    }

                    if (j + 1 < y)
                    {
                        //create line
                        init_line(particles[indexed_xyz[i, j, k]], particles[indexed_xyz[i, j + 1, k]]);
                        // update lines[child index] to line_se(index of start, index of end);
                        line_se l;
                        l.start_index = indexed_xyz[i, j, k];
                        l.end_index = indexed_xyz[i, j + 1, k];
                        lines.Add(l);
                    }

                    if (k + 1 < z)
                    {
                        //create line
                        init_line(particles[indexed_xyz[i, j, k]], particles[indexed_xyz[i, j, k + 1]]);
                        // update lines[child index] to line_se(index of start, index of end);
                        line_se l;
                        l.start_index = indexed_xyz[i, j, k];
                        l.end_index = indexed_xyz[i, j, k + 1];
                        lines.Add(l);
                    }

                }

         Debug.Log("Init line finished...");
         Debug.Log("Checking line_se nums...." + lines.Count + ", should be " + line_num);
    }

    void update_pos_cubelines() {
        int i;
        for (i = 0; i < line_num; i++) {
            LineRenderer lr = transform.GetChild(i).GetComponent<LineRenderer>();
            update_pos_line(pars[lines[i].start_index], pars[lines[i].end_index],lr);
        }
    }

}

