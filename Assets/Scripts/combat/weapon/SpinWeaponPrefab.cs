using UnityEngine;

/// <summary>
/// SpinWeaponPrefab rappresenta una singola lama rotante.
/// Gestisce la posizione orbitale e il danno ai nemici.
/// </summary>
public class SpinWeaponPrefab : MonoBehaviour
{
    private float angle; // Angolo corrente della lama rispetto al centro
    public SpinWeapon weapon; // Riferimento all'arma principale per accedere ai parametri

    /// <summary>
    /// Inizializza la lama con un offset di rotazione.
    /// </summary>
    /// <param name="rotationOffset">Angolo iniziale in gradi</param>
    public void Init(float rotationOffset)
    {
        this.angle = rotationOffset;
        transform.localScale = Vector3.one; // Imposta la scala di default
    }

    /// <summary>
    /// Aggiorna la posizione orbitale della lama attorno al centro.
    /// Calcola la nuova posizione e orienta la lama nella direzione del movimento.
    /// </summary>
    void Update()
    {
        if (weapon == null) return; // Se non c'è riferimento all'arma, esci

        // Incrementa l'angolo in base alla velocità dell'arma
        angle += weapon.speed * 360f * Time.deltaTime;
        if (angle > 360f) angle -= 360f; // Mantieni l'angolo tra 0 e 360

        // Calcola la posizione orbitale usando seno e coseno
        float rad = angle * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f) * weapon.range;
        transform.localPosition = offset; // Aggiorna la posizione locale
        transform.up = offset.normalized; // Orienta la lama verso l'esterno
    }

  
    /// <summary>
    /// Gestisce la collisione con i nemici.
    /// Se il collider ha il tag "Enemy" e implementa IDamageable, infligge danno.
    /// </summary>
    /// <param name="collider">Collider che entra in contatto</param>
    private void OnTriggerEnter2D(Collider2D collider)
    {
        // Controlla se il riferimento all'arma è nullo
        if (weapon == null)
        {
            return;
        }
        if (collider.CompareTag("Enemy"))
        {
            if (collider.TryGetComponent<IDamageable>(out var target))
            {
                AudioManager.Instance?.PlayMeleeSound();
                target.TakeDamage(weapon.weaponDamage); // Infligge danno usando il valore dell'arma
            }
        }
    }
}
