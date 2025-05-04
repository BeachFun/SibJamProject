using System.Collections;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using RGames.Core;
using UniRx;
using UnityEngine;
using Zenject;

public class MusicManager : MonoBehaviour, IManager
{
    [Header("Settings")]
    [SerializeField] private float _fadeInDuration = 1.5f;
    [SerializeField] private float _fadeOutDuration = 1.5f;
    [SerializeField] private AudioClip _ambientClip;
    [SerializeField] private AudioClip _suspenseClip;
    [SerializeField] private AudioClip _transitionClip;
    [SerializeField] private AudioClip _pursuitClip;
    [Header("Bindings")]
    [SerializeField] private AudioSource _source1;
    [SerializeField] private AudioSource _source2;

    [Inject] IGameManager _gameManager;

    private bool _isBusy;
    private float _volume1;
    private float _volume2;
    private ManagerStatus _lastStatus;

    public ReactiveProperty<ManagerStatus> Status { get; } = new();


    private void Awake()
    {
        _volume1 = _source1.volume;
        _volume2 = _source2.volume;

        _gameManager.CurrentGameState.Subscribe(OnGameStateChangedHandler).AddTo(this);
        this.Status.Subscribe(OnManagerStatusChangedHandler).AddTo(this);
    }

    private void Start()
    {
        OnGameStateChangedHandler(_gameManager.CurrentGameState.Value);
    }


    public void ChangeClip(AudioClip clip)
    {
        // Включение второй дорожки
        if (_isBusy)
        {
            Shutdown(_source1);
            Shutup(_source2, clip, _volume2);
        }
        // Включение первой дорожки
        else
        {
            Shutdown(_source2);
            Shutup(_source1, clip, _volume1);
        }

        _isBusy = !_isBusy;
    }


    private void Shutdown(AudioSource source)
    {
        if (source == null || !source.isPlaying) return;

        Sequence sequence = DOTween.Sequence();

        sequence.Append(source.DOFade(0f, _fadeOutDuration))
                .AppendCallback(() =>
                {
                    source.Stop();
                    source.clip = null;
                });
    }

    private void ShutPause(AudioSource source)
    {
        if (source == null || !source.isPlaying) return;

        Sequence sequence = DOTween.Sequence();

        sequence.Append(source.DOFade(0f, _fadeOutDuration))
                .AppendCallback(() =>
                {
                    source.Pause();
                });

    }

    private void ShutUnpause(AudioSource source, float volume)
    {
        if (source == null) return;

        source.volume = 0f;
        source.Play();

        source.DOFade(volume, _fadeInDuration);
    }

    private void Shutup(AudioSource source, AudioClip clip, float volume)
    {
        if (source == null || clip == null) return;

        source.volume = 0f;
        source.clip = clip;
        source.Play();

        source.DOFade(volume, _fadeInDuration);
    }


    private void OnGameStateChangedHandler(GameState state)
    {
        switch (state)
        {
            case GameState.Played: ChangeClip(_ambientClip); break;
            case GameState.Suspense: ChangeClip(_suspenseClip); break;
            case GameState.PursuitTransition: ChangeClip(_transitionClip); break;
            case GameState.Pursuit: ChangeClip(_pursuitClip); break;
        }

        if (state == GameState.Paused)
        {
            _lastStatus = Status.Value;
            Status.Value = ManagerStatus.Suspended;
        }
        else
        {
            _lastStatus = Status.Value;
            Status.Value = ManagerStatus.Started;
        }
    }

    private void OnManagerStatusChangedHandler(ManagerStatus status)
    {
        if (status == ManagerStatus.Started && _lastStatus != ManagerStatus.Started)
        {
            if (_isBusy) ShutUnpause(_source2, _volume2);
            else ShutUnpause(_source1, _volume1);
        }
        else if (_lastStatus == ManagerStatus.Started)
        {
            ShutPause(_source1);
            ShutPause(_source2);
        }
    }
}