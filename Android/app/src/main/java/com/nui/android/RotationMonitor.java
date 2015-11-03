package com.nui.android;

import android.content.Context;
import android.hardware.Sensor;
import android.hardware.SensorEvent;

/**
 * Created by ericv on 10/14/2015.
 */
public class RotationMonitor extends SensorMonitor {

    public RotationMonitor(IServer server, Context context){
        super(server, context, Sensor.TYPE_ORIENTATION);
    }

    private float averagePitch = 0f;
    private float currentPitch = 0f;

    public boolean IsForwardTilt(){
        if(currentPitch < averagePitch) {
            return true;
        }
        else return false;
    }


    @Override
    public void onSensorChanged(SensorEvent event) {
        if (averagePitch == 0) {
            averagePitch = event.values[1];
        } else {
            averagePitch = (averagePitch + event.values[1]) / 2;
        }
        currentPitch = event.values[1];
    }

    @Override
    public void onAccuracyChanged(Sensor sensor, int accuracy) {

    }
}
