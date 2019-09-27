#include "Settings.h"
#include <SPI.h>
#include "ESP8266WiFi.h"
#include <WiFiUdp.h>
#include "TimeLib.h"
#include <EEPROM.h>

#define DEBUG
//#define TEMPERATURE
// led output for valve control
#define LED 14

// setup for temperature measurements
  // which analog pin to connect
  #define THERMISTORPIN A0         
  // resistance at 25 degrees C
  #define THERMISTORNOMINAL 10000      
  // temp. for nominal resistance (almost always 25 C)
  #define TEMPERATURENOMINAL 25   
  // how many samples to take and average
  #define NUMSAMPLES 5
  // The beta coefficient of the thermistor (usually 3000-4000)
  #define BCOEFFICIENT 3950
  // the value of the other resistor
  #define SERIESRESISTOR 10000
  
  WiFiServer server(SETTING_PORT);
  WiFiClient client;

// time stuff
  // local SETTING_PORT to listen for UDP packets
  const int localSETTING_PORT = 2390;
  // time.nist.gov NTP server address
  IPAddress timeServerIP;
  // NTP server - change for countries outside of europe
  const char* ntpServerName = "0.europe.pool.ntp.org";
  // NTP time stamp is in the first 48 bytes of the message
  const int NTP_PACKET_SIZE = 48;
  
  const unsigned long seventyYears = 2208988800UL;
  // how often refresh time - set to 10 days 
  const unsigned long refresh = 864000;
  
  const int numOfTries = 10; 
  int timeZone = 3600;
  time_t localTime;
  // time when we last checked time with server
  time_t lastCheckTime;

  WiFiUDP udp;

  const int SCHEDULE_SIZE = 1008;
  byte schedule [SCHEDULE_SIZE];
  byte scheduleAlternative [SCHEDULE_SIZE];
  byte valveValue;

// holidays
  int holYear = 1900, holMonth = 1, holDay = 1;
  byte holTemp = 6;

void setup() {
  Serial.begin(9600);

  connectToWifi();
  startServer();
  setUpTime();
  defaultSchedule();
  // time provider for TimeLib
  setSyncProvider(getTime);
  pinMode(LED, OUTPUT);

  // call if you want to reset id
  /*setDefaultId();*/
}

void loop() {
  delay(100);
  connectToClient();
  readFromClient();

  digitalWrite(LED, map(getValveOpening(), 0, 100, 0, 255));
}

// sets ESP to client mode a connects to defined WiFi
void connectToWifi() {
  #ifdef DEBUG
  Serial.print("Connecting to: ");
  Serial.println(SETTING_SSID);
  #endif
  // sets ESP8266 to be WiFi-client
  WiFi.mode(WIFI_STA);
  WiFi.begin(SETTING_SSID, SETTING_PASS);
  
  while (WiFi.status() != WL_CONNECTED) {
    #ifdef DEBUG
    Serial.print(".");
    #endif
    delay(500);
  }
  #ifdef DEBUG
  Serial.println("");
  Serial.println("Connected");
  Serial.print("IP adress: ");
  Serial.println(WiFi.localIP());
  #endif
}

void startServer() {
  #ifdef DEBUG
  Serial.print("Starting server on SETTING_PORT: ");
  Serial.println(SETTING_PORT);
  #endif
  server.begin();
}

// connects to client if one is available
// doesn't connect when client exists
void connectToClient() {
  if (client) {
    #ifdef DEBUG
    Serial.println("Client already connected");
    #endif
  }
  client = server.available();
  if (client) {
    if (client.connected()) {
      #ifdef DEBUG
      Serial.println("Client connected");
      #endif
    }
    else {
      #ifdef DEBUG
      Serial.println("Client not connected");
      #endif
    }
  }
}

void readFromClient() {
  while (client.available()) {
    char res = static_cast<char>(client.read());
    switch (res) {
    case 's' :
      receiveSchedule(schedule);
      break;
    case 'v' :
      receiveSchedule(scheduleAlternative);
      break;
    case 'a' :
      identification();
      break;
    case 'c' :
      changeId();
      break;
    case 'h' :
      setDateOfReturnAndTemperature();
      break;
    case 't' :
      getTemperature();
      break;
    }
  }
}

void sendToClient(char * data, size_t n) {
  client.write((uint8_t*) data, n);
}

/* Day = "0" | "1" | "2" | "3" | "4" | "5" | "6"
// Num 4 = "0" | "1" | "2" | "3"
// Num 5 = "0" | "1" | "2" | "3" | "4"
// Num 9 = "1" | "2" | "3" | "4" | "5" | "6" | "7" | "8" | "9"
// Num 10 = "0" | "1" | "2" | "3" | "4" | "5" | "6" | "7" | "8" | "9"
// Minutes = Num 10 | Num 9, Num 10 | "1", Num 4, Num 10 | | "1", "4", Num 5
// Temperature = Num 10 | Num 9, Num 10 | "1", Num 10, Num 10 | "200"
// Node = Day, Minutes, Temperature
// Message = {Node}, "7"
*/
void receiveSchedule(byte * sched) {
  #ifdef DEBUG
    Serial.println("Receiving new schedule");
  #endif
  
  byte day, _time, temp;
  byte lday = 0, ltime = 0;
  while (client.available()) {
    day = client.read();
    #ifdef DEBUG
      Serial.print("Day: ");
      Serial.println(day);
    #endif
    if (day == 7)
      break;

    if (client.available())
      _time = client.read();

    if (client.available())
      temp = client.read();

    #ifdef DEBUG
      Serial.print("Time: ");
      Serial.println(_time);
      Serial.print("Temp: ");
      Serial.println(temp);
    #endif
    memset(sched + lday * SCHEDULE_SIZE / 7 + ltime, temp, (day - lday) * SCHEDULE_SIZE / 7 + _time - ltime);

    lday = day;
    ltime = _time;
  }
}

void identification() {
  char data [] = "OKxx";
  
  EEPROM.begin(16);
  data[2] = (char) EEPROM.read(0);
  data[3] = (char) EEPROM.read(1);
  EEPROM.end();

  sendToClient(data, 4);
  #ifdef DEBUG
    Serial.print("identification send: ");
    Serial.print(data[2]);
    Serial.print(", ");
    Serial.println(data[3]);
  #endif
}

void changeId() {
  byte b0 = client.read();
  byte b1 = client.read();

  EEPROM.begin(16);
  EEPROM.put(0, b0);
  EEPROM.put(1, b1);
  EEPROM.end();

  #ifdef DEBUG
    Serial.print("ID changed to: ");
    Serial.print((char) b0);
    Serial.print(", ");
    Serial.println((char) b1);
  #endif
}

// message = "hDAY,MONTH,YEAR,TEMPERATURE"
void setDateOfReturnAndTemperature() {
  byte b;
  int day, month, year, temp;
  
  if (client.available()) {
    b = client.read();
    day = (b - '0') * 10;
  }
  if (client.available()) {
    b = client.read();
    day += b - '0';
  }

  if (client.available()) {
    b = client.read();
    month = (b - '0') * 10;
  }
  if (client.available()) {
    b = client.read();
    month += b - '0';
  }

  if (client.available()) {
    b = client.read();
    year = (b - '0') * 1000;
  }
  if (client.available()) {
    b = client.read();
    year = (b - '0') * 100;
  }
  if (client.available()) {
    b = client.read();
    year = (b - '0') * 10;
  }
  if (client.available()) {
    b = client.read();
    year += b - '0';
  }

  if (client.available()) {
    b = client.read();
    temp = (b - '0') * 10;
  }
  if (client.available()) {
    b = client.read();
    temp += b - '0';
  }
  else {
    Serial.println("setDateOfReturnAndTemperature() in wrong format");
  }

  holYear = year;
  holMonth = month;
  holDay = day;
  if (temp == 0){
    // temperature is not set -- heater will get temperature from alternative schedule
    holTemp = 255;
  }
  else {
    // temperatures saved are shifted by 10 degrees
    holTemp = temp * 10 - 100;
  }
}

void getTemperature() {
  // two decimal places
  char buff[6];

  snprintf(buff, sizeof(buff), "%.2f", measureTeperature());

  sendToClient(buff, sizeof(buff));

  #ifdef DEBUG
    Serial.print("Temperature send: ");
    Serial.println(buff);
  #endif
}

float measureTeperature() {
  uint8_t i;
  float average;
  uint16_t samples[NUMSAMPLES];
 
  // take N samples in a row, with a slight delay
  for (i=0; i< NUMSAMPLES; i++) {
   samples[i] = analogRead(THERMISTORPIN);
   delay(10);
  }

  // average all the samples out
  average = 0;
  for (i=0; i< NUMSAMPLES; i++) {
     average += samples[i];
  }
  average /= NUMSAMPLES;

  // convert the value to resistance
  average = 1023 / average - 1;
  average = SERIESRESISTOR / average;
 
  float steinhart;
  steinhart = average / THERMISTORNOMINAL;     // (R/Ro)
  steinhart = log(steinhart);                  // ln(R/Ro)
  steinhart /= BCOEFFICIENT;                   // 1/B * ln(R/Ro)
  steinhart += 1.0 / (TEMPERATURENOMINAL + 273.15); // + (1/To)
  steinhart = 1.0 / steinhart;                 // Invert
  steinhart -= 273.15;                         // convert to C
  
  return steinhart;
}

// tries to setup time until it succeeds
void setUpTime() {
  unsigned long res = getTime();
  while ( !res ) {
    res = getTime();
  }
}

time_t getTime() {
  return getTime(numOfTries);
}

// try to communicate tries times
// if communication fails return last time that was set
time_t getTime(int tries) {
  if ( localTime == 0 || localTime - refresh > lastCheckTime ) {
    udp.begin(localSETTING_PORT);
    byte packetBuffer[NTP_PACKET_SIZE];
    while (tries) {
      // get a random server from the pool
      WiFi.hostByName(ntpServerName, timeServerIP);
      // send an NTP packet to a time server
      sendNTPpacket(timeServerIP, packetBuffer);
      delay(1000);
    
      int cb = udp.parsePacket();
      if (!cb) {
        // no packet
        #ifdef DEBUG
        Serial.println("No packet");
        #endif
        tries--;
        continue;
      }
      udp.read(packetBuffer, NTP_PACKET_SIZE); // read the packet into the buffer
  
      // the timestamp starts at byte 40 of the received packet and is four bytes,
      // or two words, long. First, extract the two words:
      unsigned long highWord = word(packetBuffer[40], packetBuffer[41]);
      unsigned long lowWord = word(packetBuffer[42], packetBuffer[43]);
  
      // combine the four bytes (two words) into a long integer
      // this is NTP time (seconds since Jan 1 1900):
      unsigned long secsSince1900 = highWord << 16 | lowWord;
      localTime = secsSince1900 - seventyYears + timeZone;
      // update check time
      lastCheckTime = localTime;
      #ifdef DEBUG
        // print the hour, minute and second:
        Serial.print("The UTC time is ");       // UTC is the time at Greenwich Meridian (GMT)
        Serial.print((localTime  % 86400L) / 3600); // print the hour (86400 equals secs per day)
        Serial.print(':');
        if ( ((localTime % 3600) / 60) < 10 ) {
          // In the first 10 minutes of each hour, we'll want a leading '0'
          Serial.print('0');
        }
        Serial.print((localTime  % 3600) / 60); // print the minute (3600 equals secs per minute)
        Serial.print(':');
        if ( (localTime % 60) < 10 ) {
          // In the first 10 seconds of each minute, we'll want a leading '0'
          Serial.print('0');
        }
        Serial.println(localTime % 60); // print the second
      #endif
      udp.stop();
      break;
    }
  }
  return localTime;
}

void sendNTPpacket(IPAddress& address, byte * packetBuffer) {
  // set all bytes in the buffer to 0
  memset(packetBuffer, 0, NTP_PACKET_SIZE);
  // Initialize values needed to form NTP request
  packetBuffer[0] = 0b11100011;   // LI, Version, Mode
  packetBuffer[1] = 0;     // Stratum, or type of clock
  packetBuffer[2] = 6;     // Polling Interval
  packetBuffer[3] = 0xEC;  // Peer Clock Precision
  // 8 bytes of zero for Root Delay & Root Dispersion
  packetBuffer[12]  = 49;
  packetBuffer[13]  = 0x4E;
  packetBuffer[14]  = 49;
  packetBuffer[15]  = 52;

  // all NTP fields have been given values, now
  // you can send a packet requesting a timestamp:
  udp.beginPacket(address, 123); //NTP requests are to SETTING_PORT 123
  udp.write(packetBuffer, NTP_PACKET_SIZE);
  udp.endPacket();
}

// default temperature 7:00 to 22:00 22°C
//                     22:00 to 7:00 18°C
void defaultSchedule() {
  for (int i = 0; i < SCHEDULE_SIZE; i += 144) {
    memset(schedule + i, 80, 42);
    memset(schedule + 42 + i, 120, 90);
    memset(schedule + 132 + i, 80, 12);
  }
}

// returns between 0 and 100 how much open valve
byte getValveOpening() {
  if (getTempDiff() <= -0.5)
    valveValue = 100;
  if (getTempDiff() >= 0.5)
    valveValue = 0;
  return valveValue;
}

// returns negative if actual temperature is lower than set temperature
float getTempDiff() {
  float des = ( (float) (getWantedTepm() + 100) ) / 10.0;
  #ifdef TEMPERATURE
    Serial.print("Temperature: ");
    Serial.println(measureTeperature());
    Serial.print("Temperature diff: ");
    Serial.println(measureTeperature() - des);
  #endif
  return measureTeperature() - des;
}

byte getWantedTepm() {
  if (holYear < year() ||
      holYear == year() && holMonth < month() ||
      holYear == year() && holMonth == month() && holDay < day()) {
        if (holTemp == 255){
          return getWantedTepmAlternative(weekday(), hour(), minute());
        }
        return holTemp;
      }
  return getWantedTepm(weekday(), hour(), minute());
}

byte getWantedTepm(int day, int hour, int minute) {
  return schedule[scheduleIndex(day, hour, minute)];
}

byte getWantedTepmAlternative(int day, int hour, int minute) {
  return scheduleAlternative[scheduleIndex(day, hour, minute)];
}

int scheduleIndex(int day, int hour, int minute) {
  // weekday() returns day of the week (1-7), Sunday is day 1
  int _day = day - 2;
  if (day == 1)
    _day = 6;

  int _minute = minute - (minute % 10);
  _minute /= 10;
  
  return SCHEDULE_SIZE / 7 * _day + SCHEDULE_SIZE / 7 / 24 * hour + _minute;
}

void setDefaultId() {
  byte b1 = 97;
  EEPROM.begin(16);
  EEPROM.put(0, b1);
  EEPROM.put(1, b1);
  EEPROM.end();
}
