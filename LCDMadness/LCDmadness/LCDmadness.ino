const int TxPin = 22;

#include <SoftwareSerial.h>
SoftwareSerial mySerial = SoftwareSerial(255, TxPin);

void setup() {
    
  pinMode(TxPin, OUTPUT);
  digitalWrite(TxPin, HIGH);
  
  mySerial.begin(9600);
  delay(100);
  mySerial.write(12);                 // Clear             
  mySerial.write(17);                 // Turn backlight on
  delay(5);                           // Required delay
  mySerial.print("LCD Madness");  // First line
  mySerial.write(13);                 // Form feed
  mySerial.print("version 1.0");   // Second line
  delay(3000);                           // Required delay
  mySerial.write(12);                 // Clear  
  mySerial.print("Hello World"); // display
  mySerial.write(13);                 // Form feed
  mySerial.print("I am alive"); // display
  delay(5000);                        // Wait 5 seconds
  
  mySerial.print("That's it!");   // Second line
  delay(5000);                        // Wait 5 seconds
  
  
  mySerial.write(12);                 // Clear 
  mySerial.write(18);                 // Turn backlight off

}

void loop() {
    
  
}
