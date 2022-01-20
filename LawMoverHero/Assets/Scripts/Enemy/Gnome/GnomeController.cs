using System;
using Player;
using UnityEngine;

namespace Enemy.Gnome
{
    public class GnomeController : EnemyMovement
    {
        public float moveSpeed = 1;
        public Vector2 direction = new(0, 1);
        public int attackDamage = 1;
        private bool m_Dead;

        private void Start()
        {
            PlayAnim4Directions("Gnome_back","Gnomefront","Gnome_left","Gnome_right");
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (m_Dead) return;
            switch (other.tag)
            {
                case "Up":
                    direction = new Vector2(0, 1);
                    SnapToGrid(true);
                    PlayAnim("Gnome_back");
                    break;
                case "Down":
                    direction = new Vector2(0, -1);
                    SnapToGrid(true);
                    PlayAnim("Gnomefront");
                    break;
                case "Left":
                    direction = new Vector2(-1, 0);
                    SnapToGrid(false);
                    PlayAnim("Gnome_left");
                    break;
                case "Right":
                    direction = new Vector2(1, 0);
                    SnapToGrid(false);
                    PlayAnim("Gnome_right");
                    break;
                case "HurtBox":
                    PlayDeathAnim();
                    
                    break;
            }
        }

        
        private void PlayAnim4Directions(string up, string down, string left, string right)
        {
            switch (direction.y)
            {
                case > 0:
                    PlayAnim(up);
                    break;
                case < 0:
                    PlayAnim(down);
                    break;
            }
            switch (direction.x)
            {
                case > 0:
                    PlayAnim(right);
                    break;
                case < 0:
                    PlayAnim(left);
                    break;
            }
        }
        private void OnCollisionEnter2D(Collision2D other)
        {
            if (m_Dead) return;
            var health = other.gameObject.GetComponent<PlayerController>();
            health.DecreaseHealth(attackDamage);
            GetComponent<BoxCollider2D>().enabled = false;
            PlayDeathAnim();
        }

        

        private void PlayDeathAnim()
        {
            m_Dead = true;
            audioSource.PlayOneShot(deathSound);
            PlayAnim4Directions("GnomeDeath_back","GnomeDeath_front","GnomeDeath_left","GnomeDeath_right");
        }
        

        private void FixedUpdate()
        {
            if (m_Dead) return;
            transform.Translate(direction * moveSpeed * Time.deltaTime);
        }
    }
}