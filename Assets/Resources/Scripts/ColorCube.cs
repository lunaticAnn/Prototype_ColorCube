using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorCube : MonoBehaviour {
    //vertice number in 3 dimensions
    public const float EDGE_LENGTH = 1f;
    const float DELTA_V = 0.02f;
    const float LINE_WIDTH = 0.05f;
    const float BREATH = 0.0003f;

    public int x, y, z;
    public Material mat;
    public Color color_zero;
    public Color color_one;

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

    /// <summary>
    /// instantiate a new colorcube
    /// </summary>
    /// <param name="x"> vertices on x -axis</param>
    /// <param name="y"> vertices on y-axis</param>
    /// <param name="z"> vertices on z-axis</param>
    /// <param name="mat"> material for line renderers </param>
    /// <param name="center"> center postion </param>
    /// <param name="quick_instance"> whether to perform the initializing animation </param>
    public void setColorCube(int _x, int _y, int _z, Material _mat, Color _color_zero, Color _color_one) {
        this.x = _x;
        this.y = _y;
        this.z = _z;
        this.mat = _mat;
        this.color_zero = _color_zero;
        this.color_one = _color_one;
    }

    public void Init_my_cube(bool quick_instance = false, int x_min = 0, int x_max = 0, int y_min = 0, int y_max = 0, int z_min = 0, int z_max = 0)
    {
        _par = GetComponent<ParticleSystem>();
        indexing();
        init_pos();
        var ma = _par.main;
        ma.maxParticles = vertice_cnt;

        //emit particles, and set it on the correct position
        _par.Emit(vertice_cnt);
        particle_init(x_min, x_max, y_min, y_max, z_min, z_max);
        IEnumerator c = init_colorcube(quick_instance);
        StartCoroutine(c);
    }

    void indexing() {
        int i, j, k;
        indexed_xyz = new int[x, y, z];
        for (i = 0; i < x; i++)
            for (j = 0; j < y; j++)
                for (k = 0; k < z; k++)
                    indexed_xyz[i, j, k] = (i * y + j) * z + k;
        Debug.Log("Indexing finished.");
    }
    

    void init_pos() {
        vertice_cnt = x * y * z;
        pos = new Vector3[vertice_cnt];
        color = new Color[vertice_cnt];
        Vector3 recenter_offset = -0.5f * EDGE_LENGTH * new Vector3(x-1, y-1, z-1);
        int i, j, k;
        for (i = 0; i < x; i++)
            for (j = 0; j < y; j++)
                for (k = 0; k < z; k++){
                    pos[indexed_xyz[i, j, k]] = recenter_offset + new Vector3(i * EDGE_LENGTH, j * EDGE_LENGTH, k * EDGE_LENGTH);

                    //index color, bilinear interpolation
                    float inter_r = (i * color_zero.r + (x - i) * color_one.r) / x;
                    float inter_g = (j * color_zero.g + (y - j) * color_one.g) / y;
                    float inter_b = (k * color_zero.b + (z - k) * color_one.b) / z;
                    color[indexed_xyz[i, j, k]] = new Color (inter_r, inter_g, inter_b);
                }

        }

    //index particles to their x,y,z
    void particle_init(int x_min = 0, int x_max = 0, int y_min = 0, int y_max = 0, int z_min = 0, int z_max = 0 ){
        int i,j,k;
        pars = new ParticleSystem.Particle[vertice_cnt];
        int current_alive = _par.GetParticles(pars);
        if (current_alive != vertice_cnt) {
            Debug.LogError(current_alive+"Particle number does not match vertices number, check initialization order.");
            return;
        }

        for (i = 0; i < x; i++)
            for (j = 0; j < y; j++)
                for (k = 0; k < z; k++)
                {
                    pars[indexed_xyz[i, j, k]].startLifetime = Mathf.Infinity;
                    pars[indexed_xyz[i, j, k]].velocity = Vector3.zero;
                    pars[indexed_xyz[i, j, k]].startColor = color[indexed_xyz[i, j, k]];
                    if (i < x_max && i >= x_min && j < y_max && j >= y_min && k < z_max && k >= z_min)
                        pars[indexed_xyz[i, j, k]].position = pos[indexed_xyz[i, j, k]];
                    else
                        pars[indexed_xyz[i, j, k]].position = Vector3.zero;

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

    
    IEnumerator init_colorcube(bool quick_instance = false) {
        int i,j,k;
        float max_v = -1f ;
        //without animation
        if (quick_instance)
        {
            int current_alive = _par.GetParticles(pars);
            if (current_alive != vertice_cnt)
            {
                Debug.LogError(current_alive + "Particle number does not match vertices number, check initialization order.");
            }
            // put all particles onto traget position
            for (i = 0; i < x; i++)
                for (j = 0; j < y; j++)
                    for (k = 0; k < z; k++){
                        pars[indexed_xyz[i, j, k]].position = pos[indexed_xyz[i, j, k]];
                    }
            //init cube lines
            _par.SetParticles(pars, vertice_cnt);
            init_cubelines(pars);
            yield return new WaitForEndOfFrame();
        }
        //with animation
        else
            do
            {
                max_v = -1f;
                int current_alive = _par.GetParticles(pars);
                if (current_alive != vertice_cnt)
                {
                    Debug.LogError(current_alive + "Particle number does not match vertices number, check initialization order.");
                }
                for (i = 0; i < x; i++)
                    for (j = 0; j < y; j++)
                        for (k = 0; k < z; k++)
                        {
                            float v = fly_to_dest(ref pars[indexed_xyz[i, j, k]], pos[indexed_xyz[i, j, k]]);
                            max_v = Mathf.Max(v, max_v);
                        }
                _par.SetParticles(pars, vertice_cnt);

                if (lines.Count == 0)
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
        lr.useWorldSpace = false;
        lr.material = mat;
        short_line.transform.localPosition = Vector3.zero;
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
        lr.startColor = p1.GetCurrentColor(_par);
        lr.endColor = p2.GetCurrentColor(_par);
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

    public void delete_me(bool quick = true) {
        if (quick)
        {
            Destroy(gameObject);
        }
        else
            StartCoroutine("vanishing");
    }

    IEnumerator vanishing() {
        StopCoroutine("slightly_movement");
        yield return new WaitForEndOfFrame();
        int i, j, k, cnt;
        //target positions
        for (cnt = 0; cnt < vertice_cnt; cnt++) {
            pos[cnt] += new Vector3(Random.value - 0.5f, Random.value - 0.5f, Random.value - 0.5f);
        }
        
        for (cnt = 0; cnt < 90; cnt++)
        {
            int current_alive = _par.GetParticles(pars);
            if (current_alive != vertice_cnt)
            {
                Debug.LogError(current_alive + "Particle number does not match vertices number, check initialization order.");
            }
            for (i = 0; i < x; i++)
                for (j = 0; j < y; j++)
                    for (k = 0; k < z; k++)
                    {
                        float v = fly_to_dest(ref pars[indexed_xyz[i, j, k]],pos[indexed_xyz[i,j,k]]);
                        Color _c = pars[indexed_xyz[i, j, k]].GetCurrentColor(_par);
                        pars[indexed_xyz[i, j, k]].startColor = _c - DELTA_V * ( _c - new Color(1, 1, 1, 0) );
                        pars[indexed_xyz[i, j, k]].startSize -= 0.002f; 
                    }
            _par.SetParticles(pars, vertice_cnt);
            update_pos_cubelines();
            yield return new WaitForEndOfFrame();
        }
        Destroy(gameObject);
    }
}

