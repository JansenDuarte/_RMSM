using UnityEngine;
using ExtensionMethods;

[DisallowMultipleComponent]
public class BackgroundScroller : MonoBehaviour
{
    #region VARIABLES

    [Header("Metrics"), Space]
    [Tooltip("Speed normalized by the frame rate")]
    [Range(0f, 5f)] public float Background_Speed;


    [Tooltip("Wrong values may cause jitters")]
    public float Travel_Limit;

    private Vector3 m_initialPosition;

    #endregion // VARIABLES
    private void Awake() { m_initialPosition = transform.localPosition; }

    private void Update()
    {
        m_initialPosition.AddScalar_OnAxis(Background_Speed * Time.deltaTime, Axis.X);

        if (m_initialPosition.x >= Travel_Limit)
        {
            m_initialPosition.x = -Travel_Limit;
            transform.localPosition = new Vector3(-Travel_Limit, 0f, 0f);
            return;
        }

        transform.localPosition = m_initialPosition;
    }
}
