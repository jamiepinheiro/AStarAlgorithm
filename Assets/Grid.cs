using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid : MonoBehaviour {

	static int size = 50;
	public GameObject cube;
	public int startX, startY;
	public int endX, endY;
	GameObject[,] grid = new GameObject[size, size];
	bool pathFinding = false;
	List<GameObject> openList = new List<GameObject>();
	List<GameObject> closedList = new List<GameObject>();
	int x, y;
	GameObject currentNode;
	int[,] around = new int[8, 2]{{0, -1}, {1, -1}, {1, 0}, {1, 1}, {0, 1}, {-1, 1}, {-1, 0}, {-1, -1}};

	// Use this for initialization
	void Start () {

		for(int x = 0; x < size; x++){
			for(int y = 0; y < size; y++){
				grid[y, x] = (GameObject) Instantiate(cube, new Vector3(x, y, 0), transform.rotation);
				grid[y, x].GetComponent<Renderer>().material.color = Color.black;
				grid[y, x].gameObject.tag = "Wall";
				if(x == startX && y == startY){
					grid[y, x].tag = "Start";
					grid[y, x].GetComponent<Renderer>().material.color = Color.blue;
				}else if(x == endX && y == endY){
					grid[y, x].tag = "End";
					grid[y, x].GetComponent<Renderer>().material.color = Color.cyan;
				}
			}
		}
	
	}
	
	// Update is called once per frame
	void Update () {
		
		if(Input.GetMouseButton(0)){
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if(Physics.Raycast(ray, out hit) && hit.collider.tag != "Start" && hit.collider.tag != "End"){
					hit.collider.gameObject.GetComponent<Renderer>().material.color = Color.black;
					hit.collider.gameObject.tag = "Wall";
			}
		}

		if(Input.GetMouseButton(1)){
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if(Physics.Raycast(ray, out hit) && hit.collider.tag != "Start" && hit.collider.tag != "End"){
				hit.collider.gameObject.GetComponent<Renderer>().material.color = Color.white;
				hit.collider.gameObject.tag = "Node";
			}
		}

		if(pathFinding){
			if(currentNode != grid[endY, endX]){

				foreach(GameObject g in openList){
					g.GetComponent<Renderer>().material.color = Color.green;
				}
				foreach(GameObject g in closedList){
					g.GetComponent<Renderer>().material.color = Color.red;
				}

				if(currentNode != null){
					currentNode.GetComponent<Renderer>().material.color = Color.yellow;
				}

				for(int i = 0; i < around.Length/2; i++){
					int newY = y + around[i, 0];
					int newX = x + around[i, 1];


					if(newX >= 0 && newX < size && newY >= 0 && newY < size){

						if(grid[newY, newX].tag != "Wall" && grid[newY, newX].tag != "Start" && grid[newY, newX].tag != "Closed"){

							if(!openList.Contains(grid[newY, newX])){

								grid[newY, newX].GetComponent<Node>().g = GetCost(newX, newY, startX, startY);
								grid[newY, newX].GetComponent<Node>().h = GetCost(newX, newY, endX, endY);
								grid[newY, newX].GetComponent<Node>().f = grid[newY, newX].GetComponent<Node>().h + grid[newY, newX].GetComponent<Node>().g;
								grid[newY, newX].GetComponent<Node>().parent = currentNode;

								openList.Add(grid[newY, newX]);
							}
						}
					}
				}
					
				openList.Remove(currentNode);
				closedList.Add(currentNode);
				currentNode.tag = "Closed";

				List<GameObject> copy = openList;
				openList = OrderList(openList);

				currentNode = openList.ToArray()[0];
				x = (int) currentNode.transform.position.x;
				y = (int) currentNode.transform.position.y;

			}else{
				closedList.Clear();
				openList.Clear();

				GameObject trail = currentNode;

				while(trail != grid[startY, startX] && trail != null){
					trail.GetComponent<Renderer>().material.color = Color.blue;
					trail = trail.GetComponent<Node>().parent;
				}
				pathFinding = false;
			}
		}

		if(Input.GetKeyDown(KeyCode.Space)){

			y = startY;
			x = startX;

			for(int i = 0; i < around.Length/2; i++){
				
				int newY = y + around[i, 0];
				int newX = x + around[i, 1];
				if(newX >= 0 && newX < size && newY >= 0 && newY < size){
					
					if(grid[newY, newX].tag != "Wall"){

						grid[newY, newX].GetComponent<Node>().g = GetCost(newX, newY, startX, startY);
						grid[newY, newX].GetComponent<Node>().h = GetCost(newX, newY, endX, endY);
						grid[newY, newX].GetComponent<Node>().f = grid[newY, newX].GetComponent<Node>().h + grid[newY, newX].GetComponent<Node>().g;
						grid[newY, newX].GetComponent<Node>().parent = currentNode;
						openList.Add(grid[newY, newX]);

					}

				}

			}
				
			List<GameObject> copy = openList;
			openList = OrderList(openList);

			currentNode = openList.ToArray()[0];
			x = (int) currentNode.transform.position.x;
			y = (int) currentNode.transform.position.y;

			/*
			foreach(GameObject g in openList){
				Debug.Log(g.transform.position.x + " " + g.transform.position.y + "   " + g.GetComponent<Node>().f);
			}
			*/

			pathFinding = true;

			
		}

	}

	int GetCost(int x1, int y1, int x2, int y2){

		int xDifference = Mathf.Abs(x1 - x2);
		int yDifference = Mathf.Abs(y1 - y2);

		int cost = 0;

		while(xDifference != 0 && yDifference != 0){
			xDifference--;
			yDifference--;
			cost += 14;
		}

		if(yDifference == 0){
			cost += xDifference*10;
		}else if(xDifference == 0){
			cost += yDifference*10;
		}

		return cost;

	}

	List<GameObject> OrderList(List<GameObject> list){

		if (list.Capacity > 0) {
			list.Sort(delegate(GameObject a, GameObject b) {
				return (a.GetComponent<Node>().f).CompareTo(b.GetComponent<Node>().f);
			});
		}

		return list;

	}

}
