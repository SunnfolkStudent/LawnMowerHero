using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Enemy
{
    public class EnemyMovement : Animations
    {
        public AudioClip deathSound;
        public AudioSource audioSource;
        public Tilemap grassTilemap;
        protected void SnapToGrid(bool isX)
        {
            var currentTransform = transform;
            var position = currentTransform.position;
            var cellNumber = grassTilemap.WorldToCell(position) + new Vector3(0.5f, 0.5f);
            position = !isX ? new Vector3(position.x, cellNumber.y) : new Vector3(cellNumber.x, position.y);
            currentTransform.position = position;
        }

        public void Die()
        {
            Destroy(gameObject);
        }
    }
}
