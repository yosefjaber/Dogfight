using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightGearLogic : MonoBehaviour
{
    public GameObject rightGear;
    private Animation animation;
    public AnimationClip RightGearFoldIn;
    public AnimationClip RightGearFoldOut;
    public float landingLevel = 35f;
    private float lastFrameY;

    // Start is called before the first frame update
    void Start()
    {
        animation = rightGear.GetComponent<Animation>();
        lastFrameY = rightGear.transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        float currentY = rightGear.transform.position.y;

        if(lastFrameY <= landingLevel && currentY > landingLevel)
        {
            //landing
            animation.Play(RightGearFoldIn.name);
        }
        else if(lastFrameY >= landingLevel && currentY < landingLevel)
        {
            //takeoff
            animation.Play(RightGearFoldOut.name);
        }

        lastFrameY = currentY;
    }
}
