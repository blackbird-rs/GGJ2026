using Audio;
using UnityEngine;

public class JudgyAudioManager : AudioManager
{
    public new static JudgyAudioManager Instance => (JudgyAudioManager)AudioManager.Instance;

    [Header("LevelMusic")]
    [SerializeField] private float musicDefaultVolume = 0.5f;
    [SerializeField] private float musicFadeDuration = 1;
    [SerializeField] private AudioClipSettings defaultState;
    [SerializeField] private AudioClipSettings[] levelStates;

    [Header("Misc")]
    [SerializeField] private AudioClipSettings steps;

    private int? _currentState;

    private void Start()
    {
        StartGame();
    }

    public void StartGame()
    {
        _currentState = null;
        PlayAudio(defaultState);
        foreach (var state in levelStates)
        {
            PlayAudio(state);
        }

        FadeToDefaultState();
    }

    public void FadeToDefaultState()
    {
        if (_currentState == null) return;

        FadeAudio(levelStates[_currentState.Value], 0, musicFadeDuration);
        FadeAudio(defaultState, musicDefaultVolume, musicFadeDuration);
        _currentState = null;
    }

    public void FadeToState(int state)
    {
        if (_currentState == state) return;

        FadeAudio(defaultState, 0, musicFadeDuration);
        FadeAudio(levelStates[state], musicDefaultVolume, musicFadeDuration);
        _currentState = state;
    }

    public void Steps() => PlayAudio(steps);
}