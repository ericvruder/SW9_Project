package com.nui.android;

import android.util.Log;
import android.view.ScaleGestureDetector;

import com.nui.android.activities.BaseActivity;

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
            server.SendData(new MobileGesture(BaseActivity.GetSelectedShape(), "Pinch", "Push"));
            Log.d("PINCH", "OnScale() " + BaseActivity.GetSelectedShape());
        }
        return true;
    }
}
