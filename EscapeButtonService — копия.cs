using UnityEngine;

namespace Game.Sections.Common
{
    public class EscapeButtonService : MonoBehaviour
    {
        private IEscapeProcessStrategy _processStrategy;
        private bool _canProcess;

        public void SetStrategy(IEscapeProcessStrategy processStrategy)
        {
            _processStrategy = processStrategy;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) && _canProcess)
            {
                _processStrategy?.ProcessEscape();
            }
            
        }
    }
    
    public interface IEscapeProcessStrategy
    {
        void ProcessEscape();
    }
}