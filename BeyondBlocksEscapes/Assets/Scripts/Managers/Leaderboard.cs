using System.Collections.Generic;
using UnityEngine;
namespace BBE
{
    [CreateAssetMenu(fileName = "Leaderboard", menuName = "Leaderboard")]
    public class Leaderboard : ScriptableObject
    {
        public List<float> RaceCompletionTimes = new List<float>();
        public List<float> LapCompletionTimes = new List<float>();
        public List<string> UsernameRaceCompletionList = new List<string>();
        public List<string> UsernameLapCompletionList = new List<string>();

        public string CurrentUsername;
        public float GameMode;
    }
}