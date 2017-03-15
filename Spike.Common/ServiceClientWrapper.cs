
/*  // Create a new service client [Configured in Config] ()
 *  ServiceClientWrapper<[ServiceClient], [Service]>([Binding], [Endpoint]);
 *  Tip: Not sure which types to use? Check the inheritence of the generated client.
 *  
 *  e.g. var consumer = new ServiceClientWrapper<AuthorServiceClient, AuthorService>();                    // Configured In Config
 *  e.g. var consumer = new ServiceClientWrapper<AuthorServiceClient, AuthorService>(binding, endpoint);   // Configured In Code
 *    
 *  // Use the client wrapper to excecute the client operations
 *  author = consumer.Excecute(service => service.AddAuthor(request));
 */

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;

namespace Spike.Common
{
    public class ServiceClientWrapper<TClient, TIService> : IDisposable
          where TClient : ClientBase<TIService>, TIService
          where TIService : class
    {
        private TClient _serviceClient;
        private Binding _binding;
        private EndpointAddress _endpoint;
        private const int RetryCoolDownInSeconds = 1;

        public ServiceClientWrapper() { }
        public ServiceClientWrapper(Binding binding, EndpointAddress endpointAddress)
        {
            _binding = binding;
            _endpoint = endpointAddress;
        }

        public TClient ServiceClient
        {
            get
            {
                return this._serviceClient = this._serviceClient ?? this.CreateClient();
            }
        }

        public void Excecute(
            Action<TIService> serviceCall,
            int retryAttempts = 1,
            Action<CommunicationException> exceptionHandler = null)
        {
            Excecute<object>(
                service => { serviceCall.Invoke(service); return null; },
                retryAttempts,
                exceptionHandler);
        }

        private void OnException(string errorMessage, string originalMethodName, ref int errors, ref int retryAttempts)
        {
            errors++;
            var logErrorMessage = $"WCF Operation Failure: Service [{typeof(TClient)}].[{originalMethodName}] Attempt ({errors}/{retryAttempts}). Exception [{errorMessage}]";
            Console.WriteLine(logErrorMessage);

            if (retryAttempts <= 1) return;

            var logSleepMessage = $"Retry cooldown initiated ({RetryCoolDownInSeconds}s)";
            Console.WriteLine(logSleepMessage);

            Thread.Sleep(RetryCoolDownInSeconds * 1000);
        }

        public TResult Excecute<TResult>(
            Func<TIService, TResult> serviceCall,
            int retryAttempts = 1,
            Action<CommunicationException> exceptionHandler = null)
        {
            var errors = 0;
            var completed = false;
            CommunicationException exception = null;
            var response = default(TResult);

            while (!completed && errors < retryAttempts)
            {
                try
                {
                    if (!this.ServiceClient.State.IsReady())
                    {
                        this.DisposeClient();

                        if (!this.ServiceClient.State.IsReady())
                        {
                            throw new CommunicationObjectFaultedException($"WCF Client state is not valid. Connection Status [{this.ServiceClient.State}]");
                        }
                    }
                    
                    response = serviceCall.Invoke(this.ServiceClient);
                    completed = true;
                }
                catch (CommunicationException comsException)
                {
                    if (exceptionHandler != null)
                    {
                        try
                        {
                            exceptionHandler.Invoke(exception);
                        }
                        catch (CommunicationException reThrowException)
                        {
                            exception = reThrowException;
                        }
                    }

                    OnException(comsException.Message, serviceCall.Method.Name, ref errors, ref retryAttempts);
                }
                catch (Exception ex)
                {
                    OnException(ex.Message, serviceCall.Method.Name, ref errors, ref retryAttempts);
                }
                finally
                {
                    if (!completed)
                    {
                        this.DisposeClient();
                    }
                    else
                    {
                        this.ServiceClient.Close();
                    }
                }
            }

            if (!completed)
            {
                throw exception ?? new CommunicationException($"WCF Operation Failure: Service [{typeof(TClient)}].[{serviceCall.Method.Name}]");
            }

            return response;
        }

        public void Dispose()
        {
            this.DisposeClient();
        }

        protected virtual TClient CreateClient()
        {
            if (_binding != null && _endpoint != null)
            {
                return (TClient)Activator.CreateInstance(typeof(TClient), _binding, _endpoint);
            }

            return (TClient)Activator.CreateInstance(typeof(TClient));
        }

        private void DisposeClient()
        {
            if (this._serviceClient == null)
            {
                return;
            }

            try
            {
                switch (this._serviceClient.State)
                {
                    case CommunicationState.Closing:
                    case CommunicationState.Faulted:
                        this._serviceClient.Abort();
                        break;
                    case CommunicationState.Closed:
                        break;
                    default:
                        this._serviceClient.Close();
                        break;
                }
            }
            catch
            {
                this._serviceClient.Abort();
            }
            finally
            {
                this._serviceClient = null;
            }
        }
    }
    public static class ServiceExtentions
    {
        public static bool IsReady(this CommunicationState original)
        {
            switch (original)
            {
                case CommunicationState.Created:
                case CommunicationState.Opened:
                case CommunicationState.Opening:
                    return true;

                default:
                    return false;
            }
        }
    }
}
