#include <SoftwareSerial.h>

#define PIN_SCI_TX 10
#define PIN_SCI_RX 11

// TODO: Handle SCI baud rate change
//    1. Intercept baud rate command
//    2. Intercept reply from PCM
//    3. If reply success: change sciSerial baud rate

SoftwareSerial sciSerial(PIN_SCI_RX, PIN_SCI_RX, true);

void setup() {
  Serial.begin(115200);
  sciSerial.begin(7812);
}

// int rxIndex = 0;
// int txIndex = 0;
// uint8_t rxBuffer[32];
// uint8_t txBuffer[32];
uint8_t data;

void loop() {
  if (Serial.available()) {
    data = Serial.read();
    sciSerial.write(data);
  }

  if (sciSerial.available()) {
    data = sciSerial.read();
    Serial.write(data);
  }
}