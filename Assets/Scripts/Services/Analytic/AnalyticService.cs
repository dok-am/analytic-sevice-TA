
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

        public event Action AnalyticEventAdded;
        public event Action AnalyticSendStarted;
        public event Action<WebResponseInfo> AnalyticSendCompleted;

        private const string AnalyticDataKey = "AnalyticServiceDataKey";
        
        private AnalyticsData _dataToSend = new AnalyticsData();
        private Coroutine _sendingCoroutine;

        private void Start()
        {
            CheckIfPresavedEventsExists();
        }

        #region Public methods
        
        public void TrackEvent(string type, string data)
        {
            AnalyticsEvent newEvent = _dataToSend.AddEvent(type, data);
            UpdatePresavedEvents();
            AnalyticEventAdded?.Invoke();
            StartCoroutineIfNot();
        }

        public AnalyticsEvent[] GetCurrentEventsList() => _dataToSend.events.ToArray();
        
        #endregion
        
        #region Private methods

        private void CheckIfPresavedEventsExists()
        {
            if (PlayerPrefs.HasKey(AnalyticDataKey))
            {
               _dataToSend.AddEventsFromString(PlayerPrefs.GetString(AnalyticDataKey));

               if (_dataToSend.events.Count > 0)
               {
                   AnalyticEventAdded?.Invoke();
                   StartCoroutineIfNot();
               }
            }
        }

        private void UpdatePresavedEvents()
        {
            PlayerPrefs.SetString(AnalyticDataKey, JsonUtility.ToJson(_dataToSend));
            PlayerPrefs.Save();
        }
        
        #endregion

        #region Sending coroutines
        
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
                UpdatePresavedEvents();
            }
            
            WebResponseInfo responseInfo = new WebResponseInfo() { success = success, error = request.error ?? "" };
            AnalyticSendCompleted?.Invoke(responseInfo);
            
        }
        
        #endregion
    }
}