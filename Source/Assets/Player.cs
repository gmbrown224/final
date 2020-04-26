using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using Priority_Queue;

public class Player : MonoBehaviour {

	IDictionary<Vector3, Vector3> nodeParents = new Dictionary<Vector3, Vector3>();
	public IDictionary<Vector3, bool> walkablePositions;
	IDictionary<Vector3, Sprite> prevSprite = new Dictionary<Vector3, Sprite>();
	public Nodes n;
	Nodes grid;
	IList<Vector3> path = new List<Vector3>();
	bool move;
    Camera camera;
	int i;

	// Use this for initialization
	void Start()
    {
		move = false;
        camera = FindObjectOfType<Camera>();
        grid = GameObject.Find("Grid").GetComponent<Nodes>();
		walkablePositions = grid.walkablePositions;
    }
	
	// Update is called once per frame
	void Update() {

		// Set camera view position
        camera.transform.position = new Vector3(grid.boardWidth/2, 2, grid.boardHeight/2);
		if (move) {
			// Check if we have reached the goal (i < 0)
			if (i >= 0) {
				if (Input.GetKeyUp("w") || Input.GetKeyUp(KeyCode.UpArrow)) {
					// Check if the player can move up a tile
					if (transform.position.z < grid.boardHeight - 1) {
						// Check if the tile above is in the path
						if (transform.position.z + 1 == path[i].z ) {
							// Move to the tile above and iterate through to
							// the next tile in the path
							transform.position = Vector3.MoveTowards (transform.position, path[i], 1);
							if (transform.position.Equals (path [i]) && i >= 0) i--;
						// Check if the tile above is the previous tile in the path
						} else if ((i + 2) < path.Count - 1 && transform.position.z + 1 == path[i+2].z) {
							// Move to the tile above and iterate back through the path
							transform.position = Vector3.MoveTowards (transform.position, path[i+2], 1);
							if (transform.position.Equals (path [i+2]) && i >= 0) i++;
						} else
							// If the tile is not in the path at all, send the player back to the start
							StartCoroutine(CustomWait(1, transform.position.x, transform.position.y, transform.position.z + 1));
					}
				} else if (Input.GetKeyUp("s") || Input.GetKeyUp(KeyCode.DownArrow)) {
					// Check if the player can move down a tile
					if (transform.position.z > 0) {
						if (transform.position.z - 1 == path[i].z ) {
							transform.position = Vector3.MoveTowards (transform.position, path[i], 1);
							if (transform.position.Equals (path [i]) && i >= 0) i--;
						} else if ((i + 2) < path.Count - 1 && transform.position.z - 1 == path[i+2].z) {
							transform.position = Vector3.MoveTowards (transform.position, path[i+2], 1);
							if (transform.position.Equals (path [i+2]) && i >= 0) i++;
						} else
							StartCoroutine(CustomWait(1, transform.position.x, transform.position.y, transform.position.z - 1));
					}
				} else if (Input.GetKeyUp("a") || Input.GetKeyUp(KeyCode.LeftArrow)) {
					// Check if the player can move left a tile
					if (transform.position.x > 0) {
						if (transform.position.x - 1 == path[i].x ) {
							transform.position = Vector3.MoveTowards (transform.position, path[i], 1);
							if (transform.position.Equals (path [i]) && i >= 0)
								i--;
						} else if ((i + 2) < path.Count - 1 && transform.position.x - 1 == path[i+2].x) {
							transform.position = Vector3.MoveTowards (transform.position, path[i+2], 1);
							if (transform.position.Equals (path [i+2]) && i >= 0) i++;
						} else
							StartCoroutine(CustomWait(1, transform.position.x - 1, transform.position.y, transform.position.z));
					}
				} else if (Input.GetKeyUp("d") || Input.GetKeyUp(KeyCode.RightArrow)) {
					// Check if the player can move right a tile
					if (transform.position.x < grid.boardWidth - 1) {
						if (transform.position.x + 1 == path[i].x ) {
							transform.position = Vector3.MoveTowards (transform.position, path[i], 1);
							if (transform.position.Equals (path [i]) && i >= 0) i--;
						} else if ((i + 2) < path.Count - 1 && transform.position.x + 1 == path[i+2].x) {
							transform.position = Vector3.MoveTowards (transform.position, path[i+2], 1);
							if (transform.position.Equals (path [i+2]) && i >= 0) i++;
						} else
							StartCoroutine(CustomWait(1, transform.position.x + 1, transform.position.y, transform.position.z));
					}
				}
			} else {
				// If we have reached the goal, display the end screen
				SceneManager.LoadScene(3);
			}
		}
	}
	
	Vector3 FindPath(Vector3 startPosition, Vector3 goalPosition){

        // Contains the shortest distance so far from the start to a given node
        IPriorityQueue<Vector3, int> priority = new SimplePriorityQueue<Vector3, int>();

        // A list of all nodes that are walkable
        IDictionary<Vector3, int> distances = walkablePositions
            .Where(x => x.Value == true)
            .ToDictionary(x => x.Key, x => int.MaxValue);
	
		// Start the algorithm by enqueueing startPosition
        distances[startPosition] = 0;
        priority.Enqueue(startPosition, 0);

        while (priority.Count > 0) {

            Vector3 curr = priority.Dequeue();

			// If the goal has been reached, return the path so far
            if (curr == goalPosition)
                return goalPosition;

			// If not, look at all adjacent nodes
			IList<Vector3> nodes = GetWalkableNodes (curr);

			foreach (Vector3 node in nodes) {

                int dist = distances[curr] + 1;

                // If dist is less than the node's current distance,
				// update the distance and the path
                if (dist < distances [node]) {
					distances[node] = dist;
					nodeParents[node] = curr;

					// Add each adjacent node to the queue to be processed
                    if (!priority.Contains(node))
                        priority.Enqueue(node, dist);
                }
			}
		}

        return startPosition;
	}

	public IEnumerator DisplayShortestPath() {
		
		Vector3 goal;
        goal = FindPath(this.transform.localPosition, GameObject.Find("Goal").transform.localPosition);

		// If a path cannot be found, reinitialize the grid with new obstacles and try to find a new path
		while (goal == this.transform.localPosition || !nodeParents.ContainsKey(nodeParents[goal])) {
			n.InitializeGrid(20);
			goal = FindPath(this.transform.localPosition, GameObject.Find("Goal").transform.localPosition);
		}

		// Traverse the path through edges, adding each node in the path to the vector 'path'
		Vector3 curr = goal;
		while (curr != this.transform.localPosition) {
			path.Add (curr);
			curr = nodeParents [curr];
		}

		// Easy mode:
		// Show the path by changing the sprite for each node in the path.
		// Wait 3 seconds
		// Reset sprites back to their original state
		if (Menu.mode == 0) {
			foreach (Vector3 node in path) {
				prevSprite[node] = grid.nodeReference[node].GetComponent<SpriteRenderer>().sprite;
				grid.nodeReference[node].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("path");
			}
			yield return new WaitForSeconds(3);
			foreach (Vector3 node in path)
				grid.nodeReference [node].GetComponent<SpriteRenderer> ().sprite = prevSprite[node];
		}

		i = path.Count - 1;
		// Allow the player to start moving
		move = true;

	}
	
	public IEnumerator CustomWait(int numseconds, float x, float y, float z) {
		Vector3 current = new Vector3(x, y, z);
		Vector3 start = new Vector3(0, 0, 0);
		if (current != start) {
			// Change sprite to show the player made a wrong move
			// Set sprite of that tile to a landmine
			Sprite prev = this.GetComponent<SpriteRenderer> ().sprite;
			this.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("fail");
			grid.nodeReference[current].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("landmine");
			// Block movement during the wait
			move = false;
			yield return new WaitForSeconds(numseconds);
			move = true;
			this.GetComponent<SpriteRenderer>().sprite = prev;
		}
		// Reset player to start position
		Vector3 v = new Vector3(0, 0, 0);
		transform.position = v;
		i = path.Count - 1;
	}
		
	IList<Vector3> GetWalkableNodes(Vector3 curr) {

		IList<Vector3> walkableNodes = new List<Vector3> ();

		// Vector with all adjacent node positions
		IList<Vector3> possibleNodes = new List<Vector3> () {
			new Vector3 (curr.x + 1, curr.y, curr.z),
			new Vector3 (curr.x - 1, curr.y, curr.z),
			new Vector3 (curr.x, curr.y, curr.z + 1),
			new Vector3 (curr.x, curr.y, curr.z - 1)
        };

		// Check each adjacent node to see if it's walkable
		foreach (Vector3 node in possibleNodes)
			if (walkablePositions.ContainsKey(node) && walkablePositions[node])
				walkableNodes.Add(node);

		return walkableNodes;
	}
}
