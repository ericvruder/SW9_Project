package com.nui.android;
import android.os.Bundle;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;
//network
//import java.net.InetAddress;
//import java.net.Socket;
//import java.net.UnknownHostException;
import com.github.nkzawa.socketio.client.IO;
import com.github.nkzawa.socketio.client.Socket;
//JSON
import com.google.gson.Gson;
/**
 * Created by Elias on 08-10-2015.
 */



public class Network {

    public Socket mySocket;

    Network()
    {
        mSocket.connect();
    }

    public void SendObject (Object obj)
    {
        Gson gson = new Gson();
        String msg = gson.toJson(obj);

        // send it on our connection
        mSocket.emit("JSON msg",msg);
        System.out.println(msg);
    }

    public Socket mSocket;
    {
        try {
            //mSocket = IO.socket("http://127.0.0.1:21");
            mSocket = IO.socket("http://192.168.1.3:21");
        }
        catch (java.net.URISyntaxException e) {
            System.err.println("Socket Error");
        }

    }


}


