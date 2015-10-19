package com.nui.android;

import android.util.Log;

import java.io.PrintWriter;
import java.net.Socket;

/**
 * Created by Elias on 19-10-2015.
 */



public class TCPDiscover implements Runnable {
    private String ipaddr;
    private int port;
    private Socket tSocket;
    public TCPDiscover(String ipaddr, int port) {
        this.ipaddr = ipaddr;
        this.port = port;
    }

    public void run() {
        try {
            tSocket = new Socket(ipaddr, port);
            //out = new PrintWriter(clientSocket.getOutputStream(), true);
        }
        catch (Exception e ){
            Thread.currentThread().interrupt();
        }

        if (tSocket.isConnected())
        {
            Network.tcpIp = ipaddr;
            Thread.currentThread().getThreadGroup().interrupt();
        }
    }
}
