using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FadePanel : MonoBehaviour
{
    [SerializeField] private GameObject panelToFade;
    [SerializeField] private GameObject fadePanelIn;
    public float fadeDuration = 1f;
    [SerializeField] private float fadeInDuration = 3.5f;
    // private CanvasGroup canvasGroup;
    [SerializeField] private CameraSwitcher cameraSwitcher;
    private bool isPastTitleScreen;
    [SerializeField] private GameObject _mainMenuFirstButton;
    
    private void Start()
    {
        // canvasGroup = panelToFade.GetComponent<CanvasGroup>();
        isPastTitleScreen = false;
    }

    private void Update()
    {
        if (Input.anyKeyDown && !isPastTitleScreen)
        {
            StartFadeOut(panelToFade);
            StartFadeIn(fadePanelIn);
        }
    }

    public void StartFadeOut(GameObject panelToFade)
    {
        StartCoroutine(FadeOutCoroutine(panelToFade));
    }

    // private IEnumerator FadeOutCoroutine()
    // {
    //     float startAlpha = canvasGroup.alpha;
    //     float timer = 0f;
    //
    //     while (timer < fadeDuration)
    //     {
    //         timer += Time.deltaTime;
    //         canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, timer / fadeDuration);
    //         yield return null;
    //     }
    //
    //     panelToFade.SetActive(false);
    //     canvasGroup.alpha = startAlpha;
    //     SwitchFromTitleScreen();
    // }

    private IEnumerator FadeOutCoroutine(GameObject panelToDisable)
    {
        CanvasGroup canvasGroup = panelToDisable.GetComponent<CanvasGroup>();
        float startAlpha = canvasGroup.alpha;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, timer / fadeDuration);
            yield return null;
        }

        panelToDisable.SetActive(false);
        canvasGroup.alpha = startAlpha;
        if (!isPastTitleScreen)
        {
            SwitchFromTitleScreen();
            isPastTitleScreen = true;
        }
    }

    public void StartFadeIn(GameObject panelToFade)
    {
        StartCoroutine(FadeInCoroutine(panelToFade));
    }

    private IEnumerator FadeInCoroutine(GameObject panelToEnable)
    {
        panelToEnable.SetActive(true);
        CanvasGroup canvasGroup2 = panelToEnable.GetComponent<CanvasGroup>();
        canvasGroup2.alpha = 0f;
        float timer = 0f;

        while (timer < fadeInDuration)
        {
            timer += Time.deltaTime;
            canvasGroup2.alpha = Mathf.Lerp(0f, 1f, timer / fadeInDuration);
            yield return null;
        }
    }
    
    // switch cameras
    private void SwitchFromTitleScreen()
    {
        cameraSwitcher.SwitchFromStartingCamera();
        EventSystem.current.SetSelectedGameObject(_mainMenuFirstButton);
    }
}