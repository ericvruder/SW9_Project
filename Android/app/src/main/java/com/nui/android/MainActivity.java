package com.nui.android;

import android.content.Intent;
import android.os.Bundle;
import android.view.MotionEvent;
import android.view.View;
import android.widget.Button;

/**
 * An example full-screen activity that shows and hides the system UI (i.e.
 * status bar and navigation/system bar) with user interaction.
 */
public class MainActivity extends BaseActivity {

    private Button sendMessageButton;
    private Network network;
    private SensorMonitor sensorMonitor;

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
                Intent i = new Intent(getApplicationContext(),PointAndThrowActivity.class);
                startActivity(i);
            }
        });

        sendMessageButton.setOnTouchListener(new View.OnTouchListener() {
            @Override
            public boolean onTouch(View view, MotionEvent motionEvent) {
                network.SendMessage("Testing");
                return true;
            }
        });

        network = new Network(false);
        sensorMonitor = new AccelerometerMonitor(network, this);
        sensorMonitor = new RotationMonitor(network,this);
    }

    @Override
    protected void onPause(){
        sensorMonitor.Pause();
        network.Pause();
        super.onPause();
    }

    @Override
    protected void onResume(){
        network.Resume();
        sensorMonitor.Resume();
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


