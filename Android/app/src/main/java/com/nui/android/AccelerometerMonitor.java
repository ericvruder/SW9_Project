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
            if(IsThrown(x,y,z)){
                ThrowGesture data = new ThrowGesture(BaseActivity.GetSelectedShape());
                server.SendData(data);
            }
        }
    }

    private static final boolean ADAPTIVE_ACCEL_FILTER = true;
    float lastAccel[] = new float[3];
    float accelFilter[] = new float[3];

    public boolean IsThrown(float accelX, float accelY, float accelZ) {
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
            Log.d("ACCELLEROMETERMONITOR", "THROW");
            return true;
        }

        return false;
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
    private RotationMonitor rMonitor;
    public AccelerometerMonitor(IServer server, RotationMonitor monitor, Context context){
        super(server,context, Sensor.TYPE_ACCELEROMETER);
        rMonitor = monitor;
    }
}
