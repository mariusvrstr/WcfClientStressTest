
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
 
namespace WcfServiceProxy
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Channels;

    public class ServiceClientWrapper<TClient, TIService> : IDisposable
        where TClient : ClientBase<TIService>, TIService
        where TIService : class
    {
        private TClient _serviceClient;
        private Binding _binding;
        private EndpointAddress _endpoint;
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
        public TResult Excecute<TResult>(
            Func<TIService, TResult> serviceCall,
            int retryAttempts = 1,
            Action<CommunicationException> exceptionHandler = null)
        {
            var errors = 0;
            CommunicationException exception = null;
            while (errors < retryAttempts)
            {
                this.DisposeClient();
                try
                {
                    return serviceCall.Invoke(this.ServiceClient);
                }
                catch (CommunicationException comsException)
                {
                    exception = comsException;
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
                    errors++;
                }
                finally
                {
                    this.DisposeClient();
                }
            }
            throw exception ?? new CommunicationException(@"Excecution unsuccessfull with no exceptions. Invalid state reached inside 'Service Client Wrapper' for opperation.");
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
                if (this._serviceClient.State == CommunicationState.Faulted)
                {
                    this._serviceClient.Abort();
                }
                else
                {
                    this._serviceClient.Close();
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
}
