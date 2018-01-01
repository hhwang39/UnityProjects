using UnityEngine;
using System.Collections;

public class mainGUI : MonoBehaviour {
	BluetoothLE ble;
	string text;
	string str;
	// Use this for initialization
	void Start () {
		ble = GameObject.Find("BluetoothLE").GetComponent<BluetoothLE>();
		ble.Initialize();
		text = "";
		str = "Not sent";
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	/// <summary>
	/// Raises the GU event.
	/// </summary>
	void OnGUI() {
		if (GUI.Button(new Rect(100, 100, 100, 100), "SEND")) {
			ble.Send(text);
			str = "Sent: " + text;
		}
		if (GUI.Button(new Rect(300, 100, 100, 100), "Scan")) {
			ble.scanBLE();
		}
		if (GUI.Button(new Rect(400, 100, 100, 100), "Connect")) {
			ble.connect();
		}
		GUI.Label(new Rect(Screen.width / 2 - 100, 100, 200, 100), ble.SystemStr);
		GUI.Label(new Rect(3 * Screen.width / 4 - 100, 100, 200, 100), str);
		GUI.Label(new Rect(50, 50, 100, 100), ble.showDevList());
		text = GUI.TextField(new Rect(150, 50, 250, 50), text);
	}
}
