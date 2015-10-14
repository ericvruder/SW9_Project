package com.nui.android;

import android.content.Context;
import android.hardware.Sensor;
import android.hardware.SensorEvent;
import android.hardware.SensorEventListener;
import android.hardware.SensorManager;

/**
 * Created by ericv on 10/13/2015.
 */
public class AccelerometerMonitor extends SensorMonitor{

    @Override
    public void onSensorChanged(SensorEvent event) {

        if(event.sensor.getType() == Sensor.TYPE_ACCELEROMETER){
            long curTime = System.currentTimeMillis();
            if((curTime - lastUpdate) > lastUpdateThreshold){
                lastUpdate = curTime;
                float x = event.values[0];
                float y = event.values[1];
                float z = event.values[2];

                AccelerometerData data = new AccelerometerData(x, y, z, curTime);
                server.SendData(data);
            }
        }
    }

    @Override
    public void onAccuracyChanged(Sensor sensor, int accuracy) {

    }

    public AccelerometerMonitor(IServer server, Context context){
        super(server,context, Sensor.TYPE_ACCELEROMETER);
    }
}
