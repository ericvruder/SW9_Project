package com.nui.android.activities;

import android.os.Bundle;
import android.view.View;

import com.nui.android.R;

/**
 * An example full-screen activity that shows and hides the system UI (i.e.
 * status bar and navigation/system bar) with user interaction.
 */
public class MainActivity extends BaseActivity {

    @Override
    protected int getLayoutResourceId() {
        return R.layout.activity_main;
    }

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        /**
         * Dummy/simulation/test buttons
         */
        findViewById(R.id.start_pull_test).setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                StartPullTest();
            }
        });

        findViewById(R.id.start_push_test).setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                StartPushTest();
            }
        });

    }

}


