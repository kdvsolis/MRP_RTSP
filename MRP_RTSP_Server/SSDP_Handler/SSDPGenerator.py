from ssdp import SSDPServer
from upnp_http_server import UPNPHTTPServer
import uuid
import netifaces as ni
from time import sleep
import thread


class SSDPGenerator:

    def __init__(self, connection_type, mqtt_port):
        self.network_interface = connection_type
        self.device_uuid = uuid.uuid4()
        self.local_ip_address = self.__get_network_interface_ip_address(self.network_interface)
        self.http_server = UPNPHTTPServer(8088,
                             friendly_name="Camera Test",
                             manufacturer="L-IoT-ning",
                             manufacturer_url='http://liotningshop.azurewebsites.net/',
                             model_description='Pi Camera Test',
                             model_name="PiCamera",
                             model_number="3000",
                             model_url="",
                             serial_number="JBN425133",
                             uuid=self.device_uuid,
                             presentation_url=("tcp://{}:" + mqtt_port).format(self.local_ip_address))
                             
        self.ssdp = SSDPServer()
        self.ssdp.register('local',
              'uuid:{}::upnp:rootdevice'.format(self.device_uuid),
              'upnp:rootdevice',
              'http://{}:8088/jambon-3000.xml'.format(self.local_ip_address))

    def __get_network_interface_ip_address(self, interface='eth0'):
        """
        Get the first IP address of a network interface.
        :param interface: The name of the interface.
        :return: The IP address.
        """
        while True:
            if interface not in ni.interfaces():
                print ('Could not find interface %s.' % (interface,))
                exit(1)
            interface = ni.ifaddresses(interface)
            if (2 not in interface) or (len(interface[2]) == 0):
                print ('Could not find IP of interface %s. Sleeping.' % (interface,))
                sleep(60)
                continue
            return interface[2][0]['addr']
            
    def server_start(self):
        thread.start_new_thread(self.http_server.start,())
        thread.start_new_thread(self.ssdp.run,())
        
    def server_close(self):
        self.http_server.shutdown()
        self.ssdp.shutdown()
        self.ssdp.unregister('uuid:{}::upnp:rootdevice'.format(self.device_uuid))
