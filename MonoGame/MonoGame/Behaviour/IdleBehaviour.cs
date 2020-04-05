﻿using Microsoft.Xna.Framework;

namespace MonoGame.Behaviour
{
    class IdleBehaviour : SteeringBehaviour
    {
        private Vector2 desiredVelocity;
        public IdleBehaviour(MovingEntity me) : base(me)
        {
        }
        public override Vector2 Calculate()
        {
                return new Vector2(0, 0);
        }
    }
}
