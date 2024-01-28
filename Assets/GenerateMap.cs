using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun; 

public class GenerateMap : MonoBehaviour
{
    public GameObject[] assets;
    public GameObject test;
    public bool spawnAssets = true;

    [Header("Random Set Assets")]
    public float detailScale = 20f;
    [Range(0f, 1f)]
    public float chanceOfSpawn = 0.5f;
    public float offSet = 5f;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Generating Assets");
        GenerateAssets();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GenerateAssets()
    {
        int numSpawned = 0;

        for(int y = 0; y < detailScale; y++)
        {
            for(int x = 0; x < detailScale; x++)
            {
                if(Random.Range(0f, 1f) < chanceOfSpawn && spawnAssets)
                {
                    float xPos = -1000 + (1 / detailScale) * 2000 * x;
                    float zPos = 1000 - (1 / detailScale) * 2000 * y;
                    float randomRotation = Random.Range(0f, 360f);
                    PhotonNetwork.Instantiate(test.name, new Vector3(xPos + Random.Range(-offSet, offSet), 0, zPos + Random.Range(-offSet, offSet)), Quaternion.Euler(0, randomRotation, 0));
                    //PhotonNetwork.Instantiate(assets[Random.Range(3, assets.Length)].name, new Vector3(xPos + Random.Range(-offSet, offSet), 0, zPos + Random.Range(-offSet, offSet)), Quaternion.Euler(0, randomRotation, 0));
                    numSpawned++;
                }
            }
        }

        Debug.Log("Spawned " + numSpawned + " assets");
    }
}















// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using Photon.Pun; // Add Photon namespace

// public class GenerateMap : MonoBehaviourPunCallbacks // Inherit from MonoBehaviourPunCallbacks
// {
//     public GameObject[] assets;
//     public GameObject floor;
//     public bool spawnAssets = true;

//     [Header("Random Set Assets")]
//     public float detailScale = 20f;
//     [Range(0f, 1f)]
//     public float chanceOfSpawn = 0.5f;
//     public float offSet = 5f;

//     private int randomSeed; // Seed for random number generation

//     void Start()
//     {
//         if (PhotonNetwork.IsMasterClient) // Check if this is the master client
//         {
//             randomSeed = GenerateSeed(); // Generate or retrieve a common seed
//             PhotonView photonView = PhotonView.Get(this);
//             photonView.RPC("GenerateAssets", RpcTarget.AllBuffered, randomSeed); // Call RPC on all clients
//         }
//     }

//     [PunRPC]
//     void GenerateAssets(int seed)
//     {
//         Random.InitState(seed); // Initialize the random state with the seed

//         for (int y = 0; y < detailScale; y++)
//         {
//             for (int x = 0; x < detailScale; x++)
//             {
//                 if (Random.Range(0f, 1f) < chanceOfSpawn && spawnAssets)
//                 {
//                     float xPos = -1000 + (1 / detailScale) * 2000 * x;
//                     float zPos = 1000 - (1 / detailScale) * 2000 * y;
//                     float randomRotation = Random.Range(0f, 360f);

//                     // Instantiate assets using PhotonNetwork
//                     PhotonNetwork.Instantiate(assets[Random.Range(3, assets.Length)].name, new Vector3(xPos + Random.Range(-offSet, offSet), 0, zPos + Random.Range(-offSet, offSet)), Quaternion.Euler(0, randomRotation, 0));
//                 }
//             }
//         }
//     }

//     int GenerateSeed()
//     {
//         // Implement logic to generate or retrieve a consistent seed
//         // This can be a hardcoded value or a value synchronized across clients
//         return 12345; // Example seed
//     }

//     // Update is not needed if only used for the generation process
// }

