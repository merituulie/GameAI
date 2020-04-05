﻿using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using System.Text;
using Microsoft.Xna.Framework;
using MonoGame.Entity.StaticEntities;

namespace MonoGame.Entity
{
    public class EntityManager
    {
        private List<StaticGameEntity> staticEntities = new List<StaticGameEntity>();
        private List<MovingEntity> movingEntities = new List<MovingEntity>();

        public Texture2D palmtreeTexture;
        public Texture2D survivorTexture;
        public Texture2D tentTexture;
        public Texture2D bushTexture;
        public Texture2D seagullTexture;

        public SpriteFont fontTexture;

        public EntityManager()
        {
            staticEntities.Add(new Palmtree(new Vector2(730, 290), this, 50, 50));
            staticEntities.Add(new Palmtree(new Vector2(710, 310), this, 50, 50));
            staticEntities.Add(new Palmtree(new Vector2(650, 220), this, 50, 50));
            staticEntities.Add(new Tent(new Vector2(480, 380), this, 50, 50));
            staticEntities.Add(new Bush(new Vector2(260, 540), this, 50, 50));
            staticEntities.Add(new Bush(new Vector2(380, 150), this, 50, 50));

            Survivor survivor = new Survivor(new Vector2(500, 500), this);
            movingEntities.Add(survivor);

            Seagull seagull1 = new Seagull(new Vector2(100, 250), this);
            Seagull seagull2 = new Seagull(new Vector2(230, 230), this);
            Seagull seagull3 = new Seagull(new Vector2(140, 300), this);
            Seagull seagull4 = new Seagull(new Vector2(100, 100), this);
            Seagull seagull5 = new Seagull(new Vector2(150, 150), this);

            movingEntities.Add(seagull1);
            movingEntities.Add(seagull2);
            movingEntities.Add(seagull3);
            movingEntities.Add(seagull4);
            movingEntities.Add(seagull5);

        }

        public void LoadContent(ContentManager Content)
        {
            palmtreeTexture = Content.Load<Texture2D>("palmtree");
            tentTexture = Content.Load<Texture2D>("Tent");
            bushTexture = Content.Load<Texture2D>("bush");
                       
            survivorTexture = Content.Load<Texture2D>("Player");
            seagullTexture = Content.Load<Texture2D>("Seagull");

            fontTexture = Content.Load<SpriteFont>("CharacterInfo");
        }

        public void Update(GameTime gt)
        {
            movingEntities.ForEach(m => m.Update((float)gt.ElapsedGameTime.TotalSeconds * 0.8F));
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Begin();
            staticEntities.ForEach(s => s.Draw(sb));
            movingEntities.ForEach(m => m.Draw(sb));
            sb.DrawString(fontTexture, this.ToString(), new Vector2(50, 900), Color.Black);
            sb.End();
        }

        internal List<MovingEntity> GetMovingEntities() => movingEntities;

        public void TagNeighbours(MovingEntity centralEntity, double radius)
        {
            if (centralEntity.GetType() == typeof(Survivor))
                 return;

            foreach (MovingEntity entity in movingEntities)
            {
                // Clear current tag.
                entity.Tag = false;

                // Calculate the difference in space
                Vector2 difference = Vector2.Subtract(entity.Pos, centralEntity.Pos);

                // When the entity is in range it gets tagged.
                if (entity != centralEntity && difference.LengthSquared() < radius * radius && entity.GetType() != typeof(Survivor))
                    entity.Tag = true;
            }
        }
        public void EnforceNonPenetrationConstraint(MovingEntity centralEntity)
        {
            if (centralEntity.GetType() == typeof(Survivor))
                return;

            foreach (MovingEntity entity in movingEntities)
            {
                if (entity.GetType() == typeof(Survivor))
                    continue;

                    //make sure we don't check against the individual
                if (entity == centralEntity) 
                    continue;

                    // calculate the distance between the positions of the entities
                Vector2 ToEntity = Vector2.Subtract(centralEntity.Pos, entity.Pos);

                float distFromEachOther = ToEntity.Length();

                //if this distance is smaller than the sum of their radii then this entity must be moved away in the direction parallel to the ToEntity vector
                float amountOfOverlap = 10 + 10 - distFromEachOther;

                    //move the entity a distance away equivalent to the amount of overlap
                if (amountOfOverlap >= 0)
                    centralEntity.Pos += Vector2.Multiply(Vector2.Divide(ToEntity, distFromEachOther), amountOfOverlap);
            }
        }

        public List<StaticGameEntity> GetStaticEntities()
        {
            return staticEntities;
        }

        public Survivor GetSurvivor()
        {
            return (Survivor)movingEntities[0];
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("Show/Hide Graph: G / g");
            stringBuilder.AppendLine("Show/Hide Characterinfo: I / i");
            stringBuilder.AppendLine("Create a new target: MouseClick");

            return stringBuilder.ToString();
        }
    }
}
