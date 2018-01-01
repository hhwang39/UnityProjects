using UnityEngine;
using System.Collections;

public class GridCreator : MonoBehaviour {
	private Vector3 startPos;
	private uint numRows;
	private uint numCols;
	private float xScale;
	private float zScale;
	private float xDist;
	private float yDist;
	private GameObject prefab;
	// Use this for initialization
	void Start () {
		xScale = 0.45f;
		zScale = 0.20f;
		startPos = new Vector3(-3.7f, 2.8f, 0);
		numRows = 3;
		numCols = 3;
		xDist = 5;
		yDist = 2.6f;
		prefab = Resources.Load("Prefabs/Grid") as GameObject;
		createGrid();
		createArrows();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	/// <summary>
	/// Creates the grid.
	/// </summary>
	void createGrid() {
		//did opposite row and column flipped
		for (int i = 0; i < numRows; ++i) {
			for (int j = 0; j < numCols; ++j) {
				//GameObject temp = GameObject.CreatePrimitive(PrimitiveType.Plane);
				Vector3 pos = new Vector3(startPos.x + xDist * i, startPos.y - yDist * j, -0.2f);
				GameObject temp = Instantiate(prefab, pos, Quaternion.identity) as GameObject;
				//temp.transform.position = pos;
				temp.AddComponent<Grid>();
				temp.transform.eulerAngles = new Vector3(90, 180, 0);
				temp.transform.localScale = new Vector3(xScale , 1, zScale);
				temp.name = (i * numCols + j).ToString();
			}
		}
	}
	/// <summary>
	/// Creates the arrows.
	/// </summary>
	void createArrows() {
		int arrRow = 3;
		int arrCol = 2;
		Vector3 newVec = new Vector3(-3.6f, 1.5f, 0);
		//we can actually use for loop
		GameObject fab = Resources.Load("Prefabs/pointer") as GameObject;
		Quaternion rot = Quaternion.identity;
		rot.eulerAngles = new Vector3(90, 180, 0);
		GameObject arr;
		for (int i = 0; i < arrRow; ++i) {
			for (int j = 0; j < arrCol; ++j) {
				arr = GameObject.Instantiate(fab, new Vector3(newVec.x + i * 5, newVec.y - 2.6f * j, newVec.z), rot) as GameObject;
				arr.name = "arrow(Copy)";
			}
		}
	}
}
