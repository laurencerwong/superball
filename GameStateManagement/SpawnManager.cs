using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Xml.Linq;
using System.Xml;


namespace GameStateManagement
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class SpawnManager : Microsoft.Xna.Framework.GameComponent
    {
        public static string startingLevelName = "Level1.xml";
        private List<XElement> levels = new List<XElement>();
        private int currentLevel = 0;
        public int gCurrentLevel
        {
            get
            {
                return currentLevel;
            }
        }
        private int allowedHammers = 0;
        public int gAllowedHammers
        {
            get
            {
                return allowedHammers;
            }
            set
            {
                allowedHammers = value;
            }
        }
        private int allowedGravityBalls = 0;
        public int gAllowedGravityBalls
        {
            get
            {
                return allowedGravityBalls;
            }
            set
            {
                allowedGravityBalls = value;
            }
        }
        private int allowedRamps = 0;
        public int gAllowedRamps
        {
            get
            {
                return allowedRamps;
            }
            set
            {
                allowedRamps = value;
            }
        }
        private int allowedSprings = 0;
        public int gAllowedSprings
        {
            get
            {
                return allowedSprings;
            }
            set
            {
                allowedSprings = value;
            }
        }
        private int allowedPlatforms = 0;
        public int gAllowedPlatforms
        {
            get
            {
                return allowedPlatforms;
            }
            set
            {
                allowedPlatforms = value;
            }
        }

        private List<Hammer> hammers = new List<Hammer>();
        public List<Hammer> getHammers
        {
            get
            {
                return hammers;
            }
        }
        private List<Platform> platforms = new List<Platform>();
        public List<Platform> getPlatforms
        {
            get
            {
                return platforms;
            }
        }
        private List<GravityBall> gravityBalls = new List<GravityBall>();
        public List<GravityBall> getGravityBalls
        {
            get
            {
                return gravityBalls;
            }
        }
        private List<Ramp> ramps = new List<Ramp>();
        public List<Ramp> getRamps
        {
            get
            {
                return ramps;
            }
        }
        private List<Spring> springs = new List<Spring>();
        public List<Spring> getSprings
        {
            get
            {
                return springs;
            }
        }
        private Door targetDoor;
        public Door getTargetDoor
        {
            get
            {
                return targetDoor;
            }
        }

        private Game game;

        Random random = new Random();

        Utils.Timer timer = new Utils.Timer();

        class coordinateXYZ
        {
            public float X { get; set; }
            public float Y { get; set; }
            public float Z { get; set; }
            public float Q_X { get; set; }
            public float Q_Y { get; set; }
            public float Q_Z { get; set; }
            public float Q_W { get; set; }
            public coordinateXYZ(float x, float y, float z)
            {
                X = x - 512.0f;
                Y = 384.0f - y;
                Z = z;
            }
            public coordinateXYZ(float x, float y, float z, float q_x, float q_y, float q_z, float q_w)
            {
                X = x - 512.0f;
                Y = 384.0f - y;
                Z = z;
                Q_X = q_x;
                Q_Y = q_y;
                Q_Z = q_z;
                Q_W = q_w;
            }
        }

        public SpawnManager(Game game)
            : base(game)
        {
            this.game = game;
            currentLevel = GameplayScreen.spawnManagerStartingLevelNumber;
            startingLevelName = GameplayScreen.spawnManagerStartingLevelName;
            if(startingLevelName.Equals("LEVEL_EDITOR")){
                allowedGravityBalls = 10000;
                allowedHammers = 10000;
                allowedPlatforms = 10000;
                allowedRamps = 10000;
                allowedSprings = 10000;
                targetDoor = new Door(game);
                game.Components.Add(targetDoor);
                targetDoor.setCoordinates(new Vector3(0, 0, 0));
                targetDoor.isUserMovable = true;
                PlayerSpawn ball = new PlayerSpawn(game);
                game.Components.Add(ball);
                Vector3 initBallPos = new Vector3(-50, 0, 0);
                //GameplayScreen.playerSpawnCoordinates = initBallPos;
                ball.setCoordinates(initBallPos);
                ball.isUserMovable = true;
                return;
            }
            XDocument doc = XDocument.Load(startingLevelName);
            XElement XnaObject = doc.Element("XnaContent");
            IEnumerable<XElement> level = XnaObject.Elements("Level");
            levels = level.ToList<XElement>();  
            if (currentLevel >= levels.Count)
            {
                GameplayScreen.spawnManagerStartingLevelNumber = 0;
                currentLevel = 0;
            }
            XElement firstLevel = levels.ElementAt(currentLevel);
            parseLevel(firstLevel);
        }

        public bool nextLevel()
        {
            currentLevel++;
            if (currentLevel >= levels.Count)
            {
                return false;
            }
            game.Components.Remove(targetDoor);
            foreach (Hammer ham in hammers)
            {
                game.Components.Remove(ham);
            }
            hammers = new List<Hammer>();
            foreach (Platform plat in platforms)
            {
                game.Components.Remove(plat);
            }
            platforms = new List<Platform>();
            foreach (Spring spr in springs)
            {
                game.Components.Remove(spr);
            }
            springs = new List<Spring>();
            foreach (GravityBall gra in gravityBalls)
            {
                game.Components.Remove(gra);
            }
            gravityBalls = new List<GravityBall>();
            foreach (Ramp rmp in ramps)
            {
                game.Components.Remove(rmp);
            }
            ramps = new List<Ramp>();
            foreach (GameComponent gc in game.Components)
            {
                if (gc is Bloom)
                {
                    ((Bloom)gc).ClearMotionBlurBuffer();
                    break;
                }
            }
            XElement level = levels.ElementAt(currentLevel);
            allowedPlatforms = 0;
            allowedHammers = 0;
            allowedRamps = 0;
            allowedSprings = 0;
            allowedGravityBalls = 0;
            parseLevel(level);
            return true;
        }

        private void parseLevel(XElement level)
        {
            XElement levelObject = level.Element("LevelObject");
            List<coordinateXYZ> levelObjects = new List<coordinateXYZ>();
            XElement player = levelObject.Element("Player");
            coordinateXYZ playerLocation = new coordinateXYZ(float.Parse(player.Attribute("X").Value), float.Parse(player.Attribute("Y").Value), float.Parse(player.Attribute("Z").Value));
            GameplayScreen.playerSpawnCoordinates.X = playerLocation.X; GameplayScreen.playerSpawnCoordinates.Y = playerLocation.Y; GameplayScreen.playerSpawnCoordinates.Z = playerLocation.Z;

            foreach (Spring s in springs)
            {
                s.Despawn();
            }
            springs.Clear();
            foreach (Hammer h in hammers)
            {
                h.Despawn();
            }
            hammers.Clear();
            foreach (Platform p in platforms)
            {
                p.Despawn();
            }
            platforms.Clear();
            foreach (GravityBall g in gravityBalls)
            {
                g.Despawn();
            }
            gravityBalls.Clear();
            foreach (Ramp r in ramps)
            {
                r.Despawn();
            }
            ramps.Clear();
            foreach (XElement el in levelObject.Elements("Floor"))
            {
                var rotX = (string)el.Attribute("Rotation_X") ?? "0";
                var rotY = (string)el.Attribute("Rotation_Y") ?? "0";
                var rotZ = (string)el.Attribute("Rotation_Z") ?? "0";
                var rotW = (string)el.Attribute("Rotation_W") ?? "0";
                levelObjects.Add(new coordinateXYZ(float.Parse(el.Attribute("X").Value), float.Parse(el.Attribute("Y").Value), float.Parse(el.Attribute("Z").Value), float.Parse(rotX),
                    float.Parse(rotY), float.Parse(rotZ), float.Parse(rotW)));
            }
            foreach (coordinateXYZ co in levelObjects)
            {
                Platform newPlatform = new Platform(game);
                game.Components.Add(newPlatform);
                newPlatform.setCoordinates(new Vector3(co.X, co.Y, co.Z));
                newPlatform.m_qRotation = new Quaternion(co.Q_X, co.Q_Y, co.Q_Z, co.Q_W);
                platforms.Add(newPlatform);
            }
            foreach(XElement el in levelObject.Elements("Spring")){
                var rotX = (string)el.Attribute("Rotation_X") ?? "0";
                var rotY = (string)el.Attribute("Rotation_Y") ?? "0";
                var rotZ = (string)el.Attribute("Rotation_Z") ?? "0";
                var rotW = (string)el.Attribute("Rotation_W") ?? "0";
                Spring newSpring = new Spring(game);
                float.Parse(rotX);
                coordinateXYZ sprloc = new coordinateXYZ(float.Parse(el.Attribute("X").Value), float.Parse(el.Attribute("Y").Value), float.Parse(el.Attribute("Z").Value), float.Parse(rotX),
                    float.Parse(rotY), float.Parse(rotZ), float.Parse(rotW));
                game.Components.Add(newSpring);
                newSpring.setCoordinates(new Vector3(sprloc.X, sprloc.Y, sprloc.Z));
                newSpring.m_qRotation = new Quaternion(sprloc.Q_X, sprloc.Q_Y, sprloc.Q_Z, sprloc.Q_W);
                springs.Add(newSpring);
            }
            foreach(XElement el in levelObject.Elements("Hammer")){
                var rotX = (string)el.Attribute("Rotation_X") ?? "0";
                var rotY = (string)el.Attribute("Rotation_Y") ?? "0";
                var rotZ = (string)el.Attribute("Rotation_Z") ?? "0";
                var rotW = (string)el.Attribute("Rotation_W") ?? "0";
                Hammer newHammer = new Hammer(game);
                coordinateXYZ hamloc = new coordinateXYZ(float.Parse(el.Attribute("X").Value), float.Parse(el.Attribute("Y").Value), float.Parse(el.Attribute("Z").Value), float.Parse(rotX),
                    float.Parse(rotY), float.Parse(rotZ), float.Parse(rotW));
                game.Components.Add(newHammer);
                newHammer.setCoordinates(new Vector3(hamloc.X, hamloc.Y, hamloc.Z));
                newHammer.m_qRotation = new Quaternion(hamloc.Q_X, hamloc.Q_Y, hamloc.Q_Z, hamloc.Q_W);
                newHammer.select();
                newHammer.deselect();
                hammers.Add(newHammer);
            }

            foreach (XElement el in levelObject.Elements("GravityBall"))
            {
                var rotX = (string)el.Attribute("Rotation_X") ?? "0";
                var rotY = (string)el.Attribute("Rotation_Y") ?? "0";
                var rotZ = (string)el.Attribute("Rotation_Z") ?? "0";
                var rotW = (string)el.Attribute("Rotation_W") ?? "0";
                GravityBall newball = new GravityBall(game);
                coordinateXYZ ballloc = new coordinateXYZ(float.Parse(el.Attribute("X").Value), float.Parse(el.Attribute("Y").Value), float.Parse(el.Attribute("Z").Value));
                game.Components.Add(newball);
                newball.setCoordinates(new Vector3(ballloc.X, ballloc.Y, ballloc.Z));
                newball.triggerRadius = float.Parse(el.Attribute("TriggerRadius").Value);
                gravityBalls.Add(newball);
            }

            foreach (XElement el in levelObject.Elements("Ramp"))
            {
                var rotX = (string)el.Attribute("Rotation_X") ?? "0";
                var rotY = (string)el.Attribute("Rotation_Y") ?? "0";
                var rotZ = (string)el.Attribute("Rotation_Z") ?? "0";
                var rotW = (string)el.Attribute("Rotation_W") ?? "0";
                Ramp newramp = new Ramp(game);
                coordinateXYZ ramploc = new coordinateXYZ(float.Parse(el.Attribute("X").Value), float.Parse(el.Attribute("Y").Value), float.Parse(el.Attribute("Z").Value), float.Parse(rotX),
                    float.Parse(rotY), float.Parse(rotZ), float.Parse(rotW));
                game.Components.Add(newramp);
                newramp.setCoordinates(new Vector3(ramploc.X, ramploc.Y, ramploc.Z));
                newramp.m_qRotation = new Quaternion(ramploc.Q_X, ramploc.Q_Y, ramploc.Q_Z, ramploc.Q_W);
                ramps.Add(newramp);
            }
            XElement door = levelObject.Element("Door");
            var doorRotX = (string)door.Attribute("Rotation_X") ?? "0";
            var doorRotY = (string)door.Attribute("Rotation_Y") ?? "0";
            var doorRotZ = (string)door.Attribute("Rotation_Z") ?? "0";
            var doorRotW = (string)door.Attribute("Rotation_W") ?? "0";
            coordinateXYZ doorLocation = new coordinateXYZ(float.Parse(door.Attribute("X").Value), float.Parse(door.Attribute("Y").Value), float.Parse(door.Attribute("Z").Value), float.Parse(doorRotX),
                    float.Parse(doorRotY), float.Parse(doorRotZ), float.Parse(doorRotW));
            targetDoor = new Door(game);
            game.Components.Add(targetDoor);
            targetDoor.setCoordinates(new Vector3(doorLocation.X, doorLocation.Y, doorLocation.Z));
            targetDoor.m_qRotation = new Quaternion(doorLocation.Q_X, doorLocation.Q_Y, doorLocation.Q_Z, doorLocation.Q_W);
            XElement interactiveObject = level.Element("InteractiveObject");
            XElement hammer = interactiveObject.Element("Hammer");
            if (hammer != null)
            {
                allowedHammers = int.Parse(hammer.Value);
            }
            //Console.WriteLine(allowedHammers);
            XElement spring = interactiveObject.Element("Spring");
            if (spring != null)
            {
                allowedSprings = int.Parse(spring.Value);
            }
            //Console.WriteLine(allowedSprings);
            XElement platform = interactiveObject.Element("Platform");
            if (platform != null)
            {
                allowedPlatforms = int.Parse(platform.Value);
            }
            //Console.WriteLine(allowedPlatforms);
            XElement gravityBall = interactiveObject.Element("GravityBall");
            if (gravityBall != null)
            {
                allowedGravityBalls = int.Parse(gravityBall.Value);
            }
            XElement ramp = interactiveObject.Element("Ramp");
            if (ramp != null)
            {
                allowedRamps = int.Parse(ramp.Value);
            }
        }
        public Hammer placeHammer(float xPos, float yPos, float zPos)
        {
            if (allowedHammers > 0)
            {
                allowedHammers--;
                Hammer newHammer = new Hammer(game);
                hammers.Add(newHammer);
                game.Components.Add(newHammer);
                newHammer.setCoordinates(new Vector3(xPos, yPos, zPos));
                newHammer.isUserMovable = true;
                newHammer.SetHinge();
                return newHammer;
            }
            return null;
        }
        public Spring placeSpring(float xPos, float yPos, float zPos)
        {
            if (allowedSprings > 0)
            {
                allowedSprings--;
                Spring newSpring = new Spring(game);
                springs.Add(newSpring);
                game.Components.Add(newSpring);
                newSpring.setCoordinates(new Vector3(xPos, yPos, zPos));
                newSpring.isUserMovable = true;
                return newSpring;
            }
            return null;
        }
        public GravityBall placeGravityBall(float xPos, float yPos, float zPos)
        {
            if (allowedGravityBalls > 0)
            {
                allowedGravityBalls--;
                GravityBall newGravityBall = new GravityBall(game);
                gravityBalls.Add(newGravityBall);
                game.Components.Add(newGravityBall);
                newGravityBall.setCoordinates(new Vector3(xPos, yPos, zPos));
                newGravityBall.isUserMovable = true;
                return newGravityBall;
            }
            return null;
        }
        public Ramp placeRamp(float xPos, float yPos, float zPos)
        {
            if (allowedRamps > 0)
            {
                allowedRamps--;
                Ramp newRamp = new Ramp(game);
                ramps.Add(newRamp);
                game.Components.Add(newRamp);
                newRamp.setCoordinates(new Vector3(xPos, yPos, zPos));
                newRamp.isUserMovable = true;
                return newRamp;
            }
            return null;
        }
        public Platform placePlatform(float xPos, float yPos, float zPos)
        {
            if (allowedPlatforms > 0)
            {
                allowedPlatforms--;
                Platform newPlatform = new Platform(game);
                platforms.Add(newPlatform);
                game.Components.Add(newPlatform);
                newPlatform.setCoordinates(new Vector3(xPos, yPos, zPos));
                newPlatform.isUserMovable = true;
                return newPlatform;
            }
            return null;
        }



        public void saveQuaternion(Quaternion q, XElement element)
        {
            XAttribute q_x = new XAttribute("Rotation_X", q.X);
            XAttribute q_y = new XAttribute("Rotation_Y", q.Y);
            XAttribute q_z = new XAttribute("Rotation_Z", q.Z);
            XAttribute q_w = new XAttribute("Rotation_W", q.W);
            element.Add(q_x);
            element.Add(q_y);
            element.Add(q_z);
            element.Add(q_w);
        }

        public void saveLevel(string savename)
        {
            if (savename.Length == 0)
                return;
            XDocument newDoc = new XDocument();
            XElement XNAContent = new XElement("XnaContent");
            XElement Level = new XElement("Level");
            XElement LevelObject = new XElement("LevelObject");
            XElement InteractiveObject = new XElement("InteractiveObject");
            XElement Player = new XElement("Player");
            Vector3 playerPos = new Vector3(0,0,0);
           
            foreach (GameComponent comp in game.Components)
            {
                if (comp is Ball)
                {
                    //Ball playerball = (Ball)comp;
                    playerPos = new Vector3(GameplayScreen.playerSpawnCoordinates.X + 512, 384 - GameplayScreen.playerSpawnCoordinates.Y, GameplayScreen.playerSpawnCoordinates.Z); 
                    //new Vector3(playerball.body.WorldTransform.Translation.X + 512, 384 - playerball.body.WorldTransform.Translation.Y, playerball.body.WorldTransform.Translation.Z);
                    //playerPos = game.GraphicsDevice.Viewport.Project(playerPos, Projection, View, playerball.body.WorldTransform);
                }
            }
            
            XAttribute Xpos = new XAttribute("X", playerPos.X);
            XAttribute Ypos = new XAttribute("Y", playerPos.Y);
            XAttribute Zpos = new XAttribute("Z", playerPos.Z);
            Player.Add(Xpos);
            Player.Add(Ypos);
            Player.Add(Zpos);
            LevelObject.Add(Player);
            foreach (Platform plat in platforms)
            {
                XElement newPlatform = new XElement("Floor");
                Vector3 platpos = new Vector3(plat.body.WorldTransform.Translation.X + 512, 384 - plat.body.WorldTransform.Translation.Y, plat.body.WorldTransform.Translation.Z);
                //platpos = game.GraphicsDevice.Viewport.Project(platpos, Projection, View, plat.body.WorldTransform);
                XAttribute platX = new XAttribute("X", platpos.X);
                XAttribute platY = new XAttribute("Y", platpos.Y);
                XAttribute platZ = new XAttribute("Z", platpos.Z);
                newPlatform.Add(platX);
                newPlatform.Add(platY);
                newPlatform.Add(platZ);
                saveQuaternion(plat.m_qRotation, newPlatform);
                LevelObject.Add(newPlatform);
            }
            foreach (Spring spr in springs)
            {
                XElement newSpring = new XElement("Spring");
                Vector3 sprpos = new Vector3(spr.body.WorldTransform.Translation.X + 512, 384 - spr.body.WorldTransform.Translation.Y, spr.body.WorldTransform.Translation.Z);
                //sprpos = game.GraphicsDevice.Viewport.Project(sprpos, Projection, View, spr.body.WorldTransform);
                XAttribute sprX = new XAttribute("X", sprpos.X);
                XAttribute sprY = new XAttribute("Y", sprpos.Y);
                XAttribute sprZ = new XAttribute("Z", sprpos.Z);
                newSpring.Add(sprX);
                newSpring.Add(sprY);
                newSpring.Add(sprZ);
                saveQuaternion(spr.m_qRotation, newSpring);
                LevelObject.Add(newSpring);
            }
            foreach (Hammer ham in hammers)
            {
                XElement newHammer = new XElement("Hammer");
                Vector3 hampos = new Vector3(ham.body.WorldTransform.Translation.X + 512, 384 - ham.body.WorldTransform.Translation.Y, ham.body.WorldTransform.Translation.Z);
                //hampos = game.GraphicsDevice.Viewport.Project(hampos, Projection, View, ham.body.WorldTransform);
                XAttribute hamX = new XAttribute("X", hampos.X);
                XAttribute hamY = new XAttribute("Y", hampos.Y);
                XAttribute hamZ = new XAttribute("Z", hampos.Z);
                newHammer.Add(hamX);
                newHammer.Add(hamY);
                newHammer.Add(hamZ);
                saveQuaternion(ham.m_qRotation, newHammer);
                LevelObject.Add(newHammer);
            }
            foreach (GravityBall bll in gravityBalls)
            {
                XElement newBall = new XElement("GravityBall");
                Vector3 ballpos = new Vector3(bll.body.WorldTransform.Translation.X + 512, 384 - bll.body.WorldTransform.Translation.Y, bll.body.WorldTransform.Translation.Z);
                //hampos = game.GraphicsDevice.Viewport.Project(hampos, Projection, View, ham.body.WorldTransform);
                XAttribute bllX = new XAttribute("X", ballpos.X);
                XAttribute bllY = new XAttribute("Y", ballpos.Y);
                XAttribute bllZ = new XAttribute("Z", ballpos.Z);
                newBall.Add(bllX);
                newBall.Add(bllY);
                newBall.Add(bllZ);
                XAttribute ballTriggerInfluence = new XAttribute("TriggerRadius", bll.triggerRadius);
                newBall.Add(ballTriggerInfluence);
                LevelObject.Add(newBall);
            }
            foreach (Ramp rmp in ramps)
            {
                XElement newRamp = new XElement("Ramp");
                Vector3 rmppos = new Vector3(rmp.body.WorldTransform.Translation.X + 512, 384 - rmp.body.WorldTransform.Translation.Y, rmp.body.WorldTransform.Translation.Z);
                //hampos = game.GraphicsDevice.Viewport.Project(hampos, Projection, View, ham.body.WorldTransform);
                XAttribute rmpX = new XAttribute("X", rmppos.X);
                XAttribute rmpY = new XAttribute("Y", rmppos.Y);
                XAttribute rmpZ = new XAttribute("Z", rmppos.Z);
                newRamp.Add(rmpX);
                newRamp.Add(rmpY);
                newRamp.Add(rmpZ);
                saveQuaternion(rmp.m_qRotation, newRamp);
                LevelObject.Add(newRamp);
            }
            XElement Door = new XElement("Door");
            Vector3 doorpos = new Vector3(targetDoor.body.WorldTransform.Translation.X + 512, 384 - targetDoor.body.WorldTransform.Translation.Y, targetDoor.body.WorldTransform.Translation.Z);
            //doorpos = game.GraphicsDevice.Viewport.Project(doorpos, Projection, View, targetDoor.body.WorldTransform);
            XAttribute doorX = new XAttribute("X", doorpos.X);
            XAttribute doorY = new XAttribute("Y", doorpos.Y);
            XAttribute doorZ = new XAttribute("Z", doorpos.Z);
            Door.Add(doorX);
            Door.Add(doorY);
            Door.Add(doorZ);
            saveQuaternion(targetDoor.m_qRotation, Door);
            LevelObject.Add(Door);
            XAttribute type = new XAttribute("Type", "int");
            XElement Hammer = new XElement("Hammer", allowedHammers);
            Hammer.Add(type);
            XElement Spring = new XElement("Spring", allowedSprings);
            Spring.Add(type);
            XElement Platform = new XElement("Platform", allowedPlatforms);
            Platform.Add(type);
            XElement GravityBall = new XElement("GravityBall", allowedGravityBalls);
            GravityBall.Add(type);
            XElement Ramp = new XElement("Ramp", allowedRamps);
            Ramp.Add(type);
            InteractiveObject.Add(Hammer);
            InteractiveObject.Add(Spring);
            InteractiveObject.Add(Platform);
            InteractiveObject.Add(GravityBall);
            InteractiveObject.Add(Ramp);
            Level.Add(LevelObject);
            Level.Add(InteractiveObject);
            XNAContent.Add(Level);
            newDoc.Add(XNAContent);
            /*
            String saveName;
            if (startingLevelName.Equals("LEVEL_EDITOR"))
            {
                saveName = "LEVEL_EDITOR_SAVE
            }
             */
            if (savename == "")
            {
                XmlWriter writer = XmlWriter.Create("Level" + currentLevel + "Save.xml");
                newDoc.WriteTo(writer);
                writer.Close();
            }
            else
            {
                XmlWriter writer = XmlWriter.Create(savename + ".xml");
                newDoc.WriteTo(writer);
                writer.Close();
            }
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {

            base.Initialize();
        }
        /*
        public void setGameplayScreen(GameplayScreen scr)
        {
            this.gameplayScreen = scr;
        }
        */
        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            timer.Update(gameTime);
            base.Update(gameTime);
        }
    }
}
