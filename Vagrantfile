
# Install libuv for Kestrel from source code
install_libuv_cmd = 'sudo apt-get install autoconf automake build-essential' \
                    ' libtool -y;' \
                    'LIBUV_VERSION=1.0.0-rc2 && curl -sSL' \
                    ' https://github.com/joyent/libuv/archive/v${LIBUV_VERSION}.tar.gz' \
                    ' | sudo tar zxfv - -C /usr/local/src &&' \
                    ' cd /usr/local/src/libuv-$LIBUV_VERSION &&' \
                    ' sudo sh autogen.sh && sudo ./configure && sudo make &&' \
                    ' sudo make install && sudo rm -rf /usr/local/src/libuv-$LIBUV_VERSION' \
                    ' && sudo ldconfig'


Vagrant.configure(2) do |config|
  config.vm.box = "akoeplinger/mono-aspnetvnext"
  config.vm.network "forwarded_port", guest: 5004, host: 8888

  config.vm.provision "install_libuv", type: "shell", privileged: false,
        inline: install_libuv_cmd
end
