#include <SoftwareSerial.h>

#define PIN_SCI_TX 10
#define PIN_SCI_RX 11

SoftwareSerial sciSerial(PIN_SCI_RX, PIN_SCI_RX, true);

const int bufferSize = 16;
uint8_t inputBuffer[bufferSize];
uint8_t sciBuffer[bufferSize];

int numRead;
int numReadSci;

const uint8_t highSpeedCommand = 0x12;
bool highSpeedRequested = false;
bool highSpeedConfirmed = false;

const uint8_t lowSpeedCommand = 0xFE;
bool lowSpeedRequested = false;
bool lowSpeedConfirmed = false;

const uint32_t highSpeedBaud = 62500;
const uint32_t lowSpeedBaud = 7812;

void handleBaudChange();
bool handleBaudRequest();

void setup() {
  Serial.begin(115200);
  sciSerial.begin(lowSpeedBaud);

  pinMode(LED_BUILTIN, OUTPUT);
  
  for (int i = 0; i <= 6; i++) {
    digitalWrite(LED_BUILTIN, i % 2);
    delay(500);
  }
}

unsigned long long timeLastAvailable;

void loop() {
  handleBaudChange();

  if (Serial.available()) {
    numRead = Serial.readBytes(inputBuffer, bufferSize);

    if (handleBaudRequest()) {
      return;
    }

    sciSerial.write(inputBuffer, bufferSize);
    delay(1);

    while (!sciSerial.available()) {
      if (millis() - timeLastAvailable > 1000) // Time-out
        break;
      delay(1);
    }

    while (sciSerial.available()) {
      timeLastAvailable = millis();
      
      numReadSci = sciSerial.readBytes(sciBuffer, bufferSize);
      Serial.write(sciBuffer, bufferSize);

      if (highSpeedCommand == sciBuffer[0]) {
        highSpeedConfirmed = true;
      }
      else if (lowSpeedCommand == sciBuffer[0]) {
        lowSpeedConfirmed = true;
      }

      if (Serial.available()) { // Stop if data is available from serial port
        break;
      }
    }
  }
}

void handleBaudChange() {
  if (highSpeedRequested && highSpeedConfirmed) {
    sciSerial.end();
    sciSerial.begin(highSpeedBaud);
    
    digitalWrite(LED_BUILTIN, HIGH);

    highSpeedConfirmed = false;
  } 
  else if (lowSpeedRequested && lowSpeedConfirmed) {
    sciSerial.end();
    sciSerial.begin(lowSpeedBaud);

    digitalWrite(LED_BUILTIN, LOW);

    lowSpeedConfirmed = false;
  }
}

bool handleBaudRequest() {
  highSpeedRequested = highSpeedCommand == inputBuffer[0];
  lowSpeedRequested = lowSpeedCommand == inputBuffer[0];

  if (numRead == 2 && inputBuffer[1] == 0x01) {
    if (highSpeedRequested) {
      highSpeedConfirmed = true;
      return true;
    }
    else if (lowSpeedRequested) {
      lowSpeedConfirmed = true;
      return true;
    }
  }
  return false;
}
