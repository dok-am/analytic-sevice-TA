
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Systems.Analytic
{
    public class AnalyticService : MonoBehaviour
    {
        public string serverUrl;
        public float cooldownBeforeSend;

        public event Action<AnalyticEvent> AnalyticEventAdded;
        public event Action AnalyticSendStarted;
        public event Action<WebResponseInfo> AnalyticSendCompleted;
        
        private AnalyticsData _dataToSend = new AnalyticsData();

        private Coroutine _sendingCoroutine;

        public void TrackEvent(string type, string data)
        {
            AnalyticEvent newEvent = _dataToSend.AddEvent(type, data);
            AnalyticEventAdded?.Invoke(newEvent);
            StartCoroutineIfNot();
        }

        public AnalyticEvent[] GetCurrentEventsList() => _dataToSend.events.ToArray();

        private void StartCoroutineIfNot()
        {
            if (_sendingCoroutine != null)
            {
                return;
            }

            _sendingCoroutine = StartCoroutine(SendingCoroutine());
        }

        private IEnumerator SendingCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(cooldownBeforeSend);
                yield return SendCollectedEvents();
            }
        }

        private IEnumerator SendCollectedEvents()
        {
            if (_dataToSend.events.Count == 0)
            {
                //Nothing to send
                yield break;
            }
            
            if (string.IsNullOrEmpty(serverUrl))
            {
                Debug.LogError("Analytic Server address can't be empty");
                yield break;
            }

            string eventsData = JsonUtility.ToJson(_dataToSend);

            Debug.Log(eventsData);
            
            UnityWebRequest request = UnityWebRequest.Post(serverUrl, eventsData);
            
            AnalyticSendStarted?.Invoke();
            yield return request.SendWebRequest();

            Debug.Log(request.responseCode + " " + request.result.ToString() + " " + request.error);
            
            bool success = (request.responseCode == 200);

            if (success)
            {
                _dataToSend.Clear();
            }
            
            WebResponseInfo responseInfo = new WebResponseInfo() { success = success, error = request.error ?? "" };
            AnalyticSendCompleted?.Invoke(responseInfo);
            
        }
    }
}