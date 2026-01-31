using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AlertPopup : MonoBehaviour
{
    [SerializeField] private TMP_Text textField;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;

    private Action<bool> _callback;

    public void Open(string text, Action<bool> callback)
    {
        gameObject.SetActive(true);
        textField.text = text;
        _callback = callback;
        confirmButton.onClick.SetListener(Confirm);
        cancelButton.onClick.SetListener(Cancel);
    }

    private void Confirm()
    {
        Close();
        _callback.Invoke(true);
    }

    private void Cancel()
    {
        Close();
        _callback.Invoke(false);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}