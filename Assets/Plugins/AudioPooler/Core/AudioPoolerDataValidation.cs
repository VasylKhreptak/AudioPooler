namespace Plugins.AudioPooler.Core
{
    public partial class AudioPooler
    {
        private void ValidateInputData()
        {
            ValidateInitialSize();

            ValidateMaxSize();
        }

        private void ValidateInitialSize()
        {
            if (_initialSize < 1)
            {
                _initialSize = 1;
            }
        }

        private void ValidateMaxSize()
        {
            if (_maxSize < _initialSize)
            {
                _maxSize = _initialSize;
            }
        }
    }
}
