using Oculus.Voice;
using UnityEngine;
using UnityEngine.UI;

public class VoiceToggler : MonoBehaviour
{
    [SerializeField] private Toggle microphoneToggle;
    [SerializeField] private OpenAIConnector aiConnector;

    private void Awake()
    {
        microphoneToggle.onValueChanged.AddListener(ToggleVoiceExperience);
    }
    
    private void OnDestroy()
    {
        microphoneToggle.onValueChanged.RemoveListener(ToggleVoiceExperience);
    }

    private void ToggleVoiceExperience(bool isOn)
    {
        aiConnector.GetTranscriptAndSendImage("BLAH");
    }

    private void OnVoiceExperienceStarted() => microphoneToggle.isOn = true;

    private void OnVoiceExperienceStopped() => microphoneToggle.isOn = false;
}