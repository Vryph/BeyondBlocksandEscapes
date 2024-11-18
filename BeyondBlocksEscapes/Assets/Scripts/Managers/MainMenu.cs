using UnityEngine;
using UnityEngine.SceneManagement;

namespace BBE
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private Leaderboard _leaderboard;
        [SerializeField] private LevelLoader _levelLoader;

        public void PlayGame()
        {
            _levelLoader.LoadScene(1);
        }

        public void QuitGame()
        {
            Application.Quit();
        }

        public void LoadMenu()
        {
            _levelLoader.LoadScene(0);
        }

        public void SetGameMode(int gameMode)
        {
            _leaderboard.GameMode = gameMode;
        }
        public void SetUsername(string username)
        {
            if(username == null) { _leaderboard.CurrentUsername = "Mistery Man"; }
            else if (username.Length > 30) { _leaderboard.CurrentUsername = "Long Name Andy"; }
            else _leaderboard.CurrentUsername = username;
        }

        public void ReloadScene()
        {
           _levelLoader.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void PlayButtonSound()
        {
            SoundManager.PlaySound(SoundType.Button, 0.85f);
        }

        public void PlayButtonHoverSound()
        {
            SoundManager.PlaySound(SoundType.Hover, 0.7f);
        }
    }
}