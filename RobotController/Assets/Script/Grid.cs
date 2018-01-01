using UnityEngine;
using System.Collections;

public class Grid : MonoBehaviour {
	public bool isContain{get;set;}
	// Use this for initialization
	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start () {
		isContain = false;
		Destroy(gameObject.GetComponent<MeshCollider>());
	}
	
	// Update is called once per frame
	/// <summary>
	/// Update this instance.
	/// </summary>
	void Update () {
		if (isContain) {
			if (gameObject.transform.childCount == 0) {
				isContain = false;
			}
		}
	}

}
