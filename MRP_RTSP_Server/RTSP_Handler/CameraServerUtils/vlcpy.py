import re

class vlcpy:
    def __init__(self, _input):
        self.input = _input
        self.main_command_string = ""
        self.audio_options_string = ""
        self.video_options_string = ""
        self.playlist_options_string = ""
        self.input_options_string = ""
        self.caching_options_string = ""
        self.misc_options_string = ""
        self.sout_string = ""
        self.std_sout_string = ""
        self.display_sout_string = ""
        self.rtp_sout_string = ""
        self.es_sout_string = ""
        self.transcode_sout_string = ""
        self.duplicate_sout_string = ""
        
    def set_vlc_param(self,
                      audio_options=None,
                      video_options=None,
                      playlist_options=None,
                      input_options=None,
                      caching_options=None,
                      network_options=None,
                      misc_options=None,
                      sout_options=None):
        if (audio_options != None and isinstance(audio_options, dict)):
            _noaudio = audio_options['noaudio'] if 'noaudio' in audio_options.keys() else "" 
            _mono = audio_options['mono'] if 'mono' in audio_options.keys() else "" 
            _volume = audio_options['volume'] if 'volume' in audio_options.keys() else ""  
            _aout_rate = audio_options['aout_rate'] if 'aout_rate' in audio_options.keys() else ""   
            _desync = audio_options['desync'] if 'desync' in audio_options.keys() else ""  
            _audio_filter = audio_options['audio_filter'] if 'audio_filter' in audio_options.keys() else ""
            self._audio_options(noaudio=_noaudio,
                                       mono=_mono,
                                       volume=_volume,
                                       aout_rate=_aout_rate,
                                       desync=_desync,
                                       audio_filter=_audio_filter)

        if (video_options != None and isinstance(video_options, dict)):
            _no_video = video_options['no_video'] if 'no_video' in video_options.keys() else "" 
            _grayscale = video_options['grayscale'] if 'grayscale' in video_options.keys() else "" 
            _fullscreen = video_options['fullscreen'] if 'fullscreen' in video_options.keys() else ""  
            _nooverlay = video_options['nooverlay'] if 'nooverlay' in video_options.keys() else ""   
            _start_time = video_options['start_time'] if 'start_time' in video_options.keys() else ""  
            _stop_time = video_options['stop_time'] if 'stop_time' in video_options.keys() else ""
            _zoom = video_options['zoom'] if 'zoom' in video_options.keys() else ""
            _aspect_ratio = video_options['aspect_ratio'] if 'aspect_ratio' in video_options.keys() else ""
            _spumargin = video_options['spumargin'] if 'spumargin' in video_options.keys() else ""
            _video_filter = video_options['video_filter'] if 'video_filter' in video_options.keys() else ""
            _video_splitter = video_options['video_splitter'] if 'video_splitter' in video_options.keys() else ""
            _sub_filter = video_options['sub_filter'] if 'sub_filter' in video_options.keys() else ""
            self._video_options(no_video=_no_video,
                                       grayscale=_grayscale,
                                       fullscreen=_fullscreen,
                                       nooverlay=_nooverlay,
                                       start_time=_start_time,
                                       stop_time=_stop_time,
                                       zoom=_zoom,
                                       aspect_ratio=_aspect_ratio,
                                       spumargin=_spumargin,
                                       video_filter=_video_filter,
                                       video_splitter=_video_splitter,
                                       sub_filter=_sub_filter)

        if (playlist_options != None and isinstance(playlist_options, dict)):
            _random = playlist_options['random'] if 'random' in playlist_options.keys() else "" 
            _loop = playlist_options['loop'] if 'loop' in playlist_options.keys() else "" 
            _repeat = playlist_options['repeat'] if 'repeat' in playlist_options.keys() else ""  
            _play_and_stop = playlist_options['play_and_stop'] if 'play_and_stop' in playlist_options.keys() else ""   
            _no_repeat = playlist_options['no_repeat'] if 'no_repeat' in playlist_options.keys() else ""  
            _no_loop = playlist_options['no_loop'] if 'no_loop' in playlist_options.keys() else ""
            self._playlist_options(random=_random,
                                           loop=_loop,
                                           repeat=_repeat,
                                           play_and_stop=_play_and_stop,
                                           no_repeat=_no_repeat,
                                           no_loop=_no_loop)

        if (input_options != None and isinstance(input_options, dict)):
            _input_list = input_options['input_list'] if 'input_list' in input_options.keys() else "" 
            _input_slave = input_options['input_slave'] if 'input_slave' in input_options.keys() else "" 
            self._input_options(input_list=_input_list,
                                          input_slave=_input_slave)
                                          
        if (network_options != None and isinstance(network_options, dict)):
            _server_port = network_options['server_port'] if 'server_port' in network_options.keys() else "" 
            _iface = network_options['iface'] if 'iface' in network_options.keys() else "" 
            _iface_addr = network_options['iface_addr'] if 'iface_addr' in network_options.keys() else ""  
            _mtu = network_options['mtu'] if 'mtu' in network_options.keys() else ""   
            _ipv6 = network_options['ipv6'] if 'ipv6' in network_options.keys() else ""  
            _ipv4 = network_options['ipv4'] if 'ipv4' in network_options.keys() else ""
            self._network_options(server_port=_server_port,
                                          iface=_iface,
                                          iface_addr=_iface_addr,
                                          mtu=_mtu,
                                          ipv6=_ipv6,
                                          ipv4=_ipv4)
        if (caching_options != None and isinstance(caching_options, dict)):
            _file_caching = caching_options['file_caching'] if 'file_caching' in caching_options.keys() else "" 
            _live_caching = caching_options['live_caching'] if 'live_caching' in caching_options.keys() else "" 
            _disc_caching = caching_options['disc_caching'] if 'disc_caching' in caching_options.keys() else ""  
            _network_caching = caching_options['network_caching'] if 'network_caching' in caching_options.keys() else ""   
            _cr_average = caching_options['cr_average'] if 'cr_average' in caching_options.keys() else ""  
            _clock_synchro = caching_options['clock_synchro'] if 'clock_synchro' in caching_options.keys() else ""
            _clock_jitter = caching_options['clock_jitter'] if 'clock_jitter' in caching_options.keys() else ""
            self._caching_options(file_caching=_file_caching,
                                          live_caching=_live_caching,
                                          disc_caching=_disc_caching,
                                          network_caching=_network_caching,
                                          cr_average=_cr_average,
                                          clock_synchro=_clock_synchro,
                                          clock_jitter=_clock_jitter)
        if (misc_options != None and isinstance(misc_options, dict)):
            _quiet = misc_options['quiet'] if 'quiet' in misc_options.keys() else "" 
            _color = misc_options['color'] if 'color' in misc_options.keys() else "" 
            _search_path = misc_options['search_path'] if 'search_path' in misc_options.keys() else ""  
            _plugin_path = misc_options['plugin_path'] if 'plugin_path' in misc_options.keys() else ""   
            _no_plugins_cache = misc_options['no_plugins_cache'] if 'no_plugins_cache' in misc_options.keys() else ""  
            _dvd = misc_options['dvd'] if 'dvd' in misc_options.keys() else ""
            _vcd = misc_options['vcd'] if 'vcd' in misc_options.keys() else ""
            _program = misc_options['program'] if 'program' in misc_options.keys() else ""
            _audio_type = misc_options['audio_type'] if 'audio_type' in misc_options.keys() else ""
            _audio_channel = misc_options['audio_channel'] if 'audio_channel' in misc_options.keys() else ""
            _spu_channel = misc_options['spu_channel'] if 'spu_channel' in misc_options.keys() else ""
            _version = misc_options['version'] if 'version' in misc_options.keys() else ""
            _module = misc_options['module'] if 'module' in misc_options.keys() else ""
            self._misc_options(quiet=_quiet,
                                       color=_color,
                                       search_path=_search_path,
                                       plugin_path=_plugin_path,
                                       no_plugins_cache=_no_plugins_cache,
                                       dvd=_dvd,
                                       vcd=_vcd,
                                       program=_program,
                                       audio_type=_audio_type,
                                       audio_channel=_audio_channel,
                                       spu_channel=_spu_channel,
                                       version=_version,
                                       module=_module)
        if  (sout_options != None and isinstance(sout_options, dict)):
            if ('std' in sout_options.keys() and isinstance(sout_options['std'], dict)):
                _access = sout_options['std']['access'] if 'access' in sout_options['std'].keys() else ""
                _mux = sout_options['std']['mux'] if 'mux' in sout_options['std'].keys() else ""
                _dst = sout_options['std']['dst'] if 'dst' in sout_options['std'].keys() else ""
                _sap = sout_options['std']['sap'] if 'sap' in sout_options['std'].keys() else ""
                _group = sout_options['std']['group'] if 'group' in sout_options['std'].keys() else ""
                _sap_ipv6 = sout_options['std']['sap_ipv6'] if 'sap_ipv6' in sout_options['std'].keys() else ""
                _slp = sout_options['std']['slp'] if 'slp' in sout_options['std'].keys() else ""
                _name = sout_options['std']['name'] if 'name' in sout_options['std'].keys() else ""
                self._std_sout_options(access=_access,
                                              mux=_mux,
                                              dst=_dst,
                                              sap=_sap,
                                              group=_group,
                                              sap_ipv6=_sap_ipv6,
                                              slp=_slp,
                                              name=_name)
            if ('display' in sout_options.keys() and isinstance(sout_options['display'], dict)):
                _novideo = sout_options['display']['novideo'] if 'novideo' in sout_options['display'].keys() else ""
                _noaudio = sout_options['display']['noaudio'] if 'noaudio' in sout_options['display'].keys() else ""
                _delay = sout_options['display']['delay'] if 'delay' in sout_options['display'].keys() else ""
                self._display_sout_options(novideo=_novideo,
                                                  noaudio=_noaudio,
                                                  delay=_delay)
            if ('rtp' in sout_options.keys() and isinstance(sout_options['rtp'], dict)):
                _dst = sout_options['rtp']['dst'] if 'dst' in sout_options['rtp'].keys() else ""
                _port = sout_options['rtp']['port'] if 'port' in sout_options['rtp'].keys() else ""
                _port_video = sout_options['rtp']['port_video'] if 'port_video' in sout_options['rtp'].keys() else ""
                _port_audio = sout_options['rtp']['port_audio'] if 'port_audio' in sout_options['rtp'].keys() else ""
                _sdp = sout_options['rtp']['sdp'] if 'sdp' in sout_options['rtp'].keys() else ""
                _ttl = sout_options['rtp']['ttl'] if 'ttl' in sout_options['rtp'].keys() else ""
                _mux = sout_options['rtp']['mux'] if 'mux' in sout_options['rtp'].keys() else ""
                _rtcp_mux = sout_options['rtp']['rtcp_mux'] if 'rtcp_mux' in sout_options['rtp'].keys() else ""
                _proto = sout_options['rtp']['proto'] if 'proto' in sout_options['rtp'].keys() else ""
                _name = sout_options['rtp']['name'] if 'name' in sout_options['rtp'].keys() else ""
                _description = sout_options['rtp']['description'] if 'description' in sout_options['rtp'].keys() else ""
                _url = sout_options['rtp']['url'] if 'url' in sout_options['rtp'].keys() else ""
                _email = sout_options['rtp']['email'] if 'email' in sout_options['rtp'].keys() else ""
                self._rtp_sout_options(dst=_dst,
                                         port=_port,
                                         port_video=_port_video,
                                         port_audio=_port_audio,
                                         sdp=_sdp,
                                         ttl=_ttl,
                                         mux=_mux,
                                         rtcp_mux=_rtcp_mux,
                                         proto=_proto,
                                         name=_name,
                                         description=_description,
                                         url=_url,
                                         email=_email)
            if ('es' in sout_options.keys() and isinstance(sout_options['es'], dict)):
                _access_video = sout_options['es']['access_video'] if 'access_video' in sout_options['es'].keys() else ""
                _access_audio = sout_options['es']['access_audio'] if 'access_audio' in sout_options['es'].keys() else ""
                _access = sout_options['es']['access'] if 'access' in sout_options['es'].keys() else ""
                _mux_video = sout_options['es']['mux_video'] if 'mux_video' in sout_options['es'].keys() else ""
                _mux_audio = sout_options['es']['mux_audio'] if 'mux_audio' in sout_options['es'].keys() else ""
                _mux = sout_options['es']['mux'] if 'mux' in sout_options['es'].keys() else ""
                _dst_video = sout_options['es']['dst_video'] if 'dst_video' in sout_options['es'].keys() else ""
                _dst_audio = sout_options['es']['dst_audio'] if 'dst_audio' in sout_options['es'].keys() else ""
                _dst = sout_options['es']['dst'] if 'dst' in sout_options['es'].keys() else ""
                self._es_sout_options(access_video=_access_video,
                                            access_audio=_access_audio,
                                            access=_access,
                                            mux_video=_mux_video,
                                            mux_audio=_mux_audio,
                                            mux=_mux,
                                            dst_video=_dst_video,
                                            dst_audio=_dst_audio,
                                            dst=_dst)
            if ('transcode' in sout_options.keys() and isinstance(sout_options['transcode'], dict)):
                _vcodec = sout_options['transcode']['vcodec'] if 'vcodec' in sout_options['transcode'].keys() else ""
                _vb = sout_options['transcode']['vb'] if 'vb' in sout_options['transcode'].keys() else ""
                _venc = sout_options['transcode']['venc'] if 'venc' in sout_options['transcode'].keys() else ""
                _fps = sout_options['transcode']['fps'] if 'fps' in sout_options['transcode'].keys() else ""
                _deinterlace = sout_options['transcode']['deinterlace'] if 'deinterlace' in sout_options['transcode'].keys() else ""
                _croptop = sout_options['transcode']['croptop'] if 'croptop' in sout_options['transcode'].keys() else ""
                _cropbottom = sout_options['transcode']['cropbottom'] if 'cropbottom' in sout_options['transcode'].keys() else ""
                _cropleft = sout_options['transcode']['cropleft'] if 'cropleft' in sout_options['transcode'].keys() else ""
                _cropright = sout_options['transcode']['cropright'] if 'cropright' in sout_options['transcode'].keys() else ""
                _scale = sout_options['transcode']['scale'] if 'scale' in sout_options['transcode'].keys() else ""
                _width = sout_options['transcode']['width'] if 'width' in sout_options['transcode'].keys() else ""
                _height = sout_options['transcode']['height'] if 'height' in sout_options['transcode'].keys() else ""
                _acodec = sout_options['transcode']['acodec'] if 'acodec' in sout_options['transcode'].keys() else ""
                _ab = sout_options['transcode']['ab'] if 'ab' in sout_options['transcode'].keys() else ""
                _aenc = sout_options['transcode']['aenc'] if 'aenc' in sout_options['transcode'].keys() else ""
                _samplerate = sout_options['transcode']['samplerate'] if 'samplerate' in sout_options['transcode'].keys() else ""
                _channels = sout_options['transcode']['channels'] if 'channels' in sout_options['transcode'].keys() else ""
                _scodec = sout_options['transcode']['scodec'] if 'scodec' in sout_options['transcode'].keys() else ""
                _senc = sout_options['transcode']['senc'] if 'senc' in sout_options['transcode'].keys() else ""
                _soverlay = sout_options['transcode']['soverlay'] if 'soverlay' in sout_options['transcode'].keys() else ""
                _sfilter = sout_options['transcode']['sfilter'] if 'sfilter' in sout_options['transcode'].keys() else ""
                _threads = sout_options['transcode']['threads'] if 'threads' in sout_options['transcode'].keys() else ""
                _audio_sync = sout_options['transcode']['audio_sync'] if 'audio_sync' in sout_options['transcode'].keys() else ""
                _vfilter = sout_options['transcode']['vfilter'] if 'vfilter' in sout_options['transcode'].keys() else ""
                self._transcode_sout_options(vcodec=_vcodec,
                                                    vb=_vb,
                                                    venc=_venc,
                                                    fps=_fps,
                                                    deinterlace=_deinterlace,
                                                    croptop=_croptop,
                                                    cropbottom=_cropbottom,
                                                    cropleft=_cropleft,
                                                    cropright=_cropright,
                                                    scale=_scale,
                                                    width=_width,
                                                    height=_height,
                                                    acodec=_acodec,
                                                    ab=_ab,
                                                    aenc=_aenc,
                                                    samplerate=_samplerate,
                                                    channels=_channels,
                                                    scodec=_scodec,
                                                    senc=_senc,
                                                    soverlay=_soverlay,
                                                    sfilter=_sfilter,
                                                    threads=_threads,
                                                    audio_sync=_audio_sync,
                                                    vfilter=_vfilter)
            if ('duplicate' in sout_options.keys() and isinstance(sout_options['duplicate'], dict)): 
                _dst = sout_options['transcode']['dst'] if 'dst' in sout_options['transcode'].keys() else ""
                _select = sout_options['transcode']['select'] if 'select' in sout_options['transcode'].keys() else ""
                self._duplicate_sout_options(dst=_dst,
                                                    select=_select)
        
    def _audio_options(self, noaudio="", 
                      mono="",
                      volume="", 
                      aout_rate="", 
                      desync="", 
                      audio_filter=""):
        self.audio_options_string = ""
        if (noaudio != ""):
            self.audio_options_string += "--noaudio "
        if (mono != ""):
            self.audio_options_string += "--mono "
        if (volume != ""):
            self.audio_options_string += "--volume " + volume + " "
        if (aout_rate != ""):
            self.audio_options_string += "--aout-rate " + aout_rate + " "
        if (desync != ""):
            self.audio_options_string += "--desync " + desync + " "
        if (audio_filter != ""):
            self.audio_options_string += "--audio-filter=" + audio_filter
            
    def _video_options(self, 
                      no_video="",
                      grayscale="",
                      fullscreen="",
                      nooverlay="",
                      start_time="",
                      stop_time="",
                      zoom="",
                      aspect_ratio="",
                      spumargin="",
                      video_filter="",
                      video_splitter="",
                      sub_filter=""):
        self.video_options_string = ""
        if (no_video != ""):
            self.video_options_string += "--no-video "
        if (grayscale != ""):
            self.video_options_string += "--grayscale "
        if (fullscreen != ""):
            self.video_options_string += "--fullscreen "
        if (nooverlay != ""):
            self.video_options_string += "--nooverlay "
        if (start_time != ""):
            self.video_options_string += "--start-time " + start_time + " "
        if (stop_time != ""):
            self.video_options_string += "--stop-time " + stop_time + " "
        if (zoom != ""):
            self.video_options_string += "--stop-time " + zoom + " "
        if (aspect_ratio!= ""):
            self.video_options_string += "--aspect-ratio " + aspect_ratio + " "
        if (spumargin != ""):
            self.video_options_string += "--spumargin " + spumargin + " "
        if (video_filter != ""):
            self.video_options_string += "--video-filter " + video_filter + " "
        if (video_splitter  != ""):
            self.video_options_string += "--video-splitter " + video_splitter + " "
        if (sub_filter  != ""):
            self.video_options_string += "--sub-filter " + sub_filter + " "
        
    def _playlist_options(self,
                         random="",
                         loop="",
                         repeat="",
                         play_and_stop="",
                         no_repeat="",
                         no_loop=""):
        self.playlist_options_string = ""
        if (random != ""):
            self.playlist_options_string += "--random "
        if (loop != ""):
            self.playlist_options_string += "--loop "
        if (repeat != ""):
            self.playlist_options_string += "--repeat "
        if (play_and_stop != ""):
            self.playlist_options_string += "--play-and-stop "
        if (no_repeat != ""):
            self.playlist_options_string += "--no-repeat "
        if (no_loop != ""):
            self.playlist_options_string += "--no-loop "
        
    def _input_options(self,
                     input_list="",
                     input_slave=""):
        self.input_options_string = ""
        if (input_list != ""):
            self.input_options_string += "--input-list=" + input_list + " "
        if (input_slave != ""):
            self.input_options_string += "--input-slave=" + input_slave + " "

    def _network_options(self,
                        server_port="",
                        iface="",
                        iface_addr="",
                        mtu="",
                        ipv6="",
                        ipv4=""):
        self.playlist_options_string = ""
        if (server_port != ""):
            self.playlist_options_string += "--server-port=" + server_port + " "
        if (iface != ""):
            self.playlist_options_string += "--iface=" + iface + " "
        if (iface_addr != ""):
            self.playlist_options_string += "--iface-addr=" + iface_addr + " "
        if (mtu != ""):
            self.playlist_options_string += "--mtu=" + mtu + " "
        if (ipv6 != ""):
            self.playlist_options_string += "--ipv6 "
        if (ipv4 != ""):
            self.playlist_options_string += "--ipv4 "

    def _caching_options(self,
                        file_caching="",
                        live_caching="",
                        disc_caching="",
                        network_caching="",
                        cr_average="",
                        clock_synchro="",
                        clock_jitter=""):
        self.caching_options_string = ""
        if (file_caching != ""):
            self.caching_options_string += "--file-caching=" + file_caching + " "
        if (live_caching != ""):
            self.caching_options_string += "--live-caching=" + live_caching + " "
        if (disc_caching != ""):
            self.caching_options_string += "--disc-caching=" + disc_caching + " "
        if (network_caching != ""):
            self.caching_options_string += "--network-caching=" + network_caching + " "
        if (cr_average != ""):
            self.caching_options_string += "--cr-average=" + cr_average + " "
        if (clock_synchro != ""):
            self.caching_options_string += "--clock-synchro=" + clock_synchro + " "
        if (clock_jitter != ""):
            self.caching_options_string += "--clock-jitter=" + clock_jitter + " "

    def _misc_options(self,
                     quiet="",
                     color="",
                     search_path="",
                     plugin_path="",
                     no_plugins_cache="",
                     dvd="",
                     vcd="",
                     program="",
                     audio_type="",
                     audio_channel="",
                     spu_channel="",
                     version="",
                     module=""):
        if (quiet != ""):
            self.playlist_options_string += "--quiet "
        if (color != ""):
            self.playlist_options_string += "--color "
        if (search-path != ""):
            self.playlist_options_string += "--search-path " + search_path + " "
        if (plugin_path != ""):
            self.playlist_options_string += "--plugin-path " + plugin_path + " "
        if (no_plugins_cache != ""):
            self.playlist_options_string += "--no-plugins-cache "
        if (dvd != ""):
            self.playlist_options_string += "--dvd " + dvd + " "
        if (vcd != ""):
            self.playlist_options_string += "--vcd " + vcd + " "
        if (program != ""):
            self.playlist_options_string += "--program " + program + " "
        if (audio_type != ""):
            self.playlist_options_string += "--audio-type " + audio_type + " "
        if (audio_channel != ""):
            self.playlist_options_string += "--audio-channel " + audio_channel + " "
        if (spu_channel != ""):
            self.playlist_options_string += "--spu-channel " + spu_channel + " "
        if (version != ""):
            self.playlist_options_string += "--version "
        if (module != ""):
            self.playlist_options_string += "--module " + module + " "
            
    def _std_sout_options(self,
                         access="",
                         mux="",
                         dst="",
                         sap="",
                         group="",
                         sap_ipv6="",
                         slp="",
                         name=""):
        self.std_sout_string = ""
        if (access != ""):
            self.std_sout_string += "access=" + access + ", "
        if (mux != ""):
            self.std_sout_string += "mux=" + mux + ", "
        if (dst != ""):
            self.std_sout_string += "dst=" + dst + ", "
        if (sap != ""):
            self.std_sout_string += "sap=" + sap + ", "
        if (group != ""):
            self.std_sout_string += "group=" + group + ", "
        if (sap_ipv6 != ""):
            self.std_sout_string += "sap-ipv6=" + sap_ipv6 + ", "
        if (slp != ""):
            self.std_sout_string += "slp=" + slp + ", "
        if (name != ""):
            self.std_sout_string += "name=" + name
        self.std_sout_string = self.std_sout_string.rstrip(", ")

    def _display_sout_options(self,
                             novideo="",
                             noaudio="",
                             delay=""):
        self.display_sout_string = ""
        if (novideo != ""):
            self.display_sout_string += "novideo" + ", "
        if (noaudio != ""):
            self.display_sout_string += "noaudio" + ", "
        if (delay != ""):
            self.display_sout_string += "delay=" + delay
        self.display_sout_string = self.std_sout_string.rstrip(", ")

    def _rtp_sout_options(self,
                         dst="",
                         port="",
                         port_video="",
                         port_audio="",
                         sdp="",
                         ttl="",
                         mux="",
                         rtcp_mux="",
                         proto="",
                         name="",
                         description="",
                         url="",
                         email=""):
        self.rtp_sout_string = ""
        if(dst != ""):
            self.rtp_sout_string += "dst=" + dst + ", "
        if(port != ""):
            self.rtp_sout_string += "port=" + port + ", "
        if(port_video != ""):
            self.rtp_sout_string += "port-video=" + port_video + ", "
        if(port_audio != ""):
            self.rtp_sout_string +=  "port-audio=" + port_audio + ", "
        if(sdp != ""):
            self.rtp_sout_string +=  "sdp=" + sdp + ", "
        if(ttl != ""):
            self.rtp_sout_string +=  "ttl=" + ttl + ", "
        if(mux != ""):
            self.rtp_sout_string +=  "mux=" + mux + ", "
        if(rtcp_mux != ""):
            self.rtp_sout_string +=  "rtcp-mux=" + rtcp_mux + ", "
        if(proto != ""):
            self.rtp_sout_string +=  "proto=" + proto + ", "
        if(name != ""):
            self.rtp_sout_string +=  "name=" + name + ", "
        if(description != ""):
            self.rtp_sout_string +=  "description=" + description + ", "
        if(url != ""):
            self.rtp_sout_string +=  "url=" + url + ", "
        if(email != ""):
            self.rtp_sout_string +=  "email=" + email + ", "
        self.rtp_sout_string = self.rtp_sout_string.rstrip(", ")

    def _es_sout_options(self,
                        access_video="",
                        access_audio="",
                        access="",
                        mux_video="",
                        mux_audio="",
                        mux="",
                        dst_video="",
                        dst_audio="",
                        dst=""):
        self.es_sout_string = ""
        if(access_video != ""):
            self.es_sout_string += "access-video=" + access_video + ", "
        if(access_audio != ""):
            self.es_sout_string += "access-audio=" + access_audio + ", "
        if(access != ""):
            self.es_sout_string += "access=" + access + ", "
        if(mux_video != ""):
            self.es_sout_string += "mux-video=" + mux_video + ", "
        if(mux_audio != ""):
            self.es_sout_string += "mux-audio=" + mux_audio + ", "
        if(mux != ""):
            self.es_sout_string += "mux=" + mux + ", "
        if(dst_video != ""):
            self.es_sout_string += "dst-video=" + dst_video + ", "
        if(dst_audio != ""):
            self.es_sout_string += "dst-audio=" + dst_audio + ", "
        if(dst != ""):
            self.es_sout_string += "dst=" + dst + ", "
        self.es_sout_string = self.es_sout_string.rstrip(", ")

    def _transcode_sout_options(self, 
                               vcodec="",
                               vb="",
                               venc="",
                               fps="",
                               deinterlace="",
                               croptop="",
                               cropbottom="",
                               cropleft="",
                               cropright="",
                               scale="",
                               width="",
                               height="",
                               acodec="",
                               ab="",
                               aenc="",
                               samplerate="",
                               channels="",
                               scodec="",
                               senc="",
                               soverlay="",
                               sfilter="",
                               threads="",
                               audio_sync="",
                               vfilter=""):
        self.transcode_sout_string = ""
        if(vcodec != ""):
            self.transcode_sout_string += "vcodec=" + vcodec + ", "
        if(vb != ""):
            self.transcode_sout_string += "vb=" + vb + ", "
        if(venc != ""):
            self.transcode_sout_string += "venc" + venc + ", "
        if(fps != ""):
            self.transcode_sout_string += "fps=" + fps + ", "
        if(deinterlace != ""):
            self.transcode_sout_string += "deinterlace" + ", "
        if(croptop != ""):
            self.transcode_sout_string += "cropadd-croptop=" + croptop + ", "
        if(cropbottom != ""):
            self.transcode_sout_string += "cropadd-cropbottom=" + cropbottom + ", "
        if(cropleft != ""):
            self.transcode_sout_string += "cropadd-cropleft=" + cropleft + ", "
        if(cropright != ""):
            self.transcode_sout_string += "cropadd-cropright=" + cropright + ", "
        if(scale != ""):
            self.transcode_sout_string += "cropadd-cropright=" + cropright + ", "
        if(width != ""):
            self.transcode_sout_string += "width=" + width + ", "
        if(height != ""):
            self.transcode_sout_string += "height=" + height + ", "
        if(acodec != ""):
            self.transcode_sout_string += "acodec=" + acodec + ", "
        if(ab != ""):
            self.transcode_sout_string += "ab=" + ab + ", "
        if(aenc != ""):
            self.transcode_sout_string += "aenc=" + aenc + ", "
        if(samplerate != ""):
            self.transcode_sout_string += "samplerate=" + samplerate + ", "
        if(channels != ""):
            self.transcode_sout_string += "channels=" + channels + ", "
        if(scodec != ""):
            self.transcode_sout_string += "scodec=" + scodec + ", "
        if(senc != ""):
            self.transcode_sout_string += "senc=" + senc + ", "
        if(soverlay != ""):
            self.transcode_sout_string += "soverlay" + ", "
        if(sfilter != ""):
            self.transcode_sout_string += "sfilter=" + sfilter + ", "
        if(threads != ""):
            self.transcode_sout_string += "threads=" + threads + ", "
        if(audio_sync != ""):
            self.transcode_sout_string += "audio-sync" + ", "
        if(vfilter != ""):
            self.transcode_sout_string += "vfilter=" + vfilter + ", "
        self.transcode_sout_string = self.transcode_sout_string.rstrip(", ")

    def _duplicate_sout_options(self, 
                               dst="",
                               select=""):
        self.duplicate_sout_string = ""
        if(dst != ""):
            self.duplicate_sout_string += "dst=" + dst
        if(select != ""):
            self.duplicate_sout_string += "select=" + select
        self.duplicate_sout_string = self.duplicate_sout_string.rstrip(", ")
    
    def get_vlc_cmd(self):
        self.main_command_string = ""
        self.sout_string = ""
        if(self.display_sout_string != ""):
            self.sout_string += "display{" + self.display_sout_string + "}:"
        if(self.rtp_sout_string != ""):
            self.sout_string += "rtp{" + self.rtp_sout_string + "}:"
        if(self.es_sout_string != ""):
            self.sout_string += "es{" + self.es_sout_string + "}:"
        if(self.transcode_sout_string != ""):
            self.sout_string += "transcode{" + self.transcode_sout_string + "}:"
        if(self.duplicate_sout_string != ""):
            self.sout_string += "duplicate{" + self.duplicate_sout_string + "}:"
        self.sout_string = ("--sout '#" + self.sout_string).rstrip(':') + "'"
        self.main_command_string += "cvlc --ttl 1 -vvv " + self.input + " " 
        self.main_command_string += self.audio_options_string + " " 
        self.main_command_string += self.video_options_string + " " 
        self.main_command_string += self.playlist_options_string + " " 
        self.main_command_string += self.input_options_string + " " 
        self.main_command_string += self.misc_options_string + " " 
        self.main_command_string += self.sout_string + " " 
        self.main_command_string += self.caching_options_string + " "
        self.main_command_string = (re.sub(r'[ ]+', ' ', self.main_command_string)).rstrip(" ")
        return self.main_command_string