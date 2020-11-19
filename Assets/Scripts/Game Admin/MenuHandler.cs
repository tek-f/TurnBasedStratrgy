using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AztecArmy.gameManager
{
    public class MenuHandler : MonoBehaviour
    {
        bool paused = false;
        [SerializeField] GameObject pausePanel;
        [SerializeField] GameManager gameManager;
        public void ChangeScene(int sceneID)
        {
            SceneManager.LoadScene(sceneID);
        }
        public void ExitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }
        public void TogglePause()
        {
            if (paused)
            {
                Time.timeScale = 1;
                pausePanel.SetActive(false);
                gameManager.enabled = true;
                paused = false;
            }
            else
            {
                Time.timeScale = 0;
                pausePanel.SetActive(true);
                gameManager.enabled = false;
                paused = true;
            }
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                TogglePause();
            }
        }
    }
}