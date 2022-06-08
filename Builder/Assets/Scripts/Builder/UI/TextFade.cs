using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TextFade : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Text fadeableText;
    private Color _baseColor;
    public Color highlightedColor;
    public Color pressedColor;
    public float fadeDuration;
    private float _currentTime;
    private Color _firstColor;
    private Color _finalColor;


    void Awake()
    {
        _baseColor = fadeableText.color;
    }


    public void OnPointerDown(PointerEventData data)
    {
        fadeableText.color = pressedColor;
    }


    public void OnPointerEnter(PointerEventData data)
    {
        StartFade(highlightedColor);
    }


    public void OnPointerExit(PointerEventData data)
    {
        StartFade(_baseColor);
    }


    public void OnPointerClick(PointerEventData data)
    {
        StartFade(highlightedColor);
    }


    private IEnumerator Fade()
    {
        while (_currentTime < fadeDuration)
        {
            fadeableText.color = Color.Lerp(_firstColor, _finalColor, _currentTime / fadeDuration);
            _currentTime += Time.deltaTime;
            yield return null;
        }
        _currentTime = 0;
        fadeableText.color = _finalColor;
    }


    private void StartFade(Color color)
    {
        _firstColor = fadeableText.color;
        _finalColor = color;
        _currentTime = 0;
        StartCoroutine(Fade());
    }
}
