using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BulletsLeftText : MonoBehaviour
{
    public TextMeshPro bulletsLeftText;
    // Start is called before the first frame update
    void Start()
    {
        UpdateBulletsLeft(80);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void UpdateBulletsLeft(int BulletsLeft)
    {
        bulletsLeftText.text = "Shoot: " + BulletsLeft.ToString() + " Bullets Left";
    }
}
