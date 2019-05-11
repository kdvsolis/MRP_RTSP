MRP_RTSP(MQTT in Raspberry Pi with RTSP camera server)
========================
## Description
Simple camera control with cross platfrom apps to Raspberry Pi Camera via MQTT

## Dependencies
Server Side:
 Embedded Linux with Python (e.g. Raspberry Pi)
 vlc
 ffmpeg
 Python External Libraries
 -configparser
 -interfaces
 -psutil
 -paho.mqtt
Client Side:
 Visual Studio 2017
 Android 5.1 and above device
 Windows 10

## App Usage
Server Side:
 1. Have a copy of MRP_RTSP_Server
 2. You may run the server app by going to MRP_RTSP_Server and running ```python main.py``` from it
Client Side:
 1. As you install in either Android Device or Windows 10 click the scan icon shown in upper right corner of the app and it will list the detected devices in the network.
 ![Main Page Android](https://raw.githubusercontent.com/kdvsolis/MRP_RTSP/master/MRP_RTSP_Client/screenshot/AndroidScan.PNG)
 ![Main Page UWP](https://raw.githubusercontent.com/kdvsolis/MRP_RTSP/master/MRP_RTSP_Client/screenshot/UWPScan.PNG)
 2. Choose the scanned device that you want to stream on your device
 ![Camera Android](https://raw.githubusercontent.com/kdvsolis/MRP_RTSP/master/MRP_RTSP_Client/screenshot/AndroidVideoView.PNG)
 ![Camera UWP](https://raw.githubusercontent.com/kdvsolis/MRP_RTSP/master/MRP_RTSP_Client/screenshot/UWPVideoView.PNG)
 3. You can also add manually the URL for rtsp server by clicking the "+" icon also located at upper right corner of the app
 ![Add RTSP Android](https://raw.githubusercontent.com/kdvsolis/MRP_RTSP/master/MRP_RTSP_Client/screenshot/AndroidRtspList.PNG)
 ![Add RTSP UWP](https://raw.githubusercontent.com/kdvsolis/MRP_RTSP/master/MRP_RTSP_Client/screenshot/UWPRtspList.PNG)
 4. You can also play the manually added URL for rtsp server by choosing on what was added to the list
 ![Add RTSP Android](https://raw.githubusercontent.com/kdvsolis/MRP_RTSP/master/MRP_RTSP_Client/screenshot/AndroidVideoTest.PNG)
 ![Add RTSP UWP](https://raw.githubusercontent.com/kdvsolis/MRP_RTSP/master/MRP_RTSP_Client/screenshot/UWPVideoTest.PNG)


## Credits to:
- ZeWaren for providing a public UPnP example: https://github.com/ZeWaren/python-upnp-ssdp-example
- Andriy Yurchuk for providing an ffmpeg command builder with his ffmpy: https://pypi.org/project/ffmpy/
- mfkl for providing libvlc for xamarin native: https://github.com/mfkl/libvlc-nuget
- VLC staff of course for providing an open source SDK for VLC
