package com.rubon.chemistryar;
import android.os.Bundle;
import android.widget.FrameLayout;

import com.unity3d.player.UnityPlayerActivity;

public abstract class OverrideUnityActivity extends UnityPlayerActivity
{
    public static OverrideUnityActivity instance = null;

    /**
    * description - "H-1,O-2,H-3,H-4;O-5,O-6,O-7,O-8,S-9;H-10,Cl-11"
    * result - "H-1,O-2,H-3;O-5,O-6,O-7,S-9;H-10,Cl-11"
    */
    abstract protected String calculateMolecules(String description);

    @Override
    protected void onCreate(Bundle savedInstanceState)
    {
        super.onCreate(savedInstanceState);
        instance = this;
    }

    @Override
    protected void onDestroy() {
        super.onDestroy();
        instance = null;
    }
}
