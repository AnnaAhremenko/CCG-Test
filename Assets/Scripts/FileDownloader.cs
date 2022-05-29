using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public static class FileDownloader
{
    private static string url = "https://picsum.photos/200/300";

    public static async Task<Texture2D> DownloadImageAsync()
    {
        using var request = UnityWebRequestTexture.GetTexture(url);

        await request.SendWebRequest();

        return request.result == UnityWebRequest.Result.Success
            ? DownloadHandlerTexture.GetContent(request)
            : null;
    }
}
