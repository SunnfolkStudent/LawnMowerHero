using System;
using System.Collections;
using Player;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

namespace Enemy
{
    public class BossController : EnemyMovement
    {
        public bool velvetBoss;
        public float knockbackForce = 5;
        public float knockbackDuration = 1.5f;
        public float health = 3;
        public PlayerController playerController;
        public GameObject player;
        private bool m_IsHurt;
        public float moveSpeed = 1;
        public float moveBuffer = 2;
        public float attackSpeed = 10;
        private bool m_MoveUp;
        public Rigidbody2D rigidbody2Dyes;
        private bool m_IsAttacking;
        private bool m_Returning;
        public float chargeDistance = 0.5f;
        public float attackDistance = 0.5f;
        public float minWaitTime = 1;
        public float maxWaitTime = 5;
        private bool m_WaitingForAttack;
        private bool m_IsDead;
        public AnimationClip frontAnim;
        public AnimationClip leftAnim;
        public AnimationClip rightAnim;
        public AnimationClip deathAnim;


        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer != LayerMask.NameToLayer("Player") || m_IsHurt) return;
            rigidbody2Dyes.velocity = Vector2.zero;
            playerController.Knockback(knockbackForce, knockbackDuration);
            if (m_IsDead) return;
            playerController.DecreaseHealth(1);
        }
        

        private void FixedUpdate()
        {
            if (m_MoveUp)
            {
                transform.Translate(Vector2.up * moveSpeed * Time.deltaTime);
            }
            if (health <= 0)
            {
                m_IsDead = true;
                animator.Play(deathAnim.name);
                playerController.stopEverything = false;
            }
            if (m_IsAttacking || m_IsDead) return;
            if (transform.position.x < player.transform.position.x - moveBuffer)
            {
                animator.Play(rightAnim.name);
                if (velvetBoss) return;
                transform.Translate(Vector2.right * moveSpeed * Time.deltaTime);
            }
            else if (transform.position.x > player.transform.position.x + moveBuffer)
            {
                animator.Play(leftAnim.name);
                if (velvetBoss) return;
                transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);
            }
            else
            {
                animator.Play(frontAnim.name);
                if (!m_WaitingForAttack && !velvetBoss)
                {
                    StartCoroutine(WaitForRandomAmount());
                    m_WaitingForAttack = true;
                }
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (!other.collider.CompareTag("HurtBox")) return;
            playerController.stopEverything = true;
            m_IsHurt = true;
            health--;
            StartCoroutine(FlashRed());
        }

        private IEnumerator WaitForRandomAmount()
        {
            var randomWaitTime = Random.Range(minWaitTime, maxWaitTime);
            yield return new WaitForSeconds(randomWaitTime);
            StartCoroutine(Attack());
        }
        private IEnumerator Attack()
        {
            m_WaitingForAttack = false;
            m_IsAttacking = true;
            animator.Play(frontAnim.name);
            m_MoveUp = true;
            yield return new WaitForSeconds(chargeDistance);
            m_MoveUp = false;
            yield return new WaitForSeconds(0.5f);
            rigidbody2Dyes.AddForce(Vector2.down * attackSpeed, ForceMode2D.Impulse);
            yield return new WaitForSeconds(attackDistance);
            rigidbody2Dyes.velocity = Vector2.zero;
            yield return new WaitForSeconds(0.3f);
            StartCoroutine(ReturnToStart());
        }

        private IEnumerator ReturnToStart()
        {
            while (transform.position.y < 2.5f && !m_IsDead)
            {
                transform.Translate(Vector2.up * moveSpeed * Time.deltaTime);
                yield return new WaitForFixedUpdate();
            }
            SnapToGrid(false);
            m_IsAttacking = false;
        }

        private IEnumerator FlashRed()
        {
            var sprite = GetComponent<SpriteRenderer>();
            sprite.color = new Color(1, 0.5f, 0.5f);
            yield return new WaitForSeconds(0.15f);
            rigidbody2Dyes.velocity = Vector2.zero;
            playerController.Knockback(knockbackForce, knockbackDuration);
            playerController.stopEverything = false;
            m_IsHurt = false;
            sprite.color = Color.white;
            while (transform.position.y > 2.5f && !m_IsDead)
            {
                transform.Translate(Vector2.down * moveSpeed * Time.deltaTime);
                yield return new WaitForFixedUpdate();
            }
            SnapToGrid(false);
            //StartCoroutine(Attack());
        }
    }
}