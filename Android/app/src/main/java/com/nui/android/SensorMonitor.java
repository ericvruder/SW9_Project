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
    protected Sensor sensor1;
    protected Sensor sensor2;

    IServer server;

    protected long lastUpdate = 0;
    protected long lastUpdateThreshold = 100;

    public SensorMonitor(IServer server, Context context, int sensorType){
        this.server = server;
        manager = (SensorManager) context.getSystemService(Context.SENSOR_SERVICE);
        sensor1 = manager.getDefaultSensor(sensorType);
        manager.registerListener(this, sensor1, SensorManager.SENSOR_DELAY_NORMAL);
    }

    public SensorMonitor(IServer server, Context context, int sensorType1, int sensorType2){
        this.server = server;
        manager = (SensorManager) context.getSystemService(Context.SENSOR_SERVICE);
        sensor1 = manager.getDefaultSensor(sensorType1);
        manager.registerListener(this, sensor1, SensorManager.SENSOR_DELAY_NORMAL);
        sensor2 = manager.getDefaultSensor(sensorType2);
        manager.registerListener(this, sensor1, SensorManager.SENSOR_DELAY_NORMAL);
    }


    public void Pause(){
        manager.unregisterListener(this);
    }

    public void Resume(){
        if(sensor1 != null)
            manager.registerListener(this, sensor1, SensorManager.SENSOR_DELAY_NORMAL);
        if(sensor2 != null)
            manager.registerListener(this, sensor2, SensorManager.SENSOR_DELAY_NORMAL);
    }
}
