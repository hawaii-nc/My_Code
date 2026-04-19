#include <SoftwareSerial.h>
#include "RoboClaw.h"

// SoftwareSerial(RX, TX)
SoftwareSerial serial(18, 19);
RoboClaw roboclaw(&serial, 10000);

#define address 128

void setup() {
  Serial.begin(38400);        // Serial Monitor
  serial.begin(38400);        // RoboClaw baud rate
  roboclaw.begin(38400);

  Serial.println("Ready");
}

void loop() {

  if (Serial.available()) {
    char c = Serial.read();

    if (c == 'f') {   // Forward
      roboclaw.ForwardM1(address, 64);
      Serial.println("Motor Forward");
    }

    else if (c == 'b') {  // Backward
      roboclaw.BackwardM1(address, 64);
      Serial.println("Motor Backward");
    }

    else if (c == 's') {  // Stop
      roboclaw.ForwardM1(address, 0);
      Serial.println("Motor Stopped");
    }
  }
}
