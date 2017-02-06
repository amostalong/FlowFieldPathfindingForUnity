using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonMgr : MonoBehaviour {

	public enum CursorState
	{
		SetBlock,


		None,
	}

	[SerializeField]
	CursorState cursor_state;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


	public void OnBlockButtonClick ()
	{
		if (cursor_state == CursorState.SetBlock)
		{
			cursor_state = CursorState.None;

			Debug.Log("无状态。");
		}
		else
		{
			cursor_state = CursorState.SetBlock;

			Debug.Log ("设置障碍。");
		}
	}
}

