package com.rubon.chemistryar.app_level.dagger

import com.rubon.chemistryar.screen.unity.MainActivity
import com.rubon.chemistryar.app_level.dagger.module.BindModule
import dagger.Component
import javax.inject.Singleton

@Singleton
@Component(modules = [BindModule::class])
interface AppComponent {
    fun inject(activity: MainActivity)
}