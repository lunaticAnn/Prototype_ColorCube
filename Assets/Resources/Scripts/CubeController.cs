﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeController : MonoBehaviour {
    public static CubeController instance = null;
    public GameObject default_system;
    public GameObject wave;
    public Material mat;
    public int x, y, z;
    public enum cube_state {free, wait_choose};
    public cube_state st;
    public Transform righthand;
    public GameObject success_particles;

    //color pallate when x,y,z = 0;
    public Color corner000;
    //color pallate when x,y,z = 1;
    public Color corner111;
    public ForceField force_field;

    Color[] last_step = new Color[2];

    bool test = false;
    bool callsuccess = false;
    GameObject current_cube;
    GameObject cube_temp0;
    GameObject cube_temp1;
    Color color_temp0;
    Color color_temp1;

    static int split_axis = 0;
    static int[] init_bound = {0, 0, 0, 0, 0, 0};

    void Awake() {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
        
        //initialize force field
        force_field = new ForceField(ForceField.forcetype.sphere, 0f, Vector3.one);       
    }

	// Use this for initialization
	void Start () {      
        current_cube = create_color_cube(x, y, z, Vector3.zero, corner000, corner111, false);
        last_step[0] = corner000;
        last_step[1] = corner111;
    }

    // only when it is a whole cube that you can flip one step back
    public bool OneStepBack() {
        if (last_step[0] == corner000 && last_step[1] == corner111) return false;
        current_cube.GetComponent<ColorCube>().delete_me(false);
        current_cube = create_color_cube(x, y, z, Vector3.zero, last_step[0], last_step[1]);
        corner000 = last_step[0];
        corner111 = last_step[1] ;
        return true;
    }
    // Update is called once per frame
    void Update() {
        switch (st) {
            case cube_state.free:
                if (Input.GetKeyDown(KeyCode.A))
                    split_cube(0);
                else if (Input.GetKeyDown(KeyCode.S))
                    split_cube(1);
                else if (Input.GetKeyDown(KeyCode.D))
                    split_cube(2);
                else if (Input.GetKeyDown(KeyCode.B))
                    OneStepBack();
                return;
            case cube_state.wait_choose:
                if (OVRInput.GetDown(OVRInput.Button.One))
                {
                    float dis0 = Vector3.Distance(cube_temp0.transform.position, righthand.position);
                    float dis1 = Vector3.Distance(cube_temp1.transform.position, righthand.position);
                    SoundManager.instance.PlaySfx(SoundManager.instance.Select, righthand.position);
                    if (dis0 - dis1 < 0f)
                        choose(0);
                    else
                        choose(1);
                }
                else if (Input.GetKeyDown(KeyCode.Z)) {
                    choose(0);
                }
                else if (Input.GetKeyDown(KeyCode.X))
                {
                    choose(1);
                }

                return;

            default: return;
        }
        
	}

    GameObject create_color_cube(int x, int y, int z, Vector3 center_position, Color c1, Color c2, bool quick = true, bool bound = false) {
        GameObject new_cube = Instantiate(default_system);
        new_cube.transform.position = center_position;
        ColorCube c = new_cube.AddComponent<ColorCube>();
        c.setColorCube(x, y, z, mat, c1, c2);
        if(bound)
            c.Init_my_cube(quick, init_bound[0], init_bound[1], init_bound[2], init_bound[3], init_bound[4], init_bound[5]);
        else
            c.Init_my_cube(quick);
        return new_cube;
    }

    public bool split_cube(int axis = 0) {
        int a = axis % 3;
        float split_color;
        Vector3 pos0, pos1;
        st = cube_state.wait_choose;
        last_step[0] = corner000;
        last_step[1] = corner111;

        switch (a) {
            case 0:
                //x_axis
                split_axis = 0;
                StartCoroutine("changing_force");
                split_color = (corner000.r + corner111.r) * 0.5f;
                Destroy(current_cube);  
                pos0 =new  Vector3(0.25f * ColorCube.EDGE_LENGTH * x , 0, 0);
                pos1 = new Vector3(-0.25f * ColorCube.EDGE_LENGTH *x , 0, 0);
                color_temp0 = new Color(split_color, corner111.g, corner111.b);
                color_temp1 = new Color(split_color, corner000.g, corner000.b);
                cube_temp0 = create_color_cube(x / 2, y, z, pos0, corner000, color_temp0);
                cube_temp1 = create_color_cube(x / 2, y, z, pos1, color_temp1, corner111);            
                return true;
            case 1:
                //y_axis
                split_axis = 1;
                StartCoroutine("changing_force");
                split_color = (corner000.g + corner111.g) * 0.5f;
                Destroy(current_cube);
                //new halves positions
                pos0 = new Vector3(0, 0.25f * ColorCube.EDGE_LENGTH * y, 0);
                pos1 = new Vector3(0, -0.25f * ColorCube.EDGE_LENGTH * y, 0);
                color_temp0 = new Color(corner111.r, split_color, corner111.b);
                color_temp1 = new Color(corner000.r, split_color, corner000.b);
                cube_temp0 = create_color_cube(x, y / 2, z, pos0, corner000,color_temp0 );
                cube_temp1 = create_color_cube(x, y / 2, z, pos1, color_temp1, corner111);
                
                return true;
            case 2:
                split_axis = 2;
                //z_axis
                StartCoroutine("changing_force");
                split_color = (corner000.b + corner111.b) * 0.5f;
                Destroy(current_cube);
              
                pos0 = new Vector3(0, 0, 0.25f * ColorCube.EDGE_LENGTH * z);
                pos1 = new Vector3(0, 0, -0.25f * ColorCube.EDGE_LENGTH * z);
                color_temp0 = new Color(corner111.r, corner111.g, split_color);
                color_temp1 = new Color(corner000.r, corner000.g, split_color);
                cube_temp0 = create_color_cube(x, y, z / 2, pos0, corner000,color_temp0);
                cube_temp1 = create_color_cube(x, y, z / 2, pos1, color_temp1, corner111);
                
                return true;
            default:
                Debug.LogError("Invalid input.");
                return false;
        }       
    }

    IEnumerator changing_force() {      
        int i = 0;
        force_field.set_radius(new Vector3(1f, 1f, 1f));           
        while (i < 6) {
            force_field.set_strength(20f);
            i++;
            yield return new WaitForEndOfFrame();
        }
        force_field.set_strength(0f);
        yield return new WaitForEndOfFrame();
    }

    public void success() {
        if (!callsuccess)
        {
            current_cube.SendMessage("onSuccess");
            callsuccess=true;
        }
    }


    void choose(int i) {
        int ans = i % 2;
        if (ans == 0) {
            //set new corner color
            corner111 = color_temp0;
            //set init boundary
            set_init_bound(split_axis, 0);
            current_cube = create_color_cube(x, y, z, Vector3.zero, corner000, corner111, false, true);
            cube_temp0.GetComponent<ColorCube>().delete_me();
            cube_temp1.GetComponent<ColorCube>().delete_me(false);
            //set state
            st = cube_state.free;
            return;
        }
        if (ans == 1)
        {
            //set new corner color
            corner000 = color_temp1;
            set_init_bound(split_axis, 1);
            current_cube = create_color_cube(x, y, z, Vector3.zero, corner000, corner111,false, true);
            //set state
            cube_temp0.GetComponent<ColorCube>().delete_me(false);
            cube_temp1.GetComponent<ColorCube>().delete_me();
            st = cube_state.free;
            return;
        }
    }

    void set_init_bound(int axis_id, int choice) {
        if (axis_id == 0) {
            if (choice == 0)
            {
                init_bound[0] = x / 2;
                init_bound[1] = x;
                init_bound[2] = 0;
                init_bound[3] = y;
                init_bound[4] = 0;
                init_bound[5] = z;
                return;
            }
            if (choice == 1)
            {
                init_bound[0] = 0;
                init_bound[1] = x / 2;
                init_bound[2] = 0;
                init_bound[3] = y;
                init_bound[4] = 0;
                init_bound[5] = z;
                return;
            }
        }
        if (axis_id == 1)
        {
            if (choice == 0)
            {
                init_bound[0] = 0;
                init_bound[1] = x;
                init_bound[2] = y / 2;
                init_bound[3] = y;
                init_bound[4] = 0;
                init_bound[5] = z;
                return;
            }
            if (choice == 1)
            {
                init_bound[0] = 0;
                init_bound[1] = x;
                init_bound[2] = 0;
                init_bound[3] = y / 2;
                init_bound[4] = 0;
                init_bound[5] = x;
                return;
            }
        }
        if (axis_id == 2)
        {
            if (choice == 0)
            {
                init_bound[0] = 0;
                init_bound[1] = x;
                init_bound[2] = 0;
                init_bound[3] = y;
                init_bound[4] = z / 2;
                init_bound[5] = z;
                return;
            }
            if (choice == 1)
            {
                init_bound[0] = 0;
                init_bound[1] = x;
                init_bound[2] = 0;
                init_bound[3] = y;
                init_bound[4] = 0;
                init_bound[5] = z / 2;
                return;
            }
        }
        Debug.LogError("Set bound error.");
    }

}
