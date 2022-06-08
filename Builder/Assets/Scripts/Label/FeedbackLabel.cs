using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FeedbackLabel : MonoBehaviour
{
    public Image panel;
    public Text message;
    public Color successColor;
    public Color errorColor;
    
    private void Awake()
    {
        panel.gameObject.SetActive(false);
    }

    public void ShowError(string message, float seconds = 5)
    {
        panel.color = errorColor;
        ShowLabel(message, seconds);
    }

    public void ShowSuccess(string message, float seconds = 5)
    {
        panel.color = successColor;
        ShowLabel(message, seconds);
    }

    private void ShowLabel(string message, float seconds)
    {
        this.message.text = message;
        panel.gameObject.SetActive(true);
        StartCoroutine(HideLabel(seconds));
    }

    private IEnumerator HideLabel(float timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);
        panel.gameObject.SetActive(false);
    }
    
}
