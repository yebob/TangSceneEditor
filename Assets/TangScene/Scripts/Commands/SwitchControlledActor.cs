/*
 * Switch controlled actor
 *
 * Date: 2013/10/10
 * Author: zzc
 */
using PureMVC.Interfaces;
using PureMVC.Patterns;
using UnityEngine;

namespace TangScene
{
	public class SwitchControlledActorCmd : SimpleCommand
	{

		public const string NAME = "TS_SWITCH_CONTROLLED_ACTOR";

		public override void Execute (INotification notification)
		{

			long actorId = (long)notification.Body;

			if (actorId != 0) {
				if (Cache.actors.ContainsKey (actorId)) {
					GameObject ncGobj = Cache.actors [actorId];
					ControlledBhvr newControlledBhvr = ncGobj.GetComponent<ControlledBhvr> ();
					if (newControlledBhvr != null) {
		  
						// make current controlled actor ControlledBhvr disable
						if (Cache.controlledActorId != 0 
		      && Cache.actors.ContainsKey (Cache.controlledActorId)) {
							ControlledBhvr currentControlledBhvr = Cache.actors [Cache.controlledActorId].GetComponent<ControlledBhvr> (); 
							if (currentControlledBhvr != null)
								currentControlledBhvr.enabled = false;
						}

						// make new controlled actor ControlledBhvr enable
						newControlledBhvr.enabled = true;

						Cache.controlledActorId = actorId;
		  
					} else {
						ncGobj.AddComponent<ControlledBhvr> ();
					}

	      
				}
			}
		}
	}
}