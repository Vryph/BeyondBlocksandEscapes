using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace BBE
{
    public class LeaderboardText : MonoBehaviour
    {
        private string _text;
        private TextMeshProUGUI _textBox;
       [SerializeField] private Leaderboard _leaderboard;

        private void Start()
        {
            _textBox = GetComponent<TextMeshProUGUI>();
            SetText();
        }

        private void SetText()
        {
            _text = $"Top Race Times: \n";
            var topRaceTimesWithIndexes = _leaderboard.RaceCompletionTimes
                .Select((value, index) => new { Value = value, Index = index })  // Create an anonymous object with Value and Index
                .OrderBy(item => item.Value)  // Order by value (ascending)
                .Take(3)  // Take the first 3 items (the smallest ones)
                .ToList();

            foreach (var item in topRaceTimesWithIndexes)
            {
                int minutes = Mathf.FloorToInt(item.Value / 60);
                int seconds = Mathf.FloorToInt(item.Value % 60);
                _text += $"    {string.Format("{0:00}:{1:00}", minutes, seconds)}  -  {_leaderboard.UsernameRaceCompletionList[item.Index]}\n";
            }
            if (_leaderboard.RaceCompletionTimes.Count == 0) { _text += $"\n Seems like no times were registered just yet.\n\n"; }

             _text += $"\nTop Lap Times: \n";
            var topLapTimesWithIndexes = _leaderboard.LapCompletionTimes
                .Select((value, index) => new { Value = value, Index = index }) 
                .OrderBy(item => item.Value)  
                .Take(3)  
                .ToList();

            foreach (var item in topLapTimesWithIndexes)
            {
                int minutes = Mathf.FloorToInt(item.Value / 60);
                int seconds = Mathf.FloorToInt(item.Value % 60);
                int milliseconds = Mathf.FloorToInt((item.Value % 1) * 100);
                _text += $"    {string.Format("{0:00}:{1:00}.{2:00}", minutes, seconds, milliseconds)}  -  {_leaderboard.UsernameLapCompletionList[item.Index]} \n";
            }
            if (_leaderboard.LapCompletionTimes.Count == 0) { _text += $"\n Seems like no times were registered just yet.\n\n"; }
            else _text += "\nCan you best these times?";

            _textBox.text = _text ;
        }
    }
}
