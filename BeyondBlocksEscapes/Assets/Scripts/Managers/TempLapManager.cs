using UnityEngine;

namespace BBE
{
    public class TempLapManager : MonoBehaviour
    {
        BoxCollider2D _collider;
        [SerializeField] private bool _ongoingLap = false;
        [SerializeField] private float _currentTime = 0f;
        [SerializeField] public bool _checkpointTrigger = false;
        [SerializeField] private Vector3 _startPosition = new Vector3(-12,-5, 0);

        private bool _lapLimit = false;
        [SerializeField] private Rigidbody2D _player;

        
        private string _text = "00:00";
        private void Awake()
        {
            _collider = GetComponent<BoxCollider2D>();
        }

        private void Update()
        {
            if (_ongoingLap)
            {
                _currentTime += Time.deltaTime;
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                _currentTime = 0;
                _lapLimit = false;
                _ongoingLap = false;
                _player.transform.position = _startPosition;
            }
        }


        public string GetTimerText()
        {
            int minutes = Mathf.FloorToInt(_currentTime / 60);
            int seconds = Mathf.FloorToInt(_currentTime % 60);
            _text = string.Format("{0:00}:{1:00}", minutes, seconds);
            _text = "LapTime: " + _text;
            return _text;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (_ongoingLap == false && _lapLimit == false)
            {
                _ongoingLap = true;
            }

            if(_ongoingLap && _checkpointTrigger)
            {
                _ongoingLap = false;
                _lapLimit = true;
            }
        }
    }
}
