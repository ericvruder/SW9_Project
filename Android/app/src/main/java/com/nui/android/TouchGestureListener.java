package com.nui.android;

import android.util.Log;
import android.view.GestureDetector;
import android.view.MotionEvent;

import com.nui.android.activities.BaseActivity;

/**
 * Created by bml on 14-11-2015.
 */
public class TouchGestureListener extends GestureDetector.SimpleOnGestureListener {
    IServer server;

    public TouchGestureListener(IServer server){
        super();
        this.server = server;
    }

    @Override
    public boolean onSingleTapUp(MotionEvent event) {
        Log.d("TOUCH", "onTouch() " + BaseActivity.GetSelectedShape());
        server.SendData(new MobileGesture(BaseActivity.GetSelectedShape(), "Pinch", "Pull"));
        return true;
    }
}
