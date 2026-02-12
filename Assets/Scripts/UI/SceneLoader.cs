using Audio;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadSceneByIndex(int sceneIndex)
    {
        AudioManager.Instance.PlayClick();
        AudioManager.Instance.PlaySteps();
        SceneManager.LoadScene(sceneIndex);
    }

    public void LoadSceneByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void NewGame(int sceneIndex)
    {
        AudioManager.Instance.PlayClick();
        SaveSystem.ClearSave();
        SceneManager.LoadScene(sceneIndex);
    }

    public void ContinueGame(int sceneIndex)
    {
        AudioManager.Instance.PlayClick();
        SceneManager.LoadScene(sceneIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
