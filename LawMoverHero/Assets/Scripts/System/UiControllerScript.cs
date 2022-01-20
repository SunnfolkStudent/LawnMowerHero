using UnityEngine;
using UnityEngine.UI;

namespace System
{
    public class UiControllerScript : MonoBehaviour
    {
        public Button[] buttons;
        private int m_ButtonNumber;
        private PlayerActionInputs m_Input;
        private bool m_Up;
        private bool m_Down;
        private bool m_Activate;

        private void Awake()
        {
            m_Input = new PlayerActionInputs();
            //buttons[m_ButtonNumber].GetComponent<Image>().color =Color.yellow;
        }

        private void Update()
        {
            buttons[m_ButtonNumber].image.sprite = SceneController.isGamepad ? buttons[m_ButtonNumber].spriteState.highlightedSprite : buttons[m_ButtonNumber].spriteState.disabledSprite;
            m_Up = m_Input.UI.Up.triggered;
            m_Down = m_Input.UI.Down.triggered;
            m_Activate = m_Input.UI.ActivateButton.triggered;
            if (m_Activate)
            {
                buttons[m_ButtonNumber].onClick.Invoke();
            }
            if (m_Up)
            {
                buttons[m_ButtonNumber].image.sprite = buttons[m_ButtonNumber].spriteState.disabledSprite;
                if (m_ButtonNumber > 0)
                {
                    m_ButtonNumber--;
                }
                else
                {
                    m_ButtonNumber = buttons.Length - 1;
                }

            }
            if (m_Down)
            {
                buttons[m_ButtonNumber].image.sprite = buttons[m_ButtonNumber].spriteState.disabledSprite;
                if (m_ButtonNumber < buttons.Length -1)
                {
                    m_ButtonNumber++;
                }
                else
                {
                    m_ButtonNumber = 0;
                }
            }
        }
    
        private void OnEnable()
        {
            m_Input.Enable();
        }

        private void OnDisable()
        {
            m_Input.Disable();
        }
    }
}
