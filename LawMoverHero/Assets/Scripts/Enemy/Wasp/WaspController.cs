using System.Collections;
using Player;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Enemy.Wasp
{
    public class WaspController : WaspIdle
    {
        private float m_CooldownTime;
        public float minCoolDown = 1;
        public float maxCooldown = 5;
        private Vector2 m_KnockbackDir;
        public float knockbackForce = 2f;
        public float knockbackLength = 2f;
        public float stunTime = 0.2f;
        private Rigidbody2D m_Rigidbody2D;
        public int attackDamage = 1;
        public bool dontPickPoint;
        private bool m_PlayOneTime;
        

        private void Start()
        {
            moveToPosition =grassTilemap.WorldToCell(transform.position);
            m_Rigidbody2D = GetComponent<Rigidbody2D>();
        }


        private void OnTriggerStay2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            targetPlayer = true;
            moveToPosition = grassTilemap.WorldToCell(other.transform.position);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            targetPlayer = false;
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.collider.CompareTag("HurtBox"))
            {
                dead = true;
            }
            else
            {
                if (dead) return;
                var health = other.gameObject.GetComponent<PlayerController>();
                health.DecreaseHealth(attackDamage);
                var cellPos = grassTilemap.WorldToCell(transform.position);
                m_KnockbackDir = cellPos - moveToPosition;
                m_Rigidbody2D.AddForce(Vector2.ClampMagnitude(m_KnockbackDir * knockbackForce, knockbackForce), ForceMode2D.Impulse);
                StartCoroutine(KnockbackDelay());
            }
        }

        private void Update()
        {
            if (dead)
            {
                PlayAnim("Wasp_death");
                if (m_PlayOneTime) return;
                audioSource.PlayOneShot(deathSound);
                m_PlayOneTime = true;
            }
            if (yMoveN)
            {
                PlayAnim("Wasp_front'");
                
            }
            if (yMoveP)
            {
                PlayAnim("Wasp_back");
                
            }
            if (xMoveN && !yMove)
            {
                PlayAnim("Wasp_left");
                
            }
            if (xMoveP && !yMove)
            {
                PlayAnim("Wasp_right");
                
            }
           
            

            if (!targetPlayer && moveToPosition == transform.position - new Vector3(0.5f, 0.5f) && !moving && !dontPickPoint)
            {
                moving = true;
                m_CooldownTime = Random.Range(minCoolDown, maxCooldown);
                StartCoroutine(CoolDown());
            }

            if (xMove && yMove)
            {
                //actualMoveSpeed = moveSpeed / 1.5f;
            }
            else
            {
                actualMoveSpeed = moveSpeed;
            }
        }

        public void PlayDeathSound()
        {
            
        }

        private IEnumerator CoolDown()
        {
            yield return new WaitForSeconds(m_CooldownTime);
            PickPoint();
        }

        private void FixedUpdate()
        {
            GridMove();
        }

        private IEnumerator KnockbackDelay()
        {
            yield return new WaitForSeconds(knockbackLength);
            m_Rigidbody2D.velocity = Vector2.zero;
            stunned = true;
            yield return new WaitForSeconds(stunTime);
            stunned = false;
        }
    }
}