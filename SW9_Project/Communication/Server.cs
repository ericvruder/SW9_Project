using System;
using System.Threading;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SW9_Project {
    class Server {

        int port = 8000;
        int userLimit = 10;
        TcpListener listener;
        Socket socket;
        public Server() {
            listener = new TcpListener(IPAddress.Any, port);
            listener.Start();/*
            Task.Factory.StartNew(() => {
                for (int i = 0; i < userLimit; i++) {
                    StartService();
                }
            });*/
            StartService();
        }

        private void StartService() {
            socket = listener.AcceptSocket();
            User user = new User(); // This should find the correct user according to the kinect, not just create a new one. 
            user.AddMobileConnection(socket);
            
        }
    }
}
