using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftGearLogic : MonoBehaviour
{
    public GameObject leftGear;
    private Animation animation;
    public AnimationClip LeftGearFoldIn;
    public AnimationClip LeftGearFoldOut;
    public float landingLevel = 35f;
    private float lastFrameY;

    // Start is called before the first frame update
    void Start()
    {
        animation = leftGear.GetComponent<Animation>();
        lastFrameY = leftGear.transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        float currentY = leftGear.transform.position.y;

        if(lastFrameY <= landingLevel && currentY > landingLevel)
        {
            //landing
            animation.Play(LeftGearFoldIn.name);
        }
        else if(lastFrameY >= landingLevel && currentY < landingLevel)
        {
            //takeoff
            animation.Play(LeftGearFoldOut.name);
        }

        lastFrameY = currentY;
    }
}
