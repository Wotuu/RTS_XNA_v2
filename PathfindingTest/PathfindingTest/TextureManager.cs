using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using PathfindingTest.Units;
using PathfindingTest.Buildings;
using Microsoft.Xna.Framework.Content;
using PathfindingTest.UI;

namespace PathfindingTest
{
    public class TextureManager
    {
        public Texture2D resourceTex { get; set; }
        public Texture2D barracksTex { get; set; }
        public Texture2D factoryTex { get; set; }
        public Texture2D fortressTex { get; set; }
        public Texture2D sentryTex { get; set; }

        public Texture2D engineerTex { get; set; }
        public Texture2D swordsmanTex { get; set; }
        public Texture2D bowmanTex { get; set; }
        public Texture2D horsemanTex { get; set; }

        public Texture2D swordsmanHitTex { get; set; }

        public Texture2D hudMoveTex { get; set; }
        public Texture2D hudAttackTex { get; set; }
        public Texture2D hudDefendTex { get; set; }
        public Texture2D hudStopTex { get; set; }
        public Texture2D hudRepairTex { get; set; }

        public Texture2D hudResourceTex { get; set; }
        public Texture2D hudBarracksTex { get; set; }
        public Texture2D hudFactoryTex { get; set; }
        public Texture2D hudFortressTex { get; set; }
        public Texture2D hudSentryTex { get; set; }
        public Texture2D hudEngineerTex { get; set; }
        public Texture2D hudSwordsmanTex { get; set; }
        public Texture2D hudBowmanTex { get; set; }
        public Texture2D hudHorsemanTex { get; set; }

        public Texture2D arrowTex { get; set; }

        public Texture2D solidTex { get; set; }

        public static TextureManager instance { get; set; }

        public static TextureManager GetInstance()
        {
            if (instance == null)
            {
                instance = new TextureManager();
            }
            return instance;
        }

        public TextureManager()
        {
            ContentManager content = Game1.GetInstance().Content;

            resourceTex = content.Load<Texture2D>("Buildings/Resources");
            barracksTex = content.Load<Texture2D>("Buildings/Barracks");
            factoryTex = content.Load<Texture2D>("Buildings/Factory");
            fortressTex = content.Load<Texture2D>("Buildings/Fortress");
            sentryTex = content.Load<Texture2D>("Buildings/Sentry");

            engineerTex = content.Load<Texture2D>("Units/Engineer");
            swordsmanTex = content.Load<Texture2D>("Units/melee");
            bowmanTex = content.Load<Texture2D>("Units/bowman");
            horsemanTex = content.Load<Texture2D>("Units/horseman");

            swordsmanHitTex = content.Load<Texture2D>("Units/meleeHit");

            hudMoveTex = content.Load<Texture2D>("HUD/Commands/HUDMove");
            hudAttackTex = content.Load<Texture2D>("HUD/Commands/HUDAttack");
            hudDefendTex = content.Load<Texture2D>("HUD/Commands/HUDDefend");
            hudStopTex = content.Load<Texture2D>("HUD/Commands/HUDStop");
            hudRepairTex = content.Load<Texture2D>("HUD/Commands/HUDRepair");

            hudResourceTex = content.Load<Texture2D>("HUD/HUDResources");
            hudBarracksTex = content.Load<Texture2D>("HUD/HUDBarracks");
            hudFactoryTex = content.Load<Texture2D>("HUD/HUDFactory");
            hudFortressTex = content.Load<Texture2D>("HUD/HUDFortress");
            hudSentryTex = content.Load<Texture2D>("HUD/HUDSentry");
            hudEngineerTex = content.Load<Texture2D>("HUD/HUDEngineer");
            hudSwordsmanTex = content.Load<Texture2D>("HUD/HUDMelee");
            hudBowmanTex = content.Load<Texture2D>("HUD/HUDRanged");
            hudHorsemanTex = content.Load<Texture2D>("HUD/HUDHorseman");

            arrowTex = content.Load<Texture2D>("Units/Projectiles/wooden_arrow_scale");

            solidTex = content.Load<Texture2D>("Misc/solid");
        }

        public Texture2D GetTexture(Unit.Type type)
        {
            switch (type)
            {
                case Unit.Type.Engineer:
                    return engineerTex;

                case Unit.Type.Melee:
                    return swordsmanTex;

                case Unit.Type.Ranged:
                    return bowmanTex;

                case Unit.Type.Fast:
                    return horsemanTex;

                default:
                    return null;
            }
        }

        public Texture2D GetHitTexture(Unit.Type type)
        {
            switch (type)
            {
                case Unit.Type.Melee:
                    return swordsmanHitTex;

                default:
                    return null;
            }
        }

        public Texture2D GetTexture(Building.Type type)
        {
            switch (type)
            {
                case Building.Type.Barracks:
                    return barracksTex;

                case Building.Type.Factory:
                    return factoryTex;

                case Building.Type.Fortress:
                    return fortressTex;

                case Building.Type.Resources:
                    return resourceTex;

                case Building.Type.Sentry:
                    return sentryTex;

                default:
                    return null;
            }
        }

        public Texture2D GetTexture(HUDObject.Type type)
        {
            switch (type)
            {
                case HUDObject.Type.Barracks:
                    return hudBarracksTex;

                case HUDObject.Type.Engineer:
                    return hudEngineerTex;

                case HUDObject.Type.Factory:
                    return hudFactoryTex;

                case HUDObject.Type.Fast:
                    return hudHorsemanTex;

                case HUDObject.Type.Fortress:
                    return hudFortressTex;

                case HUDObject.Type.Melee:
                    return hudSwordsmanTex;

                case HUDObject.Type.Ranged:
                    return hudBowmanTex;

                case HUDObject.Type.Resources:
                    return hudResourceTex;

                case HUDObject.Type.Sentry:
                    return hudSentryTex;

                default:
                    return null;
            }
        }

        public Texture2D GetTexture(HUDCommandObject.Type type)
        {
            switch (type)
            {
                case HUDCommandObject.Type.Attack:
                    return hudAttackTex;

                case HUDCommandObject.Type.Defend:
                    return hudDefendTex;

                case HUDCommandObject.Type.Move:
                    return hudMoveTex;

                case HUDCommandObject.Type.Repair:
                    return hudRepairTex;

                case HUDCommandObject.Type.Stop:
                    return hudStopTex;

                default:
                    return null;
            }
        }

        public Texture2D GetArrowTexture()
        {
            return arrowTex;
        }

        public Texture2D GetSolidTexture()
        {
            return solidTex;
        }
    }
}
