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
	public float cost = 1;
	public float F = float.MaxValue;

	public CubePathData(int index,int x, int z, GameObject cube, MeshRenderer mesh)
	{
		this.index = index;
		this.x = x;
		this.z = z;
		this.cube = cube;
		this.mesh = mesh;
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
	public InputField input_x;
	public InputField input_z;
	public InputField input_obstacle;

	[SerializeField]
	Material base_mat;
	[SerializeField]
	Material obstacle_mat;


	public List<CubePathData> path_datas = new List<CubePathData>();
	public List<CubePathData> obstacle_datas = new List<CubePathData>();

	public int x;
	public int z;

	[SerializeField]
	State state = State.None;
	[SerializeField]
	public int start_cube = -1;
	[SerializeField]
	public int end_cube = -1;

	FlowField ff;

	public void Awake()
	{
		btn_generate.onClick.AddListener (GenerateCubeMap);
		btn_setend.onClick.AddListener (SetEnd);
		btn_setstart.onClick.AddListener (SetStart);
		btn_ff.onClick.AddListener (GenerateFlowField);
		ff = new FlowField ();
	}

	void SetStart()
	{
		state = state == State.Set_Start ? State.None : State.Set_Start;
	}

	void SetEnd()
	{
		state = state == State.Set_End ? State.None : State.Set_End;
	}

	void GenerateFlowField()
	{
		ff.GenerateFlowField (this, FlowFieldType.Four_Direction);
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

		start_cube = -1;
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
			obstacle_datas.Add (new CubePathData (ob, ox, oz, go, mr));
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
						if (start_cube == cube.index) {
							start_cube = -1;
							cube.mesh.material = null;
							cube.mesh.sharedMaterial = base_mat;
						} else {
							start_cube = cube.index;
							cube.mesh.material.color = Color.green;
						}

					}
					else if (state == State.Set_End)
					{
						if (end_cube == cube.index) {
							end_cube = -1;
							cube.mesh.material = null;
							cube.mesh.sharedMaterial = base_mat;
						} else {
							end_cube = cube.index;
							cube.mesh.material.color = Color.blue;
						}
					}
				}
			}
		}

	}


}
