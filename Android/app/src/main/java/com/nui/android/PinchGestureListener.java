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
    BaseActivity baseActivity;
    private long lastUpdate = 0;
    public PinchGestureListener(IServer server, SwipeGestureListener swiper, BaseActivity baseActivity){
        this.server = server;
        this.swiper = swiper;
        this.baseActivity = baseActivity;
    }
    @Override
    public boolean onScale(ScaleGestureDetector detector){
        long curTime = System.currentTimeMillis();
        if(curTime - lastUpdate > 500) {
            lastUpdate = curTime;
            swiper.Pinching();
            server.SendData(new PinchGesture(baseActivity.GetSelectedShape()));
            Log.d("PINCH", "OnScale() " + baseActivity.GetSelectedShape());
        }
        return true;
    }
}
