using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SamLightScript : MonoBehaviour
{
    private GameObject sam;
    private Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        sam = GameObject.Find("Sam");
        offset = new Vector3(0f, 3f, -10f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = sam.transform.position + offset;
    }
}
