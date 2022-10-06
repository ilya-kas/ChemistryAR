package com.rubon.chemistryar.screen.unity

import android.os.Bundle
import android.util.Log
import com.rubon.chemistryar.OverrideUnityActivity
import com.rubon.chemistryar.app_level.App
import javax.inject.Inject

class MainActivity : OverrideUnityActivity() {

    @Inject
    lateinit var viewModel: MainViewModel

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        App.appComponent.inject(this)
    }

    override fun onDestroy() {
        mUnityPlayer!!.quit()
        super.onDestroy()
    }

    override fun calculateMolecules(description: String): String {
        Log.d("Unity app", description)
        return viewModel.getMolecules(description)
    }


    override fun onPause() {
        super.onPause()
        mUnityPlayer!!.pause()
    }


    override fun onResume() {
        super.onResume()
        mUnityPlayer!!.resume()
    }


    override fun onWindowFocusChanged(hasFocus: Boolean) {
        super.onWindowFocusChanged(hasFocus)
        mUnityPlayer!!.windowFocusChanged(hasFocus)
    }
}