using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Networking;

//参考
//https://gamefbb.com/%E3%80%90unity%E3%80%91%E3%82%B9%E3%82%AF%E3%83%AA%E3%83%BC%E3%83%B3%E3%82%B7%E3%83%A7%E3%83%83%E3%83%88%E3%82%92%E3%83%84%E3%82%A4%E3%83%BC%E3%83%88%E3%81%99%E3%82%8B%E3%80%90twitter%E3%80%91/

/// <summary>
/// 
/// Unity 2018.2.11f1
/// 
/// ClientIDは https://api.imgur.com/oauth2/addclient で取得
/// 
/// ClientIDを忘れた場合は https://imgur.com/account/settings/apps で確認
/// 
/// </summary>

public class OpenTwitterURL : MonoBehaviour
{
    // Plugins
    [DllImport("__Internal")] private static extern void OpenNewWindow(string URL);

    // Inspector
    [SerializeField] private string imgurClientID;


    // ツイートボタンを押したときの処理
    public void ClickTweetButton()
    {
        SoundManager.Instance.Play(SoundName.ti);
        // 画像リンク付きツ
        StartCoroutine(Tweet("もじだけMMOで"+ SaveDataManager.saveData.charaInfo.name +"を" + SaveDataManager.saveData.charaInfo.GetCurrentJobInfo().jobData.name 
            + "(Lv" + SaveDataManager.saveData.charaInfo.GetCurrentJobInfo().lv + ")まで育てたよ "
            + "アイテム収集率:"+SaveDataManager.saveData.charaInfo.inventory.Count + "/" + (DatabaseManager.Instance.itemDatabase.itemDatas.Count-1), "もじだけMMO")); ;
    }


    // 画像リンク付きツイート
    private IEnumerator Tweet(string comment, string tag)
    {
        // 1. スクリーンショットを撮る
        string fileName = string.Format("{0:yyyyMMddHmmss}", DateTime.Now);
        string filePath = Application.persistentDataPath + "/" + fileName + ".png";
        ScreenCapture.CaptureScreenshot(filePath);

        float startTime = Time.time;
        while (File.Exists(filePath) == false)
        {
            if (Time.time - startTime > 6.0f)
            {
                yield break;
            }
            else
            {
                yield return null;
            }
        }

        byte[] imageData = File.ReadAllBytes(filePath);
        File.Delete(filePath);

        // 2. imgurへアップロード
        WWWForm wwwForm = new WWWForm();
        wwwForm.AddField("image", Convert.ToBase64String(imageData));
        wwwForm.AddField("type", "base64");

        UnityWebRequest www;
        www = UnityWebRequest.Post("https://api.imgur.com/3/image.xml", wwwForm);
        www.SetRequestHeader("AUTHORIZATION", "Client-ID " + imgurClientID);
        yield return www.SendWebRequest();

        string uploadedURL = "";

        if (www.isNetworkError)
        {
            Debug.Log(www.error);
        }
        else
        {
            XDocument xDoc = XDocument.Parse(www.downloadHandler.text);
            uploadedURL = xDoc.Element("data").Element("link").Value;
        }

        // 3. 画像リンク付きツイート
        string tweetURL = "http://twitter.com/intent/tweet?text=" + comment + "&url=" + Path.ChangeExtension(uploadedURL, null) + "&hashtags=" + tag;

#if UNITY_EDITOR
        Application.OpenURL(tweetURL);
#elif UNITY_WEBGL
          OpenNewWindow(tweetURL);
#else
          Application.OpenURL(tweetURL);
#endif
    }
}