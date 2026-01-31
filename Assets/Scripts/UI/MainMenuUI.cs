using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    public GameObject continueButton;

    void Start()
    {
        continueButton.SetActive(SaveSystem.HasSave());
    }
}
