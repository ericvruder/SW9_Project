package com.nui.android;

import android.graphics.Color;
import android.hardware.Sensor;
import android.hardware.SensorEvent;
import android.hardware.SensorEventListener;
import android.hardware.SensorManager;
import android.os.Bundle;
import android.view.View;
import android.widget.TextView;
import android.widget.Toast;

/**
 * Created by bml on 23-10-2015.
 */
public class PointAndThrowActivity extends BaseActivity implements SensorEventListener {

    private SensorManager mSensorManager;
    private TextView mXViewA, mYViewA, mZViewA, mXViewO, mYViewO, mZViewO;

    private long lastUpdate;
    private boolean color = false;
    private View testView;

    @Override
    protected int getLayoutResourceId() {
        return R.layout.activity_point_and_throw;
    }

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        testView = findViewById(R.id.textView);
        testView.setBackgroundColor(Color.GREEN);

        mSensorManager = (SensorManager) getSystemService(SENSOR_SERVICE);
        mXViewA = (TextView) findViewById(R.id.xViewA);
        mYViewA = (TextView) findViewById(R.id.yViewA);
        mZViewA = (TextView) findViewById(R.id.zViewA);
        mXViewO = (TextView) findViewById(R.id.xViewO);
        mYViewO = (TextView) findViewById(R.id.yViewO);
        mZViewO = (TextView) findViewById(R.id.zViewO);
    }

    @Override
    public void onSensorChanged(SensorEvent event) {
        if (event.sensor.getType() == Sensor.TYPE_ACCELEROMETER) {
            getAccelerometer(event);
        }
        if (event.sensor.getType() == Sensor.TYPE_MAGNETIC_FIELD) {
            getOrientation(event);
        }
    }

    /**
     *
     * @param event
     */
    private void getOrientation(SensorEvent event) {
        float[] values = event.values;

        float x = values[0];
        float y = values[1];
        float z = values[2];
        mXViewO.setText(Float.toString(x));
        mYViewO.setText(Float.toString(y));
        mZViewO.setText(Float.toString(z));
    }

    private void getAccelerometer(SensorEvent event) {
        float[] values = event.values;
        // Movement
        float x = values[0];
        float y = values[1];
        float z = values[2];
        mXViewA.setText(Float.toString(x));
        mYViewA.setText(Float.toString(y));
        mZViewA.setText(Float.toString(z));

        // Some color text shit
        float accelationSquareRoot = (x * x + y * y + z * z)
                / (SensorManager.GRAVITY_EARTH * SensorManager.GRAVITY_EARTH);
        long actualTime = event.timestamp;
        if (accelationSquareRoot >= 2) //
        {
            if (actualTime - lastUpdate < 4E9) {
                return;
            }
            lastUpdate = actualTime;
            Toast.makeText(this, "Device was shuffed", Toast.LENGTH_SHORT)
                    .show();
            if (color) {
                testView.setBackgroundColor(Color.GREEN);
            } else {
                testView.setBackgroundColor(Color.RED);
            }
            color = !color;
        }
    }

    @Override
    public void onAccuracyChanged(Sensor sensor, int accuracy) {

    }

    @Override
    protected void onResume() {
        super.onResume();
        mSensorManager.registerListener(this, mSensorManager.getDefaultSensor(Sensor.TYPE_ACCELEROMETER), mSensorManager.SENSOR_DELAY_NORMAL);
        mSensorManager.registerListener(this, mSensorManager.getDefaultSensor(Sensor.TYPE_MAGNETIC_FIELD), mSensorManager.SENSOR_DELAY_NORMAL);
    }

    @Override
    protected void onPause() {
        super.onPause();
        mSensorManager.unregisterListener(this);
    }
}
