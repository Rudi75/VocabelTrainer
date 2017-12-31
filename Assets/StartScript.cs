using Assets;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScript : MonoBehaviour {


    // Use this for initialization
    void Start () {
        VocabelDb.instance.fill();
   
    }
	

    public void openManager()
    {
        SceneManager.LoadScene("VocabelOverview");
    }

    public void openTrainer()
    {
        SceneManager.LoadScene("VocabelTraining");
    }

    public void exit()
    {
        Application.Quit();
    }
}
