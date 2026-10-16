using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    static public GameObject POI;
    [Header("Dynamic")]
    public float camZ;
    void Awake()
    {
        camZ = this.transform.position.z;
    }
    // Start is called before the first frame update
    void FixedUpdate()
    {
        if (POI == null) return;
        Vector3 destination = POI.transform.position;
        destination.z = camZ;
        transform.position = destination;
    }
}
