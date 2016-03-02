package com.nui.android;

import android.content.Context;
import android.hardware.Sensor;
import android.hardware.SensorEvent;
import android.hardware.SensorManager;
import android.util.Log;

import com.nui.android.activities.BaseActivity;

import java.text.DecimalFormat;

/**
 * Created by ericv on 10/13/2015.
 */
public class AccelerometerMonitor extends SensorMonitor {

    private float[] mGravity;
    private float[] mMagnetic;

    private long time = 0;

    private boolean tilt = false;

    public void SetTiltorThrow(String gesture){
        if(gesture.equals("tilt")){
            tilt = true;
        }
        else if(gesture.equals("throw")){
            tilt = false;
        }
    }

    public long getLatestTimestamp() {
        return time;
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
            if(tilt){
                ThrowGesture data = IsTilt(x, y, z);
                if(data != null) {
                    server.SendData(data);
                }
            }
            else if(!tilt && IsThrow(x,y,z, curTime)){
                ThrowGesture data = new ThrowGesture(BaseActivity.GetSelectedShape());
                server.SendData(data);
            }
        }

        if(event.sensor.getType() == Sensor.TYPE_MAGNETIC_FIELD) {
            mMagnetic = event.values.clone();
        }

        if(mGravity != null && mMagnetic != null) {
            //Log.d("sensor1: ", Double.toString(Math.toDegrees(getDirection()[0])) + " " + Double.toString(Math.toDegrees(getDirection()[1])) + " " + Double.toString(Math.toDegrees(getDirection()[2])));
        }
    }

    private boolean IsThrow(float x, float y, float z, long curTime){
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

    private static final boolean ADAPTIVE_ACCEL_FILTER = true;
    float lastAccel[] = new float[3];
    float accelFilter[] = new float[3];

    public ThrowGesture IsTilt(float accelX, float accelY, float accelZ) {
        // high pass filter
        float updateFreq = 30; // match this to your update speed
        float cutOffFreq = 0.9f;
        float RC = 1.0f / cutOffFreq;
        float dt = 1.0f / updateFreq;
        float filterConstant = RC / (dt + RC);
        float alpha = filterConstant;
        float kAccelerometerMinStep = 0.033f;
        float kAccelerometerNoiseAttenuation = 3.0f;

        if(ADAPTIVE_ACCEL_FILTER)
        {

            float d = (float)(clamp(Math.abs(norm(accelFilter[0], accelFilter[1], accelFilter[2]) - norm(accelX, accelY, accelZ)) / kAccelerometerMinStep - 1.0f, 0.0f, 1.0f));
            alpha = d * filterConstant / kAccelerometerNoiseAttenuation + (1.0f - d) * filterConstant;
        }

        accelFilter[0] = (float) (alpha * (accelFilter[0] + accelX - lastAccel[0]));
        accelFilter[1] = (float) (alpha * (accelFilter[1] + accelY - lastAccel[1]));
        accelFilter[2] = (float) (alpha * (accelFilter[2] + accelZ - lastAccel[2]));

        float t = accelFilter[2];

        lastAccel[0] = accelX;
        lastAccel[1] = accelY;
        lastAccel[2] = accelZ;

        if(t>3.0f){
            return new ThrowGesture(BaseActivity.GetSelectedShape(), "Push");
        }

        else if(t< -3.0f){
            return new ThrowGesture(BaseActivity.GetSelectedShape(), "Pull");
        }

        return null;
    }

    double clamp(double v, double min, double max) {

        if(v > max)

            return max;

        else if(v < min)

            return min;

        else

            return v;
    }
    double norm(double x, double y, double z) {

        return Math.sqrt(x * x + y * y + z * z);

    }

    @Override
    public void onAccuracyChanged(Sensor sensor, int accuracy) {

    }

    public AccelerometerMonitor(IServer server, RotationMonitor monitor, Context context){
        super(server, context, new int[]{Sensor.TYPE_ACCELEROMETER, Sensor.TYPE_MAGNETIC_FIELD});
    }
}
