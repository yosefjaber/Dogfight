using UnityEngine;
using System;             
using System.Collections; 

public class ActionScheduler : MonoBehaviour
{
    public static ActionScheduler Instance { get; private set; }

    private void Awake()
    {
        // Set up the singleton instance
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        // Make persistent across scenes (optional)
        DontDestroyOnLoad(gameObject);
    }

    public void InvokeAction(Action action, float delay)
    {
        StartCoroutine(InvokeCoroutine(action, delay));
    }

    private IEnumerator InvokeCoroutine(Action action, float delay)
    {
        yield return new WaitForSeconds(delay);
        // Null Guard
        action?.Invoke();
    }
}
