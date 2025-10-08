//This is NOT for unity. just wanted to have a copy of the arduino code in the files


#include <Keyboard.h>
#include <Mouse.h>

// Joysticks
const int joy1X = A0;
const int joy1Y = A1;
const int joy2X = A2;
const int joy2Y = A3;

// Buttons
const int btn1 = 2;  // Left click
const int btn2 = 3;  // Space
const int btn3 = 4;  // Z
const int btn4 = 5;  // X

// Deadzone to prevent noise
const int deadzone = 200;

// Last known button states
bool btn1State = false;
bool btn2State = false;
bool btn3State = false;
bool btn4State = false;

void setup() {
  pinMode(btn1, INPUT_PULLUP);
  pinMode(btn2, INPUT_PULLUP);
  pinMode(btn3, INPUT_PULLUP);
  pinMode(btn4, INPUT_PULLUP);

  Keyboard.begin();
  Mouse.begin();
}

void loop() {
  // --- Joystick 1: WASD ---
  int x1 = analogRead(joy1X) - 512;
  int y1 = analogRead(joy1Y) - 512;

  // X-axis (A/D)
  if (x1 < -deadzone) Keyboard.press('a');
  else Keyboard.release('a');
  if (x1 > deadzone) Keyboard.press('d');
  else Keyboard.release('d');

  // Y-axis (W/S)
  if (y1 < -deadzone) Keyboard.press('w');
  else Keyboard.release('w');
  if (y1 > deadzone) Keyboard.press('s');
  else Keyboard.release('s');

  // --- Joystick 2: Arrow Keys ---
  int x2 = analogRead(joy2X) - 512;
  int y2 = analogRead(joy2Y) - 512;

  if (x2 < -deadzone) Keyboard.press(KEY_LEFT_ARROW);
  else Keyboard.release(KEY_LEFT_ARROW);
  if (x2 > deadzone) Keyboard.press(KEY_RIGHT_ARROW);
  else Keyboard.release(KEY_RIGHT_ARROW);

  if (y2 < -deadzone) Keyboard.press(KEY_UP_ARROW);
  else Keyboard.release(KEY_UP_ARROW);
  if (y2 > deadzone) Keyboard.press(KEY_DOWN_ARROW);
  else Keyboard.release(KEY_DOWN_ARROW);

  // --- Buttons ---
  bool currentBtn1 = !digitalRead(btn1);
  bool currentBtn2 = !digitalRead(btn2);
  bool currentBtn3 = !digitalRead(btn3);
  bool currentBtn4 = !digitalRead(btn4);

  // Button 1 - Left Click
  if (currentBtn1 && !btn1State) Mouse.press(MOUSE_LEFT);
  if (!currentBtn1 && btn1State) Mouse.release(MOUSE_LEFT);
  btn1State = currentBtn1;

  // Button 2 - Space
  if (currentBtn2) Keyboard.press(' ');
  else Keyboard.release(' ');

  // Button 3 - Z
  if (currentBtn3) Keyboard.press('z');
  else Keyboard.release('z');

  // Button 4 - X
  if (currentBtn4) Keyboard.press('x');
  else Keyboard.release('x');

  delay(20);
}

