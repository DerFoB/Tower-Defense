using UnityEngine;

public class DisinfectionCloudScript : MonoBehaviour
{
    public void Init(GridPoint location, float duration)
    {
        this.GetComponent<SpriteRenderer>().sortingOrder = Constants.ProjectileSortingOrder;
        this.transform.position = Util.GetWorldPosCentered(location);
        Destroy(this.gameObject, duration);
    }
}