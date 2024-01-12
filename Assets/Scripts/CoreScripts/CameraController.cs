using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float XLimit, YLimit;

    public static bool isFollow;    //相机是否跟随目标
    [SerializeField] private Transform target;  //相机跟随的目标

    private void LateUpdate()
    {
        if (!isFollow)
            return;
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, target.position.x - XLimit, target.position.x + XLimit),
        Mathf.Clamp(transform.position.y, target.position.y - YLimit, target.position.y + YLimit),
            transform.position.z);
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
        isFollow = true;
    }

    public void Disable()
    {
        target = null;
        isFollow = false;
    }
}
