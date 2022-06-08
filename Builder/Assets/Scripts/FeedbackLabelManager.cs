using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedbackLabelManager : MonoBehaviour
{
    private List<FeedbackLabel> _labels = new List<FeedbackLabel>();
    private int initialLabelCount = 4;
    public FeedbackLabel labelPrefab;
    public float timeDisplayed = 5;
    public static FeedbackLabelManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            throw new Exception("There are more than one Feedback Label manager instance");
        
        for (int i = 0; i < initialLabelCount; i++)
        {
            CreateLabel();
        }
    }

    public void ShowSuccess(string message)
    {
        var label = GetLabel();
        label.ShowSuccess(message, timeDisplayed);
    }

    public void ShowError(string message)
    {
        var label = GetLabel();
        label.ShowError(message, timeDisplayed);
    }

    private FeedbackLabel GetLabel()
    {
        foreach (var label in _labels)
        {
            if (!label.isActiveAndEnabled) return label;
        }

        return CreateLabel();
    }

    private FeedbackLabel CreateLabel()
    {
        var label = GameObject.Instantiate(labelPrefab, transform);
        _labels.Add(label);

        return label;
    }
}
