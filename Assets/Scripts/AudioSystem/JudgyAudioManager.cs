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
    [SerializeField] private AudioClipSettings applause;
    [SerializeField] private AudioClipSettings click;
    [SerializeField] private AudioClipSettings putOn;
    [SerializeField] private AudioClipSettings steps;
    [SerializeField] private AudioClipSettings take;

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

    public void Applause() => PlayAudio(applause);
    public void Click() => PlayAudio(click);
    public void PutOn() => PlayAudio(putOn);
    public void Steps() => PlayAudio(steps);
    public void Take() => PlayAudio(take);
}