package com.nui.android;

import android.util.Log;
import android.view.ScaleGestureDetector;

/**
 * Created by ericv on 11/2/2015.
 */
public class PinchGestureListener extends ScaleGestureDetector.SimpleOnScaleGestureListener {
    IServer server;
    SwipeGestureListener swiper;
    MainActivity mainActivity;
    private long lastUpdate = 0;
    public PinchGestureListener(IServer server, SwipeGestureListener swiper, MainActivity mainActivity){
        this.server = server;
        this.swiper = swiper;
        this.mainActivity = mainActivity;
    }
    @Override
    public boolean onScale(ScaleGestureDetector detector){
        long curTime = System.currentTimeMillis();
        if(curTime - lastUpdate > 500) {
            lastUpdate = curTime;
            swiper.Pinching();
            server.SendData(new PinchGesture(mainActivity.GetSelectedShape()));
            Log.d("PINCH", "OnScale() " + mainActivity.GetSelectedShape());
        }
        return true;
    }
}
