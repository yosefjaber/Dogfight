using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwitcher : MonoBehaviour
{
    public Animation animation;
    public AnimationClip draw;
    private int selectedWeapon = 0;

    // Start is called before the first frame update
    void Start()
    {
        SelectWeapon();
    }

    // Update is called once per frame
    void Update()
    {
        CheckInputs();
    }

    void SelectWeapon()
    {
        if(selectedWeapon >= transform.childCount)
        {
            selectedWeapon = transform.childCount - 1;
        }

        animation.Stop();
        animation.Play(draw.name);

        int i = 0;

        foreach(Transform weapon in transform)
        {
            if(i == selectedWeapon)
            {
                weapon.gameObject.SetActive(true);
            }
            else
            {
                weapon.gameObject.SetActive(false);
            }

            i++;
        }
    }

    void CheckInputs()
    {
        int previousSelectedWeapon = selectedWeapon;

        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectedWeapon = 0;
        }
        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            selectedWeapon = 1;
        }
        if(Input.GetKeyDown(KeyCode.Alpha3))
        {
            selectedWeapon = 2;
        }
        if(Input.GetKeyDown(KeyCode.Alpha4))
        {
            selectedWeapon = 3;
        }
        if(Input.GetKeyDown(KeyCode.Alpha5))
        {
            selectedWeapon = 4;
        }
        if(Input.GetKeyDown(KeyCode.Alpha6))
        {
            selectedWeapon = 5;
        }
        if(Input.GetKeyDown(KeyCode.Alpha7))
        {
            selectedWeapon = 6;
        }
        if(Input.GetKeyDown(KeyCode.Alpha8))
        {
            selectedWeapon = 7;
        }
        if(Input.GetKeyDown(KeyCode.Alpha9))
        {
            selectedWeapon = 8;
        }

        if(Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            if(selectedWeapon >= transform.childCount - 1)
            {
                selectedWeapon = 0;
            }
            else
            {
                selectedWeapon++;
            }
        }

        if(Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            if(selectedWeapon <= 0)
            {
                selectedWeapon = transform.childCount - 1;
            }
            else
            {
                selectedWeapon--;
            }
        }

        if(previousSelectedWeapon != selectedWeapon)
        {
            SelectWeapon();
        }
    }
}
