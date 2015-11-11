package com.nui.android;

import android.graphics.drawable.Drawable;
import android.os.Bundle;
import android.support.v4.view.GestureDetectorCompat;
import android.util.Log;
import android.view.MotionEvent;
import android.view.ScaleGestureDetector;
import android.view.View;
import android.widget.Button;
import android.widget.ImageView;
import android.widget.TextView;

/**
 * An example full-screen activity that shows and hides the system UI (i.e.
 * status bar and navigation/system bar) with user interaction.
 */
public class MainActivity extends BaseActivity {

    private Network network;
    private SensorMonitor acceloremeterSensor;
    private RotationMonitor rotationSensor;
    private ImageView circle;
    private ImageView square;
    private ImageView triangle;
    private ImageView pentagon;
    private TextView fullscreen_content;
    public String shape;
    GestureDetectorCompat swipeDetector;
    ScaleGestureDetector pinchDetector;

    @Override
    protected int getLayoutResourceId() {
        return R.layout.activity_main;
    }

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

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

        findViewById(R.id.send_message).setOnTouchListener(new View.OnTouchListener() {
            @Override
            public boolean onTouch(View view, MotionEvent motionEvent) {
                return true;
            }
        });

        network = new Network();
        SwipeGestureListener swipeGestureListener = new SwipeGestureListener(network, this);
        rotationSensor = new RotationMonitor(network, this);
        acceloremeterSensor = new AccelerometerMonitor(network, rotationSensor, this);
        swipeDetector = new GestureDetectorCompat(this, swipeGestureListener);
        pinchDetector = new ScaleGestureDetector(this, new PinchGestureListener(network, swipeGestureListener, this));

        circle = (ImageView) findViewById(R.id.circle);
        square = (ImageView) findViewById(R.id.square);
        triangle = (ImageView) findViewById(R.id.triangle);
        pentagon = (ImageView) findViewById(R.id.pentagon);
        fullscreen_content = (TextView) findViewById(R.id.fullscreen_content);


        fullscreen_content.setOnTouchListener(new View.OnTouchListener() {
            @Override
            public boolean onTouch(View v, MotionEvent event) {
                shape = null;
                return false;
            }

        });


        circle.setOnTouchListener(new View.OnTouchListener() {
            @Override
            public boolean onTouch(View v, MotionEvent event) {
                shape = "Circle";
                return false;
            }

        });

        square.setOnTouchListener(new View.OnTouchListener() {
            @Override
            public boolean onTouch(View v, MotionEvent event) {
                shape = "Square";
                return false;
            }

        });

        triangle.setOnTouchListener(new View.OnTouchListener() {
            @Override
            public boolean onTouch(View v, MotionEvent event) {
                shape = "Triangle";
                return false;
            }

        });

        pentagon.setOnTouchListener(new View.OnTouchListener() {
            @Override
            public boolean onTouch(View v, MotionEvent event) {
                shape = "Pentagon";
                return false;
            }

        });

    }


    public String GetSelectedShape(){
        return this.shape;
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


