using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleAStar : MonoBehaviour {

	public List<NodeData> open_list = new List<NodeData>();
	public List<NodeData> close_list = new List<NodeData>();

	[SerializeField]
	NodeCore node_core;

	[SerializeField]
	NodeData start_node;
	[SerializeField]
	NodeData end_node;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	/// <summary>
	/// Calculates the node g. Here use Manhattan Block to get resulte
	/// </summary>
	/// <returns>The node g.</returns>
	/// <param name="node">Node.</param>
	public int CalculateNodeG(NodeData node)
	{
		int h = node_core.Row_Num;
		int w = node_core.Column_Num;

		int end_row = end_node.row;
		int end_column = end_node.column;

		int res = Mathf.Abs (end_row - node.row) + Mathf.Abs( end_column - node.column);

		return res;
	}

	public NodeData GetLowestF()
	{
		int index = 0;
		int lowest = int.MaxValue;

		for (int i = 0; i < open_list.Count; ++i)
		{
			if (lowest > open_list[i].F)
			{
				index = i;
			}
		}

		return open_list[index];
	}


}
