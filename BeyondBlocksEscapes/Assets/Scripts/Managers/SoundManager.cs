using UnityEngine;
using System;
using System.Collections;

namespace BBE
{
    public enum SoundType
    {
        WallTouch,
        WallHit,
        Fall,
        Jump,
        Dash,
        LapFinish,
        RaceStart,
        Step,
        RaceLost,
        RaceWon,
        Button,
        Hover
    }

    [Serializable]
    public struct SoundList
    {
        public AudioClip[] Sounds { get => _sounds; }
        [HideInInspector] public string name;
        [SerializeField] private AudioClip[] _sounds;
    }

    [RequireComponent(typeof(AudioSource)), ExecuteInEditMode]
    public class SoundManager : MonoBehaviour
    {
        [SerializeField] private SoundList[] _soundList;
        private static SoundManager _instance;
        private AudioSource _audioSource;

        private void Awake()
        {
            _instance = this;
        }

        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
        }

#if UNITY_EDITOR
        private void OnEnable()
        {
            string[] names = Enum.GetNames(typeof(SoundType));
            Array.Resize(ref _soundList, names.Length);
            for (int i = 0; i < _soundList.Length; i++)
                _soundList[i].name = names[i];

        }
#endif

        public static void PlaySound(SoundType sound, float volume = 1)
        {
            AudioClip[] _possibleClips = _instance._soundList[(int)sound].Sounds;
            AudioClip _randomClip = _possibleClips[UnityEngine.Random.Range(0, _possibleClips.Length)];
            _instance._audioSource.PlayOneShot(_randomClip, volume);
        }

    }
}
