using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bar : MonoBehaviour
{
    [SerializeField] private RectTransform fill;
    [SerializeField] [Range(0, 1)] private float startValue = 1;
    [SerializeField] private bool hideWhenUnchanged;

    [Space] [Header("Variables related to hiding the bar")]
    [SerializeField] private bool startHidden = true;
    [SerializeField] private float animationTime = 1;
    [Tooltip("Time after a change in HP and it starts to fade out")]
    [SerializeField] private float inactivityTime = 3;
    
    private enum BetterBool {False, True, Maybe}
    private BetterBool _hidden;
	private float _barLength;
	private Image[] _colorables;
	private float _visibilityTime;
	private float _barValue;
	private float _barValueLastFrame;
	
	

	private void Awake()
	{
		_barLength = fill.sizeDelta.x;
		SetBar(startValue);
		_colorables = GetComponentsInChildren<Image>();
		if (hideWhenUnchanged && startHidden)
		{
			foreach (Image image in _colorables)
			{
				image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
			}
			_hidden = BetterBool.True;
		}
	}

	private void LateUpdate()
	{
		if (hideWhenUnchanged)
		{
			if (!Mathf.Approximately(_barValueLastFrame, _barValue))
			{
				_visibilityTime = 0;
			}

			switch (_hidden)
			{
				case BetterBool.False:
				{
					_visibilityTime += Time.deltaTime;
			
					if (_visibilityTime > inactivityTime)
						StartCoroutine(SetVisible(false, animationTime));
					break;
				}
				case BetterBool.True when !Mathf.Approximately(_barValueLastFrame, _barValue):
					StartCoroutine(SetVisible(true, animationTime));
					break;
			}
		}


		_barValueLastFrame = _barValue;
	}

	public void SetBar(float valueFrom0To1)
	{
		_barValue = Mathf.Lerp(0, _barLength, valueFrom0To1);
		
		fill.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _barValue);
    }

	private IEnumerator SetVisible(bool visible, float at)
	{
		yield return null;
		_hidden = BetterBool.Maybe;
		
		for (float i = 0; i < 1; i += Time.deltaTime / at)
		{
			foreach (Image image in _colorables)
			{
				image.color = new Color(image.color.r, image.color.g, image.color.b, visible ? i : 1 - i);
			}

			yield return null;
		}

		_hidden = visible ? BetterBool.False : BetterBool.True;
	}
}
