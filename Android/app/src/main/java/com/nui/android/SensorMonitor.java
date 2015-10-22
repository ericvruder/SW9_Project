package com.nui.android;

import android.content.Context;
import android.hardware.Sensor;
import android.hardware.SensorEventListener;
import android.hardware.SensorManager;

/**
 * Created by ericv on 10/14/2015.
 */
public abstract class SensorMonitor implements SensorEventListener {

    private static SensorManager manager;
    protected Sensor sensor;

    IServer server;

    protected long lastUpdate = 0;
    protected long lastUpdateThreshold = 100;

    public SensorMonitor(IServer server, Context context, int sensorType){
        this.server = server;
        manager = (SensorManager) context.getSystemService(Context.SENSOR_SERVICE);
        sensor = manager.getDefaultSensor(sensorType);
        manager.registerListener(this, sensor , SensorManager.SENSOR_DELAY_NORMAL);
    }


    public void Pause(){
        manager.unregisterListener(this);
    }

    public void Resume(){
        manager.registerListener(this, sensor, SensorManager.SENSOR_DELAY_NORMAL);
    }
}
