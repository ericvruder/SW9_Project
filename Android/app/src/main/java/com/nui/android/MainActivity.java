package com.nui.android;

import android.graphics.drawable.Drawable;
import android.os.Bundle;
import android.support.v4.content.ContextCompat;
import android.support.v4.view.GestureDetectorCompat;
import android.util.Log;
import android.view.MotionEvent;
import android.view.ScaleGestureDetector;
import android.view.View;
import android.widget.ImageView;

import java.util.Random;

/**
 * An example full-screen activity that shows and hides the system UI (i.e.
 * status bar and navigation/system bar) with user interaction.
 */
public class MainActivity extends BaseActivity {

    private Network network;
    private SensorMonitor acceloremeterSensor;
    private RotationMonitor rotationSensor;
    private ImageView circleView;
    private ImageView squareView;
    public String shape;

    private final Random random = new Random();
    private int TopShapeTop;
    private int TopShapeBottom;
    private int BottomShapeTop;
    private int BottomShapeBottom;

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

        Drawable drawableCircle = ContextCompat.getDrawable(this, R.drawable.circle);
        Drawable drawableSquare = ContextCompat.getDrawable(this, R.drawable.square);
        circleView = (ImageView) findViewById(R.id.circle);
        squareView = (ImageView) findViewById(R.id.square);

        circleView.setImageDrawable(drawableCircle);
        squareView.setImageDrawable(drawableSquare);



        circleView.setOnTouchListener(new View.OnTouchListener() {
            @Override
            public boolean onTouch(View v, MotionEvent event) {
                shape = "Circle";
                swipeDetector.onTouchEvent(event);
                pinchDetector.onTouchEvent(event);

                if (event.getAction() == android.view.MotionEvent.ACTION_UP) {
                    if (random.nextBoolean()) {
                        TopShapeTop = circleView.getTop();
                        TopShapeBottom = circleView.getBottom();
                        BottomShapeTop = squareView.getTop();
                        BottomShapeBottom = squareView.getBottom();

                        circleView.setTop(BottomShapeTop);
                        circleView.setBottom(BottomShapeBottom);
                        squareView.setTop(TopShapeTop);
                        squareView.setBottom(TopShapeBottom);
                    }
                    Log.d("MAIN", "Circle Touch");
                }
                return true;
            }

        });

        squareView.setOnTouchListener(new View.OnTouchListener() {
            @Override
            public boolean onTouch(View v, MotionEvent event) {
                shape = "Square";
                swipeDetector.onTouchEvent(event);
                pinchDetector.onTouchEvent(event);

                if (event.getAction() == android.view.MotionEvent.ACTION_UP) {
                    if (random.nextBoolean()) {
                        TopShapeTop = circleView.getTop();
                        TopShapeBottom = circleView.getBottom();
                        BottomShapeTop = squareView.getTop();
                        BottomShapeBottom = squareView.getBottom();

                        circleView.setTop(BottomShapeTop);
                        circleView.setBottom(BottomShapeBottom);
                        squareView.setTop(TopShapeTop);
                        squareView.setBottom(TopShapeBottom);
                    }
                    Log.d("MAIN", "Square Touch");
                }
                return true;
            }

        });

    }

    public String GetSelectedShape(){
        return this.shape;
    }

    /*
    @Override
    public boolean onTouchEvent(MotionEvent event){
        this.swipeDetector.onTouchEvent(event);
        this.pinchDetector.onTouchEvent(event);
        return super.onTouchEvent(event);
    }
    */


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


