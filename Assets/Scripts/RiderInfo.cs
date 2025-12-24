using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;

public class RiderInfo : MonoBehaviour
{
    public List<GameObject> riders = new List<GameObject>();
    public GameObject Driver; // Null by default

    public GameObject Gunner;

    public PhotonView photonView;

    [PunRPC]
    public void SetDriver(int driverViewID)
    {
        PhotonView driverView = PhotonView.Find(driverViewID);
        if (driverView == null)
        {
            Debug.LogError("Driver View is null");
            return;
        }
        Driver = driverView.gameObject;
    }

    public bool IsDriver()
    {
        return !(Driver == null);
    }

    [PunRPC]
    public void PopDriver()
    {
        if (!Driver) // Driver is null (There is no driver)
        {
            Debug.LogError("There is no Driver");
            return;
        }
        Driver = null;
    }

    [PunRPC]
    public void SetGunner(int gunnerViewID)
    {
        PhotonView gunnerView = PhotonView.Find(gunnerViewID);
        if (gunnerView == null)
        {
            Debug.LogError("Gunner View is null");
            return;
        }
        Gunner = gunnerView.gameObject;
    }

    public bool IsGunner()
    {
        return !(Gunner == null);
    }

    [PunRPC]
    public void PopGunner()
    {
        if (!Gunner) // Gunner is null (There is no Gunner)
        {
            return;
        }
        Gunner = null;
    }

    [PunRPC]
    public void AddRider(int riderViewID)
    {
        PhotonView riderView = PhotonView.Find(riderViewID);
        if (riderView == null)
        {
            Debug.LogError("Rider is null");
            return;
        }
        riders.Add(riderView.gameObject);
    }
}
