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

    private float[] mGravity;
    private float[] mMagnetic;

    private float[] getDirection()
    {

        float[] temp = new float[9];
        float[] R = new float[9];
        //Load rotation matrix into R
        SensorManager.getRotationMatrix(temp, null,
                mGravity, mMagnetic);

        //Remap to camera's point-of-view
        /*SensorManager.remapCoordinateSystem(temp,
                SensorManager.AXIS_X,
                SensorManager.AXIS_Z, R);*/

        //Return the orientation values
        float[] values = new float[3];
        SensorManager.getOrientation(temp, values);

        return values;
    }

    @Override
    public void onSensorChanged(SensorEvent event) {

        if(event.sensor.getType() == Sensor.TYPE_ACCELEROMETER){
            mGravity = event.values.clone();

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

        if(event.sensor.getType() == Sensor.TYPE_MAGNETIC_FIELD) {
            mMagnetic = event.values.clone();
        }

        if(mGravity != null && mMagnetic != null) {
            Log.d("sensor1: ", Double.toString(Math.toDegrees(getDirection()[0])) + " " + Double.toString(Math.toDegrees(getDirection()[1])) + " " + Double.toString(Math.toDegrees(getDirection()[2])));
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
        super(server, context, Sensor.TYPE_ACCELEROMETER, Sensor.TYPE_MAGNETIC_FIELD);
        rMonitor = monitor;
    }
}
