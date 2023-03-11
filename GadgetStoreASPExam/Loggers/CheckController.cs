namespace GadgetStoreASPExam.Loggers
{
    public class CheckController
    {
        private readonly ILogger<CheckController> logger;
        public CheckController(ILogger<CheckController> logger)
        {
            this.logger = logger;
        }
    }
}
