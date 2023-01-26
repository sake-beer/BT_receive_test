
namespace BT_test.Model
{
    public class TopModel
    {
        // Property
        public TopLog Log { get { return _log; } }

        // Member
        private TopLog _log;
        private BTControl _bt;
        private SerialControl _serial;

        public TopModel()
        {
            _log = new TopLog();
            _bt = new BTControl(_log);
            _bt.Init();
            _serial = new SerialControl(_log, _bt.Com);
            _serial.Start();

        }


    }
}
