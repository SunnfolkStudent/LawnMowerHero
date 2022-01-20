using System;
using System.Collections;
using UnityEngine;

namespace Player
{
    public class PlayerHealth : Animations
    {
        protected int health;
        public GameObject[] healthSprites;
        public bool stopEverything;
        public AudioSource audioSource;
        public AudioClip collideSound;
        public AudioClip takeDamage;
        public Vector2 mDirection = new(0, 1);
        public float invincibilityLength = 0.5f;
        private bool m_Invincible;
        public float activeLenght = 0.1f;
        public float inactiveLenght = 0.1f;
        public GameObject mainBody;

        public void DecreaseHealth(int amount)
        {
            if (m_Invincible) return;
            for (var i = 0; i < amount; i++)
            {
                if (health <= 0)
                {
                    PlayAnim4Directions("BoomUp", "BoomDown", "BoomRight", "BoomLeft");
                    stopEverything = true;
                }
                else
                {
                    audioSource.PlayOneShot(takeDamage);
                    healthSprites[health].SetActive(false);
                    health--;
                    StartCoroutine(InvincibilityFrames());
                    StartCoroutine(InvincibilityFramesAnim());
                }
            }
        }

        protected void PlayAnim4Directions(string up, string down, string right, string left)
        {
            switch (mDirection.y)
            {
                case > 0:
                    animator.Play(up);
                    break;
                case < 0:
                    animator.Play(down);
                    break;
            }

            switch (mDirection.x)
            {
                case > 0:
                    animator.Play(right);
                    break;
                case < 0:
                    animator.Play(left);
                    break;
            }
        }

        private IEnumerator InvincibilityFramesAnim()
        {
            while (m_Invincible)
            {
                var mainBodySprite = mainBody.GetComponent<SpriteRenderer>();
                mainBodySprite.enabled = false;
                yield return new WaitForSeconds(activeLenght);
                mainBodySprite.enabled = true;
                yield return new WaitForSeconds(inactiveLenght);
            }
        }

        private IEnumerator InvincibilityFrames()
        {
            m_Invincible = true;
            yield return new WaitForSeconds(invincibilityLength);
            m_Invincible = false;
        }

        public void Death()
        {
            SceneController.LoadScene("GameOverScene");
        }
    }
}