namespace DependencyInjectionWorkshop.Adapter
{
    public class NLogAdapter
    {
        public void Log(string message)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info(message);
        }
    }
}