using UnityEngine;
using System.Collections;

public class Model : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	//draw spaces using Gizmos
	void OnDrawGizmos() {
		Gizmos.color = Color.blue;
		Gizmos.DrawWireCube(transform.position, new Vector3 (3, 0, 4));
		Gizmos.color = Color.white;
		Gizmos.DrawCube(transform.position, new Vector3 (3, 0, 4));
	}
}
