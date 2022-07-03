#include<Wire.h>
#include <math.h>
#include <WiFi.h>
#include <ArduinoOSCWiFi.h>

const int MPU=0x68;
int16_t AcX,AcY,AcZ,Tmp,GyX,GyY,GyZ;
double pitch,roll;


int AcXoff,AcYoff,AcZoff,GyXoff,GyYoff,GyZoff;

const char* ssid     = "cpc01";
const char* password = "";

const int   rxPort   = 9000;
const int   txPort   = 8000;
int uptime = 0;

void setup(){
  delay(10);
  Wire.begin();
  Wire.beginTransmission(MPU);
  Wire.write(0x6B);
  Wire.write(0x00);
  Wire.endTransmission(true);
  Serial.begin(9600);
  
  calculate_IMU_error();

  setupWIFI();
}

void loop(){
  Wire.beginTransmission(MPU);
  Wire.write(0x3B);
  Wire.endTransmission(false);
  Wire.requestFrom(MPU,14,true);
  
 
  int temp,toff;
  double t,tx,tf;
  
  
  
  //read accel data
  AcX=(Wire.read()<<8|Wire.read()) + AcXoff;
  AcY=(Wire.read()<<8|Wire.read()) + AcYoff;
  AcZ=(Wire.read()<<8|Wire.read()) + AcYoff;
  
  //read gyro data
  GyX=(Wire.read()<<8|Wire.read()) + GyXoff;
  GyY=(Wire.read()<<8|Wire.read()) + GyYoff;
  GyZ=(Wire.read()<<8|Wire.read()) + GyZoff;
  
  //get pitch/roll
  getAngle(AcX,AcY,AcZ);
  
  //send the data out the serial port
  Serial.print("Angle: ");
  Serial.print("Pitch = "); Serial.print(pitch);
  Serial.print(" | Roll = "); Serial.println(roll);
  
 
  Serial.println(" ");

    OscWiFi.update();  // must be called to receive + send osc

    Serial.print("IP address: ");
    Serial.println(WiFi.softAPIP());

    Serial.print("Uptime: ");
    Serial.print(++uptime, DEC);
    Serial.println();

    //String sendString = String(pitch);
   // sendString.concat(" ");
   // sendString.concat(roll);
    OscWiFi.send("255.255.255.255", txPort, "/pos/", pitch,roll);
    


  delay(50);
}

//convert the accel data to pitch/roll
void getAngle(int Vx,int Vy,int Vz) {
  double x = Vx;
  double y = Vy;
  double z = Vz;
  
  pitch = atan(x/sqrt((y*y) + (z*z)));
  roll = atan(y/sqrt((x*x) + (z*z)));
  //convert radians into degrees
  pitch = pitch * (180.0/3.14);
  roll = roll * (180.0/3.14) ;
}


void calculate_IMU_error() {
  // We can call this funtion in the setup section to calculate the accelerometer and gyro data error. From here we will get the error values used in the above equations printed on the Serial Monitor.
  // Note that we should place the IMU flat in order to get the proper values, so that we then can the correct values
  // Read accelerometer values 200 times
int c = 0;
  float AccErrorX, AccErrorY, GyroErrorX, GyroErrorY, GyroErrorZ;
  float AccX, AccY, AccZ;
  float GyroX, GyroY, GyroZ;
  


  while (c < 200) {
    Wire.beginTransmission(MPU);
    Wire.write(0x3B);
    Wire.endTransmission(false);
    Wire.requestFrom(MPU, 6, true);
    AccX = (Wire.read() << 8 | Wire.read()) / 16384.0 ;
    AccY = (Wire.read() << 8 | Wire.read()) / 16384.0 ;
    AccZ = (Wire.read() << 8 | Wire.read()) / 16384.0 ;
    // Sum all readings
    AccErrorX = AccErrorX + ((atan((AccY) / sqrt(pow((AccX), 2) + pow((AccZ), 2))) * 180 / PI));
    AccErrorY = AccErrorY + ((atan(-1 * (AccX) / sqrt(pow((AccY), 2) + pow((AccZ), 2))) * 180 / PI));
    c++;
  }
  //Divide the sum by 200 to get the error value
  AccErrorX = AccErrorX / 200;
  AccErrorY = AccErrorY / 200;
  c = 0;
  // Read gyro values 200 times
  while (c < 200) {
    Wire.beginTransmission(MPU);
    Wire.write(0x43);
    Wire.endTransmission(false);
    Wire.requestFrom(MPU, 6, true);
    GyroX = Wire.read() << 8 | Wire.read();
    GyroY = Wire.read() << 8 | Wire.read();
    GyroZ = Wire.read() << 8 | Wire.read();
    // Sum all readings
    GyroErrorX = GyroErrorX + (GyroX / 131.0);
    GyroErrorY = GyroErrorY + (GyroY / 131.0);
    GyroErrorZ = GyroErrorZ + (GyroZ / 131.0);
    c++;
  }
  //Divide the sum by 200 to get the error value
  GyroErrorX = GyroErrorX / 200;
  GyroErrorY = GyroErrorY / 200;
  GyroErrorZ = GyroErrorZ / 200;
  // Print the error values on the Serial Monitor
  
  AcXoff = AccErrorX;
  AcYoff = AccErrorY;
  AcZoff = 0;
  GyXoff = GyroErrorX;
  GyYoff = GyroErrorY;
  GyZoff = GyroErrorZ;


}

void setupWIFI(){
     // create and broadcast access point ssid
    WiFi.mode(WIFI_AP);
    WiFi.softAP(ssid, NULL);

    Serial.println();
    Serial.println();
    Serial.print("Broadcasting as WiFi SSID: ");
    Serial.println(ssid);

    int tick = 0;
    while (++tick < 5) {
        delay(500);
        Serial.print(".");
    }

    Serial.print("Listening for OSC on port: ");
    Serial.println(rxPort, DEC);
    OscWiFi.subscribe(rxPort, "/ping", onPing);
    
    Serial.print("Transmitting OSC on port: ");
    Serial.println(txPort, DEC);
    OscWiFi.send("255.255.255.255", txPort, "/loadbang");

   
}

