package com.nui.android;

import android.content.Context;
import android.hardware.Sensor;
import android.hardware.SensorEvent;
import android.hardware.SensorManager;
import android.util.Log;

import com.nui.android.activities.BaseActivity;

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
                ThrowGesture data = new ThrowGesture(BaseActivity.GetSelectedShape());
                server.SendData(data);
                Log.d("ACCELEROMETER", "Thrown");
            }
        }
    }

    public boolean IsThrown(float x, float y, float z, long curTime){

        float accelationSquareRoot = ((y * y + z * z) * 1.33f)
                / (SensorManager.GRAVITY_EARTH * SensorManager.GRAVITY_EARTH);
        if (accelationSquareRoot >= 1.9) //
        {
            long timeDiff = curTime - lastUpdate;
            if (timeDiff < 500) {
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
