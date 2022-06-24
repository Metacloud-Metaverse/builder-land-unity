using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Test : MonoBehaviour
{
    private TMP_InputField _inputField;
     
    private void Awake()
    {
        _inputField = GetComponent<TMP_InputField>();
    }

    private void Start()
    {
        StartCoroutine(SendCaretToEnd());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
            StartCoroutine(SendCaretToEnd());
    }

    private IEnumerator SendCaretToEnd()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            // _inputField.MoveTextEnd(false);
            _inputField.Select();
            _inputField.caretPosition = _inputField.text.Length;
            

            print(_inputField);
        }
    }
}
