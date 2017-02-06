using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NodeData : MonoBehaviour {

	public enum FlowDirection
	{
		left = 1,
		left_up,
		up,
		right_up,
		right,
		right_down,
		down,
		left_down,
		None,
	}

	#region flow filed property
	/// <summary>
	/// 1 is well road, 2 - 254 is worse, 255 is impassable
	/// Store the cost of the Node, use to build integration data
	/// </summary>
	public System.Byte cost = 0;

	/// <summary>
	/// Store the cast to goal, used to calculate direction
	/// </summary>
	public System.Int32 integration = 0xFFFF;
	#endregion

	/// <summary>
	/// Store path goal direction
	/// </summary>
	public FlowDirection direction = FlowDirection.None;

	#region A start property
	/// <summary>
	/// how many tile passed to reach this tile
	/// </summary>
	public int G;

	/// <summary>
	/// The h. estimate to reach the target point
	/// </summary>
	public int H;

	/// <summary>
	/// F is the A* value of this node. F = G + H.
	/// </summary>
	/// <value>The f.</value>
	public int F
	{
		get
		{
			return H + G;
		}
	}
	#endregion

	public int row;

	public int column;

	public int index;

	[SerializeField]
	Image background;
	[SerializeField]
	Text text_identify;
	[SerializeField]
	Image image_identify;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void OnClick()
	{
		
	}

	public void SetColor(Color c)
	{
		background.color = c;
	}
}
