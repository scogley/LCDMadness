const int TxPin = 22;
int incomingByte = 0;   // for incoming serial data
//String incomingString;

#include <SoftwareSerial.h>
SoftwareSerial mySerial = SoftwareSerial(255, TxPin);


void setup()
{
  pinMode(TxPin, OUTPUT);
  digitalWrite(TxPin, HIGH);
  Serial.begin(9600);  //Sets up communication with the serial monitor
  mySerial.begin(9600);
  delay(100);
  mySerial.write(12);                 // Clear             
  mySerial.write(17);                 // Turn backlight on
  delay(5);     // Required delay
  
  // The incoming String built up one byte at a time.
//  incomingString = "";
}

void loop()
{
if (Serial.available()>0) // checks for a character in the serial monitor
{ 
// Read a byte from the serial buffer.
char incomingByte = (char)Serial.read();
//incomingString += incomingByte;

// read the incoming byte:
// incomingByte = Serial.read();
  
  
switch (incomingByte)
{
  case 94:
  mySerial.write(12);                 // Clear = ^ key
  break;
  
  case 64:
  mySerial.write(18);                 // Turn backlight off = @ key
  break;
  
  case 69:
  mySerial.write(227);                 // play E = E key
  break;
  
  case 62:
  mySerial.write(13);                 // Form feed = > key
  
  case 126:
  mySerial.write(17);                 // Turn backlight on = ~ key
  break;
  
  default:
  // say what you got:
  Serial.print("I received: ");
  Serial.println(incomingByte, DEC);  
  mySerial.print(incomingByte);
  
}
  
  
  
//  if (incomingByte == 48)
//{ 
//  mySerial.write(12);                 // Clear   
//}

//if (incomingByte == 49)
//{
// mySerial.write(18);                 // Turn backlight off
//}
//
//if (incomingByte == 50)
//{
//  mySerial.write(17);                 // Turn backlight on
//}




//// Checks for null termination of the string.
//if (incomingByte == '\0') 
//{
//  mySerial.print(incomingString);
//  incomingString = "";
//}

}
}
