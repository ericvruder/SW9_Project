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
        mySocket = mSocket;
    }

    public void SendObject (Class obj)
    {
        Gson gson = new Gson();
        gson.toJson(obj);

        // send it on our connection

    }

    public Socket mSocket;
    {
        try {
            mSocket = IO.socket("127.0.0.1:21");

        } catch (java.net.URISyntaxException e) {}

    }


}
