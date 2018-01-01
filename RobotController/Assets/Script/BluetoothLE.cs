using UnityEngine;
using System.Collections;

public class BluetoothLE : MonoBehaviour {
	private AndroidJavaObject jObj;
	public string SystemStr;
	private ArrayList DeviceList;
	public string dataStr;
	public bool connected;
	public bool scanned;
	// Use this for initialization
	void Start () {
		connected = false;
		scanned = false;
		DeviceList = new ArrayList();
		SystemStr = "System: ";
	}
	/// <summary>
	/// Initialize Android java object so that 
	/// we can call android functions
	/// </summary>
	public void Initialize() {
		AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		jObj = jc.GetStatic<AndroidJavaObject>("currentActivity");
		jObj.Call("createUART");
	}
	/// <summary>
	/// Sets the system string.
	/// </summary>
	/// <param name="str">String.</param>
	void setSystemStr(string str) {
		SystemStr = "System: " + str;
	}
	/// <summary>
	/// Send the specified str.
	/// </summary>
	/// <param name="str">String.</param>
	public void Send(string str) {
		jObj.Call("sendData", new object[] {str});
	}
	/// <summary>
	/// Adds the device.
	/// </summary>
	/// <param name="dev">Dev.</param>
	void AddDevice(string dev) {
		if (!DeviceList.Contains(dev)) DeviceList.Add(dev);
	}
	/// <summary>
	/// Shows the dev list.
	/// </summary>
	/// <returns>The dev list.</returns>
	public string showDevList() {
		string list = "Device List:\n";
		for (int i = 0; i < DeviceList.Count; ++i) {
			list += (string) DeviceList[i] + "\n";
		}
		//DeviceList.Clear();
		return list;
	}
	/// <summary>
	/// Scans the Bluetooth low energy devices around (must be periperial)
	/// </summary>
	public void scanBLE() {
		jObj.Call("startScan");
	}
	/// <summary>
	/// Connect to bluetooth.
	/// </summary>
	public void connect() {
		jObj.Call("connectToUART");
	}
	/// <summary>
	/// Reads the data ** will be called from android and assign it to string data **
	/// </summary>
	/// <param name="data">Data.</param>
	public void readData(string data) {
		dataStr = data;
	}
	/// <summary>
	/// Notifieds the disonnection
	/// </summary>
	public void notifiedDisonnection() {
		connected = false;
	}
	/// <summary>
	/// Notifieds after scanning is done
	/// </summary>
	public void notifiedDoneScanning() {
		scanned = true;
	}
	public void notifiedConnection() {
		connected = true;
	}
}

