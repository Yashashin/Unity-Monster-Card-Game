using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Loading : MonoBehaviour
{
    void Start()
    {
        Invoke("SwitchScene", 5);
    }

    public void SwitchScene()
    {
        SceneManager.LoadScene(2); //go to lobby scene
    }

}
