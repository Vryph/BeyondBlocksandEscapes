using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace BBE
{
    public class LapTimeTimer : MonoBehaviour
    {
        [SerializeField] private RaceManager _manager;
        private TextMeshProUGUI _text;

        private void Awake()
        {
            _text = GetComponent<TextMeshProUGUI>();
        }
        void Update()
        {
            _text.text = _manager.GetLapTimerText();
        }
    }
}