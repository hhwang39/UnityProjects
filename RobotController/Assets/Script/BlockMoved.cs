using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TouchScript.Gestures;
public class BlockMoved : MonoBehaviour {
	private Vector3 screenPoint;
	private Vector3 offset;
	private int currInd;
	/// <summary>
	/// The identifier. -1 for normal blocks and some unique value for for loop block
	/// </summary>
	public int id;
	//private string Tag;
	void Start() {

	}
	/// <summary>
	/// Raises the mouse down event.
	/// </summary>
	void OnMouseDown() {
		if (gameObject.transform.parent != null) currInd = int.Parse(gameObject.transform.parent.name);
		Camera.main.gameObject.GetComponent<playScript>().makeChoiceInvisible();
		//Debug.Log("hello");
		screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
		offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
	}
	/// <summary>
	/// Raises the mouse drag event.
	/// </summary>
	void OnMouseDrag() {
		//Debug.Log("curPos: " + gameObject.transform.position.ToString());
		//if (gameObject.transform.parent != null) gameObject.transform.parent.gameObject.GetComponent<Grid>().isContain = false;
		Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
		Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
		gameObject.transform.position = curPosition;
	}
	/// <summary>
	/// Raises the mouse up event.
	/// </summary>
	void OnMouseUp() {
		//Debug.Log("mouse pos: " + gameObject.GetComponent<RectTransform>().localPosition.ToString());
		Vector3 curPos = gameObject.transform.position;
		if (gameObject.transform.parent != null) gameObject.transform.parent.gameObject.GetComponent<Grid>().isContain = false;
		gameObject.transform.SetParent(null);
		Debug.Log("curPos: " + curPos);
		checkifPrevGridEmpty();
		checkCloestGrid(curPos);
		checkIntegBlocks();
		//Camera.main.gameObject.GetComponent<playScript>().setChoiceMenu(gameObject);
	}
	/// <summary>
	/// Raises the collision enter event.
	/// when collided with trashcan, destroy the current object and move blocks apporiately
	/// </summary>
	/// <param name="col">collided object.</param>
	void OnCollisionEnter (Collision col) {
		if (col.gameObject.name == "TrashCan") {
			destroySameUniqueId();
			//gameObject.transform.parent.GetComponent<Grid>().isContain = false;
			if (gameObject.name.StartsWith("F")) {
				shiftBlocks();
			} else {
				checkifPrevGridEmpty();
			}
			//notify move block
			if ((gameObject.name.StartsWith("0"))) {
				GameObject.Find("MoveBlock").GetComponent<Block>().notified();
			} else if (gameObject.name.StartsWith("1")) {
				GameObject.Find("RotateBlock").GetComponent<Block>().notified();
			} else if (gameObject.name.StartsWith("2")) {
				GameObject.Find("SpeakBlock").GetComponent<Block>().notified();	
			} else if (gameObject.name.StartsWith("F")) {
				GameObject.Find("ForBlock").GetComponent<Block>().notified();
			}
			Camera.main.gameObject.GetComponent<playScript>().makeChoiceInvisible();
			Destroy(gameObject);
		}
	}
	void OnEnable() {
		gameObject.GetComponent<LongPressGesture>().LongPressed += longPressHandler;
	}
	void OnDisable() {
		gameObject.GetComponent<LongPressGesture>().LongPressed -= longPressHandler;
	}
	void longPressHandler(object sender, System.EventArgs e) {
		Camera.main.gameObject.GetComponent<playScript>().setChoiceMenu(gameObject);
	}
	/// <summary>
	/// Check if the previous grid empty and shift it up
	/// </summary>
	void checkifPrevGridEmpty() {
		GameObject[] grids = GameObject.FindGameObjectsWithTag("Grid");
		while (currInd != grids.Length - 1 && grids[currInd +1].GetComponent<Grid>().isContain) {
			//Debug.Log("ind : " + currInd);
			GameObject g = grids[currInd  + 1].transform.GetChild(0).gameObject;
			g.transform.SetParent(null);
			g.transform.position = grids[currInd].transform.position;
			g.transform.SetParent(grids[currInd].transform);
			grids[currInd].GetComponent<Grid>().isContain = true;
			grids[currInd + 1].GetComponent<Grid>().isContain = false;
			currInd++;
		}
	}
	/// <summary>
	/// Checks the cloest grid.
	/// </summary>
	public void checkCloestGrid(Vector3 pos) {
		//bool = isGridFull() || isGridFullFor();
		if (!(isGridFull())) {
			//Debug.Log("grid is : true");
			GameObject[] grids = GameObject.FindGameObjectsWithTag("Grid");
//			for (int i = 0; i < grids.Length; ++i) {
//				Debug.Log(grids[i].name);
//			}
			int closestIndex = findMinDistance(grids, pos);
			//Debug.Log("cloesetIndex: " + closestIndex);
			if (grids[closestIndex].GetComponent<Grid>().isContain) {
				moveLogic(grids, closestIndex);
			}
			gameObject.transform.position = grids[closestIndex].transform.position;
			gameObject.transform.SetParent(grids[closestIndex].transform);
			grids[closestIndex].GetComponent<Grid>().isContain = true;
		} else {
			//Debug.Log("grid is : YOLO");
			
			if ((gameObject.name.StartsWith("0"))) {
				GameObject.Find("MoveBlock").GetComponent<Block>().notified();
			} else if (gameObject.name.StartsWith("1")) {
				GameObject.Find("RotateBlock").GetComponent<Block>().notified();
			} else if (gameObject.name.StartsWith("2")) {
				GameObject.Find("SpeakBlock").GetComponent<Block>().notified();	
			} else if (gameObject.name.StartsWith("F")) {
				GameObject.Find("ForBlock").GetComponent<Block>().notified();
			}
			Destroy(gameObject);
		}

	}
	/// <summary>
	/// Shifts the block down if there is a block that is currently occupying
	/// </summary>
	/// <param name="grids">Grids objects in array.</param>
	/// <param name="index">closest index.</param>
	void moveLogic(GameObject[] grids, int index) {
		Debug.Log("Move Index: " + index);
		bool logic = true;
		for (int i = index + 1; i < grids.Length && logic; ++i) {
			Grid gr = grids[i].GetComponent<Grid>();
			Grid gr2 = grids[i - 1].GetComponent<Grid>();
			GameObject g = gr2.transform.GetChild(0).gameObject;
			g.transform.SetParent(null);
			g.transform.position = grids[i].transform.position;
			g.transform.SetParent(grids[i].transform);
			if (!gr.isContain) {
				gr.isContain = true;
				logic = false;
			}
		}
	}
	/// <summary>
	/// Checks if all the grids are full.
	/// </summary>
	/// <returns><c>true</c>, if all grids are full <c>false</c> otherwise.</returns>
	bool isGridFull() {
		bool nameBool = gameObject.name.StartsWith("F");
		if (!nameBool) {
			GameObject[] gTemp = GameObject.FindGameObjectsWithTag("Grid");
			foreach (GameObject g in gTemp) {
				if (!g.GetComponent<Grid>().isContain) {
					return false;
				}
			}
			return true;
		}
		return false;
	}
	/// <summary>
	/// Finds the grid within the minimum distance, but if previous grids are empty find the lowest 
	/// (by name) grid that it can find
	/// </summary>
	int findMinDistance(GameObject[] grids, Vector3 pos) {
		float minDist = Mathf.Infinity;
		int ind = 0;
		//Debug.Log("pos: " + gameObject.GetComponent<RectTransform>().localPosition.ToString());
		//find the minimum distance
		for (int i = 0; i < grids.Length; ++i) {
			//float dist2 = Mathf.Sqrt(Mathf.Abs(Mathf.Pow(grids[i].transform.position.x, 2.0f) - Mathf.Pow(gameObject.transform.position.x, 2.0f)) + Mathf.Abs(Mathf.Pow(grids[i].transform.position.y, 2.0f) - Mathf.Pow(gameObject.transform.position.y, 2.0f)));
			float dist = Vector2.Distance(grids[i].transform.position, pos);//CalDistBetweenTwoPoints(grids[i].transform.position, pos);
			//Debug.Log("my Cal at " + i + " dis: " + dist2.ToString());
			Debug.Log("dis: " + dist + " at " + i + " at grids " + grids[i].name + " grids pos: " + grids[i].transform.position.ToString());
			if (dist < minDist) {
				minDist = dist;
				ind = i;
			}
		}
		//if index is first, there is no previous, and if previous is occupied, just return that index
		Debug.Log("new Ind: " + ind);
		if ((ind == 0) || (grids[ind - 1].GetComponent<Grid>().isContain)) {
			return ind;	
		}
		while ( ind != 0 && !(grids[ind - 1].GetComponent<Grid>().isContain)) {
			ind--;
		}
		return ind;
	}
	/// <summary>
	/// Calculates the distance between two points not using given Unity Dist function.
	/// This calculates without considering z axis. (only x and y axis)
	/// </summary>
	/// <returns>The dist between two points.</returns>
	/// <param name="fpos">First point.</param>
	/// <param name="spos">Secon point.</param>
	float CalDistBetweenTwoPoints(Vector3 fpoint, Vector3 spoint) {
		float dist = Mathf.Sqrt(Mathf.Abs(Mathf.Pow(fpoint.x, 2.0f) - Mathf.Pow(spoint.x, 2.0f))
		           + Mathf.Abs(Mathf.Pow(fpoint.y, 2.0f) - Mathf.Pow(spoint.y, 2.0f)));
		return dist;
	} 
	/// <summary>
	/// Assigns the unique id only applies to for loop id has to be positive
	/// </summary>
	/// <param name="nid">New id.</param>
	public void assignUniqueid(int nid) {
		id = nid;
	}
	/// <summary>
	/// Destroies the same unique identifier applies to only for loop.
	/// </summary>
	void destroySameUniqueId() {
		if (gameObject.name.StartsWith("F")) {
			GameObject[] gArr = GameObject.FindGameObjectsWithTag("objTag");
			foreach (GameObject g in gArr) {
				BlockMoved b = g.GetComponent<BlockMoved>();
				if (b != null && b.id == id && g.name != gameObject.name) {
					g.transform.GetComponentInParent<Grid>().isContain = false;
					//Debug.Log(g.name);
					Destroy(g);
				}
			}
		}
	}
	/// <summary>
	/// Shifts the blocks special purpose used for only For Loop
	/// </summary>
	void shiftBlocks() {
		GameObject[] grids = GameObject.FindGameObjectsWithTag("Grid");
		List<GameObject> gArr = new List<GameObject>();
		for (int i = 0; i < grids.Length; ++i) {
			if (grids[i].GetComponent<Grid>().transform.childCount > 0) {
				//get block moved component in gameobject grid
				BlockMoved g = grids[i].GetComponent<Grid>().transform.GetChild(0).gameObject.GetComponent<BlockMoved>(); 
				if (g.id != id) {
					gArr.Add(grids[i].transform.GetChild(0).gameObject);
				}
			}
		}
		for (int i = 0; i < gArr.Count; ++i) {
			gArr[i].transform.SetParent(null);
			Debug.Log(gArr[i].name);
			gArr[i].transform.position = grids[i].transform.position;
			gArr[i].transform.SetParent(grids[i].transform);
			grids[i].GetComponent<Grid>().isContain = true;
		}
	}
	/// <summary>
	/// Checks the integrity of for loop blocks.
	/// If for loop start is after for loop end block
	/// </summary>
	public void checkIntegBlocks() {
		GameObject[] grids = GameObject.FindGameObjectsWithTag("Grid");
		List<GameObject> fstartList = new List<GameObject>();
		List<GameObject> fendList = new List<GameObject>();
		//find all the for loop blocks
		for (int i = 0; i < grids.Length; ++i) {
			if (grids[i].transform.childCount > 0 &&
			    grids[i].transform.GetChild(0).gameObject.name.StartsWith("F")) {
				if (grids[i].transform.GetChild(0).gameObject.name == "FE") {
					fendList.Add(grids[i].transform.GetChild(0).gameObject);
				} else {
					fstartList.Add(grids[i].transform.GetChild(0).gameObject);
				}
	
			}
		}
		//find for loop start and end. If start is behind the end, swap two.
		int ind2 =0;
		int ind1 = 0;
		for (int k = 0; k < fendList.Count; ++k) {
			BlockMoved bm = fstartList[k].GetComponent<BlockMoved>();
			ind1 = int.Parse(fstartList[k].transform.parent.name);
			for (int j = 0; j < fstartList.Count; ++j) {
				if (fendList[j].GetComponent<BlockMoved>().id == bm.id) {
					ind2 = int.Parse(fendList[j].transform.parent.name);
				}
			}
			if (ind1 > ind2) {
				fstartList[k].transform.SetParent(null);
				GameObject fend = grids[ind2].transform.GetChild(0).gameObject;
				fend.transform.SetParent(null);
				fstartList[k].transform.position = grids[ind2].transform.position;
				fstartList[k].transform.SetParent(grids[ind2].transform);
				fend.transform.position = grids[ind1].transform.position;
				fend.transform.SetParent(grids[ind1].transform);
			}
		}

	}
}
