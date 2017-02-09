using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FlowFieldType
{
	Four_Direction = 4,

	Eight_Direcion = 8,
}

public class Direction
{
	public int x_move;
	public int z_move;

	public Direction(int x, int z)
	{
		x_move = x;
		z_move = z;
	}
}

public class FlowField {


	List<CubePathData> open_cube = new List<CubePathData>();
	List<CubePathData> close_cube = new List<CubePathData> ();

	Direction[] direction;
	float max_F = 0;

	public void GenerateFlowField(CubeManager cm, FlowFieldType type)
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

		//left, leftup, up, rightup, right, rightdown, down, leftdown
		direction = new Direction[8] {
			new Direction(-1,0), new Direction(0,1), new Direction(1,0), new Direction(0,-1),
			new Direction(-1,1), new Direction(1,1), new Direction(1,-1), new Direction(-1,-1)
		};

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
				CheckF (open_cube [i], cm, type);
			}
		}

		Debug.Log ("Max F: " + max_F);
		for (int i = 0; i < cm.path_datas.Count; ++i)
		{
			if (i == cm.end_cube)
				cm.path_datas [i].mesh.material.color = Color.blue;
			else if (cm.path_datas[i].cost != 65535f)
				cm.path_datas [i].mesh.material.color = new Color (1, 1, 1f - cm.path_datas [i].F / max_F);

			cm.path_datas [i].cube.name = cm.path_datas [i].cube.name + " F: " + cm.path_datas[i].F;
		}
	}

	private void CheckF(CubePathData end_cube, CubeManager cm, FlowFieldType type)
	{
		for (int i = 0; i < (int)type; ++i)
		{
			if (end_cube.x + direction [i].x_move < cm.x && end_cube.x + direction [i].x_move >= 0
				&& end_cube.z + direction [i].z_move < cm.z && end_cube.z + direction [i].z_move >= 0) {

				int index = end_cube.index + direction [i].x_move + direction [i].z_move * cm.x;
				
				if (index >=0 && index < cm.path_datas.Count) {
					
					var n_cube = cm.path_datas [index];
					
					float F = n_cube.cost + end_cube.F;

					if (n_cube.F > F) {
						n_cube.F = F;
						if (F > max_F)
							max_F = F;
						if (!open_cube.Contains (n_cube)) {
							open_cube.Add (n_cube);
						}
					}
				} else
					Debug.LogError (string.Format ("Index Out: {0}-{1}", (end_cube.x + direction [i].x_move), (end_cube.z + direction [i].z_move)));
			}
		}

		open_cube.Remove (end_cube);
		close_cube.Add (end_cube);
	}
}
