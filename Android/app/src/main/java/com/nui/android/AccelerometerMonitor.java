package com.nui.android;

import android.content.Context;
import android.hardware.Sensor;
import android.hardware.SensorEvent;
import android.hardware.SensorEventListener;
import android.hardware.SensorManager;

/**
 * Created by ericv on 10/13/2015.
 */
public class AccelerometerMonitor implements SensorEventListener{

    IServer server;

    private SensorManager senSensorManager;
    private Sensor senAccelerometer;

    private long lastUpdate = 0;
    private long lastUpdateThreshold = 100;

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

    public void Pause(){
        senSensorManager.unregisterListener(this);
    }

    public void Resume(){
        senSensorManager.registerListener(this, senAccelerometer, SensorManager.SENSOR_DELAY_NORMAL);
    }

    public AccelerometerMonitor(IServer server, Context context){
        this.server = server;
        senSensorManager = (SensorManager) context.getSystemService(Context.SENSOR_SERVICE);
        senAccelerometer = senSensorManager.getDefaultSensor(Sensor.TYPE_ACCELEROMETER);
        senSensorManager.registerListener(this, senAccelerometer , SensorManager.SENSOR_DELAY_NORMAL);
    }
}
