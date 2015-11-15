package com.nui.android.activities;

import android.app.Activity;
import android.content.Intent;
import android.content.pm.ActivityInfo;
import android.support.v4.view.GestureDetectorCompat;
import android.os.Bundle;
import android.view.Menu;
import android.view.MenuItem;
import android.view.ScaleGestureDetector;
import android.view.View;
import android.view.WindowManager;

import com.nui.android.AccelerometerMonitor;
import com.nui.android.Network;
import com.nui.android.PinchGestureListener;
import com.nui.android.R;
import com.nui.android.RotationMonitor;
import com.nui.android.SensorMonitor;
import com.nui.android.SwipeGestureListener;
import com.nui.android.TouchGestureListener;

/**
 * Base activity
 */
public abstract class BaseActivity extends Activity {

    protected abstract int getLayoutResourceId();

    private Network network;
    GestureDetectorCompat swipeDetector;
    ScaleGestureDetector pinchDetector;
    GestureDetectorCompat touchDetector;
    private SensorMonitor acceloremeterSensor;
    private RotationMonitor rotationSensor;

    public String shape;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(getLayoutResourceId());
        setRequestedOrientation(ActivityInfo.SCREEN_ORIENTATION_PORTRAIT);

        getWindow().getDecorView().setSystemUiVisibility(View.SYSTEM_UI_FLAG_LAYOUT_STABLE
                | View.SYSTEM_UI_FLAG_LAYOUT_HIDE_NAVIGATION
                | View.SYSTEM_UI_FLAG_LAYOUT_FULLSCREEN
                | View.SYSTEM_UI_FLAG_HIDE_NAVIGATION
                | View.SYSTEM_UI_FLAG_FULLSCREEN
                | View.SYSTEM_UI_FLAG_IMMERSIVE);
        getWindow().addFlags(WindowManager.LayoutParams.FLAG_KEEP_SCREEN_ON);

        network = new Network(this);
        SwipeGestureListener swipeGestureListener = new SwipeGestureListener(network, this);
        rotationSensor = new RotationMonitor(network, this);
        acceloremeterSensor = new AccelerometerMonitor(network, rotationSensor, this);
        swipeDetector = new GestureDetectorCompat(this, swipeGestureListener);
        pinchDetector = new ScaleGestureDetector(this, new PinchGestureListener(network, swipeGestureListener, this));
        touchDetector = new GestureDetectorCompat(this, new TouchGestureListener(network, this));
    }

    public void StartPullTest(){
        Intent intent = new Intent(this, PullTestActivity.class);
        startActivity(intent);
    }

    public void StartPushTest(){
        Intent intent = new Intent(this, PushTestActivity.class);
        startActivity(intent);
    }

    public String NextShape(){
        return "circle";
    }

    public boolean ReadyToStart(){
        return true;
    }

    public String GetSelectedShape(){
        return this.shape;
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

        menu.add(Menu.NONE, R.id.reconnect_action, Menu.NONE, R.string.reconnect_action);
        menu.add(Menu.NONE, R.id.close_app_action, Menu.NONE, R.string.close_app_action);

        return super.onCreateOptionsMenu(menu);
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        switch (item.getItemId()) {
            case R.id.reconnect_action:
                network.Reconnect();
                return true;
            case R.id.close_app_action:
                CloseApp();
                return true;
            default:
                return super.onOptionsItemSelected(item);
        }
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