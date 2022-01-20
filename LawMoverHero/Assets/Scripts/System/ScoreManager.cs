using System.Collections;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

namespace System
{
    public class ScoreManager : MonoBehaviour
    {
        private bool m_StageOver;
        public Tilemap grassTilemap;
        public Tilemap wallTilemap;
        public int tilesLeft;
        public GameObject newHighScore;
        public TMP_Text endScoreText;
        public TMP_Text highScoreText;
        public TMP_Text finalScoreText;
        public TMP_Text scoreListText;
        public GameObject upperRightCorner;
        public GameObject downLeftCorner;
        public float score = 10000;
        public float minScore = 1000;
        private static float _levelScore;
        private float m_FinalScore;
        public float increaseSpeed = 0.001f;
        public bool isFinalLevel;
        public static bool levelHasStarted;
        public int increaseAmount = 10;
        private float m_HighScore;
        public GameObject boss;
        private bool m_BossLevel;
        private string m_ScoreStrings;
        public TileBase[] gateTiles;

        private void Start()
        {
            if (highScoreText != null)
            {
                GetHighscore();
                m_ScoreStrings = PlayerPrefs.GetString("ScoreList");
                scoreListText.text += m_ScoreStrings;
                /*foreach (var levelScore in Scores)
                {
                    scoreListText.text += "\n"+levelScore;
                }*/
            }
            if (boss != null)
            {
                m_BossLevel = true;
            }
            if (endScoreText == null) return;
            print("Level Score: " + _levelScore);
            print("Final Score: " + m_FinalScore);
            endScoreText.text = "Score: " + _levelScore;
            //StartCoroutine(CountScore());
            
        }

        private void Update()
        {
            
            if (grassTilemap == null) return;
            tilesLeft = grassTilemap.GetTilesRangeCount(grassTilemap.WorldToCell(upperRightCorner.transform.position), grassTilemap.WorldToCell(downLeftCorner.transform.position));
            if (tilesLeft <= 0 && !m_BossLevel)
            {
               EndLevel();
            }
            else if (!m_BossLevel)
            {
                m_StageOver = false;
            }

            if (boss == null && m_BossLevel)
            {
                EndLevel();
            }
            else if (m_BossLevel)
            {
                m_StageOver = false;
            }
            
        }

        private void EndLevel()
        {
            print("level over");
            var increase =0;
            for (var i = -1; i <= 1; i += 2)
            {
                var gateToDelete = transform.position + new Vector3(i, 1);
                wallTilemap.SetTile(wallTilemap.WorldToCell(gateToDelete), gateTiles[increase++]);
            }
            increase = 2;
            for (var i = -1; i <= 1; i ++)
            {
                var gatePos= wallTilemap.WorldToCell(transform.position + new Vector3(i, 0));
                wallTilemap.SetTile(gatePos, gateTiles[increase++]);
            }
            m_StageOver = true;
            levelHasStarted = false;
        }

        private void GetHighscore()
        {
            m_HighScore = PlayerPrefs.GetFloat("HighScore");
            m_FinalScore = PlayerPrefs.GetFloat("FinalScore");
            if (m_HighScore < m_FinalScore)
            {
                m_HighScore = m_FinalScore;
                newHighScore.SetActive(true);
                PlayerPrefs.SetFloat("HighScore", m_HighScore);
            }
            else
            {
                newHighScore.SetActive(false);
            }
            highScoreText.text = "High Score: " + m_HighScore;
            finalScoreText.text = "Total Score: " + m_FinalScore;

        }

        private void FixedUpdate()
        {
            if (grassTilemap == null) return;
            if (score > minScore)
            {
                score -= 1;
            }
        }

        private IEnumerator CountScore()
        {
            m_FinalScore = PlayerPrefs.GetFloat("FinalScore");
            for (var i = 0; i <= m_FinalScore; i += increaseAmount)
            {
                endScoreText.text = "Score: " + i;
                yield return new WaitForSeconds(increaseSpeed);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") && m_StageOver)
            {
                var currentLevel = PlayerPrefs.GetInt("CurrentLevel");
                currentLevel++;
                m_FinalScore = PlayerPrefs.GetFloat("FinalScore");
                m_FinalScore += score;
                _levelScore = score;
                var currentLevelDisplay = currentLevel - 1;
                m_ScoreStrings = PlayerPrefs.GetString("ScoreList");
                m_ScoreStrings += "Level "+currentLevelDisplay+" Score: "+score+"\n";
                //Scores.Add(finalScore);
                /*if (finalScore > m_HighScore)
                {
                    m_HighScore = finalScore;
                    PlayerPrefs.SetFloat("HighScore", m_HighScore);
                }*/
                PlayerPrefs.SetFloat("FinalScore",m_FinalScore);
                PlayerPrefs.SetString("ScoreList",m_ScoreStrings);
                PlayerPrefs.SetInt("CurrentLevel", currentLevel);
                SceneController.LoadScene(!isFinalLevel ? "ScoreScene" : "FinalScoreScene");
            }
            /*else if (levelHasStarted)
            {
                var playerController = other.GetComponent<PlayerController>();
                playerController.Knockback(playerController.knockbackForce,playerController.stunTime);
                //other.transform.rotation = Quaternion.Euler(0, 0, 0);
            }*/
        }
    }
}