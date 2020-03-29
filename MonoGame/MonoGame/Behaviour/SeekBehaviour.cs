﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGame
{
    class SeekBehaviour : SteeringBehaviour
    {
        private Vector2 Target;
        public SeekBehaviour(MovingEntity me, Vector2 target) : base(me)
        {
            Target = target;
        }
        public override Vector2 Calculate()
        {
            Vector2 targetpos = Target;

            if (targetpos == null)
                return new Vector2(0, 0);

            Vector2 desiredVelocity = Vector2.Subtract(targetpos, ME.Pos);
            desiredVelocity.Normalize();
            desiredVelocity = Vector2.Multiply(desiredVelocity, ME.MaxSpeed);
            return desiredVelocity = Vector2.Subtract(desiredVelocity, ME.Velocity);
        }
    }
}
