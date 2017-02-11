
namespace WcfService
{
    using System.ServiceModel;

    [ServiceContract]
    public interface ITestService
    {
        [OperationContract]
        string TestCall(int value, bool simulateExceptions = false);

        [OperationContract]
        void ResetCache();
    }
}
