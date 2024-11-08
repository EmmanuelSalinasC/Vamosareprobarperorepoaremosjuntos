#include <Servo.h>

const int analogin = A0;
const int ServoPin = 9;
Servo Servo1;

void setup() {
  Serial.begin(9600);
  Servo1.attach(ServoPin);
  Servo1.write(90);
}

void loop() {
  if (Serial.available() > 0) {
    String command = Serial.readStringUntil('\n');

    if (command == "1") {
      Servo1.write(0);
    } else if (command == "0") {
      Servo1.write(90);
    } else if (command == "GET_TEMP") {
      float temperature = readTemperature();
      Serial.print("Temperatura: ");
      Serial.println(temperature);
    }
  }
  
  delay(500);
}

float readTemperature() {
  int RawValue = analogRead(analogin);
  float voltage = (RawValue / 1023.0) * 5000;
  float tempC = voltage * 0.1;
  return tempC;
}