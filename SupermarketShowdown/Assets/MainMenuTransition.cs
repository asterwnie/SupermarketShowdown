using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuTransition : MonoBehaviour
{
    public ScreenTransitions screenTransition;

    // Start is called before the first frame update
    void Start()
    {
        screenTransition.ShowBlack();
        screenTransition.FadeFromBlack();
        StartCoroutine(AttemptAfterDelay());
    }

    IEnumerator AttemptAfterDelay()
    {
        yield return new WaitForSeconds(0.01f);
        screenTransition.ShowBlack();
        screenTransition.FadeFromBlack();
    }
}
