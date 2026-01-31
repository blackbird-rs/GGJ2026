using System;
using System.Collections;
using Audio;
using UnityEngine;

public class JudgyAudioManager : AudioManager
{
    public new static JudgyAudioManager Instance => (JudgyAudioManager)AudioManager.Instance;

    [Header("MainMenuMusic")]
    [SerializeField] private AudioClipSettings mainMenuStart;
    [SerializeField] private AudioClipSettings mainMenuLoop;

    [Header("LevelMusic")]
    [SerializeField] private float musicFadeDuration = 1;
    [SerializeField] private AudioClipSettings levelBase;
    [SerializeField] private AudioClipSettings[] levelStates;

    [Header("WinCheers")]
    [SerializeField] private AudioClipSettings[] winCheers;

    [Header("LoseBoos")]
    [SerializeField] private AudioClipSettings[] loseBoos;

    [Header("CharacterSelection")]
    [SerializeField] private AudioClipSettings selectComedian;
    [SerializeField] private AudioClipSettings selectWhisperer;

    [Header("Misc")]
    [SerializeField] private AudioClipSettings pageFlip;
    [SerializeField] private AudioClipSettings buttonClick;

    private int _currentState;
    private bool _mainMenuMusicPlaying;
    private Coroutine _startMainMenuLoopCoroutine;

    public void StartMainMenuMusic()
    {
        StopAudio(levelBase);
        foreach (var state in levelStates)
        {
            StopAudio(state);
        }

        if (!_mainMenuMusicPlaying)
        {
            _mainMenuMusicPlaying = true;
            PlayAudio(mainMenuStart);
            _startMainMenuLoopCoroutine = StartCoroutine(StartMainMenuLoop());
        }
    }

    private IEnumerator StartMainMenuLoop()
    {
        yield return new WaitForSeconds(mainMenuStart.Variants[0].length);

        StopAudio(mainMenuStart);
        PlayAudio(mainMenuLoop);
    }

    public void StartLevelMusic()
    {
        StopAudio(mainMenuStart);
        StopAudio(mainMenuLoop);
        _mainMenuMusicPlaying = false;

        if (_startMainMenuLoopCoroutine != null)
        {
            StopCoroutine(_startMainMenuLoopCoroutine);
            _startMainMenuLoopCoroutine = null;
        }

        PlayAudio(levelBase);
        foreach (var state in levelStates)
        {
            PlayAudio(state);
        }

        _currentState = 1;
    }

    public void FadeToState(int state)
    {
        if (state == _currentState) return;

        FadeAudio(levelStates[_currentState - 1], 0, musicFadeDuration);
        FadeAudio(levelStates[state - 1], 1, musicFadeDuration);
        _currentState = state;
    }

    public void WinCheer(float crowdSize01)
    {
        var index = (int)(crowdSize01 * winCheers.Length);
        index = Math.Clamp(index, 0, winCheers.Length - 1);

        var settings = winCheers[index];
        PlayAudio(settings);
    }

    public void LoseBoo(float crowdSize01)
    {
        var index = (int)(crowdSize01 * loseBoos.Length);
        index = Math.Clamp(index, 0, loseBoos.Length - 1);

        var settings = loseBoos[index];
        PlayAudio(settings);
    }

    public void SelectComedian() => PlayAudio(selectComedian);

    public void SelectWhisperer() => PlayAudio(selectWhisperer);

    public void FlipPage() => PlayAudio(pageFlip);

    public void ClickButton() => PlayAudio(buttonClick);
}