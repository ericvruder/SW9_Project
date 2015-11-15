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
    BaseActivity baseActivity;

    public TouchGestureListener(IServer server, BaseActivity baseActivity){
        super();
        this.server = server;
        this.baseActivity = baseActivity;
    }

    @Override
    public boolean onSingleTapUp(MotionEvent event) {
        server.SendData(new ThrowGesture(baseActivity.GetSelectedShape()));
        server.SendData(new TiltGesture(baseActivity.GetSelectedShape()));
        Log.d("TOUCH", "onTouch() " + baseActivity.GetSelectedShape());
        return true;
    }
}
