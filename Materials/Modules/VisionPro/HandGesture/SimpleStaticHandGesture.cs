/// <summary>
/// Copyright (c) 2026 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.6.1
/// </summary>

using UnityEngine.Events;
using UnityEngine.XR.Hands.Gestures;

namespace UnityEngine.XR.Hands.Samples.GestureSample
{
  /// <summary>
  /// һ�ּ���ֱ��־�̬��״�ͷ�����Сʱ������ơ�
  /// A gesture that detects when a hand is held in a static shape and orientation for a minimum amount of time.
  /// </summary>
  public class SimpleStaticHandGesture : MonoBehaviour
  {

    [SerializeField]
    [Tooltip("The hand tracking events component to subscribe to receive updated joint data to be used for gesture detection.")]
    XRHandTrackingEvents m_HandTrackingEvents;//�����ֲ������¼�����Խ����������Ƽ��ĸ��µ��������ݡ�

    [SerializeField]
    [Tooltip("The hand shape or pose that must be detected for the gesture to be performed.")]
    ScriptableObject m_HandShapeOrPose;//Ҫִ�����Ʊ����⵽���ֵ���״������


    [SerializeField]
    [Tooltip("The event fired when the gesture is performed.")]
    UnityEvent m_GesturePerformed; //���¼���ִ������ʱ������

    [SerializeField]
    [Tooltip("The event fired when the gesture is ended.")]
    UnityEvent m_GestureEnded;//���¼������ƽ���ʱ������

    [SerializeField]
    [Tooltip("The minimum amount of time the hand must be held in the required shape and orientation for the gesture to be performed.")]
    float m_MinimumHoldTime = 0.2f; //�ֱ��뱣������Ҫ�����״�ͷ�������������Ƶ����ʱ�䡣

    [SerializeField]
    [Tooltip("The interval at which the gesture detection is performed.")]
    float m_GestureDetectionInterval = 0.1f; //ִ�����Ƽ���ʱ����


    XRHandShape m_HandShape;
    XRHandPose m_HandPose;
    bool m_WasDetected;
    bool m_PerformedTriggered;
    float m_TimeOfLastConditionCheck;
    float m_HoldStartTime;

    /// <summary>
    ///�ָ����¼�������Ľ��ո��µ��������ݣ��������Ƽ�⡣
    /// The hand tracking events component to subscribe to receive updated joint data to be used for gesture detection.
    /// </summary>
    public XRHandTrackingEvents handTrackingEvents
    {
      get => m_HandTrackingEvents;
      set => m_HandTrackingEvents = value;
    }

    /// <summary>
    /// Ҫִ�����Ʊ����⵽���ֵ���״�����ơ�
    /// The hand shape or pose that must be detected for the gesture to be performed.
    /// </summary>
    public ScriptableObject handShapeOrPose
    {
      get => m_HandShapeOrPose;
      set => m_HandShapeOrPose = value;
    }


    /// <summary>
    /// ���¼���ִ������ʱ������
    /// The event fired when the gesture is performed.
    /// </summary>
    public UnityEvent gesturePerformed
    {
      get => m_GesturePerformed;
      set => m_GesturePerformed = value;
    }

    /// <summary>
    /// ���¼������ƽ���ʱ������
    /// The event fired when the gesture is ended.
    /// </summary>
    public UnityEvent gestureEnded
    {
      get => m_GestureEnded;
      set => m_GestureEnded = value;
    }

    /// <summary>
    /// �ֱ��뱣������Ҫ�����״�ͷ�������������Ƶ����ʱ�䡣
    /// The minimum amount of time the hand must be held in the required shape and orientation for the gesture to be performed.
    /// </summary>
    public float minimumHoldTime
    {
      get => m_MinimumHoldTime;
      set => m_MinimumHoldTime = value;
    }

    /// <summary>
    /// ִ�����Ƽ���ʱ�����
    /// The interval at which the gesture detection is performed.
    /// </summary>
    public float gestureDetectionInterval
    {
      get => m_GestureDetectionInterval;
      set => m_GestureDetectionInterval = value;
    }

    void OnEnable()
    {
      m_HandTrackingEvents.jointsUpdated.AddListener(OnJointsUpdated);

      m_HandShape = m_HandShapeOrPose as XRHandShape;
      m_HandPose = m_HandShapeOrPose as XRHandPose;

    }

    void OnDisable() => m_HandTrackingEvents.jointsUpdated.RemoveListener(OnJointsUpdated);

    void OnJointsUpdated(XRHandJointsUpdatedEventArgs eventArgs)
    {
      if (!isActiveAndEnabled || Time.timeSinceLevelLoad < m_TimeOfLastConditionCheck + m_GestureDetectionInterval)
        return;

      var detected =
          m_HandTrackingEvents.handIsTracked &&
          m_HandShape != null && m_HandShape.CheckConditions(eventArgs) ||
          m_HandPose != null && m_HandPose.CheckConditions(eventArgs);

      if (!m_WasDetected && detected) //��ʶ��
      {
        m_HoldStartTime = Time.timeSinceLevelLoad;
      }
      else if (m_WasDetected && !detected)  //���񵽹����ͷ� 
      {
        m_PerformedTriggered = false;
        m_GestureEnded?.Invoke(); //�������ƶ�ʧ
      }

      m_WasDetected = detected; //����⵽

      if (!m_PerformedTriggered && detected) //��⵽û�д������Ҵﵽ����
      {
        var holdTimer = Time.timeSinceLevelLoad - m_HoldStartTime; //ͣ��ʱ��
        if (holdTimer > m_MinimumHoldTime) //ͣ��ʱ�������Сʱ��
        {
          m_GesturePerformed?.Invoke(); //��������ʶ��
          m_PerformedTriggered = true;
        }
      }

      m_TimeOfLastConditionCheck = Time.timeSinceLevelLoad; // ���һ��״̬���ʱ��
    }
  }
}
