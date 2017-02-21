
namespace Spike.Common
{
    public static class SampleWorker
    {
        public static bool IsSample(int position)
        {
            var sample = (position > 100) ? (position > 1000 ? 1000 : 100) : 10;

            if (position % sample != 0)
            {
                return false;
            }

            return true;
        }
    }
}
