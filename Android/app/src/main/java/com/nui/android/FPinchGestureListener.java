package com.nui.android;

import android.util.Log;
import android.view.ScaleGestureDetector;

import com.nui.android.activities.BaseActivity;
import com.nui.android.activities.Bboard;

/**
 * Created by ericv on 11/2/2015.
 */
public class FPinchGestureListener extends ScaleGestureDetector.SimpleOnScaleGestureListener {
    IServer server;
    private long lastUpdate = 0;
    public FPinchGestureListener(IServer server){
        this.server = server;
    }

    boolean running = false;
    public void Stop(){
        running = false;
    }
    public void Start(){
        running = true;
    }

    @Override
    public boolean onScale(ScaleGestureDetector detector){
        if(running) {
            long curTime = System.currentTimeMillis();
            if (curTime - lastUpdate > 500) {
                lastUpdate = curTime;
                server.SendData(new MobileGesture(Bboard.GetSelectedShape(), "Pinch", "Push"));
                Log.d("PINCH", "OnScale() " + BaseActivity.GetSelectedShape());
            }

        }
        return true;
    }
}
