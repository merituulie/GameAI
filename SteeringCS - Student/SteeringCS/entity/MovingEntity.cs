﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteeringCS.entity
{

    abstract class MovingEntity : BaseGameEntity
    {
        public Vector2D Velocity { get; set; }
        public Vector2D Heading { get; set; }
        public Vector2D Side { get; set; }
        public float Mass { get; set; }
        public float MaxSpeed { get; set; }

        public SteeringBehaviour SB { get; set; }

        public MovingEntity(Vector2D pos, World w) : base(pos, w)
        {
            Mass = 30;
            MaxSpeed = 150;
            Velocity = new Vector2D();
        }

        public override void Update(float timeElapsed)
        {
            Vector2D SteeringForce = SB.Calculate();

            Vector2D acceleration = SteeringForce.divide(Mass);

            Velocity = Velocity.Add(acceleration.Multiply(timeElapsed));

            Velocity.truncate(MaxSpeed);

            Pos = Pos.Add(Velocity.Multiply(timeElapsed));
            Pos = MyWorld.WrapAround(Pos);
            
            if(Velocity.LengthSquared() > 0.0000001)
            {
                Vector2D heading = Velocity.Clone();
                Heading = heading.Normalize();
                Side = Heading.Perp();
            }

            Console.WriteLine(ToString());
        }

        public override string ToString()
        {
            return String.Format("{0}", Velocity);
        }
    }
}
