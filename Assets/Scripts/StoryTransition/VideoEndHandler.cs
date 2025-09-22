using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
public class VideoEndHandler : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    void Start()
    {
        // Assicuriamoci che ci sia un VideoPlayer collegato
        if (videoPlayer == null)
            videoPlayer = GetComponent<VideoPlayer>();

        // Registriamo il metodo da chiamare quando il video finisce
        if(videoPlayer.clip == null)
        {
            Debug.LogError("Nessun video assegnato al VideoPlayer.");
            SceneManager.LoadScene("GameScene");
            return;
        }
        videoPlayer.loopPointReached += OnVideoEnd;
    }

    // Questo metodo viene chiamato quando il video arriva alla fine
    void OnVideoEnd(VideoPlayer vp)
    {
        Debug.Log("Il video è terminato!");
        // Qui puoi mettere la logica che vuoi, ad esempio:
        // Caricare una nuova scena, mostrare un pannello UI, avviare un altro video...
    }
}
