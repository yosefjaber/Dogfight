using UnityEngine;
using Photon.Pun;

public class NetworkDestroyer : MonoBehaviourPun
{
    public static NetworkDestroyer Instance { get; private set; }

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

    public void RequestDestroy(GameObject obj)
    {
        PhotonView photonView = obj.GetComponent<PhotonView>();
        if (photonView == null)
        {
            Debug.LogError("Object does not have a PhotonView component");
            return;
        }
        
        if (PhotonNetwork.IsMasterClient)
        {
            // Directly destroy if the client is MasterClient
            PhotonNetwork.Destroy(obj);
        }
        else
        {
            // Send RPC to MasterClient to handle destruction
            photonView.RPC("MasterDestroy", RpcTarget.MasterClient, photonView.ViewID);
        }
    }
    
    [PunRPC]
    public void MasterDestroy(int viewID)
    {
        PhotonView view = PhotonView.Find(viewID);
        if (view == null)
        {
            Debug.LogError("Object with viewID " + viewID + " not found. It may have already been destroyed.");
            return;
        }
        
        PhotonNetwork.Destroy(view.gameObject);
    }
}
