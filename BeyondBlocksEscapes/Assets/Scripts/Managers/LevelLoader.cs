using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace BBE
{
    public class LevelLoader : MonoBehaviour
    {
        [SerializeField] private Animator _transition;
        [SerializeField] private float _transitionTime;

        public void LoadScene(int levelIndex)
        {
            StartCoroutine(LoadLevel(levelIndex));
        }

        IEnumerator LoadLevel(int levelIndex)
        {
            _transition.SetTrigger("Start");

            yield return new WaitForSeconds(_transitionTime);

            SceneManager.LoadScene(levelIndex);
        }
    }
}
