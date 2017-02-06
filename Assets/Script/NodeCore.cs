using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum TileDirection
{
	Up,
	Down,
	Left,
	Right,
}

public class NodeCore : MonoBehaviour {

	public Color block_color = Color.gray;
	public Color cur_color = Color.red;

	List<NodeData> nodes = new List<NodeData>();

	int row_num;
	int colum_num;
	public int Row_Num
	{
		get
		{
			return row_num;
		}
	}
	public int Column_Num
	{
		get
		{
			return colum_num;
		}
	}


	private NodeData cur_node;

	[SerializeField]
	Transform nodes_root;

	// Use this for initialization
	void Awake () {

		GridLayoutGroup grid = nodes_root.GetComponent<GridLayoutGroup> ();

		if(grid.constraint == GridLayoutGroup.Constraint.FixedColumnCount)
		{
			colum_num = grid.constraintCount;
			row_num = nodes_root.childCount / colum_num;
		}
		else if (grid.constraint == GridLayoutGroup.Constraint.FixedRowCount)
		{
			row_num = grid.constraintCount;
			colum_num = nodes_root.childCount / row_num;
		}
		else
		{
			Debug.LogError ("Grid 需要使用行固定或者列固定");
		}

		int i = 0;
		int r = 0;
		int c = 0;
		while(i < nodes_root.childCount)
		{
			var data = nodes_root.GetChild (i).GetComponent<NodeData> ();
			data.row = r;
			data.column = c;
			data.index = i;

			++i;
			if (c < colum_num)
			{
				c++;
			}
			else
			{
				c = 0;
				r++;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public NodeData GetNext(TileDirection direction)
	{
		switch(direction)
		{
		case TileDirection.Up:
			if (cur_node.row == 0)
				return null;

			return nodes [cur_node.index - colum_num];
		case TileDirection.Down:
			if (cur_node.row == row_num - 1)
				return null;

			return nodes [cur_node.index + colum_num];

		case TileDirection.Left:
			if (cur_node.column == 0)
				return null;

			return nodes [cur_node.index - 1];

		case TileDirection.Right:
			if (cur_node.column == colum_num - 1)
				return 	null;

			return nodes [cur_node.index + 1];
		}

		return null;
	}
}
