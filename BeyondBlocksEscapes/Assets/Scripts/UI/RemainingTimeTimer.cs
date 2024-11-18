using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace BBE
{
    public class RemainingTimeTimer : MonoBehaviour
    {
        [SerializeField] private RaceManager _manager;
        private TextMeshProUGUI _textString;

        private void Awake()
        {
            _textString = GetComponent<TextMeshProUGUI>();
        }
        void Update()
        {
            _textString.text = _manager.GetRemainingTimerText();
        }
    }
}