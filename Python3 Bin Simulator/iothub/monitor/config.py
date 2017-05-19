import json
import os

class Config():

    @property
    def sampleRateInSeconds(self):
        return self._sampleRate

    @sampleRateInSeconds.setter
    def sampleRateInSeconds(self, value):
        try:
            self._sampleRate = float(value)

            if self._sampleRate < 0.1:
                self._sampleRate = 0.1
            if self._sampleRate > 100000:
                self._sampleRate = 100000
        except:
            self._sampleRate = self._sampleRate
            

    def config_load(self, configFile):
        print('Loading {0} settings'.format(configFile))

        config_data = open(configFile)
        config = json.load(config_data)

        self.sensor = __import__(config['SensorModule']) 
        self.hubAddress = config['IotHubAddress']
        self.deviceId = config['DeviceId']
        self.sharedAccessKey = config['SharedAccessKey']
        self.binId = config['BinId']
        self.hourlyFillRateCMs = config['HourlyFillRateCMs']
        self.sampleRateInSeconds = config['SampleRateSeconds']
        if len(self.hourlyFillRateCMs) != 24:
            raise Exception('Expected 24 hourly fill rates in JSON config file. %d were defined.' % len(self.hourlyFillRateCMs))


    def __init__(self, configFile):
        self.sampleRateInSeconds = 12 #set publishing rate in seconds. every 12 seconds (5 times a minute) good for an 8000 msg/day free Azure IoT Hub
        self.config_load(configFile)