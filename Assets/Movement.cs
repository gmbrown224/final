using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		Vector2 v2 = transform.position;
        if (Input.GetKeyUp("w") || Input.GetKeyUp(KeyCode.UpArrow)) {
			if (v2.y < 4) {
				v2.y += 2.0f;
			}
		} else if (Input.GetKeyUp("s") || Input.GetKeyUp(KeyCode.DownArrow)) {
			if (v2.y > -4) {
				v2.y -= 2.0f;
			}
		} else if (Input.GetKeyUp("a") || Input.GetKeyUp(KeyCode.LeftArrow)) {
			if (v2.x > -8) {
				v2.x -= 2.0f;
			}
		} else if (Input.GetKeyUp("d") || Input.GetKeyUp(KeyCode.RightArrow)) {
			if (v2.x < 8) {
				v2.x += 2.0f;
			}
		}
		transform.position = v2;
    }
}
