#include <WiFi.h>
#include <HTTPClient.h>
#include <OneWire.h>
#include <DallasTemperature.h>

// Визначення пінів для підключення сенсорів і світлодіодів
#define TEMP_SENSOR_PIN 23
#define POT_PIN 34
#define DOOR_BUTTON_PIN 27
#define DOOR_OPEN_LED 16
#define DOOR_CLOSED_LED 4
#define NETWORK_STATUS_LED 2

OneWire oneWire(TEMP_SENSOR_PIN); // Ініціалізація OneWire для температурного сенсора
DallasTemperature sensors(&oneWire); // Ініціалізація бібліотеки DallasTemperature

// Глобальні змінні для збереження налаштувань
String ssid, password, API_BASE_URL, FRIDGE_ID, JWT_TOKEN;
bool lastButtonState = LOW;
bool orderRequested = false;
String currentOrderId = "";

// Налаштування історії температур
#define HISTORY_SIZE 10
double temperatureHistory[HISTORY_SIZE];
int currentIndex = 0;

void setup() {
  Serial.begin(115200);

  pinMode(DOOR_BUTTON_PIN, INPUT);
  pinMode(DOOR_OPEN_LED, OUTPUT);
  pinMode(DOOR_CLOSED_LED, OUTPUT);
  pinMode(NETWORK_STATUS_LED, OUTPUT);

  sensors.begin();

  promptForWiFiCredentials();
  promptForAPIBaseURL();
  
  connectToWiFi();
  
  promptForFridgeID();
  promptForJWTToken();
  
  digitalWrite(DOOR_OPEN_LED, LOW);
  digitalWrite(DOOR_CLOSED_LED, HIGH);
}

void loop() {
  if (WiFi.status() == WL_CONNECTED) {
    handleButtonPress();
    handleFridgeStatus();
  } else {
    connectToWiFi();
  }

  delay(1500);
}

// Функція для запиту даних WiFi
void promptForWiFiCredentials() {
  Serial.println("Enter SSID:");
  while (Serial.available() == 0) {}
  ssid = Serial.readStringUntil('\n');
  ssid.trim();

  Serial.println("Enter Password:");
  while (Serial.available() == 0) {}
  password = Serial.readStringUntil('\n');
  password.trim();
}

// Функція для запиту базового URL API
void promptForAPIBaseURL() {
  Serial.println("Enter API Base URL:");
  while (Serial.available() == 0) {}
  API_BASE_URL = Serial.readStringUntil('\n');
  API_BASE_URL.trim();
}

// Функція для підключення до WiFi
void connectToWiFi() {
  if (WiFi.status() != WL_CONNECTED) {
    Serial.println("Connecting to WiFi...");
    WiFi.begin(ssid, password);
    while (WiFi.status() != WL_CONNECTED) {
      delay(1000);
      Serial.println("Connecting...");
    }
    Serial.println("WiFi connected!");
    digitalWrite(NETWORK_STATUS_LED, HIGH);
  }
}

// Функція для запиту ID холодильника
void promptForFridgeID() {
  Serial.println("Enter Fridge ID:");
  while (Serial.available() == 0) {}
  FRIDGE_ID = Serial.readStringUntil('\n');
}

// Функція для запиту JWT токена
void promptForJWTToken() {
  Serial.println("Enter JWT Token:");
  while (Serial.available() == 0) {}
  JWT_TOKEN = Serial.readStringUntil('\n');
}

// Функція обробки стану дверей
void handleButtonPress() {
  bool currentButtonState = digitalRead(DOOR_BUTTON_PIN);

  if (currentButtonState == HIGH && lastButtonState == LOW) {
    orderRequested = true; 
    Serial.println("Enter Order ID:"); 
  }

  if (orderRequested && Serial.available() > 0) {
    currentOrderId = Serial.readStringUntil('\n');
    currentOrderId.trim();

    orderRequested = false;

    if (currentOrderId.length() > 0) {
      if (updateDoorStatus(currentOrderId)) {
        Serial.println("Door status updated successfully!");
        digitalWrite(DOOR_OPEN_LED, HIGH);
        digitalWrite(DOOR_CLOSED_LED, LOW);
      } else {
        Serial.println("Failed to update door status.");
        digitalWrite(DOOR_OPEN_LED, LOW); 
        digitalWrite(DOOR_CLOSED_LED, HIGH);
      }
    } else {
      Serial.println("Order ID is empty. Door remains closed.");
      digitalWrite(DOOR_OPEN_LED, LOW);
      digitalWrite(DOOR_CLOSED_LED, HIGH);
    }
  }

  lastButtonState = currentButtonState;
}

// Функція обробки статусу холодильника
void handleFridgeStatus() {
  double currentTemp = readTemperature();
  storeTemperature(currentTemp);

  double avg = calculateAverage();
  double variance = calculateVariance();
  double lastTemp = temperatureHistory[(currentIndex - 1 + HISTORY_SIZE) % HISTORY_SIZE];
  double fluctuation = abs(currentTemp - lastTemp);

  int inventoryLevel = readInventoryLevel();
  bool doorStatus = digitalRead(DOOR_BUTTON_PIN) == HIGH;

  String logMessage = "Temperature: " + String(currentTemp) + "°C\n";
  logMessage += "Average Temp: " + String(avg) + "°C, Variance: " + String(variance) + "\n";
  logMessage += "Inventory Level: " + String(inventoryLevel) + "%\n";

  if (fluctuation > 5.0) {
    logMessage += "WARNING: High temperature fluctuation detected (" + String(fluctuation) + "°C)!\n";
    sendFluctuationWarning(currentTemp, lastTemp);
  }

  Serial.println(logMessage);

  updateFridgeStatus(currentTemp, inventoryLevel, doorStatus);
  sendTemperatureLog();
}

// Функція оновлення статусу дверей
bool updateDoorStatus(String orderId) {
  if (orderId.length() == 0) return false;

  HTTPClient http;
  String url = API_BASE_URL + "/fridge/" + FRIDGE_ID + "/door-status";
  http.begin(url);
  http.addHeader("Content-Type", "application/json");
  http.addHeader("Authorization", "Bearer " + JWT_TOKEN);

  String payload = "{";
  payload += "\"OrderId\": \"" + orderId + "\",";
  payload += "\"IsDoorOpened\": " + String(digitalRead(DOOR_BUTTON_PIN) == HIGH ? "true" : "false");
  payload += "}";

  int httpResponseCode = http.POST(payload);

  if (httpResponseCode > 0) {
    Serial.println("Door status sent successfully: " + String(httpResponseCode));
    http.end();
    return httpResponseCode == 200;
  } else {
    Serial.println("Error sending door status: " + String(httpResponseCode));
    http.end();
    return false;
  }
}

// Функція оновлення статусу холодильника (температура, рівень запасів і стан дверей)
void updateFridgeStatus(double temperature, int inventoryLevel, bool doorStatus) {
  HTTPClient http;
  String url = API_BASE_URL + "/fridge/" + FRIDGE_ID + "/status";
  http.begin(url);
  http.addHeader("Content-Type", "application/json");
  http.addHeader("Authorization", "Bearer " + JWT_TOKEN);

  String payload = "{";
  payload += "\"CurrentTemperature\": " + String(temperature) + ",";
  payload += "\"InventoryLevel\": " + String(inventoryLevel) + ",";
  payload += "\"DoorStatus\": " + String(doorStatus ? "true" : "false");
  payload += "}";

  int httpResponseCode = http.POST(payload);

  if (httpResponseCode > 0) {
    Serial.println("Status sent successfully: " + String(httpResponseCode));
  } else {
    Serial.println("Error sending status: " + String(httpResponseCode));
  }

  http.end();
}

// Функція зчитування температури з датчика
double readTemperature() {
  sensors.requestTemperatures();
  return sensors.getTempCByIndex(0);
}

// Функція зчитування рівня запасів з потенціометра
int readInventoryLevel() {
  int potValue = analogRead(POT_PIN);
  return map(potValue, 0, 4095, 0, 100);
}

// Функція збереження нового значення температури в історії для подальшого аналізу
void storeTemperature(double newTemp) {
  temperatureHistory[currentIndex] = newTemp;
  currentIndex = (currentIndex + 1) % HISTORY_SIZE;
}

// Функція розраховування середнього значення температури
double calculateAverage() {
  double sum = 0;
  for (int i = 0; i < HISTORY_SIZE; i++) {
    sum += temperatureHistory[i];
  }
  return sum / HISTORY_SIZE;
}

// Функція обчислення дисперсії температури
double calculateVariance() {
  double avg = calculateAverage();
  double variance = 0;
  for (int i = 0; i < HISTORY_SIZE; i++) {
    variance += (temperatureHistory[i] - avg) * (temperatureHistory[i] - avg);
  }
  return variance / HISTORY_SIZE;
}

// Функція відправки журналу температури на сервер
void sendTemperatureLog() {
  HTTPClient http;
  String url = API_BASE_URL + "/fridge/" + FRIDGE_ID + "/temperature-log";
  http.begin(url);
  http.addHeader("Content-Type", "application/json");
  http.addHeader("Authorization", "Bearer " + JWT_TOKEN);

  String payload = "{";
  payload += "\"Temperature\": " + String(temperatureHistory[(currentIndex - 1 + HISTORY_SIZE) % HISTORY_SIZE]);
  payload += "}";

  int httpResponseCode = http.POST(payload);

  if (httpResponseCode > 0) {
    Serial.println("Temperature log sent: " + String(httpResponseCode));
  } else {
    Serial.println("Error sending log: " + String(httpResponseCode));
  }
  http.end();
}

// Функція відправки попередження про значне коливання температури на сервер
void sendFluctuationWarning(double currentTemp, double lastTemp) {
  HTTPClient http;
  String url = API_BASE_URL + "/fridge/" + FRIDGE_ID + "/temperature-log";
  http.begin(url);
  http.addHeader("Content-Type", "application/json");
  http.addHeader("Authorization", "Bearer " + JWT_TOKEN);

  double fluctuation = abs(currentTemp - lastTemp);
  String payload = "{";
  payload += "\"Title\": \"Warning: High temperature fluctuation detected!\",";
  payload += "\"Text\": \"Temperature fluctuated by " + String(fluctuation, 1) + "°C. Last: " + String(lastTemp, 1) + "°C, Current: " + String(currentTemp, 1) + "°C.\",";
  payload += "\"Temperature\": " + String(currentTemp);
  payload += "}";

  int httpResponseCode = http.POST(payload);

  if (httpResponseCode > 0) {
    Serial.println("Fluctuation warning sent: " + String(httpResponseCode));
  } else {
    Serial.println("Error sending warning: " + String(httpResponseCode));
  }
  http.end();
}
