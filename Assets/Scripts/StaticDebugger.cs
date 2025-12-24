using UnityEngine;

public class StaticDebugger : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            Debug.Log("Hello");
            NetworkDestroyer.Instance.RequestDisable(this.gameObject);
        }
    }
}
