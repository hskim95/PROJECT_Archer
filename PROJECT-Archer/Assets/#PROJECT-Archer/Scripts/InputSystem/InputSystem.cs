using UnityEngine;

namespace Archer
{
    public class InputSystem : SingletonBase<InputSystem>
    {
        public Vector2 Movement => movement;
        public Vector2 Look => look;

        public bool IsLeftShift => isLeftShift;

        private Vector2 movement;
        private Vector2 look;
        private bool isLeftShift;
        private bool isShowCursor = false;

        public System.Action OnClickSpace;
        public System.Action OnClickLeftMouseButtonDown;

        public System.Action OnClickRightMouseButtonDown;
        public System.Action OnClickRightMouseButtonUp;

        public System.Action OnClickInteract;
        public System.Action<float> OnMouseScrollWheel;

        protected override void Awake()
        {
            base.Awake();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                OnClickInteract?.Invoke();
            }

            if (Input.GetMouseButtonDown(0))
            {
                OnClickLeftMouseButtonDown?.Invoke();
            }

            if (Input.GetMouseButtonDown(1))
            {
                OnClickRightMouseButtonDown?.Invoke();
            }

            if (Input.GetMouseButtonUp(1))
            {
                OnClickRightMouseButtonUp?.Invoke();
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                OnClickSpace?.Invoke();
            }

            isShowCursor = Input.GetKey(KeyCode.LeftAlt);
            if (isShowCursor)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            float inputX = Input.GetAxis("Horizontal");
            float inputY = Input.GetAxis("Vertical");
            movement = new Vector2(inputX, inputY);

            float lookX = Input.GetAxis("Mouse X");
            float lookY = Input.GetAxis("Mouse Y");
            look = isShowCursor ? Vector2.zero : new Vector2(lookX * 1.5f, lookY);

            isLeftShift = Input.GetKey(KeyCode.LeftShift);

            float mouseScroll = Input.GetAxis("Mouse ScrollWheel");
            if (mouseScroll != 0)
            {
                OnMouseScrollWheel?.Invoke(mouseScroll);
            }
        }
    }
}

