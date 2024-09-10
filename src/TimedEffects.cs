
using MyBox;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using UnityEngine;

namespace BepinControl
{
    public enum TimedType
    {
        PLAYER_ULTRA_SLOW,
        PLAYER_SLOW,
        PLAYER_FAST,
        PLAYER_ULTRA_FAST,
        PLAYER_FREEZE,
        JUMP_LOW,
        JUMP_HIGH,
        JUMP_ULTRA,
        WIDE_CAMERA,
        NARROW_CAMERA,
        FLIP_CAMERA,
        INF_STAM,
        NO_STAM,
        INVUL,
        OHKO
    }
    public class Timed
    {
        public TimedType type;
        float oldjump;
        float oldmove;

        public static List<(TimedType, string)> msgs = new List<(TimedType, string)>();
        public static int msgcool = 0;

        public static void msgtick()
        {
            if (msgcool > 0) msgcool--;
            else
            {
                if (msgs.Count > 0)
                {
                    var msg = msgs[0];
                    msgs.RemoveAt(0);
                    HelmetText helmetText = Singleton<HelmetText>.Instance;
                    helmetText.SetHelmetText($"{msg.Item2}", 1.9f);
                    msgcool = 180;
                }
            }
        }

        public Timed(TimedType t) { 
            type = t;
        }

        public void addEffect()
        {
            switch (type)
            {

                case TimedType.INF_STAM:
                    msgs.Add((type, "INFINITE STAMINA"));
                    break;
                case TimedType.NO_STAM:
                    msgs.Add((type, "NO STAMINA"));
                    break;
                case TimedType.INVUL:
                    msgs.Add((type, "INVULNERABLE"));
                    break;
                case TimedType.OHKO:
                    msgs.Add((type, "ONE HIT KO"));
                    break;

                case TimedType.PLAYER_ULTRA_SLOW:
                case TimedType.PLAYER_SLOW:
                case TimedType.PLAYER_FAST:
                case TimedType.PLAYER_ULTRA_FAST:
                case TimedType.PLAYER_FREEZE:
                    {
                        TestMod.ActionQueue.Enqueue(() =>
                        {
                            oldmove = Player.localPlayer.refs.controller.movementForce;
                        });
                        break;
                    }
                case TimedType.JUMP_LOW:
                    {
                        TestMod.ActionQueue.Enqueue(() =>
                        {
                            msgs.Add((type, "LOW JUMP"));
                            oldjump = Player.localPlayer.refs.controller.jumpImpulse;
                            Player.localPlayer.refs.controller.jumpImpulse = oldjump / 3.0f;
                        });
                        break;
                    }
                case TimedType.JUMP_HIGH:
                    {
                        TestMod.ActionQueue.Enqueue(() =>
                        {
                            msgs.Add((type, "HIGH JUMP"));
                            oldjump = Player.localPlayer.refs.controller.jumpImpulse;
                            Player.localPlayer.refs.controller.jumpImpulse = oldjump * 4.0f;
                        });
                        break;
                    }

                case TimedType.JUMP_ULTRA:
                    {
                        TestMod.ActionQueue.Enqueue(() =>
                        {
                            msgs.Add((type, "HIGH JUMP"));
                            oldjump = Player.localPlayer.refs.controller.jumpImpulse;
                            Player.localPlayer.refs.controller.jumpImpulse = oldjump * 8.0f;
                        });
                        break;
                    }
                case TimedType.FLIP_CAMERA:
                    {
                        TestMod.ActionQueue.Enqueue(() =>
                        {
                            Camera cam = (Camera)CrowdDelegates.getProperty(MainCamera.instance, "cam");
                            cam.transform.Rotate(0, 0, 180);
                        });
                        break;
                    }
            }

                
            
        }

        public void removeEffect()
        {
            switch (type)
            {
                case TimedType.OHKO:
                    {
                        TestMod.ActionQueue.Enqueue(() =>
                        {
                            Player.localPlayer.CallHeal(Player.PlayerData.maxHealth - 0.01f);
                        });
                        break;
                    }
                case TimedType.FLIP_CAMERA:
                    {
                        TestMod.ActionQueue.Enqueue(() =>
                        {
                            Camera camera = (Camera)CrowdDelegates.getProperty(MainCamera.instance, "cam");
                            Vector3 pos = camera.transform.position;
                            Quaternion rotation = Quaternion.Euler(0, 0, 0);
                            camera.transform.SetPositionAndRotation(pos, rotation);
                        });
                        break;
                    }
                case TimedType.WIDE_CAMERA:
                case TimedType.NARROW_CAMERA:
                    {
                        TestMod.ActionQueue.Enqueue(() =>
                        {
                            CrowdDelegates.setProperty(MainCamera.instance, "baseFOV", 70.0f);
                        });
                        break;
                    }
                case TimedType.PLAYER_ULTRA_SLOW:
                case TimedType.PLAYER_SLOW:
                case TimedType.PLAYER_FAST:
                case TimedType.PLAYER_ULTRA_FAST:
                case TimedType.PLAYER_FREEZE:
                    {
                        TestMod.ActionQueue.Enqueue(() =>
                        {
                            Player.localPlayer.refs.controller.movementForce = oldmove;
                        });
                        break;
                    }
                case TimedType.JUMP_LOW:
                case TimedType.JUMP_HIGH:
                case TimedType.JUMP_ULTRA:
                    {
                        TestMod.ActionQueue.Enqueue(() =>
                        {
                            Player.localPlayer.refs.controller.jumpImpulse = oldjump;
                        });
                        break;
                    }
            }
        }
        static int frames = 0;

        public void tick()
        {
            frames++;

            switch (type)
            {
                case TimedType.INVUL:
                    {
                        TestMod.ActionQueue.Enqueue(() =>
                        {
                            if (Player.localPlayer.data.health < Player.PlayerData.maxHealth)
                                CrowdDelegates.callFunc(Player.localPlayer, "Heal", new object[] { Player.PlayerData.maxHealth });
                        });
                        break;
                    }
                case TimedType.OHKO:
                    {
                        TestMod.ActionQueue.Enqueue(() =>
                        {
                            if (Player.localPlayer.data.health > 1.0f)
                            {
                                UnityEngine.Vector3 force = new UnityEngine.Vector3(0, 0, 0);
                                CrowdDelegates.callFunc(Player.localPlayer, "CallTakeDamageAndAddForceAndFall", new object[] { Player.localPlayer.data.health - 1.0f, force, 0 });
                            }
                        });
                        break;
                    }
                case TimedType.INF_STAM:
                { 
                    TestMod.ActionQueue.Enqueue(() =>
                    {
                        Player.localPlayer.data.currentStamina = 10f;
                    });
                    break;
                }
                case TimedType.NO_STAM:
                    {
                        TestMod.ActionQueue.Enqueue(() =>
                        {
                            Player.localPlayer.data.currentStamina = 0f;
                        });
                        break;
                    }
                case TimedType.WIDE_CAMERA:
                    {
                        TestMod.ActionQueue.Enqueue(() =>
                        {
                            CrowdDelegates.setProperty(MainCamera.instance, "baseFOV", 100.0f);
                        });
                        break;
                    }
                case TimedType.NARROW_CAMERA:
                    {
                        TestMod.ActionQueue.Enqueue(() =>
                        {
                            CrowdDelegates.setProperty(MainCamera.instance, "baseFOV", 40.0f);
                        });
                        break;
                    }
                case TimedType.PLAYER_ULTRA_SLOW:
                    {
                        TestMod.ActionQueue.Enqueue(() =>
                        {
                            Player.localPlayer.refs.controller.movementForce = 2.5f;
                        });
                        break;
                    }
                case TimedType.PLAYER_FREEZE:
                    {
                        TestMod.ActionQueue.Enqueue(() =>
                        {
                            Player.localPlayer.refs.controller.movementForce = 0f;
                        });
                        break;
                    }
                case TimedType.PLAYER_SLOW:
                    {
                        TestMod.ActionQueue.Enqueue(() =>
                        {
                            Player.localPlayer.refs.controller.movementForce = 5.0f;
                        });
                        break;
                    }
                case TimedType.PLAYER_FAST:
                    {
                        TestMod.ActionQueue.Enqueue(() =>
                        {
                            Player.localPlayer.refs.controller.movementForce = 20.0f;
                        });
                        break;
                    }
                case TimedType.PLAYER_ULTRA_FAST:
                    {
                        TestMod.ActionQueue.Enqueue(() =>
                        {
                            Player.localPlayer.refs.controller.movementForce = 40.0f;
                        });
                        break;
                    }

            }
        }
    }
    public class TimedThread
    {
        public static List<TimedThread> threads = new List<TimedThread>();

        public readonly Timed effect;
        public int duration;
        public int remain;
        public int id;
        public bool paused;

        public static bool isRunning(TimedType t)
        {
            foreach (var thread in threads)
            {
                if (thread.effect.type == t) return true;
            }
            return false;
        }


        public static void tick()
        {
            foreach (var thread in threads)
            {
                if (!thread.paused)
                    thread.effect.tick();
            }
        }
        public static void addTime(int duration)
        {
            try
            {
                lock (threads)
                {
                    foreach (var thread in threads)
                    {
                        Interlocked.Add(ref thread.duration, duration+5);
                        if (!thread.paused)
                        {
                            int time = Volatile.Read(ref thread.remain);
                            new TimedResponse(thread.id, time, CrowdResponse.Status.STATUS_PAUSE).Send(ControlClient.Socket);
                            thread.paused = true;
                        }
                    }
                }
            }
            catch(Exception e)
            {
                TestMod.mls.LogInfo(e.ToString());
            }
        }

        public static void tickTime(int duration)
        {
            try
            {
                lock (threads)
                {
                    foreach (var thread in threads)
                    {
                        int time = Volatile.Read(ref thread.remain);
                        time -= duration;
                        if (time < 0) time = 0;
                        Volatile.Write(ref thread.remain, time);
                    }
                }
            }
            catch (Exception e)
            {
                TestMod.mls.LogInfo(e.ToString());
            }
        }

        public static void unPause()
        {
            try
            {
                lock (threads)
                {
                    foreach (var thread in threads)
                    {
                        if (thread.paused)
                        {
                            int time = Volatile.Read(ref thread.remain);
                            new TimedResponse(thread.id, time, CrowdResponse.Status.STATUS_RESUME).Send(ControlClient.Socket);
                            thread.paused = false;
                        }
                    }
                }
            }
            catch(Exception e)
            {
                TestMod.mls.LogInfo(e.ToString());
            }
        }    
        
        public TimedThread(int id, TimedType type, int duration)
        {
            this.effect = new Timed(type);
            this.duration = duration;
            this.remain = duration;
            this.id = id;
            paused = false;

            try
            {
                lock (threads)
                {
                    threads.Add(this);
                }
            }
            catch (Exception e)
            {
                TestMod.mls.LogInfo(e.ToString());
            }
        }

        public void Run()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            effect.addEffect();

            try
            {
                int time = Volatile.Read(ref duration); ;
                while (time > 0)
                {
                    Interlocked.Add(ref duration, -time);
                    Thread.Sleep(time);

                    time = Volatile.Read(ref duration);
                }
                effect.removeEffect();
                lock (threads)
                {
                    threads.Remove(this);
                }
                new TimedResponse(id, 0, CrowdResponse.Status.STATUS_STOP).Send(ControlClient.Socket);
            }
            catch (Exception e)
            {
                TestMod.mls.LogInfo(e.ToString());
            }
        }
    }
}
