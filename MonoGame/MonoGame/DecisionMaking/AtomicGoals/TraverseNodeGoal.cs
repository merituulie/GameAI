﻿using Microsoft.Xna.Framework;
using MonoGame.DecisionMaking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGame.GoalBehaviour.GoalBehaviours
{
    class TraverseNodeGoal : Goal
    {
        public GoalStatus GoalStatus { get; set; }

        private MovingEntity ME;
        private Vector2 Target;

        private int NodeCount;

        public TraverseNodeGoal(MovingEntity me, Vector2 target, int nodeCount) 
        {
            ME = me;
            Target = target;
            NodeCount = nodeCount;
        }

        public void Activate()
        {
            GoalStatus = GoalStatus.Active;
        }

        public GoalStatus Process()
        {

            if (GoalStatus == GoalStatus.Inactive)
                Activate();

            if (GoalStatus == GoalStatus.Completed || GoalStatus == GoalStatus.Failed)
                return GoalStatus;

            if (Vector2.Subtract(Target, ME.Pos).Length() < 10)
            {
                Terminate();
            }

            ME.SB = new ArriveBehaviour(ME, Target, SteeringBehaviour.Deceleration.Slow);

            return GoalStatus;
        }

        public void Terminate()
        {
            GoalStatus = GoalStatus.Completed;
        }

        public override string ToString()
        {
            return "Traverse node: " + Target;
        }
    }
}