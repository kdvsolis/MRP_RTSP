import paho.mqtt.client as mqtt

class CoreCommunicator:

    __instance = None
    message_parser = None

    def __init__(self, _address, _port, _topics, _ssdpGenerator):
        self.client = mqtt.Client()
        self.broker_address = _address
        self.broker_port = _port
        self.topics = _topics
        self.ssdpGenerator = _ssdpGenerator
        if CoreCommunicator.__instance != None:
            raise Exception("This class is a singleton!")
        else:
            CoreCommunicator.__instance = self

    def __on_connect(self, client, userdata, flags, rc):
        print("Connected with result code "+str(rc))
        client.subscribe(self.topics)

    def __on_message(self, client, userdata, msg):
        self.message_parser(self, msg.topic, msg.payload.decode())

    def getInstance(self):
        if CoreCommunicator.__instance == None:
            CoreCommunicator(self.broker_address, self.broker_address, self.topics)
        return CoreCommunicator.__instance 

    def connect_to_broker(self):
        try:
            self.ssdpGenerator.server_start()
            self.client.connect(self.broker_address, self.broker_port, 60)
            self.client.on_connect = self.__on_connect
            self.client.on_message = self.__on_message
            self.client.loop_forever()
        except KeyboardInterrupt:
            self.disconnect_to_broker()
        
    def respond_to_sender(self, topic, message):
        self.client.publish(topic, message)
        
    def disconnect_to_broker(self):
        self.ssdpGenerator.server_close()
        self.client.loop_stop()
        self.client.disconnect()


