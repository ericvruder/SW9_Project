package com.nui.android;

import android.util.Log;
import android.view.GestureDetector;
import android.view.MotionEvent;

import com.nui.android.activities.BaseActivity;

import java.util.Timer;
import java.util.TimerTask;

/**
 * Created by ericv on 11/2/2015.
 */
public class SwipeGestureListener extends GestureDetector.SimpleOnGestureListener {

    IServer server;
    BaseActivity baseActivity;
    boolean pinching = false;

    public SwipeGestureListener(IServer server, BaseActivity baseActivity){
        super();
        this.server = server;
        this.baseActivity = baseActivity;
    }

    public void Pinching(){
        if(!pinching) {
            pinching = true;
            Timer stopPinching = new Timer();
            TimerTask t = new TimerTask() {
                @Override
                public void run() {
                    pinching = false;
                }
            };
            stopPinching.schedule(t, 500);
        }
    }

    @Override
    public boolean onDown(MotionEvent event){
        return true;
    }

    @Override
    public boolean onFling(MotionEvent firstEvent, MotionEvent secondEvent, float vx, float vy){
        if(!pinching) {
            float diff = firstEvent.getY() - secondEvent.getY();
            if (diff >= 100f) {
                server.SendData(new SwipeGesture("push", baseActivity.GetSelectedShape()));
                Log.d("SWIPE", "onFling() push " + baseActivity.GetSelectedShape() );
            } else if (diff <= -100f) {
                server.SendData(new SwipeGesture("pull", baseActivity.GetSelectedShape()));
                Log.d("SWIPE", "onFling() pull " + baseActivity.GetSelectedShape());
            }
        }
        return true;
    }

}
