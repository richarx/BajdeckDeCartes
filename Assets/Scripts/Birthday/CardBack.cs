using UnityEngine;

public class CardBack : MonoBehaviour
{
    [SerializeField] Canvas front;
    [SerializeField] Canvas back;


    // true si (x,y) ∈ [0,180)×[0,180) OU [180,360)×[180,360)
    public bool InAllowedQuadrant()
    {
        Vector3 euler = transform.localEulerAngles;
        float x = Mathf.DeltaAngle(0f, euler.x);
        float y = Mathf.DeltaAngle(0f, euler.y);

        bool qCenter = Mathf.Abs(x) <= 90f && Mathf.Abs(y) <= 90f;      // [-90,90]
        bool qOpp = Mathf.Abs(x) > 90f && Mathf.Abs(y) > 90f;       // (90,180]∪[-180,-90)
        return qCenter || qOpp;
    }


    // Update is called once per frame
    void Update()
    {
        if (InAllowedQuadrant() && !front.gameObject.activeInHierarchy )
        {
            front.gameObject.SetActive(true);
            back.gameObject.SetActive(false);
        }
        else if (!InAllowedQuadrant() && front.gameObject.activeInHierarchy)
        {
            back.gameObject.SetActive(true);
            front.gameObject.SetActive(false);
        }
    }
}
