using System;
using System.Collections;
using System.Collections.Generic;
using General.Player;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class DisplayTooltips : MonoBehaviour
{
    private BoxCollider trigger;

    // [SerializeField] private GameObject displayTooltip;
    [SerializeField] private TextMeshProUGUI tooltipTextSprint;
    [SerializeField] private TextMeshProUGUI tooltipTextSprintGamepad;
    [SerializeField] private TextMeshProUGUI tooltipTextHint;
    [SerializeField] private TextMeshProUGUI tooltipTextHintGamepad;
    [SerializeField] private TextMeshProUGUI tooltipTextHint2;
    // [SerializeField] private TextMeshProUGUI tooltipTextHintGamepad2;
    [SerializeField] private PlayerController player;
    [SerializeField] private float timeToDisplayTooltip = 3.5f;
    
    [SerializeField] private float fadeDuration = 1f;

    private bool tooltipsDisplayed = false;

    private InputSystem_Actions isa;
    private InputAction hintAction;
    
    private void Awake()
    {
        trigger = GetComponent<BoxCollider>();
        
    }

    private void OnEnable()
    {
        isa = new InputSystem_Actions();
        hintAction = isa.Player.Hint;
        hintAction.Enable();
    }

    private void OnDisable()
    {
        hintAction.Disable();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out Player player)) 
            return;
        
        trigger.enabled = false;
        DisplayTooltip();
    }

    private void Update()
    {
        if (hintAction.triggered && !trigger.enabled && !tooltipsDisplayed)
            StartCoroutine(DisplayTooltipNext());
    }

    private void DisplayTooltip()
    {
        StartCoroutine(FadeCoroutine(tooltipTextHint, 1f));
        StartCoroutine(FadeCoroutine(tooltipTextHintGamepad, 1f));
    }

    private IEnumerator DisplayTooltipNext()
    {
        tooltipsDisplayed = true;
        StartCoroutine(FadeCoroutine(tooltipTextHint, 0f));
        StartCoroutine(FadeCoroutine(tooltipTextHint2, 1f));
        StartCoroutine(FadeCoroutine(tooltipTextHintGamepad, 0f));
        // StartCoroutine(FadeCoroutine(tooltipTextHintGamepad2, 1f));
        
        yield return new WaitForSeconds(timeToDisplayTooltip);
        
        StartCoroutine(FadeCoroutine(tooltipTextHint2, 0f));
        StartCoroutine(FadeCoroutine(tooltipTextSprint, 1f));
        // StartCoroutine(FadeCoroutine(tooltipTextHintGamepad2, 0f));
        StartCoroutine(FadeCoroutine(tooltipTextSprintGamepad, 1f));
        player._canSprint = true;
        
        yield return new WaitForSeconds(timeToDisplayTooltip);
        
        StartCoroutine(FadeCoroutine(tooltipTextSprint, 0f));
        StartCoroutine(FadeCoroutine(tooltipTextSprintGamepad, 0f));
        
        GameObject.FindFirstObjectByType<ToggleHUD>().ToggleChoiceHUD(true);
    }
    
    private IEnumerator FadeCoroutine(TextMeshProUGUI objectToFade, float targetAlpha)
    {
        Color startColor = objectToFade.color;
        float startAlpha = startColor.a;

        float timer = 0f;

        while (timer <= fadeDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / fadeDuration;

            // Correct way to set alpha for TextMeshPro:
            objectToFade.color = new Color(startColor.r, startColor.g, startColor.b, Mathf.Lerp(startAlpha, targetAlpha, progress));

            yield return null;
        }

        // Ensure final alpha (important for TMP)
        objectToFade.color = new Color(startColor.r, startColor.g, startColor.b, targetAlpha);
    }

    private IEnumerator WaitForTime(float time)
    {
        yield return new WaitForSeconds(time);
    }
}
