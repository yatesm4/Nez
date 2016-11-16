﻿using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using Microsoft.Xna.Framework;


namespace Nez.Farseer
{
	public class FSWorld : Component, IUpdatable
	{
		public World world;

		/// <summary>
		/// minimum delta time step for the simulation. The min of Time.deltaTime and this will be used for the physics step
		/// </summary>
		public float minimumUpdateDeltaTime = 1f / 30;

		/// <summary>
		/// if true, the left mouse button will be used for picking and dragging physics objects around
		/// </summary>
		public bool enableMousePicking = true;

		FixedMouseJoint _mouseJoint;


		public FSWorld() : this( new Vector2( 0, 9.82f ) )
		{}


		public FSWorld( Vector2 gravity )
		{
			world = new World( gravity );
		}


		public override void onAddedToEntity()
		{
			// we want our World to tick first, before any other Entities/Components
			entity.setUpdateOrder( int.MinValue );
			setUpdateOrder( int.MinValue );
		}


		public override void onRemovedFromEntity()
		{
			world.clear();
			world = null;
		}


		void IUpdatable.update()
		{
			if( enableMousePicking )
			{
				if( Input.leftMouseButtonPressed )
				{
					var pos = entity.scene.camera.screenToWorldPoint( Input.mousePosition );
					var fixture = world.testPoint( FSConvert.displayToSim * pos );
					if( fixture != null && !fixture.body.isStatic && !fixture.body.isKinematic )
						_mouseJoint = fixture.body.createFixedMouseJoint( pos );
				}

				if( Input.leftMouseButtonDown && _mouseJoint != null )
				{
					var pos = entity.scene.camera.screenToWorldPoint( Input.mousePosition );
					_mouseJoint.worldAnchorB = FSConvert.toSimUnits( pos );
				}

				if( Input.leftMouseButtonReleased && _mouseJoint != null )
				{
					world.removeJoint( _mouseJoint );
					_mouseJoint = null;
				}
			}

			world.step( MathHelper.Min( Time.deltaTime, minimumUpdateDeltaTime ) );
		}


		public static implicit operator World( FSWorld self )
		{
			return self.world;
		}

	}
}
