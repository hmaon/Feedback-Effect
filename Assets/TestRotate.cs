using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRotate : MonoBehaviour
{
	// Update is called once per frame
	void Update ()
    {
        transform.Rotate(0, 90 * Time.deltaTime, 0);
	}
}
