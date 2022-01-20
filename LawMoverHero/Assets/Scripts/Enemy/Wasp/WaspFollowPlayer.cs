using UnityEngine;

namespace Enemy.Wasp
{
    public class WaspFollowPlayer : EnemyMovement
    {
        public Vector3 moveToPosition;
        public float moveSpeed;
        protected float actualMoveSpeed;
        public float marginOfError = 0.47f;
        private Vector3 m_PositionNOffset;
        private Vector3 m_PositionPOffset;
        public bool xMoveN;
        public bool xMoveP;
        public bool yMoveN;
        public bool yMoveP;
        protected bool targetPlayer;
        public bool xMove;
        public bool yMove;
        protected bool stunned;
        protected bool dead;

        protected void GridMove()
        {
            if (stunned || dead) return;
            var position = transform.position;
            m_PositionNOffset = position - new Vector3(marginOfError, marginOfError);
            m_PositionPOffset = position + new Vector3(marginOfError, marginOfError);
            MoveY();
            MoveX();
        }
        
        private void MoveY()
        {
            if (!xMove)
            {
                SnapToGrid(true);
            }
            if (grassTilemap.WorldToCell(m_PositionNOffset).y < moveToPosition.y)
            {
                yMoveP = true;
                transform.Translate(new Vector2(0, 1) * actualMoveSpeed * Time.deltaTime);
            }
            else
            {
                yMoveP = false;
            }

            if (grassTilemap.WorldToCell(m_PositionPOffset).y > moveToPosition.y)
            {
                yMoveN = true;
                transform.Translate(new Vector2(0, -1) * actualMoveSpeed * Time.deltaTime);
            }
            else
            {
                yMoveN = false;
            }

            if (!yMoveP && !yMoveN)
            {
                yMove = false;
                SnapToGrid(false);
            }
            else
            {
                yMove = true;
            }
        }

        private void MoveX()
        {
            if(yMove) return;
            SnapToGrid(false);
            if (grassTilemap.WorldToCell(m_PositionNOffset).x < moveToPosition.x)
            {
                xMoveP = true;
                transform.Translate(new Vector2(1, 0) * actualMoveSpeed * Time.deltaTime);
            }
            else
            {
                xMoveP = false;
            }

            if (grassTilemap.WorldToCell(m_PositionPOffset).x > moveToPosition.x)
            {
                xMoveN = true;
                transform.Translate(new Vector2(-1, 0) * actualMoveSpeed * Time.deltaTime);
            }
            else
            {
                xMoveN = false;
            }

            if (!xMoveP && !xMoveN)
            {
                xMove = false;
                SnapToGrid(true);
            }
            else
            {
                xMove = true;
            }
        }
    }
}