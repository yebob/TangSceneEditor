/**
 * Created by emacs
 * Date: 2013/10/10
 * Author: zzc
 */

using UnityEngine;
using Tang;

namespace TangScene
{
  public class Selectable : MonoBehaviour
  {

    private SpriteAnimate spriteAnimate;

    void Start()
    {
      spriteAnimate = GetComponent<SpriteAnimate>();
      if( spriteAnimate != null )
	spriteAnimate.lateSpriteReadyHandler += LateSpriteReady;

    }
    
    public void LateSpriteReady(TASprite sprite)
    {

      if( sprite.GetComponent<MeshCollider>() == null )
	{
	  MeshFilter mf = sprite.GetComponent<MeshFilter>();
	  if( mf != null )
	    {
	      MeshCollider mc = sprite.gameObject.AddComponent<MeshCollider>();
#if UNITY_EDITOR
	      mc.sharedMesh = mf.sharedMesh;
#else
	      mc.sharedMesh = mf.mesh;
#endif
	      mc.isTrigger = true;
	    }
	}
      
    }
  }
}