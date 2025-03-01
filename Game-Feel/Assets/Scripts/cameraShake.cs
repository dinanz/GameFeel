using UnityEngine;

public class cameraShake : MonoBehaviour
{
    [SerializeField] private float shakeAmount = 0.02f;
    [SerializeField] public bool shakeOn = false;
    private Vector3 initialPos;

    void Start()
    {
        initialPos = transform.position;
    }

    void Update()
    {
        if(shakeOn){
            transform.position = initialPos + Random.insideUnitSphere* shakeAmount;
        }
    }

    public void useToggle(bool val){
        shakeOn = val;
    }
}
