using UnityEngine;
using UnityEngine.UI;

public class FeedbackLabel : MonoBehaviour
{
    public Image panel;
    public Text message;
    public Color successColor;
    public Color errorColor;

    private static FeedbackLabel _instance;
    public static FeedbackLabel instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance == null) _instance = this;
        else
            Debug.LogError("There are more than one Feedback Label instances");

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
        Invoke("HideLabel", seconds);
    }

    public void HideLabel()
    {
        panel.gameObject.SetActive(false);
    }
    
}
