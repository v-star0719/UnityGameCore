using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Threading;
using System.Net;
using System;
using GameCore.Core;

namespace GameCore.Unity.RemoteProfile
{
    public class ProfileServer : MonoBehaviour
    {
        public static ProfileServer ActiveServer { get; private set; }

        public int Port = 9527;

        private class RequestEntry
        {
            public RequestEntry(HttpListenerRequest request, HttpListenerResponse response)
            {
                Request = request;
                Response = response;
            }

            public HttpListenerRequest Request { get; private set; }
            public HttpListenerResponse Response { get; private set; }
        }

        private HttpListener httpListener;
        private Queue<RequestEntry> requestQueue = new Queue<RequestEntry>();
        private Dictionary<string, ProfileBase> handlerDict = new Dictionary<string, ProfileBase>();

        private void InitHandlerDict()
        {
            handlerDict["/api/get-count"] = new ObjectCounter();
        }


        void Awake()
        {
            ActiveServer = this;
            InitHandlerDict();
        }


        // Use this for initialization
        void Start()
        {
            httpListener = new HttpListener();
            httpListener.Prefixes.Add("http://*:" + Port + "/");
            httpListener.Start();
            ThreadPool.QueueUserWorkItem(new WaitCallback(HttpListenerProc));
        }

        void OnDestroy()
        {
            if (httpListener != null)
            {
                httpListener.Stop();
                httpListener.Close();
            }

            if (ActiveServer == this)
            {
                ActiveServer = null;
            }
        }

        void Update()
        {
            RequestEntry requestEntry = null;
            lock (requestQueue)
            {
                if (requestQueue.Count > 0)
                {
                    requestEntry = requestQueue.Dequeue();
                }
            }

            if (requestEntry != null)
            {
                HandleRequest(requestEntry.Request, requestEntry.Response);
            }
        }


        private void HandleRequest(HttpListenerRequest request, HttpListenerResponse response)
        {
            try
            {
                LoggerX.Debug("Got request w/ path: " + request.Url.LocalPath);

                if (handlerDict.TryGetValue(request.Url.LocalPath, out var profileBase))
                {
                    profileBase.HandleRequest(request, response);
                }
                else
                {
                    var textAsset = Resources.Load(request.Url.LocalPath.Substring(1)) as TextAsset;
                    if (textAsset != null)
                    {
                        response.ContentLength64 = textAsset.bytes.LongLength;
                        response.OutputStream.Write(textAsset.bytes, 0, textAsset.bytes.Length);
                    }
                    else
                    {
                        response.StatusCode = 404;
                        response.ContentLength64 = 0;
                    }
                }
            }
            catch (Exception e)
            {
                LoggerX.Debug("ProfileServer exception: " + e.Message);
            }
            finally
            {
                response.OutputStream.Close();
            }
        }


        private void HttpListenerProc(object state)
        {
            while (httpListener.IsListening)
            {
                try
                {
                    var context = httpListener.GetContext();
                    requestQueue.Enqueue(new RequestEntry(context.Request, context.Response));
                }
                catch (Exception e)
                {
                    LoggerX.Debug("ProfileServer exception: " + e.Message);
                }
            }
        }
    }
}