using System.Collections.Generic;


namespace Client.Scripts.Game
{
    public abstract class GameSequence
    {
        protected readonly Dictionary<string, object> Context = new Dictionary<string, object>();
        protected readonly List<ILevelSequenceElement> Elements = new List<ILevelSequenceElement>();
        private int _currentElement;

        public void Start()
        {
            _currentElement = -1;
            Next();
        }

        private void Next()
        {
            _currentElement++;

            if (_currentElement < Elements.Count)
            {
                Elements[_currentElement].Finished += OnElementFinished;
                Elements[_currentElement].Start(Context);
            }
            else
            {
                Finish();
            }
        }

        protected virtual void Finish()
        {
            Context.Clear();
            Elements.Clear();
        }

        private void OnElementFinished()
        {
            Elements[_currentElement].Finished -= OnElementFinished;
            Next();
        }
    }
}