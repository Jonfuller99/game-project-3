using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.Xna.Framework;
using myRandom;



namespace Particles
{
    public class RainParticleSystem: ParticleSystem
    {
        public Rectangle Source {get; set;}
        public bool IsRaining{get; set;} = true;

        

        public RainParticleSystem(Game game) : base(game, 4000)
        {
            
        }

        protected override void InitializeConstants()
        {
            textureFilename = "RainDrop";
            minNumParticles = 10;
            maxNumParticles = 20;
        }

        protected override void InitializeParticle(ref Particle p, Vector2 where)
        {
            p.Initialize(where, Vector2.UnitY * 260, Vector2.Zero, Color.LightSkyBlue, scale: RandomHelper.NextFloat(0.1f, 0.4f), lifetime: 3);
        }


        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (IsRaining) AddParticles(Source);
        }
    }
}