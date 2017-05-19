#!/bin/bash
echo "starting IoT Hub Client"

sleep 10

sudo killall python3

cd /home/pi/iothub/monitor

python3 environment.py config_bin_1.json&
python3 environment.py config_bin_2.json&
python3 environment.py config_bin_3.json&
python3 environment.py config_bin_4.json&
python3 environment.py config_bin_5.json&
python3 environment.py config_bin_6.json&
#python3 environment.py config_bin_10.json&
#python3 environment.py config_bin_11.json&
#python3 environment.py config_envirophat.json&
#python3 environment.py config_sensehat.json&

