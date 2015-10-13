package com.nui.android;
import android.os.Bundle;

import java.net.InetAddress;
import java.net.Socket;
import java.io.PrintWriter;
import android.util.Log;

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

    public void SendData(AccelerometerData data){
        SendMessage(gsonConverter.toJson(data));
    }

    public void Pause(){
        //TODO: IMPLEMENT
    }

    public void Resume(){
        //TODO: IMPLEMENT
    }


}


