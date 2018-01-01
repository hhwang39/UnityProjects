using UnityEngine;
using System.Collections;

public class Bluetooth : MonoBehaviour {
	public string genStr;
	public string readStr;
	private AndroidJavaObject jObj;
	public void initialize () {
		genStr = "";
		AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		jObj = jc.GetStatic<AndroidJavaObject>("currentActivity");
		//genStr = "Ready";
	}
	void BluetoothMessage(string str) {
		genStr = str;
	}
	public void connect() {
		jObj.Call("connectToServer");
	}
	public void write(string str) {
		jObj.Call("write", new object[] {str});
	}
	void readData(string str) {
		readStr = str;
	}
	public bool isEmptyData() {
		if (readStr == "") {
			return true;
		}
		return false;
	}
}
