using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;

public class CallJavaCode : MonoBehaviour {
	private AndroidJavaObject jObj;
	private string announceString;
	private string str;
	private ArrayList deviceArray;
	private int buttonX = 0;
	private int buttonY = 0;
	private List<string> uuidList = new List<string>();
	private string	stringToEdit = "";
	//private IntPtr BluetoothSampleActivityClass; // ［1］
	//private IntPtr ptr_checkBluetooth; // ［2］
	void Awake ()
	{

	}
	void Start () {
		AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		jObj = jc.GetStatic<AndroidJavaObject>("currentActivity");
		// attach our thread to the java vm; obviously the main thread is already attached but this is good practice..
		announceString = "";
		str = "";
		deviceArray = new ArrayList();
		//checkBluetooth();
	}
	private void checkBluetooth(){ // ［5］
		deviceArray.Clear();
		jObj.Call("checkBluetooth");//(BluetoothSampleActivityClass, ptr_checkBluetooth, new jvalue[1]);
		//AndroidJNI.CallVoidMethod(BluetoothSampleActivityClass, ptr_checkBluetooth);
		//jObj.Call("getUUID")
	}
	void BluetoothMessage(string message) { // ［6］
		announceString = "Status Msg: " + message;
	}
	void setMSG(string msg) {
		str = "Data Recieved: " + msg; 
	}
	void connectButton() {
		if (GUI.Button(new Rect(200, 200, 100, 100), "Connect")) {
			jObj.Call("connectToServer");
		}
	}
	void OnGUI (){
		resetButton();
		showMessage(announceString, 200, 50,Screen.width / 8, Screen.height / 10);
		showMessage (str, 200, 50, Screen.width / 8, Screen.height / 10 + 50);
		showDiscoveredDeviceList();
		if(makeButton("Search Devices")){
			checkBluetooth();
		}
		connectButton();
		dataToSend();
		serverButton();
	}
	void addBTDevice(string deviceinfo){ // ［7］
		deviceArray.Add(deviceinfo);
	}
	void resetButton(){
		buttonX = 0;
		buttonY = 0;
	}
	void showMessage(string message, int W, int H, int x, int y){
		int labelW = W;
		int labelH = H;
		GUI.Label(new Rect(x, y,labelW,labelH), message);
		
	}
	void showDiscoveredDeviceList(){
		string message = "Around You:\n";
		for(int i=0;i<deviceArray.Count ;i++){
			message += " " + (string)deviceArray[i] + "\n";
		}
		int labelW = 400;
		int labelH = 500;
		GUI.Label(new Rect(10,250,labelW,labelH), message);
	}
	void dataToSend() {
		stringToEdit = GUI.TextField(new Rect(400, 10, 200, 50), stringToEdit);
		if (GUI.Button(new Rect(500, 60, 100, 50), "Send")) {
			jObj.Call("write", new object[] {stringToEdit});
		}
	}
	bool makeButton(string labelR){
		int buttonW = 150;
		int buttonH = 50;
		int buttonsInRow = 2;
		bool b = GUI.Button(new Rect(Screen.width / 2, Screen.height / 2, buttonW, buttonH), labelR);
		buttonX++;
		if(buttonX==buttonsInRow){
			buttonX = 0;
			buttonY++;
		}
		return b;
	}
	void _log(string logstring){
		Debug.Log (logstring);
	}
	void addUUID(string str) {
		uuidList.Add(str);
	}
	void serverButton() {
		if (GUI.Button(new Rect(3 * Screen.width / 4, 3 * Screen.height / 4, 50, 50), "Server")) {
			jObj.Call("startAsServer");
		}
	}
}