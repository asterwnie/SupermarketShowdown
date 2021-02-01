using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenTransitions : MonoBehaviour
{
    public GameObject loadingText;
    Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        loadingText.SetActive(false);
    }


    public void ShowBlack()
    {
        loadingText.SetActive(true);
        animator.SetTrigger("Black");
    }

    public void ShowWhite()
    {
        loadingText.SetActive(false);
        animator.SetTrigger("White");
    }

    public void FadeToBlack()
    {
        animator.SetTrigger("FadeToBlack");
        StartCoroutine(DisplayLoadingText());
    }

    IEnumerator DisplayLoadingText()
    {
        yield return new WaitForSeconds(0.5f);
        loadingText.SetActive(true);
    }

    public void FadeFromBlack()
    {
        loadingText.SetActive(false);
        animator.SetTrigger("FadeFromBlack");
    }
}
