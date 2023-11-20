using System;
using System.Collections.Generic;

using Client.Scripts.DataStorage;
using Client.Scripts.Game;
using Client.Scripts.GoldAndZombies.Configs;

using UnityEngine;


namespace Client.Scripts.GoldAndZombies.StartSequence
{
    public class FPSElement : ILevelSequenceElement
    {
        public event Action Finished = delegate {};

        
        public void Start(Dictionary<string, object> context)
        {
            if (!Data.Settings.IsFPS60Inited.Value)
            {
                Data.Settings.FPS60Enabled.Value = RemoteConfigWrapper.DefaultFPS;
                Data.Settings.IsFPS60Inited.Value = true;
            }
            
            Application.targetFrameRate = Data.Settings.FPS60Enabled.Value ? 60 : 30;
            
            Finished?.Invoke();
        }
    }
}