package com.nui.android;

import android.view.ScaleGestureDetector;

/**
 * Created by ericv on 11/2/2015.
 */
public class PinchGestureListener extends ScaleGestureDetector.SimpleOnScaleGestureListener {
    IServer server;
    SwipeGestureListener swiper;
    private long lastUpdate = 0;
    public PinchGestureListener(IServer server, SwipeGestureListener swiper){
        this.server = server;
        this.swiper = swiper;
    }
    @Override
    public boolean onScale(ScaleGestureDetector detector){
        long curTime = System.currentTimeMillis();
        if(curTime - lastUpdate > 500) {
            lastUpdate = curTime;
            swiper.Pinching();
            server.SendData(new PinchGesture("circle"));
        }
        return true;
    }
}
