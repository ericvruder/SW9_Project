package com.nui.android;

import android.content.Context;
import android.hardware.Sensor;
import android.hardware.SensorEvent;
import android.hardware.SensorManager;

/**
 * Created by ericv on 10/13/2015.
 */
public class AccelerometerMonitor extends SensorMonitor {

    @Override
    public void onSensorChanged(SensorEvent event) {

        if(event.sensor.getType() == Sensor.TYPE_ACCELEROMETER){
            long curTime = System.currentTimeMillis();
            float x = event.values[0];
            float y = event.values[1];
            float z = event.values[2];
            String values = "X: " + x + " Y: " + y + " Z: " + z;
            if(IsThrown(x,y,z,curTime)){
                ThrowGesture data = new ThrowGesture("circle");
                server.SendData(data);
            }
            else if(IsTilt(x,y,z,curTime)){
                String direction = "";
                if(rMonitor.IsForwardTilt()) { direction = "push"; }
                else { direction = "pull"; }
                TiltGesture data = new TiltGesture("circle", direction);
                server.SendData(data);
            }
        }
    }

    private boolean IsThrown(float x, float y, float z, long curTime){

        float accelationSquareRoot = (x * x + y * y + z * z)
                / (SensorManager.GRAVITY_EARTH * SensorManager.GRAVITY_EARTH);
        if (accelationSquareRoot >= 5) //
        {
            long timeDiff = curTime - lastUpdate;
            if (timeDiff < 1000) {
                return false;
            }
            lastUpdate = curTime;
            return true;
        }
        return false;
    }
    private boolean IsTilt(float x, float y, float z, long curTime){

        float accelationSquareRoot = (x * x + y * y + z * z)
                / (SensorManager.GRAVITY_EARTH * SensorManager.GRAVITY_EARTH);
        if (accelationSquareRoot >= 2) //
        {
            long timeDiff = curTime - lastUpdate;
            if (timeDiff < 1000) {
                return false;
            }
            lastUpdate = curTime;
            return true;
        }
        return false;
    }

    @Override
    public void onAccuracyChanged(Sensor sensor, int accuracy) {

    }
    private RotationMonitor rMonitor;
    public AccelerometerMonitor(IServer server, RotationMonitor monitor, Context context){
        super(server,context, Sensor.TYPE_ACCELEROMETER);
        rMonitor = monitor;
    }
}
