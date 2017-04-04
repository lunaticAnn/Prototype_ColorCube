using System.Collections;
using System.Collections.Generic;

public class int3{
	public int i;
	public int j;
	public int k;
	
	public int3(int x, int y, int z) {
		i = x;
		j = y;
		k = z;
	}

	public static int3[] structual = {new int3( 1, 0, 0), 
							   new int3( 0, 1, 0),
							   new int3( 0, 0, 1),
							   new int3(-1, 0, 0), 
							   new int3(0, -1, 0), 
							   new int3(0, 0, -1)};

	public static int3[] shear = {new int3( 1, 1, 0),
							   new int3( 1, -1, 0),
							   new int3( -1, 1, 0),
							   new int3(-1, -1, 0),
							   new int3( 1, 0, 1),
							   new int3( 1, 0, -1),
							   new int3(-1, 0, 1),
							   new int3(-1, 0, -1),
							   new int3( 0, 1, 1),
							   new int3( 0, -1, 1),
							   new int3( 0, 1, -1),
                               new int3( 0, -1,-1)};

	public static int3[] bend = {new int3( 2, 0, 0),
							   new int3( 0, 2, 0),
							   new int3( 0, 0, 2),
							   new int3(-2, 0, 0),
							   new int3(0, -2, 0),
							   new int3(0, 0, -2) };
}
