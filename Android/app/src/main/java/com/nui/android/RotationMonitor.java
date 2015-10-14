package com.nui.android;

import android.content.Context;
import android.hardware.Sensor;
import android.hardware.SensorEvent;
import android.hardware.SensorManager;

/**
 * Created by ericv on 10/14/2015.
 */
public class RotationMonitor extends SensorMonitor {

    private static final int FROM_RADS_TO_DEGS = -57;

    public RotationMonitor(IServer server, Context context){
        super(server, context, Sensor.TYPE_ORIENTATION);
    }

    @Override
    public void onSensorChanged(SensorEvent event) {
        if (event.sensor == sensor) {
            long curTime = System.currentTimeMillis();
            if((curTime - lastUpdate) > lastUpdateThreshold) {
                lastUpdate = curTime;
                /*
                if (event.values.length > 4) {
                    float[] truncatedRotationVector = new float[4];
                    System.arraycopy(event.values, 0, truncatedRotationVector, 0, 4);
                    updateVector(truncatedRotationVector);
                } else {
                    updateVector(event.values);
                }*/
                float pitch = event.values[1];
                float roll = event.values[2];
                server.SendData(new RotationData(pitch,roll,lastUpdate));
            }
        }
    }

    private void updateVector(float[] vectors) {
        float[] rotationMatrix = new float[9];
        SensorManager.getRotationMatrixFromVector(rotationMatrix, vectors);
        int worldAxisX = SensorManager.AXIS_X;
        int worldAxisZ = SensorManager.AXIS_Z;
        float[] adjustedRotationMatrix = new float[9];
        SensorManager.remapCoordinateSystem(rotationMatrix, worldAxisX, worldAxisZ, adjustedRotationMatrix);
        float[] orientation = new float[3];
        SensorManager.getOrientation(adjustedRotationMatrix, orientation);
        float pitch = orientation[1] * FROM_RADS_TO_DEGS;
        float roll = orientation[2] * FROM_RADS_TO_DEGS;
        server.SendData(new RotationData(pitch, roll, lastUpdate));
    }

    @Override
    public void onAccuracyChanged(Sensor sensor, int accuracy) {

    }
}
