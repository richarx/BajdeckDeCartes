using UnityEngine;

public class Fall : MonoBehaviour
{

    [SerializeField] float maxDown = 20f;
    [SerializeField] public float velocity = 1f;
   
    private float _currentDown = 0f;

    
    // Update is called once per frame
    void Update()
    {
        _currentDown += Time.deltaTime * velocity;
        transform.Translate(Vector3.down * Time.deltaTime * velocity);
        if (_currentDown >= maxDown)
        {
            Destroy(this.gameObject);
        }
    }
}
