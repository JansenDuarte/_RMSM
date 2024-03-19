using UnityEngine;
using ExtensionMethods;

[DisallowMultipleComponent]
public class BackgroundScroller : MonoBehaviour
{
    private void Awake() { position = transform.localPosition; }

    void Update()
    {
        position.AddScalar_OnAxis(backgroundSpeed * Time.deltaTime, Axis.X);

        if (position.x >= travelLimit)
        {
            position.x = -travelLimit;
            transform.localPosition = new Vector3(-travelLimit, 0f, 0f);
            return;
        }

        transform.localPosition = position;
    }


    #region VARIABLES

    [Header("Metrics"), Space]
    [Tooltip("Speed normalized by the frame rate")]
    public float backgroundSpeed;
    [Tooltip("Wrong values may cause jitters")]
    public float travelLimit;

    private Vector3 position;

    #endregion // VARIABLES
}
