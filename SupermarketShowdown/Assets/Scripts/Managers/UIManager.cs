using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public ScreenTransitions screenTransition;
    bool hasTransitioned = false;

    public Text momStateText;
    public Text itemsCollectedText;

    public Movement player;
    public PlayerActionManager playerActionManager;

    public GameObject endScreenPanel;
    public Text endResultText;

    public GameObject pauseMenuPanel;
    bool canGiveControlToPlayer = false;

    private void Start()
    {
        endScreenPanel.SetActive(false);
        pauseMenuPanel.SetActive(false);
        screenTransition.ShowBlack();
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasTransitioned && GameManager.instance.isFinishedInit)
        {
            hasTransitioned = true;
            screenTransition.FadeFromBlack();
        }

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

        if(pauseMenuPanel.activeInHierarchy || endScreenPanel.activeInHierarchy)
        {
            player.canMove = false;
            playerActionManager.canPerformActions = false;
            canGiveControlToPlayer = true;
        }
        else if (!endScreenPanel.activeInHierarchy && canGiveControlToPlayer)
        {
            canGiveControlToPlayer = false;
            player.canMove = true;
            playerActionManager.canPerformActions = true;
        }

    }

    public void TogglePauseScreen()
    {
        pauseMenuPanel.SetActive(!pauseMenuPanel.activeInHierarchy);
    }
}
