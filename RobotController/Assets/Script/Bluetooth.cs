using UnityEngine;
using System.Collections;

public class Bluetooth : MonoBehaviour {
	public string genStr;
	public string readStr;
	private AndroidJavaObject jObj;
	/// <summary>
	/// Initialize this instance.
	/// </summary>
	public void initialize () {
		genStr = "";
		AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		jObj = jc.GetStatic<AndroidJavaObject>("currentActivity");
		//genStr = "Ready";
	}
	/// <summary>
	/// Bluetooths the message.
	/// </summary>
	/// <param name="str">String.</param>
	void BluetoothMessage(string str) {
		genStr = str;
	}
	/// <summary>
	/// Connect to bluetooth. If bluetooth is not enabled, enable it and connect
	/// </summary>
	public void connect() {
		jObj.Call("checkBluetoothEnableClient");
	}
	/// <summary>
	/// Write the specified string to bluetooth.
	/// </summary>
	/// <param name="str">String.</param>
	public void write(string str) {
		jObj.Call("write", new object[] {str});
	}
	/// <summary>
	/// Reads the data.
	/// </summary>
	/// <param name="str">String.</param>
	void readData(string str) {
		readStr = str;
	}
	/// <summary>
	/// Check if data is empty
	/// </summary>
	/// <returns><c>true</c>, if empty data was ised, <c>false</c> otherwise.</returns>
	public bool isEmptyData() {
		if (readStr == "") {
			return true;
		}
		return false;
	}
}
