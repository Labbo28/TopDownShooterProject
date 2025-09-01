using UnityEngine;

public class DamageNumberController : MonoBehaviour
{

    public DamageNumber prefab;



        public void CreateNumber(float value, Vector3 location){
        Instantiate(prefab, location, transform.rotation, transform);
        
    }
}
