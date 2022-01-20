using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using PlayerInput = Player.PlayerInput;

namespace System
{
    public class SceneController : MonoBehaviour
    {
        public GameObject pauseMenu;
        public PlayerInput playerinput;
        public static bool gameIsPaused;
        private int m_CurrentLevel;
        public static bool isGamepad;
        private PlayerActionInputs m_Input;
        public TMP_Text highScoreText;
        public AudioLowPassFilter lowPassFilter;
        public AudioSource mowerAudioSource;
        private void Awake()
        {
            m_Input = new PlayerActionInputs();
        }

        private void Start()
        {
            m_CurrentLevel = PlayerPrefs.GetInt("CurrentLevel");
            if (m_CurrentLevel < 1)
            {
                PlayerPrefs.SetInt("CurrentLevel", 1);
            }
        }

        private void Update()
        {
            if (m_Input.UI.Gamepad.triggered)
            {
                isGamepad = true;
            }
            else if (Keyboard.current.anyKey.wasPressedThisFrame || m_Input.UI.Mouse.triggered)
            {
                isGamepad = false;
            }

            if (playerinput == null) return;
            if (playerinput.pause)
            {
                if (gameIsPaused)
                {
                    Resume();
                }
                else
                {
                    Pause();
                }
            }
        }

        public void NextLevel()
        {
            m_CurrentLevel = PlayerPrefs.GetInt("CurrentLevel");
            if (m_CurrentLevel >= 8)
            {
                m_CurrentLevel = 1;
                PlayerPrefs.DeleteKey("ScoreList");
                PlayerPrefs.SetInt("CurrentLevel", m_CurrentLevel);
            }
            SceneManager.LoadScene(m_CurrentLevel);
            print("Current Level: " + m_CurrentLevel);
        }

        public static void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
            Time.timeScale = 1f;
            gameIsPaused = false;
            ScoreManager.levelHasStarted = false;
        }

        public static void LoadScene(int sceneNumber)
        {
            SceneManager.LoadScene(sceneNumber);
            Time.timeScale = 1f;
            gameIsPaused = false;
            ScoreManager.levelHasStarted = false;
        }

        public void NewGame()
        {
            PlayerPrefs.DeleteKey("ScoreList");
            PlayerPrefs.DeleteKey("FinalScore");
            PlayerPrefs.SetInt("CurrentLevel", 1);
            NextLevel();
        }

        public void ResetHighScore()
        {
            PlayerPrefs.DeleteKey("HighScore");
            highScoreText.text = "Highscore: " + 0;
        }

        public void ResetScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            Resume();
            ScoreManager.levelHasStarted = false;
        }

        public static void QuitGame()
        {
            ScoreManager.levelHasStarted = false;
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        public void Pause()
        {
            pauseMenu.SetActive(true);
            Time.timeScale = 0f;
            gameIsPaused = true;
            lowPassFilter.enabled = true;
            mowerAudioSource.mute = true;
        }

        public void Resume()
        {
            Time.timeScale = 1f;
            gameIsPaused = false;
            lowPassFilter.enabled = false;
            mowerAudioSource.mute = false;
            if (pauseMenu == null) return;
            pauseMenu.SetActive(false);
        }

        private void OnEnable()
        {
            m_Input.Enable();
        }

        private void OnDisable()
        {
            m_Input.Disable();
        }
    }
}