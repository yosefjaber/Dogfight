using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void Button_StartGame(){
        Debug.Log("Start Button Pressed");

        SceneManager.LoadScene(1);

    }

    public void Button_Return(){
        Debug.Log("Return Button Pressed");

        Application.Quit();
    }

}
