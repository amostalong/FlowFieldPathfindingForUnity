using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class FlowField {


	List<CubePathData> open_cube = new List<CubePathData>();
	List<CubePathData> close_cube = new List<CubePathData> ();


	float max_F = 0;

	public void GenerateFlowField(CubeManager cm)
	{
		if (cm.end_cube == -1)
		{
			Debug.LogError("Please Set End Cube!");
			return;
		}

		if (cm.x == 1 || cm.z == 1)
		{
			Debug.LogError("No need Flow Field, there is only one way");
		}

		max_F = 0;



		open_cube.Clear ();
		close_cube.Clear ();

		open_cube.Add (cm.path_datas [cm.end_cube]);

		cm.path_datas [cm.end_cube].F = 0;

		//防止死循环
		int n = 10000;


		while(open_cube.Count > 0 && --n > 0)
		{
			for(int i = 0; i < open_cube.Count; ++i)
			{
				CheckF (open_cube [i], cm);
			}
		}

		Debug.Log ("Max F: " + max_F);
		for (int i = 0; i < cm.path_datas.Count; ++i)
		{
			if (i == cm.end_cube)
				cm.path_datas [i].mesh.material.color = Color.blue;
			else if (cm.path_datas [i].cost != 65535f) {
				float v = 1f - cm.path_datas [i].F / max_F;
				cm.path_datas [i].mesh.material.color = new Color (v, v, v);
			}

			cm.path_datas [i].cube.name = cm.path_datas [i].cube.name + " F: " + cm.path_datas[i].F;
		}
	}

	private void CheckF(CubePathData end_cube, CubeManager cm)
	{

		var nears = cm.GetNearPathData (end_cube);
		for (int i = 0; i < nears.Count; ++i) {
			var n_cube = nears [i];
					
			float F = n_cube.cost + end_cube.F;

			if (n_cube.F > F) {
				n_cube.F = F;
				if (F > max_F)
					max_F = F;
				if (!open_cube.Contains (n_cube)) {
					open_cube.Add (n_cube);
				}
			}					
		}

		open_cube.Remove (end_cube);
		close_cube.Add (end_cube);
	}
}
