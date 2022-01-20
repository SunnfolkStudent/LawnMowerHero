using UnityEngine;

namespace Player
{
    public class PlayerInput : MonoBehaviour
    {
        /*[HideInInspector]*/public Vector2 moveVector;
        [HideInInspector]public bool attack;
        [HideInInspector] public bool pause;
        private PlayerActionInputs m_Inputs;

        private void Awake()
        {
            m_Inputs = new PlayerActionInputs();
        }

        private void Update()
        {
            moveVector = m_Inputs.Player.Move.ReadValue<Vector2>();
            attack = m_Inputs.Player.Attack.triggered;
            pause = m_Inputs.Player.Pause.triggered;
        }
        private void OnEnable()
        {
            m_Inputs.Enable();
        }

        private void OnDisable()
        {
            m_Inputs.Disable();
        }
    }
}
