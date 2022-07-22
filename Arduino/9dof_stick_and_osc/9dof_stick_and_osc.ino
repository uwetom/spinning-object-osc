#include<Wire.h>
#include <SPI.h>
#include <SparkFunLSM9DS1.h>
#include <WiFi.h>
#include <ArduinoOSCWiFi.h>



float roll;
float pitch;
float heading;

LSM9DS1 imu;

#define PRINT_CALCULATED
#define PRINT_SPEED 250 // 250 ms between prints
static unsigned long lastPrint = 0; // Keep track of print time

// http://www.ngdc.noaa.gov/geomag-web/#declination
#define DECLINATION 0.19 // Declination (degrees) in Bristol, UK


const char* ssid     = "Deputised Object";
const char* password = "";

const int   rxPort   = 9000;
const int   txPort   = 8000;
int uptime = 0;


void calcAttitude(float ax, float ay, float az, float mx, float my, float mz);


void setup(){
  delay(10);

  Serial.begin(9600);

  Wire.begin();
  
  if (imu.begin() == false) // with no arguments, this uses default addresses (AG:0x6B, M:0x1E) and i2c port (Wire).
  {
    Serial.println("Failed to communicate with LSM9DS1.");
    Serial.println("Double-check wiring.");
    Serial.println("Default settings in this sketch will " \
                   "work for an out of the box LSM9DS1 " \
                   "Breakout, but may need to be modified " \
                   "if the board jumpers are.");
    while (1);
  }

  imu.settings.temp.enabled = false;

  delay(1000);
 
  Serial.println("Done!\n");

  setupWIFI();
}

void loop(){
 
  if ( imu.gyroAvailable() )
  {
      imu.readGyro();
  }
  
  if ( imu.accelAvailable() )
  {
     imu.readAccel();
  }
  
  if ( imu.magAvailable() )
  {
    imu.readMag();
  }

  calcAttitude(imu.ax, imu.ay, imu.az,
                  -imu.my, -imu.mx, imu.mz);
                  
  OscWiFi.update();  // must be called to receive + send osc

  OscWiFi.send("255.255.255.255", txPort, "/pos/", roll,pitch,heading);
  
  delay(50);
}



void setupWIFI(){
     // create and broadcast access point ssid
    WiFi.mode(WIFI_AP);
    WiFi.softAP(ssid, NULL);

    int tick = 0;
    while (++tick < 5) {
        delay(500);
        Serial.print(".");
    }

}


void calcAttitude(float ax, float ay, float az, float mx, float my, float mz)
{
  roll = atan2(ay, az);
  
  //pitch = atan2(-ax, sqrt(ay * ay + az * az));

  pitch = atan2(ax, az);

  if (my == 0)
    heading = (mx < 0) ? PI : 0;
  else
    heading = atan2(mx, my);

  heading -= DECLINATION * PI / 180;

  if (heading > PI) heading -= (2 * PI);
  else if (heading < -PI) heading += (2 * PI);

  // Convert everything from radians to degrees:
  heading *= 180.0 / PI;
  pitch *= 180.0 / PI;
  roll  *= 180.0 / PI;


  heading = heading + 180;

  roll = roll + 180;

   pitch = pitch + 180;

 //Serial.println(pitch);


 Serial.print(heading);
 Serial.print(" - ");
 Serial.print(pitch);
 Serial.print(" - ");
 Serial.println(roll);
 Serial.println(" ");
 
 
  
}
