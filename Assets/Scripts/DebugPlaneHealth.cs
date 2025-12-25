using UnityEngine;

public class DebugPlaneHealth : MonoBehaviour
{
    public Health health;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(health.health);
    }
}
