using UnityEngine;
using UnityEngine.UI; // Assicurati di includere questo se usi UI Text

public class GameTimeDisplay : MonoBehaviour
{
    public Text timeText; // Riferimento al componente Text per mostrare il tempo

    private void Update()
    {
        // Assumi che GameManager.Instance.gameTime sia accessibile
        string formattedTime = FormatGameTime(GameManager.Instance.GetGameTime());
        
    }

    // Metodo statico per formattare il tempo
    public static string FormatGameTime(float totalSeconds)
    {
        int minutes = Mathf.FloorToInt(totalSeconds / 60);
        int seconds = Mathf.FloorToInt(totalSeconds % 60);
        
        // Usa String.Format per garantire due cifre per minuti e secondi
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}