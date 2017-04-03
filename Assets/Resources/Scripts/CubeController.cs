using System.Collections;
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

    //color pallate when x,y,z = 0;
    public Color corner000;
    //color pallate when x,y,z = 1;
    public Color corner111;

    GameObject current_cube;
    GameObject cube_temp0;
    GameObject cube_temp1;
    Color color_temp0;
    Color color_temp1;

    void Awake() {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
        
    }

	// Use this for initialization
	void Start () {
        current_cube = create_color_cube(x, y, z, Vector3.zero, corner000, corner111,false);
	}

    // Update is called once per frame
    void Update() {
        switch (st) {
            case cube_state.free:
                if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger)||Input.GetKeyDown(KeyCode.A))
                    split_cube(0);
                else if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger) ||Input.GetKeyDown(KeyCode.S))
                    split_cube(1);
                else if (OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger)|| Input.GetKeyDown(KeyCode.D))
                    split_cube(2);
                return;
            case cube_state.wait_choose:
                if (OVRInput.GetDown(OVRInput.Button.One) || Input.GetKeyDown(KeyCode.Z))
                    choose(0);
                else if (OVRInput.GetDown(OVRInput.Button.Three)|| Input.GetKeyDown(KeyCode.X))
                    choose(1);
                return;

            default: return;
        }
        
	}

    GameObject create_color_cube(int x, int y, int z, Vector3 center_position, Color c1, Color c2, bool quick = true) {
        GameObject new_cube = Instantiate(default_system);
        new_cube.transform.position = center_position;
        ColorCube c = new_cube.AddComponent<ColorCube>();
        c.setColorCube(x, y, z, mat, c1, c2);
        c.Init_my_cube(quick);
        return new_cube;
    }

    bool split_cube(int axis = 0) {
        int a = axis % 3;
        float split_color;
        Vector3 pos0, pos1;
        st = cube_state.wait_choose;
        switch (a) {
            case 0:
                //x_axis
                split_color = (corner000.r + corner111.r) * 0.5f;
                Destroy(current_cube);
                wave.transform.localRotation = Quaternion.Euler(0f, 90f, 90f);
                wave.GetComponent<TestMask>().start_wave();
                pos0 =new  Vector3(0.25f * ColorCube.EDGE_LENGTH * x , 0, 0);
                pos1 = new Vector3(-0.25f * ColorCube.EDGE_LENGTH *x , 0, 0);
                color_temp0 = new Color(split_color, corner111.g, corner111.b);
                color_temp1 = new Color(split_color, corner000.g, corner000.b);
                cube_temp0 = create_color_cube(x / 2, y, z, pos0, corner000, color_temp0);
                cube_temp1 = create_color_cube(x / 2, y, z, pos1, color_temp1, corner111);
                return true;
            case 1:
                //y_axis
                split_color = (corner000.g + corner111.g) * 0.5f;
                Destroy(current_cube);
                wave.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);
                wave.GetComponent<TestMask>().start_wave();
                pos0 = new Vector3(0, 0.25f * ColorCube.EDGE_LENGTH * y, 0);
                pos1 = new Vector3(0, -0.25f * ColorCube.EDGE_LENGTH * y, 0);
                color_temp0 = new Color(corner111.r, split_color, corner111.b);
                color_temp1 = new Color(corner000.r, split_color, corner000.b);
                cube_temp0 = create_color_cube(x, y / 2, z, pos0, corner000,color_temp0 );
                cube_temp1 = create_color_cube(x, y / 2, z, pos1, color_temp1, corner111);
                return true;
            case 2:
                //z_axis
                split_color = (corner000.b + corner111.b) * 0.5f;
                Destroy(current_cube);
                wave.transform.localRotation = Quaternion.Euler(180f, 0f, 0f);
                wave.GetComponent<TestMask>().start_wave();
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

    void choose(int i) {
        int ans = i % 2;
        if (ans == 0) {
            //set new corner color
            corner111 = color_temp0;
            current_cube = create_color_cube(x, y, z, Vector3.zero, corner000, corner111);
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
            current_cube = create_color_cube(x, y, z, Vector3.zero, corner000, corner111);
            //set state
            cube_temp0.GetComponent<ColorCube>().delete_me(false);
            cube_temp1.GetComponent<ColorCube>().delete_me();
            st = cube_state.free;
            return;
        }
    }

}
