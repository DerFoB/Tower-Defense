using UnityEngine;

public class SoapEffectScript : MonoBehaviour
{
    public void Init(GridPoint position, float duration)
    {
        this.transform.position = Util.GetWorldPosCentered(position);
        this.GetComponent<SpriteRenderer>().sortingOrder = Constants.ProjectileSortingOrder;
        Destroy(this.gameObject, duration);
    }
}
