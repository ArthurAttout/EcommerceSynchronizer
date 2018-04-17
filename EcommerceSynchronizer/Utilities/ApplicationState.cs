namespace EcommerceSynchronizer.Utilities
{
    public class ApplicationState
    {
        private bool _isSynchronizerRunning;

        public bool IsSynchronizerRunning
        {
            get => _isSynchronizerRunning;
            set => _isSynchronizerRunning = value;
        }

        public string SynchronizerJobID { get; set; }
    }
}