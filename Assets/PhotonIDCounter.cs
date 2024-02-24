using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PhotonCounter : MonoBehaviour
{
    private void Start()
    {
        // Start the coroutine in the Start method to begin the periodic checks.
        StartCoroutine(CountPhotonViewsPeriodically());
    }

    private IEnumerator CountPhotonViewsPeriodically()
    {
        // Infinite loop to continue checking indefinitely.
        while (true)
        {
            // Find all active PhotonView components in the scene.
            PhotonView[] allPhotonViews = FindObjectsOfType<PhotonView>();
            int photonViewCount = allPhotonViews.Length;

            // Print the current count of PhotonViews.
            Debug.Log($"Current active PhotonView count: {photonViewCount}");
            Debug.Log("Hello?");

            // Wait for 1 second before the next check.
            yield return new WaitForSeconds(1f);
        }
    }
}
