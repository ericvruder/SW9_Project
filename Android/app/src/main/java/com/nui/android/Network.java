package com.nui.android;
import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;
//network
import java.net.InetAddress;
import java.net.Socket;
import java.net.UnknownHostException;
//JSON
import com.google.gson.Gson;
/**
 * Created by Elias on 08-10-2015.
 */
public class Network {

    public void SendMessage (String message)
    {
    Gson gson = new Gson();
            gson.toJson(message);


    }

}
