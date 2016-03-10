package com.nui.android;

import java.net.DatagramPacket;

/**
 * Created by ericv on 10/13/2015.
 */
public interface IServer {
    public void SendData(MobileGesture data);
    public boolean SendMessage(String message);
    public String GetHost();
}
