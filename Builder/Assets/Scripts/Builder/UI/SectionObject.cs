using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SectionObject : MonoBehaviour
{
    public Text title;
    public List<Thumbnail> thumbnails;
    public RectTransform thumbnailsParent;
    public RectTransform titleTransform;
    public GridLayoutGroup thumbnailGrid;
}
