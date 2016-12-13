using UnityEngine;

public class InputManager {
    public ButtonState attackButton = new ButtonState("Fire1");
    public ButtonState dashButton = new ButtonState("Fire2");
    public ButtonState swirlButton = new ButtonState("Fire3");

    public class ButtonState {
        private string name;

        public bool wasPressed = false;
        public bool isDown = false;
        public bool wasReleased = false;
        private bool pressRegistered = false;
        
        public ButtonState(string name) {
            this.name = name;
        }

        public void Update() {
            wasPressed = Input.GetButtonDown(name);
            isDown = Input.GetButton(name);

            if (isDown && !pressRegistered) {
                wasPressed = true;
            }

            if (wasPressed) {
                pressRegistered = true;
            }

            if (pressRegistered && !isDown) {
                wasReleased = true;
                pressRegistered = false;
            }
        }
    }


    public void Update() {
        attackButton.Update();
        dashButton.Update();
        swirlButton.Update();
    }
}
