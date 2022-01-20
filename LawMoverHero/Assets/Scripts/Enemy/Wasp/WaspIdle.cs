using UnityEngine;

namespace Enemy.Wasp
{
    public class WaspIdle : WaspFollowPlayer
    {
        public GameObject upperRight;
        public GameObject lowerLeft;
        private Vector3 m_RPoint;
        protected bool moving;
        


        protected void PickPoint()
        {
            var lLPosition = lowerLeft.transform.position;
            var uRPosition = upperRight.transform.position;
            m_RPoint.x = Random.Range(lLPosition.x, uRPosition.x);
            m_RPoint.y = Random.Range(lLPosition.y, uRPosition.y);
            moveToPosition = grassTilemap.WorldToCell(m_RPoint);
            moving = false;
        }

        
    }
}