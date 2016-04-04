package com.nui.android;

import android.util.Log;
import android.view.GestureDetector;
import android.view.MotionEvent;

import com.nui.android.activities.BaseActivity;
import com.nui.android.activities.Bboard;

/**
 * Created by bml on 14-11-2015.
 */
public class FTouchGestureListener extends GestureDetector.SimpleOnGestureListener {
    IServer server;

    public FTouchGestureListener(IServer server){
        super();
        this.server = server;
    }

    @Override
    public boolean onSingleTapUp(MotionEvent event) {
        Log.d("TOUCH", "onTouch() " + Bboard.GetSelectedShape());
        server.SendData(new MobileGesture(BaseActivity.GetSelectedShape(), "Pinch", "Pull"));
        return true;
    }
}
