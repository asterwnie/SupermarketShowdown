using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    /*
    public static MusicManager instance;

    // singleton setup
    private void Awake()
    {
        if (instance != null) // if an instance already exists, destroy this (and use the other one instead)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }*/

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

        if (GameManager.instance.momAIState == AIStates.PURSUE && MomAIStateMachine.isReachedCartForFirstTime)
        {
            AkSoundEngine.SetState("Mother_Aggro", "Level_2");
        }
    }
}
