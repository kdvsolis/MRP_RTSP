import CameraServerUtils.ffmpy as FFmpegUtil
import CameraServerUtils.vlcpy as VLCUtil

class RTSPServerHandler:

    __instance = None
    
    def __init__(self):
        if RTSPServerHandler.__instance != None:
            RTSPServerHandler.__instance = self
            return
        else:
            RTSPServerHandler.__instance = self
        self.vlc_cmd_handler = None
        self.ffmpeg_cmd_handler = None
        self.is_vlc_input_pipe = False
        self.pipe_input = None
        self.ffmpeg_pipe_input = None
            
    def getInstance(self):
        if RTSPServerHandler.__instance == None:
            RTSPServerHandler()
        return RTSPServerHandler.__instance 

    def set_vlc_param(self,
                      vlc_input,
                      vlc_pipe_input=None,
                      _audio_options=None,
                      _video_options=None,
                      _playlist_options=None,
                      _input_options=None,
                      _caching_options=None,
                      _network_options=None,
                      _misc_options=None,
                      _sout_options=None):
        if (vlc_input == "pipe" and vlc_pipe_input != None):
            self.is_vlc_input_pipe = True
            self.vlc_cmd_handler = VLCUtil.vlcpy("-")
            self.pipe_input = vlc_pipe_input
        else:
            self.vlc_cmd_handler = VLCUtil.vlcpy(vlc_input)
        self.vlc_cmd_handler.set_vlc_param(audio_options=_audio_options,
                                          video_options=_video_options,
                                          playlist_options=_playlist_options,
                                          input_options=_input_options,
                                          caching_options=_caching_options,
                                          network_options=_network_options,
                                          misc_options=_misc_options,
                                          sout_options=_sout_options)

    def set_ffmpeg_param(self, input={}, input_pipe_src = None, output={}):
        self.ffmpeg_pipe_input = ""
        for key, value in input.items():
            if 'pipe' in key and input_pipe_src != None:
                self.ffmpeg_pipe_input = input_pipe_src + " | "
                input['-'] = input[key]
                del input[key]
        self.ffmpeg_cmd_handler = FFmpegUtil.FFmpeg(inputs=input,outputs=output)
        
    def get_ffmpeg_cmd(self):
        return self.ffmpeg_pipe_input + self.ffmpeg_cmd_handler.cmd.replace('" "', '') if self.ffmpeg_cmd_handler != None else ""
        
    def get_vlc_cmd(self):
        return self.vlc_cmd_handler.get_vlc_cmd() if self.vlc_cmd_handler != None else ""
        
    def get_full_command(self):
        return (self.pipe_input + " | " + self.vlc_cmd_handler.get_vlc_cmd()) if self.is_vlc_input_pipe else self.vlc_cmd_handler.get_vlc_cmd()
    
    def start_server(self):
        """
        """
"""        
x = RTSPServerHandler()
x.set_ffmpeg_param({'/dev/video0': None, 'hw:0,0': '-f alsa -ac 1 -itsoffset 0.25 '}, {'pipe:':'-map 0:0 -map 1:0 -vcodec copy -acodec copy -async 1 -strict 2 -avioflags direct -fflags nobuffer -f asf'})
x.set_vlc_param("pipe", vlc_pipe_input=x.get_ffmpeg_cmd(), _sout_options=dict(rtp=dict(sdp='rtsp://:8554/unicast')), _caching_options=(dict(network_caching='150',disc_caching='0',live_caching='0',clock_jitter='0')))
print x.get_full_command()
"""