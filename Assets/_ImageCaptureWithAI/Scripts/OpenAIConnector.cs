using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;
using OVRSimpleJSON;
using Oculus.Voice;
using UnityEngine.Serialization;
using TMPro;

public class OpenAIConnector : MonoBehaviour
{
    [SerializeField] GameObject mushroomModel;

    [SerializeField] private string APIKey = "YOUR_API_KEY";

    [Header("Managers")]
    [Tooltip("The wit AppVoiceExperience, responsible for capturing the voice command.")]
    [SerializeField] private AppVoiceExperience appVoiceExperience;

    [FormerlySerializedAs("imageLoader")]
    [Tooltip("The component responsible for loading the image from the Meta Quest storage.")]
    [SerializeField] private ImageHandler imageHandler;

    [Tooltip("The component responsible for speaking the response from OpenAI.")]
    [SerializeField] private ResponseTTS voiceAssistant;

    [Tooltip("Text component that is responsible for displaying the input transcript.")]
    [SerializeField] private TextMeshProUGUI inputTranscriptText;

    [Space(20)]
    [Tooltip("Event being fired when the response from OpenAI has arrived.")]
    [SerializeField] private string baseCommand = "";
    
    [Space(20)]
    [Tooltip("Event being fired when the image is being sent to OpenAI.")]
    public UnityEvent onRequestSent;

    [Tooltip("Event being fired when the response from OpenAI has arrived.")]
    public UnityEvent<string> onResponseReceived;

    public string imageApiUrl = "https://api.openai.com/v1/images/edits";

    private string gptVisionModel = "gpt-4o";
    private string command = "";

    private void Awake()
    {
        appVoiceExperience.TranscriptionEvents.OnFullTranscription.AddListener(GetTranscriptAndSendImage);
    }

    private void OnDestroy()
    {
        appVoiceExperience.TranscriptionEvents.OnFullTranscription.RemoveListener(GetTranscriptAndSendImage);
    }

    private void SendImageToOpenAI() => StartCoroutine(SendImageRequest(imageHandler.cachedTexture));

    private IEnumerator SendImageRequest(Texture2D image)
    {
        // Convert Texture2D to byte array
        byte[] otterImageData = image.EncodeToPNG();

        //var imageBytes = image.EncodeToJPG();
        //var base64Image = Convert.ToBase64String(imageBytes);
        //var payloadJson = PreparePayload(base64Image);

        //var request = new UnityWebRequest(imageApiUrl, "POST");
        //var bodyRaw = Encoding.UTF8.GetBytes(payloadJson);

        //request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        //request.downloadHandler = new DownloadHandlerBuffer();

        //request.SetRequestHeader("Content-Type", "application/json");
        //request.SetRequestHeader("Authorization", "Bearer " + APIKey);

        //onRequestSent?.Invoke();

        //yield return request.SendWebRequest();

        //if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        //{
        //    Debug.LogError($"Error sending request: {request.error}. Response code: {request.responseCode}");
        //}
        //else
        //{
        //    var jsonResponse = JSON.Parse(request.downloadHandler.text);
        //    string imageUrl = jsonResponse["data"][0]["url"];

        //    // Download the image
        //    yield return StartCoroutine(DownloadImage(imageUrl));
        //}

        // Create form
        WWWForm form = new WWWForm();
        form.AddField("prompt", "A cute baby sea otter wearing a beret");
        form.AddField("n", "2");
        form.AddField("size", "1024x1024");
        form.AddBinaryData("image", otterImageData, "otter.png", "image/png");

        // Create request
        UnityWebRequest request = UnityWebRequest.Post(imageApiUrl, form);
        request.SetRequestHeader("Authorization", "Bearer " + APIKey);

        onRequestSent?.Invoke();


        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError($"Error sending request: {request.error}. Response code: {request.responseCode}");
            inputTranscriptText.text = request.error;
        }
        else
        {



            inputTranscriptText.text = request.downloadHandler.text;
            var jsonResponse = JSON.Parse(request.downloadHandler.text);
            string imageUrl = jsonResponse["data"][0]["url"];

            // Download the image
            yield return StartCoroutine(DownloadImage(imageUrl));
        }
    }


    private IEnumerator DownloadImage(string imageUrl)
    {
        UnityWebRequest textureRequest = UnityWebRequestTexture.GetTexture(imageUrl);

        yield return textureRequest.SendWebRequest();

        if (textureRequest.result == UnityWebRequest.Result.Success)
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(textureRequest);
            ApplyTexture(texture);
        }
        else
        {
            Debug.LogError("Failed to download image: " + textureRequest.error);
        }
    }

    private void ApplyTexture(Texture2D texture)
    {
        if (mushroomModel != null)
        {
            Renderer renderer = mushroomModel.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.mainTexture = texture;
            }
        }
    }

    private string PreparePayload(string base64Image)
    {
        //// Create the request payload
        //var jsonData = new
        //{
        //    prompt = command,
        //    n = 1, // Number of images to generate
        //    size = "1024x1024", // Size of the image
        //    image = base64Image // Include the base64 image in the request
        //};

        var jsonData = new ImageGenerationData
        {
            prompt = command,
            n = "1", // Number of images to generate
            size = "1024x1024" // Size of the image
        };


        inputTranscriptText.text = JsonUtility.ToJson(jsonData);

        return JsonUtility.ToJson(jsonData);
    }

    public void GetVoiceCommand() => appVoiceExperience.Activate();

    public void GetTranscriptAndSendImage(string transcript)
    {
        command = "What is this picture about?";
        inputTranscriptText.text = "What is this picture about?";
        SendImageToOpenAI();
    }
}

[System.Serializable]
public class ImageGenerationData
{
    public string prompt;
    public string n; // Number of images to generate
    public string size; // Size of the image
}
