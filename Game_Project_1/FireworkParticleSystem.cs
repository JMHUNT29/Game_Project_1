using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game_Project_1
{
    public class FireworkParticleSystem : ParticleSystem
    {
        Color[] colors = new Color[]
        {
            Color.LightYellow,
            Color.White,
            Color.Aqua,
            Color.LightCoral,
            Color.Turquoise
        };

        Color color;

        public FireworkParticleSystem(Game game, int maxExplosions) : base(game, maxExplosions * 35) { }

        protected override void InitializeConstants()
        {
            textureFilename = "circle";

            minNumParticles = 25;
            maxNumParticles = 35;

            blendState = BlendState.Additive;
            DrawOrder = AdditiveBlendDrawOrder;
        }

        protected override void InitializeParticle(ref Particle p, Vector2 where)
        {
            var velocity = RandomHelper.NextDirection() * RandomHelper.NextFloat(40, 200);

            var lifetime = RandomHelper.NextFloat(0.5f, 1.0f);

            var acceleration = -velocity / lifetime;

            var rotation = RandomHelper.NextFloat(0, MathHelper.TwoPi);

            var angularvelocity = RandomHelper.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4);

            var scale = RandomHelper.NextFloat(4, 6);

            p.Initialize(where, velocity, acceleration, color, lifetime: lifetime, rotation: rotation, angularVelocity: angularvelocity, scale : scale);
        }

        protected override void UpdateParticle(ref Particle particle, float dt)
        {
            base.UpdateParticle(ref particle, dt);

            float normalizedLifetime = particle.TimeSinceStart / particle.Lifetime;

            particle.Scale = .1f + .25f * normalizedLifetime;
        }

        public void PlaceFirework(Vector2 where, bool isRed)
        {
            if (!isRed) color = colors[RandomHelper.Next(colors.Length)];
            else color = Color.Red;
            AddParticles(where);
        }

    }
}
