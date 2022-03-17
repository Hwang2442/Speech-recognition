using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Demo : MonoBehaviour
{
    [Header("REST API")]
    public string key = "";
    public string uri = "http://aiopen.etri.re.kr:8000/WiseASR/Recognition";

    [Header("Components")]
    public Text recognizedText;
    public Text timerText;
    public AudioSource audioSource;

    public void OnClickOpenFilePanel()
    {
        recognizedText.text = "Recognizeing...";

#if UNITY_EDITOR
        string path = UnityEditor.EditorUtility.OpenFilePanel("Select wav file", "", "wav");

        if (string.IsNullOrEmpty(path))
            return;

        byte[] data = System.IO.File.ReadAllBytes(path);
        RequestBody body = new RequestBody();

        body.accessKey = key;
        body.argument = new RequestBody.Argument();
        body.argument.languageCode = "english";
        body.argument.audio = System.Convert.ToBase64String(data);

        StartCoroutine(Co_LoadAudioClip(path));
        StartCoroutine(Co_SendVoiceFile(Newtonsoft.Json.JsonConvert.SerializeObject(body), StartCoroutine(Co_Timer(Time.time))));
#endif
    }

    private IEnumerator Co_SendVoiceFile(string requestBody, Coroutine timer)
    {
        using (UnityWebRequest www = UnityWebRequest.Post(uri, requestBody))
        {
            www.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(requestBody));

            yield return www.SendWebRequest();

            if (!string.IsNullOrEmpty(www.error))
            {
                Debug.LogError(www.error);

                yield break;
            }

            ResponseBody responseBody = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseBody>(www.downloadHandler.text);
            recognizedText.text = string.Format("Recognized : {0}", responseBody.returnObject.recognized);
        }

        StopCoroutine(timer);
    }

    private IEnumerator Co_LoadAudioClip(string path)
    {
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(new System.Uri(path), AudioType.WAV))
        {
            yield return www.SendWebRequest();

            audioSource.PlayOneShot(DownloadHandlerAudioClip.GetContent(www));
        }
    }

    private IEnumerator Co_Timer(float start)
    {
        while (true)
        {
            timerText.text = string.Format("Recognizing time : {0}", (Time.time - start).ToString("N3"));

            yield return null;
        }
    }
}
