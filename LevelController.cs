
using Client.Scripts.GoldAndZombies.Services;
using Client.Scripts.GoldAndZombies.Services.GlobalServices.DebugLog;
using Client.Scripts.Tools.DebugTools;
using Client.Scripts.Tools.Pool;
using Client.Scripts.Tools.Services;
using Client.Scripts.Tools.Services.GlobalEvents;
using Client.Scripts.Ui;

using UnityEngine;


namespace Client.Scripts.Game
{
    public class ApplicationPauseChangedEvent : GlobalEvent
    {
        public readonly bool Paused;

        public ApplicationPauseChangedEvent(bool paused)
        {
            Paused = paused;
        }
    }
    
    public abstract class LevelController : MonoBehaviour
    {
        private Service _service;
        
        private void Start()
        {
            GlobalService.Debug.Log("LevelController.Start", DebugLogType.ApplicationFlow);
            
            InitServices();
            InitData();
            InitUiMapping();

            InitGameSequence sequence = GetInitGameSequence();
            sequence.BattleReady += OnBattleReady;
            sequence.Start();
        }
        
        private void OnDestroy()
        {
            _service?.DeInit();
            DeInitData();
            
            GlobalService.Debug.Log("LevelController.OnDestroy", DebugLogType.ApplicationFlow);
        }

        private void OnBattleReady(IBattleController battleController)
        {
            battleController.BattleWin += OnWin;
            battleController.BattleLose += OnLose;
            
            battleController.Start();
        }

        private void OnWin()
        {
            GameSequence sequence = GetWinSequence();
            sequence.Start();
        }

        private void OnLose()
        {
            GameSequence sequence = GetLoseSequence();
            sequence.Start();
        }

        protected abstract InitGameSequence GetInitGameSequence();
        
        protected abstract GameSequence GetWinSequence();
        
        protected abstract GameSequence GetLoseSequence();

        protected virtual void InitData()
        {
            
        }

        protected virtual void DeInitData()
        {
            
        }

        protected virtual void InitServices()
        {
            _service = Service.Create();
            
            Service.Register(new Pool());
            Service.Register(new TouchService());
            Service.Register(new TimestampService());
            Service.Register(new BafferService());
            Service.Register(new GlobalEventDispatcher());
            Service.Register(new TimeScale());
            Service.Register(new UiManager());
            Service.Register(new WarriorStunService());
            
#if GPIE_DEVELOPMENT
            Service.Register(new DebugService());
#endif
        }
        
        protected virtual void InitUiMapping()
        {
            
        }

        protected virtual void OnApplicationPause(bool pauseStatus)
        {
            GlobalService.Debug.Log($"OnApplicationPause: {pauseStatus}", DebugLogType.ApplicationFlow);
            if (Service.GlobalEvent == null)
            {
                return;
            }
            
            Service.GlobalEvent.Dispatch(new ApplicationPauseChangedEvent(pauseStatus));
        }
    }
}