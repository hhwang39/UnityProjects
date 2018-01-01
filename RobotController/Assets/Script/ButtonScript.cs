using UnityEngine;
using System.Collections;

public class ButtonScript : MonoBehaviour {
	/// <summary>
	/// Raises the click event.
	/// </summary>
	public void onClick() {
		//Debug.Log(gameObject.transform.name);
		Camera.main.gameObject.GetComponent<playScript>().onClick(gameObject.transform.name);
	}
}
