using System;

namespace Dream.Preprocessor.Properties 
{
    
  /// <summary>
  /// Maintain user settings and provides access to application settings.
  /// </summary>
  internal sealed partial class Settings 
  {
      
    public Settings() 
    {
      // // To add event handlers for saving and changing settings, uncomment the lines below:
      //this.SettingChanging += this.SettingChangingEventHandler;
      //this.SettingsSaving += this.SettingsSavingEventHandler;
      //this.PropertyChanged += this.PropertyChangedEventHandler;
      //this.SettingsLoaded += this.SettingsLoadedEventHandler;
    }

    //private void SettingChangingEventHandler(object sender, System.Configuration.SettingChangingEventArgs e) {
    //  Console.WriteLine("-> Entered Settings.SettingChangingEventHandler.");
    //  Console.WriteLine("e.SettingClass: {0}", e.SettingClass);
    //  Console.WriteLine("e.SettingKey: {0}", e.SettingKey);
    //  Console.WriteLine("e.NewValue: {0}", e.NewValue);
    //}
    
    //private void SettingsSavingEventHandler(object sender, System.ComponentModel.CancelEventArgs e) {
    //  Console.WriteLine("-> Entered Settings.SettingSavingEventHandler.");        
    //}

    //private void SettingsLoadedEventHandler(object sender, System.Configuration.SettingsLoadedEventArgs e)
    //{
    //  Console.WriteLine("-> Entered Settings.SettingsLoadedEventHandler.");
    //}

    //private void PropertyChangedEventHandler(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    //{
    //  Console.WriteLine("->Entered Settings.PropertyChangedEventHandler.");
    //  Console.WriteLine("e.PropertyName: {0}", e.PropertyName);
    //}

  }

}
