using System;
using System.Collections;
using System.Collections.Generic;
using IPA;
using IPA.Config.Stores;
using IPALogger = IPA.Logging.Logger;

namespace AccStandardDeviationCounter
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        internal static Plugin Instance { get; private set; }
        internal static string Name => "AccStandardDeviationCounter";
        
        [Init]
        public void Init(IPALogger logger,IPA.Config.Config config)
        {
            Configuration.Instance = config.Generated<Configuration>();
            Instance = this;
            Logger.log = logger;
            Logger.log.Debug("Logger initialized.");
        }

        #region BSIPA Config
        //Uncomment to use BSIPA's config
        /*
        [Init]
        public void InitWithConfig(Config conf)
        {
            Configuration.PluginConfig.Instance = conf.Generated<Configuration.PluginConfig>();
            Log.Debug("Config loaded");
        }
        */
        #endregion

        [OnEnable]
        public void OnEnable(){}

        [OnDisable]
        public void OnDisable(){}
    }
}
