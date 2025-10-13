using UnityEngine;

public class Instanciator : MonoBehaviour
{
    public void Instantiate(GameObject gameObject = null)
    {
        GameObject.Instantiate(gameObject);
    }
}
