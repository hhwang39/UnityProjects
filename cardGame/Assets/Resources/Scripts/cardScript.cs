using UnityEngine;
using System.Collections;

public class cardScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnMouseDown() {
		animation.Play ("Card Flip");
	}
}
