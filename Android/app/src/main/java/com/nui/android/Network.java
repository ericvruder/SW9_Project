package com.nui.android;

import java.io.BufferedReader;
import java.io.EOFException;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.net.InetAddress;
import java.net.InterfaceAddress;
import java.net.NetworkInterface;
import java.net.Socket;
import java.io.PrintWriter;
import java.security.spec.ECField;
import java.sql.Time;
import java.util.Timer;

import android.content.Context;
import android.util.Log;
import android.os.Vibrator;


//JSON
import com.google.gson.Gson;
import com.nui.android.activities.BaseActivity;

/**
 * Created by Elias on 08-10-2015.
 */

public class Network implements IServer {

    String TAG = "Network";
    private static final String SERVER_IP = "192.168.1.3";

    Socket clientSocket;
    String host;
    int port;

    Gson gsonConverter;
    BaseActivity activity;

    PrintWriter out;

    private int packets = 0;
    private long tStart = 0;
    private long tDelta;
    private double pps;

    private static Network instance;
    private DatagramSocket datagramSocket;
    private InetAddress hostInet;

    public static void initInstance(BaseActivity ba) {

        if (instance == null) {
            instance = new Network(SERVER_IP, 8000, ba);
        }
    }

    public static Network getInstance() {
        return instance;
    }

    public Network(BaseActivity activity){
        this(SERVER_IP, 8000, activity);
    }

    public Network(String host, int port, BaseActivity activity){
        gsonConverter = new Gson();

        this.activity = activity;

        this.host = host;
        this.port = port;
        Thread connectionThread = new Thread(){
            public void run(){
                SetupConnection();
            }
        };
        connectionThread.start();
    }

    public void Reconnect() {
        Log.d("NETWORK", "Reconnecting");
        Thread connectionThread = new Thread(){
            public void run(){
                SetupConnection();
            }
        };
        connectionThread.start();
    }

    private void SetupConnection() {
        try {
            clientSocket = new Socket(host, port);
            out = new PrintWriter(clientSocket.getOutputStream(), true);
            StartListener((clientSocket.getInputStream()));

            connectDatagram();

        } catch (Exception e) {
            if(e instanceof IOException || e instanceof EOFException){
                Reconnect();
            } else {
                Log.i(TAG, "Could not open a socket to " + host + ":" + port);
            }
        }
    }

    private void StartListener(InputStream inputStream){
        BufferedReader r = new BufferedReader(new InputStreamReader(inputStream));
        String line;
        try {
            while ((line = r.readLine()) != null) {
                if(line.equals("startpull")) {
                    activity.runOnUiThread(
                            new Runnable() {
                                @Override
                                public void run() {
                                    activity.StartPullTest();
                                }});
                }
                else if(line.equals("startpush")){
                    activity.runOnUiThread(
                            new Runnable() {
                                @Override
                                public void run() {
                                    activity.StartPushTest();
                                }});
                }
                else if(line.equals("nextshape:circle")){
                    activity.runOnUiThread(
                            new Runnable() {
                                @Override
                                public void run() {
                                    activity.SetCircleShape();
                                    //SendMessage("nextshape:" + activity.NextShape());
                                }
                            }
                    );
                }
                else if(line.equals("nextshape:square")){
                    activity.runOnUiThread(
                            new Runnable() {
                                @Override
                                public void run() {
                                    activity.SetSquareShape();
                                    //SendMessage("nextshape:" + activity.NextShape());
                                }
                            }
                    );
                }
                else if(line.equals("ready")){
                    SendMessage("ready:"+activity.ReadyToStart());
                }
                else if(line.equals("switch")) {
                    // randomly switch the position of the figures
                    activity.runOnUiThread(
                            new Runnable() {
                                @Override
                                public void run() {
                                    activity.SwitchPosition();
                                }
                            }
                    );
                }
            }
        } catch(Exception e) {
            Log.e(TAG, e.getMessage());
        }
    }

    public void SendMessage(String message){
        try {
            out.println(message);
            out.flush();
        }catch (Exception e){
            if(e instanceof IOException || e instanceof EOFException) {
                Reconnect();
            } else {
                Log.i(TAG, "Could not send message \"" + message + "\". Exception: " + e.getMessage());
            }
        }
    }

    public void SendData(MobileGesture data){
        if (clientSocket == null)
            return;
        if(activity.PushOrPull() && data.Shape == null)
            return;
        if (!this.clientSocket.isConnected()) //should prevent some crashes if the network isn't connected.
            return;
        try {
            String t = gsonConverter.toJson(data);
            activity.ClearShapes();
            SendMessage(t);

        }
        catch (Exception e){
            Log.i("!", " " + e.getMessage()); // java.lang.NullPointerException: println needs a message
        }
    }

    public void SendDatagram(byte[] buf) {
        DatagramPacket dp = new DatagramPacket(buf, buf.length);
        try {
            if(datagramSocket == null) {
                Thread connectionThread = new Thread() {
                    public void run() {
                        connectDatagram();
                    }
                };
                connectionThread.start();
                tStart = System.currentTimeMillis();
            }
            datagramSocket.send(dp);
            packets++;
            tDelta = System.currentTimeMillis() - tStart;
            pps = packets/(tDelta / 1000.0);

            Log.d("UDP Packets:", Double.toString(pps));
        } catch (Exception e){
            Log.e("DatagramPacket:", e.toString());
        }
    }

    private void connectDatagram() {
        try {
            datagramSocket = new DatagramSocket();
            hostInet = InetAddress.getByName(host);
            datagramSocket.connect(hostInet, 49255);
            Log.d("Network:", "Socket is bound to " + String.valueOf(datagramSocket.getLocalPort()));
        } catch (Exception e) {
            Log.e("Datagram connection:", e.toString());
        }
    }

    public void Pause(){
        //TODO: IMPLEMENT
    }

    public void Resume(){
        //TODO: IMPLEMENT
    }

    public void FindServer(boolean unicast) {
        Thread UDPThread = new Thread(new UDPDiscover(activity,unicast));
        UDPThread.start();
    }

    public void SetHost(String host){
        this.host = host;

    }

}


