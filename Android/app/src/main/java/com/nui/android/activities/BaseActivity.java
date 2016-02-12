package com.nui.android.activities;

import android.app.Activity;
import android.app.Application;
import android.content.Context;
import android.content.Intent;
import android.content.pm.ActivityInfo;
import android.hardware.Sensor;
import android.hardware.SensorEvent;
import android.hardware.SensorEventListener;
import android.hardware.SensorManager;
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

import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.net.InetAddress;
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


        sm = (SensorManager) getSystemService(SENSOR_SERVICE);
        // TODO provide support for gyroscope (rotation vector is flawed in early
        // versions of android)
        rv = sm.getDefaultSensor(Sensor.TYPE_GAME_ROTATION_VECTOR);

        // network thread
        nt = new Thread(new Runnable() {
            @Override
            public void run() {
                long lt = 0;
                // setup socket
                try {
                    // TODO select host

                    // TODO support bluetooth TCP socket

                    HOST = InetAddress.getByName("192.168.1.4");
                    //HOST = InetAddress.getByName("10.208.105.215");
                    ds = new DatagramSocket();
                    // InetAddress ia = InetAddress.getByName("192.168.1.255");
                    // ds.setBroadcast(true);
                    ds.connect(HOST, PORT);
                    Log.d("BaseActivity",
                            "Socket is bound to "
                                    + String.valueOf(ds.getLocalPort()));
                } catch (Exception e) {
                    e.printStackTrace();
                    Log.e("BaseActivity", "Failed to make a socket.");
                }
                while (true) {
                    if (end_nt) {
                        Log.d("BaseActivity", "Network thread ends.");
                        break;
                    }

                    long ct = rv_sel.getLatestTimestamp();
                    if (ct > lt) {
                        try {
                            ds.send(dp);
                            lt = ct;
                        } catch (Exception e) {
                            e.printStackTrace();
                        }
                    }
                }
            }
        }, "UdpThread");

        // nt.setPriority(Thread.MAX_PRIORITY);
        nt.start();

        // TODO rewrite the sensor acquisition with NDK
        rv_sel = new RotationVectorListener();

    }

    private boolean pushOrPull;

    public boolean PushOrPull(){
        //True = push, false = pull
        return pushOrPull;
    }

    protected void initNetwork()
    {
        Network.initInstance(this);
    }

    public void StartPullTest(){
        pushOrPull = false;
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

        pushOrPull = true;
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

    public void ClearShapes(){
        shape = null;
        squareView.setImageDrawable(ContextCompat.getDrawable(getApplicationContext(), R.drawable.square));
        circleView.setImageDrawable(ContextCompat.getDrawable(getApplicationContext(), R.drawable.circle));
    }

    public void SwitchPosition() {
        ClearShapes();
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
                //android.os.Process.killProcess(android.os.Process.myPid());
                CloseApp();
                return true;
            default:
                return super.onOptionsItemSelected(item);
        }
    }

    @Override
    protected void onPause(){
        super.onPause();
        acceloremeterSensor.Pause();
        sm.unregisterListener(rv_sel);
        Network.getInstance().Pause();
    }

    @Override
    protected void onResume(){
        super.onResume();
        Network.getInstance().Resume();
        acceloremeterSensor.Resume();
        sm.registerListener(rv_sel, rv, SensorManager.SENSOR_DELAY_FASTEST);
    }

    @Override
    public void onDestroy() {
        super.onDestroy();
        if (nt.isAlive()) {
            end_nt = true;
        }
    }

    @Override
    protected void onStop(){
        super.onStop();
    }

    @Override
    public void onBackPressed() {
        super.onBackPressed();
    }




    private InetAddress HOST;
    private final int PORT = 49255;

    private SensorManager sm;
    private Sensor rv;
    private RotationVectorListener rv_sel;

    private DatagramSocket ds;
    private byte[] msg = new byte[100];
    private DatagramPacket dp = new DatagramPacket(msg, msg.length);

    private Thread nt;
    private boolean end_nt;

    class RotationVectorListener implements SensorEventListener {
        private long time = 0;
        private long mt = 0;
        public boolean calibrated = false;
        private float calibrateZ = 0;
        private float calibrateX = 0;
        private float calibrateY = 0;

        private float virtualX = 0;
        private float virtualY = 0;
        private float virtualZ = 0;
//		private String info_text;

        public long getLatestTimestamp() {
            return time;
        }

        protected void packageSensorEvent(float time, float x, float y, float z, DatagramPacket packet) {
            byte[] buf = packet.getData();
            writeByteBuffer(buf, 0, time);
            writeByteBuffer(buf, 8, x);
            writeByteBuffer(buf, 12, y);
            writeByteBuffer(buf, 16, z);
        }

        protected void writeByteBuffer(byte[] buf, int offset, float f) {
            if (offset + 4 > buf.length) {
                // the buffer is not big enough for the data
                // TODO throws an exception
                Log.w("buffer", "Not good");
                return;
            }

            int n = Float.floatToRawIntBits(f);
            for (int i = 0; i < 4; i++) {
                buf[offset + i] = (byte) ((n >>> i * 8) & 0xff);
            }
        }

        @Override
        public void onSensorChanged(SensorEvent event) {
            if(time == 0)
                time = event.timestamp;
            float x = event.values[0];
            float y = event.values[1];
            float z = event.values[2];

            if(!calibrated){
                calibrateZ = z;
                calibrateX = x;
                calibrateY = y;
                calibrated = true;
            }

            virtualX = x-calibrateX;
            virtualY = y-calibrateY;
            virtualZ = z-calibrateZ;

            /*virtualXDeg = Math.toDegrees(x-calibrateX);
            virtualYDeg = Math.toDegrees(y-calibrateY);
            virtualZDeg = Math.toDegrees(z-calibrateZ);*/

            Log.d("Gyro: ", "X: " + virtualX + " Y: " + virtualY + " Z: " + virtualZ);
            byte[] buf = ("gyrodata:time:"+ event.timestamp +":x:"+virtualX+":y:"+virtualY+":z:"+virtualZ).getBytes();
            //packageSensorEvent(event.timestamp, virtualX, virtualY, virtualZ, dp);
            dp.setData(buf);
            time = event.timestamp;
        }

        @Override
        public void onAccuracyChanged(Sensor sensor, int accuracy) {
            // TODO Auto-generated method stub

        }
    }

}
