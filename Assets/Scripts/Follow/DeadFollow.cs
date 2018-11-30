using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadFollow : MonoBehaviour {
    [SerializeField]
    Transform target;

    private void OnEnable()
    {
        target = GameObject.FindWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {

        transform.position = new Vector3(transform.position.x, transform.position.y, target.position.z);
    }
}
