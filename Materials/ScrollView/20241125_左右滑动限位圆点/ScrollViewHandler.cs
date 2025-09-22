using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollViewHandler : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
  private ScrollRect sr;

  [SerializeField] private int srContentCount;
  private float unitPosition;

  private const float ANIMTIME = .5f;

  [SerializeField] private int currentIndex = 0;

  // ==================================================

  private void Start() => Init();

  // private void Update()
  // {
  //   if (Input.GetKeyDown(KeyCode.Q))
  //   {
  //     sr.DOHorizontalNormalizedPos(unitPosition, ANIMTIME);
  //   }
  //   if (Input.GetKeyDown(KeyCode.W))
  //   {
  //     sr.DOHorizontalNormalizedPos(unitPosition, ANIMTIME);
  //   }
  // }

  // ==================================================

  private void Init()
  {
    sr = GetComponent<ScrollRect>();
    srContentCount = transform.GetChild(0).GetChild(0).childCount;
    unitPosition = 1f / (srContentCount - 1);
  }

  // ==================================================
  #region 手势检测

  private Vector2 lastPos; // 鼠标上次位置
  private Vector2 currPos; // 鼠标当前位置
  private Vector2 offset; // 两次位置的偏移值

  public void OnBeginDrag(PointerEventData eventData) => BeginDrag();
  public void OnEndDrag(PointerEventData eventData) => EndDrag();

  private void BeginDrag() => lastPos = Input.mousePosition;
  private void EndDrag()
  {
    currPos = Input.mousePosition;
    offset = currPos - lastPos;

    if (Mathf.Abs(offset.x) > Mathf.Abs(offset.y)) // 水平移动
    {
      if (offset.x > 0)
      {
        // Debug.Log("向右");
        currentIndex--;
        Jump2Position(currentIndex);
      }
      else
      {
        // Debug.Log("向左");
        currentIndex++;
        Jump2Position(currentIndex);
      }
    }
    // else // 垂直移动
    // {
    //   if (offset.y > 0)
    //   {
    //     Debug.Log("向上");
    //   }
    //   else
    //   {
    //     Debug.Log("向下");
    //   }
    // }
  }

  #endregion
  // ==================================================
  #region 跳转控制

  public void Jump2Position(int index)
  {
    if (index <= 0)
    {
      currentIndex = 0;
      sr.DOHorizontalNormalizedPos(0, ANIMTIME);
      ControllDot(0);
      return;
    }
    if (index >= 6)
    {
      currentIndex = 6;
      sr.DOHorizontalNormalizedPos(1, ANIMTIME);
      ControllDot(6);
      return;
    }

    currentIndex = index;
    sr.DOHorizontalNormalizedPos(unitPosition * index, ANIMTIME);
    ControllDot(index);
  }

  #endregion
  // ==================================================
  #region dot控制

  [SerializeField] private List<GameObject> dots;

  public void ControllDot(int value)
  {
    for (int i = 0; i < dots.Count; i++)
    {
      if (i == value)
      {
        dots[i].transform.DOScale(new Vector3(1.5f, 1.5f, 1.5f), ANIMTIME);
        dots[i].GetComponent<Image>().DOColor(Color.white, ANIMTIME);
        continue;
      }
      dots[i].transform.DOScale(Vector3.one, 0.5f);
      dots[i].GetComponent<Image>().DOColor(Color.gray, ANIMTIME);
    }
  }

  #endregion
}