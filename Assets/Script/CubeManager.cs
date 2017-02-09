using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class CubePathData
{
	public int index;
	public int x;
	public int z;
	public MeshRenderer mesh;
	public GameObject cube;

	//rang: (0,1]
	public float cost = Random.Range(1,3);
	public float F = 65535f;


	public CubePathData(int index,int x, int z, GameObject cube, MeshRenderer mesh)
	{
		this.index = index;
		this.x = x;
		this.z = z;
		this.cube = cube;
		this.mesh = mesh;
	}
}

public enum CubeNearType
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

public class MoveCube
{
	private CubePathData _stand_cube;
	public CubePathData stand_cube
	{
		get
		{
			return _stand_cube;
		}
		set
		{
			_stand_cube = value;
			self_cube.transform.position = 
				new Vector3 (_stand_cube.cube.transform.position.x,
				_stand_cube.cube.transform.position.y + 1,
				_stand_cube.cube.transform.position.z);
		}
	}

	public GameObject self_cube;

	public MoveCube (CubePathData stand, GameObject cube)
	{
		this._stand_cube = stand;
		this.self_cube = cube;

		this.self_cube.transform.position = 
			new Vector3 (_stand_cube.cube.transform.position.x,
				_stand_cube.cube.transform.position.y + 1,
				_stand_cube.cube.transform.position.z);
	}
}

public class CubeManager : MonoBehaviour {

	enum State
	{
		Set_Start,
		Set_End,
		None,
	}



	public Button btn_setstart;
	public Button btn_setend;
	public Button btn_generate;
	public Button btn_ff;
	public Button btn_go;
	public InputField input_x;
	public InputField input_z;
	public InputField input_obstacle;

	[SerializeField]
	Material base_mat;
	[SerializeField]
	Material obstacle_mat;
	[SerializeField]
	Material mover_mat;


	public List<CubePathData> path_datas = new List<CubePathData>();
	public List<CubePathData> obstacle_datas = new List<CubePathData>();

	public int x;
	public int z;

	[SerializeField]
	State state = State.None;

	[SerializeField]
	public int end_cube = -1;

	[SerializeField]
	public List<MoveCube> goer = new List<MoveCube> ();

	[SerializeField]
	CubeNearType near_type = CubeNearType.Four_Direction;

	Direction[] direction;

	public FlowField ff;

	bool go_ff = false;

	public void Awake()
	{
		btn_generate.onClick.AddListener (GenerateCubeMap);
		btn_setend.onClick.AddListener (SetEnd);
		btn_setstart.onClick.AddListener (SetStart);
		btn_ff.onClick.AddListener (GenerateFlowField);
		btn_go.onClick.AddListener (GoFF);
		ff = new FlowField ();
		//left, leftup, up, rightup, right, rightdown, down, leftdown
		direction = new Direction[8] {
			new Direction(-1,0), new Direction(0,1), new Direction(1,0), new Direction(0,-1),
			new Direction(-1,1), new Direction(1,1), new Direction(1,-1), new Direction(-1,-1)
		};
	}

	void SetStart()
	{
		state = state == State.Set_Start ? State.None : State.Set_Start;
		if (state == State.None)
		{
			btn_setend.GetComponentInChildren<Text>().text = "Set Target";
			btn_setstart.GetComponentInChildren<Text>().text = "Cancel";
		}
		else
		{
			btn_setstart.GetComponentInChildren<Text>().text = "Set Pather";
		}
	}

	void SetEnd()
	{
		state = state == State.Set_End ? State.None : State.Set_End;

		if (state == State.None)
		{
			btn_setstart.GetComponentInChildren<Text>().text = "Set Pather";
			btn_setend.GetComponentInChildren<Text>().text = "Cancel";
		}
		else
		{
			btn_setend.GetComponentInChildren<Text>().text = "Set Target";
		}
	}

	void GenerateFlowField()
	{
		ResetAllFValue ();
		ff.GenerateFlowField (this);
	}

	void ResetAllFValue()
	{
		for (int i =0; i < path_datas.Count; ++i)
		{
			path_datas [i].F = 65535f;
		}
	}

	void GoFF()
	{
		if (go_ff == false) {
			go_ff = true;
			StartCoroutine (GoFlowField());
			btn_go.GetComponentInChildren<Text>().text = "Stop FF";
		}
		else
		{
			go_ff = false;
			StopAllCoroutines ();
			btn_go.GetComponentInChildren<Text>().text = "Go FF";
		}

	}

	IEnumerator GoFlowField()
	{
		while (true) {
			for (int i = 0; i < goer.Count; ++i) {
				if (goer == null)
					continue;
			
				CubePathData next = null;

				if (goer [i] != null)
					next = FFPather.GetNextCube (this, goer [i].stand_cube);
				if (next != null) {
					goer [i].stand_cube = next;
				} else {
					Debug.Log (string.Format ("No next cube can go!"));
				}
			}

			yield return new WaitForSeconds (.2f);
		}
	}

//	void OnGUI()
//	{
//		if (GUI.Button(new Rect(10, 10, 100, 30), "Generate Cube Map"))
//		{
//			GenerateCubeMap ();
//		}
//
//		input_x = GUI.TextField(new Rect(120,10, 50, 30), "X");
//		input_z = GUI.TextField (new Rect (180, 10, 50, 30), "Y");
//		input_obstacle = GUI.TextField(new Rect(240,10,50,30), "Obstacle");
//	}

	private void GenerateCubeMap()
	{
		for(int i = 0; i < path_datas.Count; ++i)
		{
			Destroy (path_datas [i].cube);
		}

		path_datas.Clear ();

		for(int i = 0; i < obstacle_datas.Count; ++i)
		{
			Destroy (obstacle_datas [i].cube);
		}

		obstacle_datas.Clear ();

		end_cube = -1;
		state = State.None;

		x = int.Parse (string.IsNullOrEmpty (input_x.text) ? "1" : input_x.text);
		z = int.Parse (string.IsNullOrEmpty (input_z.text) ? "1" : input_z.text);
		int ob = int.Parse (string.IsNullOrEmpty (input_obstacle.text) ? "0" : input_obstacle.text);

		for (int m = 0;m < z; ++m)
		{
			for (int n = 0; n < x; ++n) {
				var go = GameObject.CreatePrimitive (PrimitiveType.Cube);
				go.transform.SetParent (this.transform);
				go.transform.localPosition = new Vector3 (n, 0, m);
				go.transform.localScale = new Vector3 (0.95f, 1f, 0.95f);
				go.tag = "Path_Cube";
				go.name = "Paht: " + n.ToString () + "-" + m.ToString ();
				MeshRenderer mr = go.GetComponent<MeshRenderer> ();
				mr.sharedMaterial = base_mat;
				path_datas.Add (new CubePathData (n + m * x, n, m, go, go.GetComponent<MeshRenderer>()));
			}
		}

		while(ob > 0)
		{
			int ox = Random.Range (0, x);
			int oz = Random.Range (0, z);

			int ob_index = ox + oz * x;
			var go = path_datas [ob_index].cube;
			go.name = go.name + " obstacle";
			MeshRenderer mr = go.GetComponent<MeshRenderer> ();
			mr.sharedMaterial = obstacle_mat;
			//mr.material.color = Color.red;
			CubePathData data = new CubePathData (ob, ox, oz, go, mr) ;
			data.cost = 65535f;
			obstacle_datas.Add (data);
			path_datas [ob_index] = data;
			--ob;
		}
	}



	void Update()
	{
		if (EventSystem.current.IsPointerOverGameObject(0) == false &&state != State.None && Input.GetMouseButtonUp(0))
		{
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);

			RaycastHit hit_info;

			if(Physics.Raycast (ray, out hit_info))
			{
				if (hit_info.transform.tag == "Path_Cube")
				{
					Debug.Log ("Click On Path: " + hit_info.transform.name);
					Debug.Log ("Click On Path_index: " + hit_info.transform.GetSiblingIndex());

					CubePathData cube = path_datas[hit_info.transform.GetSiblingIndex()];
				
					if (state == State.Set_Start)
					{
						if (cube.cost != 65535)
						{
							var move_cube = new MoveCube(cube, GameObject.CreatePrimitive(PrimitiveType.Cube));
							move_cube.self_cube.GetComponent<MeshRenderer> ().sharedMaterial = mover_mat;
							move_cube.self_cube.transform.localScale = new Vector3 (0.5f, .5f, .5f);
							goer.Add(move_cube);
						}
						else Debug.LogError("Obstacle is there!");
					}
					else if (state == State.Set_End)
					{
						if (end_cube == cube.index) {
//							end_cube = -1;
//							cube.mesh.material = null;
//							cube.mesh.sharedMaterial = base_mat;
						} else {
							end_cube = cube.index;
							cube.mesh.material.color = Color.blue;
							GenerateFlowField ();
						}
					}
				}
			}
		}
	}

	public List<CubePathData> GetNearPathData(CubePathData end_cube)
	{
		List<CubePathData> res = new List<CubePathData> ();

		for (int i = 0; i < (int)near_type; ++i)
		{
			if (end_cube.x + direction [i].x_move < x && end_cube.x + direction [i].x_move >= 0
				&& end_cube.z + direction [i].z_move < z && end_cube.z + direction [i].z_move >= 0) {

				int index = end_cube.index + direction [i].x_move + direction [i].z_move * x;

				if (index >=0 && index < path_datas.Count) {
					res.Add (path_datas [index]);
				}
				else Debug.LogError (string.Format ("Index Out: {0} - {1}", (end_cube.x + direction [i].x_move), (end_cube.z + direction [i].z_move)));
			}
		}

		return res;
	}
}
