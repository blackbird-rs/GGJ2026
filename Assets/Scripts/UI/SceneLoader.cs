using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadSceneByIndex(int sceneIndex)
    {
        JudgyAudioManager.Instance.Click();
        JudgyAudioManager.Instance.Steps();
        SceneManager.LoadScene(sceneIndex);
    }

    public void LoadSceneByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void NewGame(int sceneIndex)
    {
        JudgyAudioManager.Instance.Click();
        SaveSystem.ClearSave();
        SceneManager.LoadScene(sceneIndex);
    }

    public void ContinueGame(int sceneIndex)
    {
        JudgyAudioManager.Instance.Click();
        SceneManager.LoadScene(sceneIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
