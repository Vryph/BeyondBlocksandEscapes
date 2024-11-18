using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;


namespace BBE
{
    public class RaceManager : MonoBehaviour
    {
        [Header("Race Configurations")]
        [SerializeField] private Move _characterMovement;
        [SerializeField] private Vector3 _RaceStartingPosition = new Vector3(-12, -5, 0);
        [SerializeField, Range(1, 10)] private int _totalLaps = 3;
        [SerializeField, Range(10f, 600)] private float _raceTimeLimit = 360;  

        [Header("Background Color Shift")]
        [SerializeField] private GameObject _backgroundObject;
        private UnityEngine.UI.Image _background;
        [SerializeField] private Color _startColor;
        [SerializeField] private Color _endColor;
        [SerializeField] private AnimationCurve _colorCurve;

        [Header("Music")]
        [SerializeField] private AudioSource _musicAudioSource;
        [SerializeField] private AudioClip[] _musicClips;

        [Header("End Screen")]
        [SerializeField] private TextMeshProUGUI _endScreenText;
        [SerializeField] private Canvas _endCanvas;

        [Header("LeaderBoard")]
        [SerializeField] private Leaderboard _leaderboard;


        
        private Checkpoint[] _checkpoints;
        
        private float[] _lapCompletionTimes;
        private float _totalRaceTime, _currentLapTime;
        private int _currentLapNumber = 0;
        [Header("Debug")]
        [SerializeField] private bool _doesLapCount, _ongoingRace, _isRaceStartSoundPlaying = false;

        private string _remainingTimerText = "", _dialogueBoxText = "", _lapTimerText = "";

        private void Start()
        {

            _checkpoints = GetComponentsInChildren<Checkpoint>();
            _background = _backgroundObject.GetComponent<UnityEngine.UI.Image>();
            _musicAudioSource.volume = 0.15f;
            _musicAudioSource.clip = _musicClips[1];
            _musicAudioSource.Play();


            int i = 0;
            foreach (Checkpoint checkpoint in _checkpoints)
            {
                checkpoint.Id = i;
                i++;
            }

            if (_leaderboard.GameMode == 1) 
            {
                _characterMovement.ShouldSwitchLockState = true;
                StartCoroutine(RaceCountdown());

            }


            _lapCompletionTimes = new float[_totalLaps];
        }

        private void Update()
        {
            if(!_doesLapCount)
                _doesLapCount = DoesLapCount();

            if (_ongoingRace)
            {
                SetLapTimerText();
                SetRemainingTimerText();
                SetBackgroundColor();
                _totalRaceTime += Time.deltaTime;
                _currentLapTime += Time.deltaTime;
                if(_raceTimeLimit - _totalRaceTime <= 0)
                {
                    RaceLost();
                }
            }
        }

        IEnumerator RaceCountdown()
        {
            _dialogueBoxText = "You must finish three laps to complete the Race!";
            yield return new WaitForSeconds(4.5f);

            _dialogueBoxText = "Use   Z / Shift OR X/Y(Gamepad)  to Dash!";
            yield return new WaitForSeconds(4.5f);

            int minutes = Mathf.FloorToInt(_raceTimeLimit / 60);
            _dialogueBoxText = $"You have {minutes} minutes, Good Luck!";
            yield return new WaitForSeconds(4f);

            float countdownTime = 4;
            while(countdownTime > 0f)
            {
                if (countdownTime < 1.1f)
                {
                    _dialogueBoxText = "GO!!!";
                    if (!_musicAudioSource.isPlaying || _musicAudioSource.clip == _musicClips[1])
                    {
                        _musicAudioSource.clip = _musicClips[0];
                        _musicAudioSource.Play();
                    }
                }
                else if (countdownTime <= 3.9f)
                {
                    if (!_isRaceStartSoundPlaying)
                    {
                        SoundManager.PlaySound(SoundType.RaceStart, 0.9f);
                        _isRaceStartSoundPlaying = true;
                    }
                    int seconds = Mathf.FloorToInt(countdownTime);
                    _dialogueBoxText = seconds.ToString();
                }

                countdownTime -= Time.deltaTime; 
                yield return null; 
            }

            if (_characterMovement.IsLocked)
                _characterMovement.ShouldSwitchLockState = true;

            _ongoingRace = true;
            _dialogueBoxText = "";
        }


        public void LapCompleted()
        {

            SoundManager.PlaySound(SoundType.LapFinish);
            _lapCompletionTimes[_currentLapNumber] = (float)Mathf.Floor(_currentLapTime * 100f) / 100f;
            _leaderboard.LapCompletionTimes.Add(_lapCompletionTimes[_currentLapNumber]);
            _leaderboard.UsernameLapCompletionList.Add(_leaderboard.CurrentUsername);

            if (_currentLapNumber + 1 < _totalLaps)
            {
                _currentLapTime = 0;
                _currentLapNumber += 1;
                _doesLapCount = false;

                foreach (Checkpoint checkpoint in _checkpoints)
                {
                    checkpoint.HasTriggered = false;
                }
            }
            else
            {
                RaceCompleted();
            }
                
        }

        public void RaceLost()
        {
            SoundManager.PlaySound(SoundType.RaceLost);
            _characterMovement.ShouldSwitchLockState = true;
            _musicAudioSource.Stop();
            _ongoingRace = false;

            _endCanvas.gameObject.SetActive(true);
            _endScreenText.text = $"TIME'S UP \n\n\n\n Seems like you didn't make it in time. \n That's quite unfortunate. \n But you can always try again!";
        }

        private void RaceCompleted()
        {
            _ongoingRace = false;
            _characterMovement.ShouldSwitchLockState = true;
            _musicAudioSource.Stop();

            _leaderboard.RaceCompletionTimes.Add(_totalRaceTime);
            _leaderboard.UsernameRaceCompletionList.Add(_leaderboard.CurrentUsername);


            int minutes = Mathf.FloorToInt(_totalRaceTime / 60);
            int seconds = Mathf.FloorToInt(_totalRaceTime % 60);
            string _finalRaceTime = string.Format("{0:00}:{1:00}", minutes, seconds);

            string _leaderboardMessage;
            if(_leaderboard.RaceCompletionTimes.Count == 1)
            {
                _leaderboardMessage = $"Wow! That's a new Record!!! \n But it seems like you're the first to complete the race \n Let's see if your time will stand the test of time.\n";
                SoundManager.PlaySound(SoundType.RaceWon, 0.8f);
            }
            if (_totalRaceTime <= _leaderboard.RaceCompletionTimes.Min())
            {
                _leaderboardMessage = "Wow! That's a new Record!!!\n";
                SoundManager.PlaySound(SoundType.RaceWon, 0.8f);  // Could put a new Sound variation here.
            }
            else
            {
                float bTime = _leaderboard.RaceCompletionTimes.Min();
                int bminutes = Mathf.FloorToInt(bTime / 60);
                int bseconds = Mathf.FloorToInt(bTime % 60);
                string bfinalRaceTime = string.Format("{0:00}:{1:00}", bminutes, bseconds);
                _leaderboardMessage = $"That's a good effort but someone's got you beat with a time of {bfinalRaceTime}\n";
                SoundManager.PlaySound(SoundType.RaceWon, 0.8f);
            }


            _endCanvas.gameObject.SetActive(true);
            _endScreenText.text = $"FINISH \n \n Congratulations you've finished the race! \n {_leaderboardMessage} \n Race Completion Time: {_finalRaceTime}. \n {_lapTimerText}";
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.GetType().ToString() == "UnityEngine.BoxCollider2D")
            {
                if (_doesLapCount && _ongoingRace)
                {
                    LapCompleted();
                    if (_currentLapNumber > _totalLaps)
                    {
                        RaceCompleted();
                    }
                }
            }
        }

        private bool DoesLapCount()
        {
            foreach (Checkpoint checkpoint in _checkpoints)
            {
                if (!checkpoint.HasTriggered)
                {
                    return false;
                }
            }

            return true;
        }


        public void SetLapTimerText()
        {
            _lapTimerText = "";
            if (_ongoingRace)
            {
                for (int i = 0; i < _lapCompletionTimes.Length; i++){
                    if (_lapCompletionTimes[i] > 0)
                    {
                        int minutes = Mathf.FloorToInt(_lapCompletionTimes[i] / 60);
                        int seconds = Mathf.FloorToInt(_lapCompletionTimes[i] % 60);
                        int milliseconds = Mathf.FloorToInt((_lapCompletionTimes[i] % 1) * 100);
                        string tempString = string.Format("{0:00}:{1:00}.{2:00}", minutes, seconds, milliseconds);
                        _lapTimerText += $"Lap {i + 1}: {tempString} \n";
                    }
                }
                int currentLapminutes = Mathf.FloorToInt(_currentLapTime / 60);
                int currentLapseconds = Mathf.FloorToInt(_currentLapTime % 60);
                int currentLapmilliseconds = Mathf.FloorToInt((_currentLapTime % 1) * 100);
                string currentLapTempString = string.Format("{0:00}:{1:00}", currentLapminutes, currentLapseconds, currentLapmilliseconds);
                _lapTimerText += $"Lap {_currentLapNumber + 1}: {currentLapTempString}";
            }
        }

        public void SetRemainingTimerText()
        {
            if (_ongoingRace)
            {
                float remainingTime = _raceTimeLimit - _totalRaceTime;
                int minutes = Mathf.FloorToInt(remainingTime / 60);
                int seconds = Mathf.FloorToInt(remainingTime % 60);
                _remainingTimerText = string.Format("{0:00}:{1:00}", minutes, seconds);
                _remainingTimerText = "Remaining Time: " + _remainingTimerText;
            }
            else
                _remainingTimerText = "";
        }

        private void SetBackgroundColor()
        {
            float normalizedTime = Mathf.Clamp01(_totalRaceTime / _raceTimeLimit);
            float curveValue = _colorCurve.Evaluate(normalizedTime);
            //Debug.Log(curveValue);

            Color _targetColor = Color.Lerp(_startColor, _endColor, curveValue);
            _background.color = _targetColor;
        }

        public string GetRemainingTimerText()
        {
            return _remainingTimerText;
        }
   
        public string GetLapTimerText()
        {
            return _lapTimerText;
        }

        public string GetDialogueText()
        {
            return _dialogueBoxText;
        }
    }
}