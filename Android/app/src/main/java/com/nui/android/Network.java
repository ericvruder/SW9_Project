package com.nui.android;

import java.io.BufferedReader;
import java.io.EOFException;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
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
import com.nui.android.activities.BaseActivity;
import com.nui.android.activities.PullTestActivity;
import com.nui.android.activities.PushTestActivity;

/**
 * Created by Elias on 08-10-2015.
 */

public class Network implements IServer {

    String TAG = "Network";
    private static final String SERVER_IP = "192.168.1.10";

    Socket clientSocket;
    String host;
    int port;

    Gson gsonConverter;
    BaseActivity activity;

    PrintWriter out;
    PushTestActivity pushTestActivity;
    PullTestActivity pullTestActivity;

    private static Network instance;

    public static void initInstance(BaseActivity ba) {
        if (instance == null) {
            instance = new Network(SERVER_IP, 8000, ba);
        }
    }

    public static Network getInstance() {
        return instance;
    }

    public static void SetActivity(PushTestActivity pushTestActivity) {
        instance.pushTestActivity = pushTestActivity;
    }

    public static void SetActivity(PullTestActivity pullTestActivity) {
        instance.pullTestActivity = pullTestActivity;
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
                    activity.StartPullTest();
                }
                else if(line.equals("startpush")){
                    activity.StartPushTest();
                }
                else if(line.equals("nextshape:circle")){
                    pullTestActivity.runOnUiThread(
                            new Runnable() {
                                @Override
                                public void run() {
                                    pullTestActivity.SetCircleShape();
                                    //SendMessage("nextshape:" + activity.NextShape());
                                }
                            }
                    );
                }
                else if(line.equals("nextshape:square")){
                    pullTestActivity.runOnUiThread(
                            new Runnable() {
                                @Override
                                public void run() {
                                    pullTestActivity.SetSquareShape();
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
                    pushTestActivity.runOnUiThread(
                            new Runnable() {
                                @Override
                                public void run() {
                                    pushTestActivity.SwitchPosition();
                                }
                            }
                    );
                }
            }
        } catch(Exception e) {
            if(e instanceof IOException || e instanceof EOFException) {
                Reconnect();
            } else {
                Log.e(TAG, e.getMessage());
            }
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
        if (!this.clientSocket.isConnected()) //should prevent some crashes if the network isn't connected.
            return;
        try {
            String t = gsonConverter.toJson(data);
            SendMessage(t);
        }
        catch (Exception e){
            if(e instanceof IOException || e instanceof EOFException) {
                Reconnect();
            } else {
                Log.i("!", " " + e.getMessage()); // java.lang.NullPointerException: println needs a message
            }
        }
    }

    public void Pause(){
        //TODO: IMPLEMENT
    }

    public void Resume(){
        //TODO: IMPLEMENT
    }


}


