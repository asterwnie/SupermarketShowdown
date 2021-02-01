using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingText : MonoBehaviour
{
    Text text;
    float timeElapsed = 0f;
    int numPeriods = 0;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        timeElapsed += Time.deltaTime;
        if(timeElapsed > 0.5f)
        {
            timeElapsed = 0f;
            numPeriods++;
            text.text = "Loading";
            for (int i = 0; i < numPeriods; i++)
                text.text += ".";

            if (numPeriods == 3)
                numPeriods = 0;
        }
            
    }
}
