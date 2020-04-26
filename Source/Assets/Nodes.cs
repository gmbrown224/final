using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Nodes : MonoBehaviour {

	public IDictionary<Vector3, bool> walkablePositions = new Dictionary<Vector3, bool>();
	public IDictionary<Vector3, GameObject> nodeReference = new Dictionary<Vector3, GameObject>();
	public List<Vector3> obstacles = new List<Vector3>();
	public Player p;
	
	public int boardWidth;
	public int boardHeight;

	// Use this for initialization
	void Start () {
		// Set up grid of nodes with barriers
		InitializeGrid (20);
		// Call Dijkstra's function to find shortest path
		StartCoroutine(p.DisplayShortestPath());
	}

	public void InitializeGrid(int numBarriers){
		
		walkablePositions.Clear();
		nodeReference.Clear();
		obstacles.Clear();
		
		// Clone 'Node' and 'Obstacle' objects
		var node = GameObject.Find ("Node");
		var obstacle = GameObject.Find ("Obstacle");

		// Creates a random vector of obstacles represented by their positions
		obstacles = GenerateObstacles (numBarriers);

		// Iterate through each tile of the grid and create an obstacle object
		// if the position is in the vector, otherwise create a normal node
		// object.
		for (int i = 0; i < boardWidth; i++) {
			for (int j = 0; j < boardHeight; j++) {
				Vector3 newPosition = new Vector3 (i, 0, j);
				GameObject copy;

				if (obstacles.Contains(newPosition)) {
					copy = Instantiate(obstacle);
					copy.transform.position = newPosition;
					walkablePositions.Add (new KeyValuePair<Vector3, bool> (newPosition, false));
				}
				else {
					copy = Instantiate(node);
					copy.transform.position = newPosition;
					walkablePositions.Add (new KeyValuePair<Vector3, bool> (newPosition, true));
				}
				
				// Set sprite of goal tile
				if (i == boardHeight - 1 && j == boardWidth - 1)
					copy.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("goal");
				
				// Add to map of nodes and their positions
				nodeReference.Add(newPosition, copy);
			}
		}
        
		// Set goal tile
        GameObject goal = GameObject.Find("Goal");
        walkablePositions[goal.transform.localPosition] = true;
        nodeReference[goal.transform.localPosition] = goal;
    }

	List<Vector3> GenerateObstacles(int numBarriers){

		// Generate 'numBarriers' random positions and add them to the vector
		while (obstacles.Count < numBarriers) {
			Vector3 nodePosition = new Vector3(Random.Range(0, boardWidth), 0, Random.Range(0, boardHeight));
			// Ignores duplicates and obstacles directly next to the start/end positions
			if(!obstacles.Contains(nodePosition) && !(nodePosition.x < 2 && nodePosition.z < 2) && !(nodePosition.x > (boardWidth - 3) && nodePosition.z > (boardHeight - 3))){
				obstacles.Add(nodePosition);
			}
		}

		return obstacles;
	}
}
