using System.Collections.Generic;
using UnityEngine;
namespace BBE
{
    [CreateAssetMenu(fileName = "Leaderboard", menuName = "Leaderboard")]
    public class Leaderboard : ScriptableObject
    {
        public List<float> RaceCompletionTimes = new List<float>();
        public List<float> LapCompletionTimes = new List<float>();
        public float GameMode;
    }
}