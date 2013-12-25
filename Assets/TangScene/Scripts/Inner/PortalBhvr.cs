using UnityEngine;
using System.Collections;

namespace TangScene
{
  
  [ExecuteInEditMode]
  public class PortalBhvr : MonoBehaviour {

    public const float height = 0;

    public int baseId;
    public SceneGrid destination;

    // Use this for initialization
    void Start () {

      Vector3 pos = transform.localPosition;
      transform.localPosition = new Vector3(pos.x, height, pos.z);

    }
  }
}
