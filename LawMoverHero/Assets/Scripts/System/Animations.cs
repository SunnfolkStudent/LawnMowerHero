using UnityEngine;

namespace System
{
    public class Animations : MonoBehaviour
    {
        public Animator animator;

        protected void PlayAnim(string animationToPlay)
        {
            animator.Play(animationToPlay);
        }
    }
}