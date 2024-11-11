using UnityEngine;
using UnityEngine.TextCore.Text;

namespace BBE
{
    public class RaceManager : MonoBehaviour
    {
        [Header("Lap Configurations")]
        [SerializeField] private Vector3 _RaceStartingPosition = new Vector3(-12, -5, 0);
        [SerializeField, Range(1, 10)] private int _totalLaps = 2;

        [Header("Debug")]
        [SerializeField] private bool _isDebugEnabled = false;
        [SerializeField] private Leaderboard _leaderboard;


        [SerializeField] private Move _character;
        private Checkpoint[] _checkpoints;

        

        private float[] _lapCompletionTimes;
        private float _totalRaceTime, _currentLapTime, _countdownTimer = 3f, _countdownPhase = 0;
        private int _currentLapNumber;
        [SerializeField] private bool _hasRaceStarted, _doesLapCount, _ongoingRace;

        private string _text = "00:00";

        private void Start()
        {
            _checkpoints = GetComponentsInChildren<Checkpoint>();

            int i = 0;
            foreach (Checkpoint checkpoint in _checkpoints)
            {
                checkpoint.Id = i;
                i++;
            }

            if (_leaderboard.GameMode == 1)
                _character.ShouldLock = true;


            _lapCompletionTimes = new float[_totalLaps];
        }

        private void Update()
        {
            if(!_hasRaceStarted)
                Countdown();

            if(!_doesLapCount)
                _doesLapCount = DoesLapCount();

            if (_ongoingRace)
            {
                SetTimerText();
                _totalRaceTime += Time.deltaTime;
                _currentLapTime += Time.deltaTime;
            }
        }


        private void Countdown()
        {

            if (_countdownTimer > 0 && _countdownPhase == 0)
            {
                _text = "You must finish three laps to complete the Race!";
                _countdownTimer -= Time.deltaTime;
            }
            else if (_countdownPhase == 0)
            {
                _text = "Use Z/Shift to dash";
                _countdownPhase = 1;
                _countdownTimer = 7f;
            }
            else if (_countdownPhase == 1 && _countdownTimer > 0)
            {
                _countdownTimer -= Time.deltaTime;
                if (_countdownTimer < 1.1f)
                    _text = "GO!!!";
                else if (_countdownTimer <= 4f)
                {
                    //play sound
                    int seconds = Mathf.FloorToInt(_countdownTimer % 60);
                    _text = seconds.ToString();
                }
            }
            else
            {
                

                if (_character.IsLocked)
                    _character.ShouldLock = true;

                _hasRaceStarted = true;
                _ongoingRace = true;
            }

            //This method will Run on the start of the race, It shuld do a visual countdown and a pan pan pan sound.
        }

        public void LapCompleted()
        {
            _lapCompletionTimes[_currentLapNumber] = (float)Mathf.Floor(_currentLapTime * 100f) / 100f; 
            _leaderboard.LapCompletionTimes.Add(_lapCompletionTimes[_currentLapNumber]);
            _currentLapTime = 0;
            _currentLapNumber += 1;
            _doesLapCount = false;
            foreach (Checkpoint checkpoint in _checkpoints)
            {
                checkpoint.HasTriggered = false;
            }

        }

        private void RaceCompleted()
        {
            _character.ShouldLock = true;

            _leaderboard.RaceCompletionTimes.Add(_totalRaceTime);
            _ongoingRace = false;

            /*
             Here I need to put the Juice.
             Feedback that the race is over, Sound effects, Show the Scores and if they're on the leaderboard.
             Show the comands to Restart the Scene or Return to Menu.
             */
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.GetType().ToString() == "UnityEngine.BoxCollider2D")
            {
                if (_doesLapCount)
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

        public void SetTimerText()
        {
            if (_hasRaceStarted)
            {
                int minutes = Mathf.FloorToInt(_currentLapTime / 60);
                int seconds = Mathf.FloorToInt(_currentLapTime % 60);
                _text = string.Format("{0:00}:{1:00}", minutes, seconds);
                _text = "LapTime: " + _text;
            }
            else
                _text = "";
        }

        public string GetTimerText()
        {
            return _text;
        }
    }
}