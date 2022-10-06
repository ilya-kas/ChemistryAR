package com.rubon.chemistryar.app_level

import android.app.Application
import com.rubon.chemistryar.app_level.dagger.AppComponent
import com.rubon.chemistryar.app_level.dagger.DaggerAppComponent

class App: Application() {
    override fun onCreate() {
        super.onCreate()
        appComponent = DaggerAppComponent.builder()
                .build()
    }

    companion object{
        lateinit var appComponent: AppComponent
    }
}