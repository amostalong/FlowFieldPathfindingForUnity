using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FFPather {

	public static CubePathData GetNextCube(CubeManager cm, CubePathData cube)
	{
		if (cube.index == cm.end_cube)
			return null;
		
		var near = cm.GetNearPathData (cube);

		float min_F = float.MaxValue;

		int n_index = -1;

		for(int i = 0; i < near.Count; ++i)
		{
			if (near[i].F < min_F && near[i].cost != 65535f)
			{
				n_index = i;
				min_F = near [i].F;
			}
		}

		if (n_index != -1)
			return near [n_index];
		else
			return null;
	}
}
