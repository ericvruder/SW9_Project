package com.nui.android;
import android.os.Bundle;

import java.net.InetAddress;
import java.net.InterfaceAddress;
import java.net.NetworkInterface;
import java.net.Socket;
import java.io.PrintWriter;
import android.util.Log;
import java.net.DatagramSocket;
import java.net.DatagramPacket;
import java.util.Enumeration;

//JSON
import com.google.gson.Gson;
/**
 * Created by Elias on 08-10-2015.
 */



public class Network implements IServer {

    String TAG = "Network";

    Socket clientSocket;
    String host;
    int port;

    Gson gsonConverter;

    PrintWriter out;

    public Network(){
        this("192.168.1.129", 8000);
    }

    public Network(String host, int port){
        gsonConverter = new Gson();

        this.host = host;
        this.port = port;
        Thread connectionThread = new Thread(){
            public void run(){
                SetupConnection();
            }
        };
        connectionThread.start();
    }

    public Network(final Boolean unicastDiscover){
        //Wont be using the paramenter, but it keeps it seperated.
        gsonConverter = new Gson();
        Thread NetworkDiscoveryThread = new Thread(){
            public void run(){
                FindServer(unicastDiscover);
            }
        };
        NetworkDiscoveryThread.start();
    }

    private void FindServer(boolean unicast){
        byte[] ipAddr;
        DatagramSocket c;
        int bport = 49255; //this port is outside IANA registartion range, so probably won't annoy anyone.
        // Find the server using UDP broadcast
        try {
            //Open a random port to send the package

            c = new DatagramSocket(bport);
            c.setBroadcast(true);
            long startTime = System.nanoTime();
            byte[] sendData = "DISCOVER_IS903SEVER_REQUEST".getBytes();
            //byte[] sendData2 = "255Flood".getBytes();
            //Try the 255.255.255.255 first (seems to work)
                try {
                    DatagramPacket sendPacket = new DatagramPacket(sendData, sendData.length, InetAddress.getByName("255.255.255.255"), bport);
                    c.send(sendPacket);
                    System.out.println(getClass().getName() + ">>> Request packet sent to: 255.255.255.255 (DEFAULT)");
                } catch (Exception e) {
                    Log.w(TAG,"255 fail: " + e.toString());
                }

            // Broadcast the message over all the network interfaces

            Enumeration interfaces = NetworkInterface.getNetworkInterfaces();
            while (interfaces.hasMoreElements()) {
                NetworkInterface networkInterface = (NetworkInterface) interfaces.nextElement();

                if (networkInterface.isLoopback() || !networkInterface.isUp()) {
                    continue; // Don't want to broadcast to the loopback interface
                }

                for (InterfaceAddress interfaceAddress : networkInterface.getInterfaceAddresses()) {
                    InetAddress broadcast = interfaceAddress.getBroadcast();

                    if (broadcast == null) {
                        continue;
                    }

                    ipAddr = broadcast.getAddress();

                    for(int i=1; i<255; i++) {
                        ipAddr[2] = (byte)i;
                        broadcast = InetAddress.getByAddress(ipAddr);

                        if(unicast){

                            for(int j=1; j<255; j++) {
                                ipAddr[3] = (byte) j;
                                broadcast = InetAddress.getByAddress(ipAddr);

                                // Send the broadcast package!
                                try {
                                    DatagramPacket sendPacket = new DatagramPacket(sendData, sendData.length, broadcast, bport);
                                    c.send(sendPacket);
                                } catch (Exception e) {
                                    Log.w(TAG, "Failed to send package to " + broadcast.toString());
                                }

                                System.out.println(getClass().getName() + ">>> Request packet sent to: " + broadcast.getHostAddress() + "; Interface: " + networkInterface.getDisplayName());
                            }
                        }
                        else{
                            // Send the broadcast package!
                            try {
                                DatagramPacket sendPacket = new DatagramPacket(sendData, sendData.length, broadcast, bport);
                                c.send(sendPacket);
                            } catch (Exception e) {
                                Log.w(TAG, "Failed to send package to " + broadcast.toString());
                            }

                            System.out.println(getClass().getName() + ">>> Request packet sent to: " + broadcast.getHostAddress() + "; Interface: " + networkInterface.getDisplayName());

                        }
                    }
                }
            }

            System.out.println(getClass().getName() + ">>> Done looping over all network interfaces. Now waiting for a reply!");
            Log.i(TAG, ">>> Done looping over all network interfaces. Now waiting for a reply!");
            long estimatedTime = System.nanoTime() - startTime;
            //System.out.println(getClass().getName() + ">>> Time elapsed: " + (estimatedTime/1000000000.0) + "seconds");
            Log.w(TAG,">>> Time elapsed: " + (estimatedTime/1000000000.0) + "seconds");
            //Wait for a response
            byte[] recvBuf = new byte[15000];
            DatagramPacket receivePacket = new DatagramPacket(recvBuf, recvBuf.length);
            boolean cLoop = true;
            while(cLoop) {
                c.receive(receivePacket);

                //We have a response
                System.out.println(getClass().getName() + ">>> Broadcast response from server: " + receivePacket.getAddress().getHostAddress());
                Log.i(TAG, ">>> Broadcast response from server: " + receivePacket.getAddress().getHostAddress());
                //Check if the message is correct
                String message = new String(receivePacket.getData()).trim();
                Log.i(TAG," ->" + message);
                if (message.equals("DISCOVER_IS903SERVER_RESPONSE")) {
                    //DO SOMETHING WITH THE SERVER'S IP (for example, store it in your controller)
                    //Controller_Base.setServerIp(receivePacket.getAddress());
                    this.host = receivePacket.getAddress().getHostAddress();
                    this.port = 8000; // can be implemented better.
                    cLoop = false;
                }
            }

            //Close the port!
            c.close();
        } catch (Exception e) {
            Log.e(TAG, "UDP socket failed: " + e.toString());
        }
        Log.w(TAG,"Done broadcasting");
        SetupConnection();
        Log.w(TAG, "Network Setup complete.");
    }

    private void SetupConnection(){
        try {
            clientSocket = new Socket(host, port);
            out = new PrintWriter(clientSocket.getOutputStream(), true);
        }
        catch (Exception e){
            Log.i(TAG, "Could not open a socket to " + host +":" + port);
        }
    }

    public void SendMessage(String message){
        try {
            out.println(message);
            out.flush();
        }catch (Exception e){
            Log.i(TAG, "Could not send message \"" + message + "\". Exception: " + e.getMessage());
        }
    }

    public void SendData(MobileData data){
        if (clientSocket == null)
            return;
        if (!this.clientSocket.isConnected()) //should prevent some crashes if the network isn't connected.
            return;
        try {
            String t = gsonConverter.toJson(data);
            SendMessage(t);
        }
        catch (Exception e){
            Log.i("!", " "+ e.getMessage()); // java.lang.NullPointerException: println needs a message
        }
    }

    public void Pause(){
        //TODO: IMPLEMENT
    }

    public void Resume(){
        //TODO: IMPLEMENT
    }


}


