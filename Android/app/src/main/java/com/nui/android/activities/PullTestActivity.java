package com.nui.android.activities;

import android.os.Bundle;
import android.view.MotionEvent;
import android.view.View;
import android.widget.ImageView;

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

        // If the starting shape should NOT be randomized, remove following if-else
        if(random.nextBoolean()) {
            circleView.setVisibility(View.INVISIBLE);
            squareView.setVisibility(View.VISIBLE);
            nextShape = Shape.Square;
        } else {
            circleView.setVisibility(View.VISIBLE);
            squareView.setVisibility(View.INVISIBLE);
            nextShape = Shape.Circle;
        }

        NextShape();

        circleView.setOnTouchListener(new View.OnTouchListener() {
            @Override
            public boolean onTouch(View v, MotionEvent event) {
                shape = Shape.Circle;

                touchDetector.onTouchEvent(event);
                swipeDetector.onTouchEvent(event);
                pinchDetector.onTouchEvent(event);

                if (event.getAction() == MotionEvent.ACTION_UP) {
                    if(count > MAX_COUNT || random.nextBoolean()) {
                        count = 0;
                        circleView.setVisibility(View.INVISIBLE);
                        squareView.setVisibility(View.VISIBLE);
                        nextShape = Shape.Square;
                    } else {
                        count++;
                        nextShape = Shape.Circle;
                    }
                    NextShape();
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

                if (event.getAction() == MotionEvent.ACTION_UP) {
                    if(count > MAX_COUNT || random.nextBoolean()) {
                        count = 0;
                        squareView.setVisibility(View.INVISIBLE);
                        circleView.setVisibility(View.VISIBLE);
                        nextShape = Shape.Circle;
                    } else {
                        count++;
                        nextShape = Shape.Square;
                    }
                    NextShape();
                }
                return true;
            }

        });

    }

}


