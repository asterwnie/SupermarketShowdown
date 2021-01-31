using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.momAIState == AIStates.SHOPPING || GameManager.instance.momAIState == AIStates.THINKING)
        {
            AkSoundEngine.SetState("Mother_Aggro", "False");
        }

        if (GameManager.instance.momAIState == AIStates.PATROL)
        {
            AkSoundEngine.SetState("Mother_Aggro", "Level_1");
        }

        if (GameManager.instance.momAIState == AIStates.PURSUE)
        {
            AkSoundEngine.SetState("Mother_Aggro", "Level_2");
        }

        else
        {
            AkSoundEngine.SetState("Mother_Aggro", "False");
        }
    }
}
