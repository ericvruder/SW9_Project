package com.nui.android.activities;

import android.os.Bundle;
import android.support.v4.content.ContextCompat;
import android.view.MotionEvent;
import android.view.View;
import android.widget.ImageView;

import com.nui.android.Network;
import com.nui.android.R;
import com.nui.android.Shape;

import java.util.Random;

/**
 * An example full-screen activity that shows and hides the system UI (i.e.
 * status bar and navigation/system bar) with user interaction.
 */
public class PullTestActivity extends BaseActivity {

    private ImageView circleView;
    private ImageView squareView;

    static boolean active = false;

    private final Random random = new Random();
    private int count;
    private static int MAX_COUNT = 2;

    @Override
    protected int getLayoutResourceId() {
        return R.layout.activity_pull_test;
    }

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        circleView = (ImageView) findViewById(R.id.circle);
        squareView = (ImageView) findViewById(R.id.square);
        count = 0;
        Network.SetActivity(this);

        // If the starting shape should NOT be randomized, remove following if-else
        /*
        if(random.nextBoolean()) {
            circleView.setVisibility(View.INVISIBLE);
            squareView.setVisibility(View.VISIBLE);
            nextShape = Shape.Square;
        } else {
            circleView.setVisibility(View.VISIBLE);
            squareView.setVisibility(View.INVISIBLE);
            nextShape = Shape.Circle;
        }
        */


        circleView.setOnTouchListener(new View.OnTouchListener() {
            @Override
            public boolean onTouch(View v, MotionEvent event) {
                shape = Shape.Circle;

                touchDetector.onTouchEvent(event);
                swipeDetector.onTouchEvent(event);
                pinchDetector.onTouchEvent(event);

                circleView.setImageDrawable(ContextCompat.getDrawable(getApplicationContext(), R.drawable.circle_stroke));

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

                squareView.setImageDrawable(ContextCompat.getDrawable(getApplicationContext(), R.drawable.square_stroke));

                return true;
            }

        });


    }

    public void SetCircleShape() {
        circleView.setVisibility(View.VISIBLE);
        squareView.setVisibility(View.INVISIBLE);
    }

    public void SetSquareShape() {
        circleView.setVisibility(View.INVISIBLE);
        squareView.setVisibility(View.VISIBLE);
    }

    public void SwitchShape() {
        squareView.setImageDrawable(ContextCompat.getDrawable(getApplicationContext(), R.drawable.square));
        circleView.setImageDrawable(ContextCompat.getDrawable(getApplicationContext(), R.drawable.circle));
        if(count > MAX_COUNT || random.nextBoolean()) {
            count = 0;
            if(squareView.getVisibility() == View.INVISIBLE) {
                squareView.setVisibility(View.VISIBLE);
                circleView.setVisibility(View.INVISIBLE);
                nextShape = Shape.Square;
            } else if(circleView.getVisibility() == View.INVISIBLE) {
                squareView.setVisibility(View.INVISIBLE);
                circleView.setVisibility(View.VISIBLE);
                nextShape = Shape.Circle;
            }
        } else {
            count++;
        }
    }

    @Override
    public void onStart() {
        super.onStart();
        active = true;
    }

    @Override
    public void onStop() {
        super.onStop();
        active = false;
    }
}


