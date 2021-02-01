using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Author:      Wesley Elmer
// Description: Provides a scene transition for buttons
public class ChangeScene : MonoBehaviour
{
    public string targetScene = "SampleScene";
    
    public void Load()
    {
        SceneManager.LoadScene(targetScene);
    }

    public void LoadAfterDelay()
    {
        StartCoroutine(Delay());
    }

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(targetScene);
    }
}
