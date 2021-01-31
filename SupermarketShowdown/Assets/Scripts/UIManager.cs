using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text momStateText;
    public Text itemsCollectedText;

    public GameObject endScreenPanel;
    public Text endResultText;


    private void Start()
    {
        endScreenPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        string momStateString = "";
        switch(GameManager.instance.momAIState)
        {
            case AIStates.SHOPPING: momStateString = "Shopping"; break;
            case AIStates.THINKING: momStateString = "Thinking..."; break;
            case AIStates.PURSUE: momStateString = "Pursue Player"; break;
            case AIStates.PATROL: momStateString = "Patrol Supermarket"; break;
        }
        momStateText.text = momStateString;
        itemsCollectedText.text = GameManager.instance.collectedItems.Count + "/" + GameManager.instance.playerObjectives.Count;


        if(GameManager.instance.isInEndState)
        {
            endScreenPanel.SetActive(true);
            if (GameManager.instance.hasWon)
                endResultText.text = "You won!!";
            else
                endResultText.text = "You lost...";
        }
    }
}
