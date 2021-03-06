﻿using Microsoft.Xna.Framework;
using MonoGame.Behaviour.GoalBasedBehaviour;
using MonoGame.DecisionMaking.AtomicGoals;
using MonoGame.Entity;
using MonoGame.Entity.StaticEntities;
using MonoGame.GoalBehaviour;
using System.Collections.Generic;
using System.Linq;

namespace MonoGame.DecisionMaking.CompositeGoals
{
    class DealWithHungerGoal : CompositeGoal
    {
        private readonly EntityManager EM;

        private Vector2 BushAsTarget;

        public DealWithHungerGoal(AwareEntity me, EntityManager em) : base(me)
        {
            EM = em;

            GoalStatus = GoalStatus.Inactive;
        }

        public override void Activate()
        {
            GoalStatus = GoalStatus.Active;

            List<Bush> bushes = new List<Bush>();

            // Add bushes to the list
            foreach (StaticGameEntity entity in EM.GetStaticEntities())
            {
                if (entity.GetType() == typeof(Bush))
                    bushes.Add((Bush)entity);
            }

            // Calculate, which bush is nearer to the entity
            if (Vector2.Subtract(ME.Pos, bushes[0].Pos).Length() < Vector2.Subtract(ME.Pos, bushes[1].Pos).Length())
                BushAsTarget = new Vector2(bushes[0].Pos.X + 40, bushes[0].Pos.Y + 50);
            else
                BushAsTarget = new Vector2(bushes[1].Pos.X + 50, bushes[1].Pos.Y + 50);
        }

        public override GoalStatus Process()
        {
            if (GoalStatus == GoalStatus.Inactive)
                Activate();

            // Make sure that eating subgoal is terminated first and terminate, when hunger is good
            if (ME.Hunger >= 3f  && SubGoals[0].GetType() == typeof(EatGoal) && SubGoals[0].GoalStatus == GoalStatus.Completed)
                Terminate();

            if (GoalStatus == GoalStatus.Completed || GoalStatus == GoalStatus.Failed)
                return GoalStatus;

            SubGoals.RemoveAll(sg => sg.GoalStatus == GoalStatus.Completed);

            // Get a path to traverse to the bush
            if (!SubGoals.OfType<FollowPathGoal>().Any())
            {
                FollowPathGoal comp = new FollowPathGoal(ME, BushAsTarget);
                AddSubGoal(comp);
            }
            
            SubGoals.ForEach(sg => sg.Process());

            // Once path is traversed, eat
            if (Vector2.Subtract(BushAsTarget, ME.Pos).Length() < 10 && !SubGoals.OfType<EatGoal>().Any())
            {

                SubGoals.RemoveAll(sg => sg.GoalStatus == GoalStatus.Completed);
                AddSubGoal(new EatGoal(ME));
            }

            SubGoals.ForEach(sg => sg.Process());

            return GoalStatus;
        }

        public override void Terminate()
        {
            GoalStatus = GoalStatus.Completed;
        }

        public override string ToString()
        {
            string valueAsString = ME.Hunger.ToString("0.00");
            return "\nDealing with hunger: " + valueAsString;
        }
    }
}
