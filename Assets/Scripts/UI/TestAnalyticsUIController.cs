using System;
using System.Text;
using Systems.Analytic;
using TMPro;
using UnityEngine;
using Zenject;

public class TestAnalyticsUIController : MonoBehaviour
{
    [SerializeField] private TMP_Text _responsesText;

    [SerializeField] private TMP_InputField _addressInputField;
    [SerializeField] private TMP_InputField _cooldownInputField;

    [SerializeField] private TMP_Text _eventsList;
    
    private AnalyticService _analyticService;

    [Inject]
    public void InjectService(AnalyticService service)
    {
        _analyticService = service;
        _analyticService.AnalyticSendStarted += AnalyticServiceOnAnalyticSendStarted;
        _analyticService.AnalyticSendCompleted += AnalyticServiceOnAnalyticSendCompleted;
        _analyticService.AnalyticEventAdded += AnalyticServiceOnAnalyticEventAdded;
    }

    private void Start()
    {
        UpdateServerAddress(_addressInputField.text);
        UpdateCooldownTime(_cooldownInputField.text);
    }
    

    public void UpdateServerAddress(string address)
    {
        _analyticService.serverUrl = address;
    }

    public void UpdateCooldownTime(string cooldown)
    {
        _analyticService.cooldownBeforeSend = float.Parse(cooldown);
    }

    public void TrackLevelStart()
    {
        _analyticService.TrackEvent("levelStart", "level:3");
    }

    public void TrackNewReward()
    {
        _analyticService.TrackEvent("newReward", "reward:chest");
    }

    public void TrackSpentMoney()
    {
        _analyticService.TrackEvent("spentMoney", "count:300");
    }
    
    private void AnalyticServiceOnAnalyticEventAdded()
    {
        StringBuilder eventsList = new StringBuilder();
        AnalyticsEvent[] events = _analyticService.GetCurrentEventsList();

        foreach (var currentEvent in events)
        {
            eventsList.Append("Type: ");
            eventsList.Append(currentEvent.type);
            eventsList.Append(", Data: ");
            eventsList.Append(currentEvent.data);
            eventsList.AppendLine();
        }

        _eventsList.text = eventsList.ToString();
    }
    
    private void AnalyticServiceOnAnalyticSendCompleted(WebResponseInfo response)
    {
        if (response.success)
        {
            _responsesText.color = Color.green;;
            _responsesText.text = "OK 200";
            _eventsList.text = "";
        }
        else
        {
            _responsesText.color = Color.red;
            _responsesText.text = "NOT OK: " + response.error;
        }
    }

    private void AnalyticServiceOnAnalyticSendStarted()
    {
        _responsesText.color = Color.white;;
        _responsesText.text = "Request in progress...";
    }
    

}
