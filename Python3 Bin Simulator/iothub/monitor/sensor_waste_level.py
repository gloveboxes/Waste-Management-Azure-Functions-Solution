import sys
from datetime import datetime
import math
from random import randint



class Sensor():

    msg_txt = "{\"Level\":%d, \"Schema\":1,\"MsgId\":%d}"
  
    sampleRateMinutes = 5
    maxBinDepthCMs = 100

    def __init__(self, binId, hourlyFillRateCMs):
        # self.openWeather = owm.Weather(owmApiKey, owmLocation)
        self.binId = binId
        self.msgId = 0
        self.currentLevel = randint(10,80)
        self.epoch = datetime(1970,1,1)
        self.fillRate = 0
        # self.hourlyFillRateCMs = {0:10, 1:10, 2:10, 3:100, 4:200, 5:300, 6:400, 7:300, 8:400, 9:400, 10:500, 11:550, 12:600, 13:500, 14:500, 15:450, 16:400, 17:400, 18:400, 19:300, 20:200, 21:200, 22:100, 23:50  }
        self.hourlyFillRateCMs = hourlyFillRateCMs
        self.dayScaler = {1:1, 2:1, 3:1, 4:2, 5:3, 6:4, 7:4}
        self.lastEpoch = math.floor((datetime.now() - datetime(1970,1,1)).total_seconds())


    def measure(self):
        global maxBinDepthCMS

        newEpoch = math.floor((datetime.now() - datetime(1970,1,1)).total_seconds())
        deltaSeconds = newEpoch - self.lastEpoch
        self.lastEpoch = newEpoch

        fillAmount = self.hourlyFillRateCMs[datetime.now().hour] / 60 / 60 * deltaSeconds * self.dayScaler[datetime.now().isoweekday()]

        self.currentLevel = self.currentLevel + fillAmount
        if self.currentLevel > 100:
            self.currentLevel = self.currentLevel % 100
        
        self.msgId += 1
        return self.msg_txt % (self.currentLevel, self.msgId)
