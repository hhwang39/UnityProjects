using UnityEngine;
using System.Collections;

public class SoundRecorder : MonoBehaviour {
	//AndroidJavaObject ajo;
	private AndroidJavaObject jObj;
	private string dataStr;
	private string sysStr;
	public string Data {
		get {
			return dataStr;
		}
	}
	public string SystemString {
		get {
			return sysStr;
		}
	}
	// Use this for initialization
	public void init () {
		AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		jObj = jc.GetStatic<AndroidJavaObject>("currentActivity");
		//ajo = ajc.GetStatic<AndroidJavaObject>("currentActivity");
		sysStr = "System: ";
		dataStr = "data: ";
	}
	// Update is called once per frame
	void Update () {
		
	}

	public void speak() {
		jObj.Call("speak");
	}
	public void getVoiceData(string str) {
		dataStr = "data: " + str;
	}
	public void setStr(string str) {
		sysStr = "System: " + str;
	}
}
