using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitPlaneLogic : MonoBehaviour
{
    public GameObject Plane;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ExitPlane(GameObject player)
    {
        Vector3 offset = new Vector3(0,4,0);
        player.transform.position = Plane.transform.position + offset;
    }
}
