﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGame
{
    public abstract class SteeringBehaviour
    {
        public MovingEntity ME { get; set; }
        public abstract Vector2 Calculate();

        public enum Decelaration { Slow = 3, Normal = 2, Fast = 1 }

        public SteeringBehaviour(MovingEntity me)
        {
            ME = me;
        }
    }
}
