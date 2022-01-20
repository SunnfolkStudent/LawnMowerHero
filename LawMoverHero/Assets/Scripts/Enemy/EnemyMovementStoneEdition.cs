using UnityEngine;

namespace Enemy
{
    public class EnemyMovementStoneEdition: MonoBehaviour
    {
        public float moveSpeed = 1f;
        public float lungeBreak = 1f;
        private Vector2 movement;

        private Rigidbody2D _Rigidbody2D;
    
        public Transform player;

        private void Start()
        {
            _Rigidbody2D = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            Vector3 direction = player.position - transform.position;
            Debug.Log(direction);
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            //_Rigidbody2D.rotation = angle;
            direction.Normalize();
            movement = direction;
        }

        private void FixedUpdate()
        {
            MoveCharacter(movement);
        }

        private void MoveCharacter(Vector2 direction)
        {
            _Rigidbody2D.MovePosition((Vector2)transform.position + (direction * moveSpeed * Time.deltaTime));
        }
    }
}
