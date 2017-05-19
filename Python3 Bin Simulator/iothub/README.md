# Azure IoT Hub Python 3 Waste Management Simulator.


This cross platform Python3 code sample demonstrates how to stream data to Azure IoT Hub from Windows, OSX, Linux including Ubuntu and Raspberry Pi Zero, 2 and 3

As at March 2017.

## Purpose

Simulate waste bin fill rate based on time of day and day of the week.

## Language: Python3

## Tested Platforms

1. Windows 10, but any version of Windows running Python3 should work
2. Apple OSX, tested on 10.12 Sirri Release, but any version of OSX running Python3 should work
3. Linux
    * Windows (10) Subsystem for Linux (Ubuntu 14.04)
    * Ubuntu 16.04
4. Rasperry Pi, Raspbian Kernel 4.4 fully patched. 
    * Raspbrry Pi Zero
    * Raspberry Pi 2
    * Raspberry Pi 3
5. Should work on any platform supporting Python3 and the Paho-Mqtt library.




# Required pip3 Package Installation

The following libraries are required.

1. Paho-MQTT - Mqtt Support


## On Raspberry Pi Rasbian and Ubuntu/Linux

    sudo pip3 install paho-mqtt

## On Windows and MacOS

    pip3 install paho-mqtt



# Startup Configuration

See the startiot.sh

## Startup example

    python3 environment.py config_bin_1.json
    python3 environment.py config_bin_2.json


# MQTT TLS Certificate

On Linux 
    
    client.tls_set("/etc/ssl/certs/ca-certificates.crt") # use builtin cert on Raspbian

On Windows using certlm.msc export one of the top level public certificate authority keys such as the Baltimore Cybertrust Root to a .cer file in base64 format.

    client.tls_set("baltimorebase64.cer") # Baltimore Cybertrust Root exported from Windows 10 using certlm.msc in base64 format

The sample includes the Baltimore Cybertrust Key exported as baltimorebase64.cer that can be used across platforms.

# Recommended Software

1. To find your Raspberry Pi on your network by name install [Apple Print Bonjour Service](https://support.apple.com/kb/dl999?locale=en_AU) on Windows for mDNS UNIX Name Resolution. .
2. My favourite SSH and SFTP Windows Client is [Bitvise](https://www.bitvise.com/)
3. [Visual Studio Code](https://code.visualstudio.com/) for Windows, Mac and Linux


# Tips and Tricks

### Visual Studio Code, Python3 and OSX

From [Debugging Python 3.x with Visual Studio Code on OSX](https://nocture.dk/2016/05/07/debugging-python-3-x-with-visual-studio-code-on-osx/)

OSX comes with Python 2.x by default, and setting the default Python version to 3.x is not without trouble. Here is how you debug Python 3.x applications with Visual Studio Code and the Python extension.

1. Open the application folder in VS Code 
2. Create a new launch configuration based on the Python template 
3. Add "pythonPath": "/Library/Frameworks/Python.framework/Versions/3.5/bin/python3" to the configuration so that it looks similar to the following: 

    {
        "name": "Python",
        "type": "python",
        "request": "launch",
        "pythonPath": "/Library/Frameworks/Python.framework/Versions/3.5/bin/python3",
        "stopOnEntry": true,
        "program": "${file}",
        "debugOptions": [
            "WaitOnAbnormalExit",
            "WaitOnNormalExit",
            "RedirectOutput"
        ]
    }

**And start debugging!**

### Handy Tip for Raspberry Pi Zero

[Raspberry Pi Zero – Programming over USB](http://blog.gbaman.info/?p=791) ONLY works with Raspberry Pi Zero and provides a quick easy way to connect your PC to your Raspberry Pi Zero.


# Setting up Python3 on Mac OS X

This is mainly for my benefit as I have limited experience with an apple mac.

1. Install [Python3](www.python3.org)
2. Update [Tcl/Tk for Idle3](www.python.org/download/mac/tcltk). Install ActiveTcl 8.5.18.0.








