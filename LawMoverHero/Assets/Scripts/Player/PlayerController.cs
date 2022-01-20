using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Player
{
    public class PlayerController : PlayerHealth
    {
        public float moveSpeed = 1;
        public Tilemap grassTilemap;
        public Tilemap wallTilemap;
        private Vector3 m_MinRange;
        private Vector3 m_MaxRange;
        public Vector3 cellNumber;
        private Rigidbody2D m_Rigidbody2D;
        private bool m_InGrid;
        public float marginOfError;
        public PlayerInput input;
        private bool m_CanTurn;
        private float m_TurnAmount;
        private bool m_IsX;
        private bool m_Stunned;
        public float knockbackForce = 2;
        public float stunTime = 0.5f;
        public float ramSpeed = 2;
        public float ramLength = 0.1f;
        [SerializeField]private bool m_CantRam;
        public float ramCooldown = 2;
        public float turnDelayAmount = 0.2f;
        public GameObject hurtBox;
        public TileBase[] gateTiles;
        private bool m_TurnWait;


        private void Start()
        {
            m_Rigidbody2D = GetComponent<Rigidbody2D>();
            health = healthSprites.Length - 1;
            PlayAnim("PlayerBack");
        }

        private void FixedUpdate()
        {
            if (m_Stunned|| stopEverything) return;
            transform.Translate(Vector2.up * moveSpeed * Time.deltaTime);
        }

        private void Update()
        {
            if (SceneController.gameIsPaused || stopEverything) return;
            mainBody.transform.position = transform.position;
            cellNumber = grassTilemap.WorldToCell(transform.position) + new Vector3(0.5f, 0.5f);
            if (input.attack && !m_CantRam)
            {
                Ram();
            }

            TurnWhenInGrid();
            switch (input.moveVector.y)
            {
                case < 0 when mDirection.y <= 0 && !m_CanTurn:
                    UpdateDirection();
                    if (mDirection == input.moveVector)
                    {
                        SetTurn(180, false);
                    }
                    break;
                case > 0 when mDirection.y >= 0 && !m_CanTurn:
                    UpdateDirection();
                    if (mDirection == input.moveVector)
                    {
                        SetTurn(0, false);
                    }
                    break;
            }

            switch (input.moveVector.x)
            {
                case < 0 when mDirection.x <= 0 && !m_CanTurn:
                    UpdateDirection();
                    if (mDirection == input.moveVector)
                    {
                        SetTurn(90, true);
                    }
                    break;
                case > 0 when mDirection.x >= 0 && !m_CanTurn:
                    UpdateDirection();
                    if (mDirection == input.moveVector)
                    {
                        SetTurn(270, true);
                    }
                    break;
            }
        }

        private void SetTurn(float turnAmount, bool isX)
        {
            if (m_CanTurn) return;
            m_CanTurn = true;
            m_TurnAmount = turnAmount;
            m_IsX = isX;
        }

        private void TurnWhenInGrid()
        {
            if (!m_CanTurn) return;
            m_MaxRange = cellNumber + new Vector3(marginOfError, marginOfError);
            m_MinRange = cellNumber - new Vector3(marginOfError, marginOfError);
            if (m_IsX)
            {
                if (transform.position.y < m_MaxRange.y && transform.position.y > m_MinRange.y && !m_TurnWait)
                {
                    Turn();
                }
            }
            else
            {
                if (transform.position.x < m_MaxRange.x && transform.position.x > m_MinRange.x && !m_TurnWait)
                {
                    Turn();
                }
            }
        }

        private void Turn()
        {
            if (stopEverything) return;
            var playerTransform = transform;
            playerTransform.rotation = Quaternion.Euler(0, 0, m_TurnAmount);
            var position = playerTransform.position;
            //m_Rigidbody2D.velocity = Vector2.zero;
            position = !m_IsX ? new Vector3(cellNumber.x, position.y) : new Vector3(position.x, cellNumber.y);
            if (stopEverything) return;
            PlayAnim4Directions("PlayerBack","PlayerFront","PlayerRight","PlayerLeft");
            playerTransform.position = position;
            m_CanTurn = false;
        }

        private IEnumerator TurnDelay()
        {
            yield return new WaitForSeconds(turnDelayAmount);
            print("can turn again");
        }

        private void UpdateDirection()
        {
            if (input.moveVector == Vector2.zero || m_CanTurn) return;
            if (input.moveVector.x == 0 && input.moveVector.y != 0 ||
                input.moveVector.x != 0 && input.moveVector.y == 0)
            {
                mDirection = input.moveVector;
            }
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            if (other.gameObject.layer != LayerMask.NameToLayer("Walls") || stopEverything) return;
            Knockback(knockbackForce, stunTime);
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            audioSource.PlayOneShot(collideSound);
        }

        public void Knockback(float force, float duration)
        {
            m_Rigidbody2D.velocity = Vector2.zero;
            var knockbackDir = -transform.up;
            m_Rigidbody2D.AddForce(Vector2.ClampMagnitude(knockbackDir * force, force), ForceMode2D.Impulse);
            StartCoroutine(StunDelay(duration));
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (stopEverything) return;
            grassTilemap.SetTile(grassTilemap.WorldToCell(transform.position), null);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Gate")) return;
            ScoreManager.levelHasStarted = true;
            for (var i = -1; i <= 1; i += 2)
            {
                var gateToDelete = other.transform.position + new Vector3(i, -0.5f);
                wallTilemap.SetTile(wallTilemap.WorldToCell(gateToDelete), null);
            }
            var increase =0;
            for (var i = -1; i <= 1; i ++)
            {
                var gatePos= wallTilemap.WorldToCell(other.transform.position + new Vector3(i, -1.5f));
                wallTilemap.SetTile(gatePos, gateTiles[increase++]);
            }
        }

        private IEnumerator StunDelay(float duration)
        {
            m_Stunned = true;
            yield return new WaitForSeconds(duration);
            m_Stunned = false;
            var position = transform.position;
            position = !m_IsX ? new Vector3(cellNumber.x, position.y) : new Vector3(position.x, cellNumber.y);
            transform.position = position;
            m_Rigidbody2D.velocity = Vector2.zero;
        }

        private void Ram()
        {
            if (stopEverything) return;
            PlayAnim4Directions("DashBack","DashFront","DashRight","DashLeft");
            hurtBox.SetActive(true);
            m_CantRam = true;
            m_TurnWait = true;
            m_Rigidbody2D.AddForce(mDirection * ramSpeed, ForceMode2D.Impulse);
            StartCoroutine(RamDelay());
        }

        

        private IEnumerator RamDelay()
        {
            if (stopEverything) yield break;
            yield return new WaitForSeconds(ramLength);
            m_TurnWait = false;
            m_Rigidbody2D.velocity = Vector2.zero;
            hurtBox.SetActive(false);
            if (!stopEverything)
            {
                PlayAnim4Directions("PlayerBack","PlayerFront","PlayerRight","PlayerLeft");
            }
            yield return new WaitForSeconds(ramCooldown);
            m_CantRam = false;
        }
    }
}