namespace ARVI.SDK
{
    using System;
    using System.Collections.Generic;

    public class Request
    {
        /// <value>Request ID</value>
        public ulong ID { get; private set; }

        /// <summary>
        /// Request constructor
        /// </summary>
        /// <param name="id">Request ID</param>
        public Request(ulong id)
        {
            ID = id;
        }

        internal virtual void HandleResponse(Response response)
        {

        }
    }

    public class Request<T> : Request where T : Response
    {
        private Action<T> callback;
        private readonly bool isValid = true;
        private readonly string errorMessage = "";

        public Request(ulong id) : base(id)
        {
            if (id == 0)
            {
                isValid = false;
                errorMessage = Errors.UNKNOWN_ERROR;
            }
        }

        public Request(string errorMessage) : base(0)
        {
            isValid = false;
            this.errorMessage = errorMessage;
        }

        public static Request<T> ConstructInvalid(string errorMessage)
        {
            return new Request<T>(errorMessage);
        }

        /// <summary>
        /// Sets the method to be executed when request is completed
        /// </summary>
        /// <param name="callback">Callback method</param>
        /// <returns>Request</returns>
        public Request<T> OnComplete(Action<T> callback)
        {
            this.callback = callback;
            if (callback != null)
            {
                if (isValid)
                    Requests.Add(this);
                else
                    callback.Invoke(new Response(ResponseType.Default, new Error(0, errorMessage)) as T);
            }

            return this;
        }

        internal override void HandleResponse(Response response)
        {
            base.HandleResponse(response);

            if ((response is T) && (callback != null))
                callback.Invoke(response as T);
        }
    }

    public static class Requests
    {
        private static readonly object requestsLock = new object();
        private static readonly Dictionary<ulong, Request> requests = new Dictionary<ulong, Request>();

        public static void InitializeRequests()
        {
            API.Requests_Initialize();
        }

        public static void FinalizeRequests(bool waitForComplete)
        {
            API.Requests_Finalize(waitForComplete);
        }

        /// <summary>
        /// Clears the list of RequestID-to-Request associations
        /// </summary>
        public static void Clear()
        {
            lock (requestsLock)
            {
                requests.Clear();
            }
        }

        /// <summary>
        /// Adds the request to list of RequestID-to-Request associations
        /// </summary>
        /// <param name="request">Request object</param>
        public static void Add(Request request)
        {
            if (request != null)
            {
                lock(requestsLock)
                {
                    requests[request.ID] = request;
                }
            }
        }

        /// <summary>
        /// Dequeues and executes available responses from internal queue
        /// </summary>
        public static void Execute()
        {
            var responses = Responses.Dequeue();
            if (responses != null)
            {
                for (int i = 0; i < responses.Length; ++i)
                    HandleResponse(responses[i]);
            }
        }

        private static void HandleResponse(Response response)
        {
            if (response != null)
            {
                Request request = null;
                lock (requests)
                {
                    if (requests.TryGetValue(response.RequestID, out request))
                        requests.Remove(response.RequestID);
                }
                if (request != null)
                    request.HandleResponse(response);
            }
        }
    }
}