package com.nui.android.activities;

import android.app.Activity;
import android.app.Application;
import android.content.Context;
import android.content.Intent;
import android.content.pm.ActivityInfo;
import android.support.v4.content.ContextCompat;
import android.support.v4.view.GestureDetectorCompat;
import android.os.Bundle;
import android.util.Log;
import android.view.Menu;
import android.view.MenuItem;
import android.view.MotionEvent;
import android.view.ScaleGestureDetector;
import android.view.View;
import android.view.WindowManager;
import android.widget.ImageView;
import android.widget.Toast;

import com.nui.android.AccelerometerMonitor;
import com.nui.android.Network;
import com.nui.android.PinchGestureListener;
import com.nui.android.R;
import com.nui.android.RotationMonitor;
import com.nui.android.SensorMonitor;
import com.nui.android.Shape;
import com.nui.android.SwipeGestureListener;
import com.nui.android.TouchGestureListener;

import java.util.Random;

/**
 * Base activity
 */
public class BaseActivity extends Activity {

    //private Network network;
    GestureDetectorCompat swipeDetector;
    ScaleGestureDetector pinchDetector;
    GestureDetectorCompat touchDetector;
    private SensorMonitor acceloremeterSensor;
    private RotationMonitor rotationSensor;

    public static String shape;
    public static String nextShape;

    private ImageView circleView;
    private ImageView squareView;

    private ImageView circleViewPull;
    private ImageView squareViewPull;

    private final Random random = new Random();
    private int count;
    private static int MAX_COUNT = 2;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_base);
        setRequestedOrientation(ActivityInfo.SCREEN_ORIENTATION_PORTRAIT);

        getWindow().getDecorView().setSystemUiVisibility(View.SYSTEM_UI_FLAG_LAYOUT_STABLE
                | View.SYSTEM_UI_FLAG_LAYOUT_HIDE_NAVIGATION
                | View.SYSTEM_UI_FLAG_LAYOUT_FULLSCREEN
                | View.SYSTEM_UI_FLAG_HIDE_NAVIGATION
                | View.SYSTEM_UI_FLAG_FULLSCREEN
                | View.SYSTEM_UI_FLAG_IMMERSIVE);
        getWindow().addFlags(WindowManager.LayoutParams.FLAG_KEEP_SCREEN_ON);


        initNetwork();
        SwipeGestureListener swipeGestureListener = new SwipeGestureListener(Network.getInstance());
        rotationSensor = new RotationMonitor(Network.getInstance(), this);
        acceloremeterSensor = new AccelerometerMonitor(Network.getInstance(), rotationSensor, this);
        swipeDetector = new GestureDetectorCompat(this, swipeGestureListener);
        pinchDetector = new ScaleGestureDetector(this, new PinchGestureListener(Network.getInstance(), swipeGestureListener));
        touchDetector = new GestureDetectorCompat(this, new TouchGestureListener(Network.getInstance()));

        circleView = (ImageView) findViewById(R.id.circle);
        squareView = (ImageView) findViewById(R.id.square);
        circleViewPull = (ImageView) findViewById(R.id.circle_pull);
        squareViewPull = (ImageView) findViewById(R.id.square_pull);
        circleViewPull.setVisibility(View.INVISIBLE);
        squareViewPull.setVisibility(View.INVISIBLE);
        circleView.setVisibility(View.INVISIBLE);
        squareView.setVisibility(View.INVISIBLE);
        count = 0;

    }

    protected void initNetwork()
    {
        Network.initInstance(this);
    }

    public void StartPullTest(){

        circleView.setVisibility(View.INVISIBLE);
        squareView.setVisibility(View.INVISIBLE);

        circleViewPull.setOnTouchListener(new View.OnTouchListener() {
            @Override
            public boolean onTouch(View v, MotionEvent event) {
                shape = Shape.Circle;

                touchDetector.onTouchEvent(event);
                swipeDetector.onTouchEvent(event);
                pinchDetector.onTouchEvent(event);

                circleViewPull.setImageDrawable(ContextCompat.getDrawable(getApplicationContext(), R.drawable.circle_stroke));

                return true;
            }

        });

        squareViewPull.setOnTouchListener(new View.OnTouchListener() {
            @Override
            public boolean onTouch(View v, MotionEvent event) {
                shape = Shape.Square;

                touchDetector.onTouchEvent(event);
                swipeDetector.onTouchEvent(event);
                pinchDetector.onTouchEvent(event);

                squareViewPull.setImageDrawable(ContextCompat.getDrawable(getApplicationContext(), R.drawable.square_stroke));

                return true;
            }

        });
    }

    public void StartPushTest(){

        circleViewPull.setVisibility(View.INVISIBLE);
        squareViewPull.setVisibility(View.INVISIBLE);
        circleView.setVisibility(View.VISIBLE);
        squareView.setVisibility(View.VISIBLE);

        circleView.setOnTouchListener(new View.OnTouchListener() {
            @Override
            public boolean onTouch(View v, MotionEvent event) {
                shape = Shape.Circle;

                touchDetector.onTouchEvent(event);
                swipeDetector.onTouchEvent(event);
                pinchDetector.onTouchEvent(event);

                if(event.getAction() == MotionEvent.ACTION_DOWN) {
                    circleView.setImageDrawable(ContextCompat.getDrawable(getApplicationContext(), R.drawable.circle_stroke));
                    squareView.setImageDrawable(ContextCompat.getDrawable(getApplicationContext(), R.drawable.square));
                }

                return true;
            }

        });

        squareView.setOnTouchListener(new View.OnTouchListener() {
            @Override
            public boolean onTouch(View v, MotionEvent event) {
                shape = Shape.Square;

                touchDetector.onTouchEvent(event);
                swipeDetector.onTouchEvent(event);
                pinchDetector.onTouchEvent(event);

                if(event.getAction() == MotionEvent.ACTION_DOWN) {
                    squareView.setImageDrawable(ContextCompat.getDrawable(getApplicationContext(), R.drawable.square_stroke));
                    circleView.setImageDrawable(ContextCompat.getDrawable(getApplicationContext(), R.drawable.circle));
                }

                return true;
            }

        });
    }

    public void SwitchPosition() {
        squareView.setImageDrawable(ContextCompat.getDrawable(getApplicationContext(), R.drawable.square));
        circleView.setImageDrawable(ContextCompat.getDrawable(getApplicationContext(), R.drawable.circle));
        if(count > MAX_COUNT || random.nextBoolean()) {
            count = 0;
            int TopShapeTop = circleView.getTop();
            int TopShapeBottom = circleView.getBottom();
            int BottomShapeTop = squareView.getTop();
            int BottomShapeBottom = squareView.getBottom();

            circleView.setTop(BottomShapeTop);
            circleView.setBottom(BottomShapeBottom);
            squareView.setTop(TopShapeTop);
            squareView.setBottom(TopShapeBottom);
        } else {
            count++;
        }
    }

    public void SetCircleShape() {
        circleViewPull.setVisibility(View.VISIBLE);
        squareViewPull.setVisibility(View.INVISIBLE);
    }

    public void SetSquareShape() {
        circleViewPull.setVisibility(View.INVISIBLE);
        squareViewPull.setVisibility(View.VISIBLE);
    }

    public boolean ReadyToStart(){
        return true;
    }

    public static String GetSelectedShape(){
        return shape;
    }

    public void CloseApp(){
        this.finish();
        Intent intent = new Intent(Intent.ACTION_MAIN);
        intent.addCategory(Intent.CATEGORY_HOME);
        intent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
        startActivity(intent);
    }

    @Override
    public boolean onCreateOptionsMenu(Menu menu) {

        menu.add(Menu.NONE, R.id.network_discovery, Menu.NONE, R.string.network_discovery);
        menu.add(Menu.NONE, R.id.reconnect_action, Menu.NONE, R.string.reconnect_action);
        menu.add(Menu.NONE, R.id.close_app_action, Menu.NONE, R.string.close_app_action);

        return super.onCreateOptionsMenu(menu);
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        switch (item.getItemId()) {
            case R.id.network_discovery:
                Network.getInstance().FindServer(true);
                return true;
            case R.id.reconnect_action:
                Network.getInstance().Reconnect();
                return true;
            case R.id.close_app_action:
                android.os.Process.killProcess(android.os.Process.myPid());
                //CloseApp();
                return true;
            default:
                return super.onOptionsItemSelected(item);
        }
    }

    @Override
    protected void onPause(){
        super.onPause();
        acceloremeterSensor.Pause();
        Network.getInstance().Pause();
    }

    @Override
    protected void onResume(){
        super.onResume();
        Network.getInstance().Resume();
        acceloremeterSensor.Resume();
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
