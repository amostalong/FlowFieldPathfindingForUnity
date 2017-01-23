using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NodeData : MonoBehaviour {

	/// <summary>
	/// 1 is well road, 2 - 254 is worse, 255 is impassable
	/// </summary>
	public int cost;

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
}
