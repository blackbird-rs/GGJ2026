using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AlertPopup : MonoBehaviour
{
    [SerializeField] private TMP_Text textField;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private Animator animator;

    private Action<bool> _callback;

    public void Open(string text, Action<bool> callback)
    { 
        gameObject.SetActive(true);
        textField.text = text;
        _callback = callback;
        confirmButton.onClick.SetListener(Confirm);
        //cancelButton.onClick.SetListener(Cancel);
        animator.Play("PopupOpen");
    }

    private void Confirm()
    {
        StartCoroutine(Close());
        _callback.Invoke(true);
    }

    private void Cancel()
    {
        StartCoroutine(Close());
        _callback.Invoke(false);
    }

    public IEnumerator Close()
    {
        animator.Play("PopupClose");
        yield return new WaitForSeconds(2);
        gameObject.SetActive(false);
    }
}