package com.nui.android;

import android.os.Bundle;
import android.support.v4.view.GestureDetectorCompat;
import android.view.MotionEvent;
import android.view.ScaleGestureDetector;
import android.view.View;
import android.widget.Button;

/**
 * An example full-screen activity that shows and hides the system UI (i.e.
 * status bar and navigation/system bar) with user interaction.
 */
public class MainActivity extends BaseActivity {

    private Button sendMessageButton;
    private Network network;
    private SensorMonitor acceloremeterSensor;
    private RotationMonitor rotationSensor;
    GestureDetectorCompat swipeDetector;
    ScaleGestureDetector pinchDetector;

    @Override
    protected int getLayoutResourceId() {
        return R.layout.activity_main;
    }

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        sendMessageButton = (Button) findViewById(R.id.send_message);

        findViewById(R.id.activity_tilt_button).setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                finishActivity(0);
            }
        });

        findViewById(R.id.reconnect_button).setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                network.Reconnect();
            }
        });

        sendMessageButton.setOnTouchListener(new View.OnTouchListener() {
            @Override
            public boolean onTouch(View view, MotionEvent motionEvent) {
                return true;
            }
        });

        network = new Network();
        SwipeGestureListener swipeGestureListener = new SwipeGestureListener(network);
        rotationSensor = new RotationMonitor(network, this);
        acceloremeterSensor = new AccelerometerMonitor(network, rotationSensor, this);
        swipeDetector = new GestureDetectorCompat(this, swipeGestureListener);
        pinchDetector = new ScaleGestureDetector(this, new PinchGestureListener(network, swipeGestureListener));
    }
    @Override
    public boolean onTouchEvent(MotionEvent event){
        this.swipeDetector.onTouchEvent(event);
        this.pinchDetector.onTouchEvent(event);
        return super.onTouchEvent(event);
    }


    @Override
    protected void onPause(){
        acceloremeterSensor.Pause();
        network.Pause();
        super.onPause();
    }

    @Override
    protected void onResume(){
        network.Resume();
        acceloremeterSensor.Resume();
        super.onResume();
    }

    @Override
    protected void onStop(){
        super.onStop();
    }

    @Override
    public void onBackPressed() {
        super.onBackPressed();
    }

}


