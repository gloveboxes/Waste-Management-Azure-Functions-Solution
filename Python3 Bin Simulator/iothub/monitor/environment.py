# https://azure.microsoft.com/en-us/documentation/articles/iot-hub-mqtt-support/
# http://stackoverflow.com/questions/35452072/python-mqtt-connection-to-azure-iot-hub/35473777
# https://azure.microsoft.com/en-us/documentation/samples/iot-hub-python-get-started/


# Mqtt Support https://www.eclipse.org/paho/clients/python/
# pip3 install paho-mqtt

# Weather data Open Weather Map using https://github.com/csparpa/pyowm
# pip3 install pyowm

import paho.mqtt.client as mqtt
from datetime import datetime
import time
import iothub
import sys
import json
import config

iotHubMode = True


def on_connect(client, userdata, flags, rc):
    print("Connected with result code: %s" % rc)
    client.subscribe(iot.hubTopicSubscribe)

def on_disconnect(client, userdata, rc):
    print("Disconnected with result code: %s" % rc)
    client.username_pw_set(iot.hubUser, iot.generate_sas_token())

def on_message(client, userdata, msg):
    #print("{0} - {1} ".format(msg.topic, str(msg.payload)))
    cfg.sampleRateInSeconds = msg.payload
    # Do this only if you want to send a reply message every time you receive one
    # client.publish("devices/mqtt/messages/events", "REPLY", qos=1)

def on_publish(client, userdata, mid):
    print("Message {0} sent from {1} at {2}".format(str(mid), cfg.deviceId, datetime.now().strftime('%Y-%m-%d %H:%M:%S')))

def publish():
    while True:
        try:
            # print(mysensor.measure())
            client.publish(iot.hubTopicPublish, mysensor.measure())            
            time.sleep(cfg.sampleRateInSeconds)
        
        except KeyboardInterrupt:
            print("IoTHubClient sample stopped")
            return

        except:
            print("Unexpected error")
            time.sleep(4)

try:
    if len(sys.argv) == 2:
        cfg = config.Config(sys.argv[1])
    else:
        cfg = config.Config("config_default.json")
except Exception as e:
    sys.exit(e)

mysensor = cfg.sensor.Sensor(cfg.binId, cfg.hourlyFillRateCMs)
iot = iothub.IotHub(cfg.hubAddress, cfg.deviceId, cfg.sharedAccessKey)

client = mqtt.Client(cfg.deviceId, mqtt.MQTTv311)

client.on_connect = on_connect
client.on_disconnect = on_disconnect
client.on_message = on_message
client.on_publish = on_publish


if iotHubMode:
    client.username_pw_set(iot.hubUser, iot.generate_sas_token())
    #client.tls_set("/etc/ssl/certs/ca-certificates.crt") # use builtin cert on Raspbian
    client.tls_set("baltimorebase64.cer") # Baltimore Cybertrust Root exported from Windows 10 using certlm.msc in base64 format
    client.connect(cfg.hubAddress, 8883)
else:
    client.connect("localhost") # connect to local mosquitto service for testing purposes

client.loop_start()

publish()
