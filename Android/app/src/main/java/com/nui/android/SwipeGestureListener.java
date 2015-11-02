package com.nui.android;

import android.view.GestureDetector;
import android.view.MotionEvent;

import java.util.Timer;
import java.util.TimerTask;

/**
 * Created by ericv on 11/2/2015.
 */
public class SwipeGestureListener extends GestureDetector.SimpleOnGestureListener {

    IServer server;
    public SwipeGestureListener(IServer server){
        super();
        this.server = server;
    }

    @Override
    public boolean onDown(MotionEvent event){
        return true;
    }
    boolean pinching = false;
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

    public boolean onFling(MotionEvent firstEvent, MotionEvent secondEvent, float vx, float vy){
        if(!pinching) {
            float diff = firstEvent.getY() - secondEvent.getY();
            if (diff >= 100f) {
                server.SendData(new SwipeGesture("push", "circle"));
            } else if (diff <= -100f) {
                server.SendData(new SwipeGesture("pull", "circle"));
            }
        }
        return true;
    }

}
