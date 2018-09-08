import Core_Network_Handler.CoreCommunicator as CoreCommunicator
import SSDP_Handler.SSDPGenerator as SSDPGenerator
import RTSP_Handler.RTSPServerHandler as RTSPServerHandler
import configparser
import os
import psutil, time
import netifaces
from subprocess import Popen, PIPE


ConnectionType = ""
RTSPPort = ""
MQTTPort = ""
camera_process = None

def config_parser():
    global ConnectionType
    global RTSPPort
    global MQTTPort
    settings = configparser.ConfigParser()
    settings.read('camera.ini')
    settings._interpolation = configparser.ExtendedInterpolation()
    ConnectionType = settings.get("Connection", "ConnectionType")
    MQTTPort = settings.get("MQTT", "port")
    RTSPPort = settings.get("RTSP", "port")

def message_parser(communicator, topic, message):
    print topic + "---" + message
    global ConnectionType
    global camera_process
    rtsp_server_handler = RTSPServerHandler.RTSPServerHandler()
    rtsp_server_handler.set_ffmpeg_param(input={'pipe:': None, 'hw:1,0': '-f alsa -ac 1 -itsoffset 0.25 '}, 
                                         input_pipe_src='raspivid -o - -t 0 -n ', 
                                         output={'pipe:1':'-map 0:0 -map 1:0 -vcodec copy -acodec copy -async 1 -strict 2 -avioflags direct -fflags nobuffer -f asf'})
    rtsp_server_handler.set_vlc_param("pipe", 
                                      vlc_pipe_input=rtsp_server_handler.get_ffmpeg_cmd(), 
                                      _sout_options=dict(rtp=dict(sdp='rtsp://:' + RTSPPort + '/unicast')), 
                                      _caching_options=(dict(network_caching='150',disc_caching='0',live_caching='0',clock_jitter='0')))
    if(topic == "event"):
        if message == "camera_server_on":
            print rtsp_server_handler.get_full_command()
            camera_process = Popen(rtsp_server_handler.get_full_command(), shell=True)
            time.sleep(7)
            communicator.respond_to_sender(topic + "_client", "Camera Server Ready")
        if message == "camera_server_off":
            print camera_process.pid
            if camera_process != None:
                os.system("sudo pkill ffmpeg")
                os.system("sudo pkill vlc")
                os.system("sudo pkill raspivid")
            communicator.respond_to_sender(topic + "_client", "Camera Server OFF")
    if(topic == "url_request"):
        print ConnectionType
        print "rtsp://" + str((netifaces.ifaddresses(ConnectionType)[netifaces.AF_INET])[0]['addr']) + ":" + RTSPPort + "/unicast"	
        communicator.respond_to_sender(topic + "_client", "rtsp://" + str((netifaces.ifaddresses(ConnectionType)[netifaces.AF_INET])[0]['addr']) + ":" + RTSPPort + "/unicast")
"""
        if message == "disconnect_to_broker":
            print("disconnect_to_broker")
            communicator.respond_to_sender(topic, "Camera Server App is now OFF")
            communicator.disconnect_to_broker()
"""
if __name__ == '__main__':
    config_parser()
    ssdpGenerator = SSDPGenerator.SSDPGenerator(ConnectionType, MQTTPort)
    coreCommunicator = CoreCommunicator.CoreCommunicator("localhost", int(MQTTPort), [("event", 0),("url_request", 0)], ssdpGenerator)
    coreCommunicator.message_parser = message_parser
    coreCommunicator.connect_to_broker()
