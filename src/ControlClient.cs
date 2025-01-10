﻿/*
 * ControlValley
 * Stardew Valley Support for Twitch Crowd Control
 * Copyright (C) 2021 TerribleTable
 * LGPL v2.1
 * 
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301
 * USA
 */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BepinControl
{
    public class ControlClient
    {
        public static readonly string CV_HOST = "127.0.0.1";
        public static readonly int CV_PORT = 51337;

        private Dictionary<string, CrowdDelegate> Delegate { get; set; }
        private IPEndPoint Endpoint { get; set; }
        private Queue<CrowdRequest> Requests { get; set; }
        private static bool Running { get; set; }

        private bool paused = false;
        public static Socket Socket { get; set; }

        public bool inGame = true;

        public ControlClient()
        {
            Endpoint = new IPEndPoint(IPAddress.Parse(CV_HOST), CV_PORT);
            Requests = new Queue<CrowdRequest>();
            Running = true;
            Socket = null;

            Delegate = new Dictionary<string, CrowdDelegate>()
            {
                //when an effect comes in with the code it will call the paired function
                {"money100", CrowdDelegates.Money100},
                {"sound", CrowdDelegates.MakeSound},
                {"kill", CrowdDelegates.Kill},

                {"damage", CrowdDelegates.Damage10},
                {"damageb", CrowdDelegates.Damage30},
                {"heal", CrowdDelegates.Heal10},
                {"healb", CrowdDelegates.Heal30},
                {"healf", CrowdDelegates.HealFull},

                {"launch", CrowdDelegates.Launch},
                {"megalaunch", CrowdDelegates.MegaLaunch},
                {"forward", CrowdDelegates.ShoveForward},
                {"backward", CrowdDelegates.YankBackwards},
                {"ragdoll", CrowdDelegates.Ragdoll},

                {"giveo210",  CrowdDelegates.GiveOxygen10},
                {"giveo230",  CrowdDelegates.GiveOxygen30},
                {"giveo2100", CrowdDelegates.GiveOxygen100},
                {"takeo210",  CrowdDelegates.TakeOxygen10},
                {"takeo230",  CrowdDelegates.TakeOxygen30},
                {"takeo2100", CrowdDelegates.TakeOxygen100},

                {"fillstam", CrowdDelegates.FillStamina},
                {"emptystam", CrowdDelegates.EmptyStamina},
                {"infstam", CrowdDelegates.InfStam},
                {"nostam", CrowdDelegates.NoStam},

                {"invul", CrowdDelegates.Invul},
                {"ohko", CrowdDelegates.OHKO},

                {"ultraslow", CrowdDelegates.UltraSlow},
                {"slow", CrowdDelegates.Slow},
                {"fast", CrowdDelegates.Fast},
                {"ultrafast", CrowdDelegates.UltraFast},
                {"freeze", CrowdDelegates.Freeze},

                {"lowjump", CrowdDelegates.LowJump},
                {"highjump", CrowdDelegates.HighJump},
                {"ultrajump", CrowdDelegates.UltraJump},
                {"jump", CrowdDelegates.Jump},

                {"giveitem_mic"    , CrowdDelegates.GiveItem},
                {"giveitem_camera" , CrowdDelegates.GiveItem},
                {"giveitem_clapper", CrowdDelegates.GiveItem},
                {"giveitem_defib"  , CrowdDelegates.GiveItem},
                {"giveitem_flare"  , CrowdDelegates.GiveItem},
                {"giveitem_gooball", CrowdDelegates.GiveItem},
                {"giveitem_hugger" , CrowdDelegates.GiveItem},
                {"giveitem_light"  , CrowdDelegates.GiveItem},
                {"giveitem_party"  , CrowdDelegates.GiveItem},
                {"giveitem_shock"  , CrowdDelegates.GiveItem},
                {"giveitem_sound"  , CrowdDelegates.GiveItem},
                {"giveitem_chorby" , CrowdDelegates.GiveItem},
                {"giveitem_brain"  , CrowdDelegates.GiveItem},
                {"giveitem_radio"  , CrowdDelegates.GiveItem},
                {"giveitem_skull"  , CrowdDelegates.GiveItem},


                {"cgiveitem_mic"    , CrowdDelegates.GiveCrewItem},
                {"cgiveitem_camera" , CrowdDelegates.GiveCrewItem},
                {"cgiveitem_clapper", CrowdDelegates.GiveCrewItem},
                {"cgiveitem_defib"  , CrowdDelegates.GiveCrewItem},
                {"cgiveitem_flare"  , CrowdDelegates.GiveCrewItem},
                {"cgiveitem_gooball", CrowdDelegates.GiveCrewItem},
                {"cgiveitem_hugger" , CrowdDelegates.GiveCrewItem},
                {"cgiveitem_light"  , CrowdDelegates.GiveCrewItem},
                {"cgiveitem_party"  , CrowdDelegates.GiveCrewItem},
                {"cgiveitem_shock"  , CrowdDelegates.GiveCrewItem},
                {"cgiveitem_sound"  , CrowdDelegates.GiveCrewItem},
                {"cgiveitem_chorby" , CrowdDelegates.GiveCrewItem},
                {"cgiveitem_brain"  , CrowdDelegates.GiveCrewItem},
                {"cgiveitem_radio"  , CrowdDelegates.GiveCrewItem},
                {"cgiveitem_skull"  , CrowdDelegates.GiveCrewItem}, 

                {"dropitem", CrowdDelegates.DropItem},
                {"takeitem", CrowdDelegates.TakeItem},

                {"spawn_Jello", CrowdDelegates.SpawnMonster},
                {"spawn_Zombe", CrowdDelegates.SpawnMonster},
                {"spawn_Spider", CrowdDelegates.SpawnMonster},
                {"spawn_Snatcho", CrowdDelegates.SpawnMonster},
                {"spawn_Bombs", CrowdDelegates.SpawnMonster},
                {"spawn_BarnacleBall", CrowdDelegates.SpawnMonster},
                {"spawn_Dog", CrowdDelegates.SpawnMonster},
                {"spawn_Ear", CrowdDelegates.SpawnMonster},
                {"spawn_BigSlap", CrowdDelegates.SpawnMonster},
                {"spawn_EyeGuy", CrowdDelegates.SpawnMonster},
                {"spawn_Flicker", CrowdDelegates.SpawnMonster},
                {"spawn_Harpoon", CrowdDelegates.SpawnMonster},
                {"spawn_Mouthe", CrowdDelegates.SpawnMonster},
                {"spawn_Whisk", CrowdDelegates.SpawnMonster},
                {"spawn_Weeping", CrowdDelegates.SpawnMonster},

                {"cspawn_Jello", CrowdDelegates.SpawnMonsterCrew},
                {"cspawn_Zombe", CrowdDelegates.SpawnMonsterCrew},
                {"cspawn_Spider", CrowdDelegates.SpawnMonsterCrew},
                {"cspawn_Snatcho", CrowdDelegates.SpawnMonsterCrew},
                {"cspawn_Bombs", CrowdDelegates.SpawnMonsterCrew},
                {"cspawn_BarnacleBall", CrowdDelegates.SpawnMonsterCrew},
                {"cspawn_Dog", CrowdDelegates.SpawnMonsterCrew},
                {"cspawn_Ear", CrowdDelegates.SpawnMonsterCrew},
                {"cspawn_BigSlap", CrowdDelegates.SpawnMonsterCrew},
                {"cspawn_EyeGuy", CrowdDelegates.SpawnMonsterCrew},
                {"cspawn_Flicker", CrowdDelegates.SpawnMonsterCrew},
                {"cspawn_Harpoon", CrowdDelegates.SpawnMonsterCrew},
                {"cspawn_Mouthe", CrowdDelegates.SpawnMonsterCrew},
                {"cspawn_Whisk", CrowdDelegates.SpawnMonsterCrew},
                {"cspawn_Weeping", CrowdDelegates.SpawnMonsterCrew},

                {"shake", CrowdDelegates.ShakeScreen},
                {"shakebig", CrowdDelegates.ShakeScreenBig},

                {"widecam", CrowdDelegates.WideCamera},
                {"narrowcam", CrowdDelegates.NarrowCamera},
                {"flipcam", CrowdDelegates.FlipCamera},

                {"money_10", CrowdDelegates.GiveMoney},
                {"money_100", CrowdDelegates.GiveMoney},
                {"money_1000", CrowdDelegates.GiveMoney},
                {"money_-10", CrowdDelegates.GiveMoney},
                {"money_-100", CrowdDelegates.GiveMoney},
                {"money_-1000", CrowdDelegates.GiveMoney},

                {"views_10", CrowdDelegates.GiveViews},
                {"views_30", CrowdDelegates.GiveViews},
                {"views_100", CrowdDelegates.GiveViews},
                {"views_-10", CrowdDelegates.GiveViews},
                {"views_-30", CrowdDelegates.GiveViews},
                {"views_-100", CrowdDelegates.GiveViews},

                {"face_0", CrowdDelegates.SetColor},
                {"face_1", CrowdDelegates.SetColor},
                {"face_2", CrowdDelegates.SetColor},
                {"face_3", CrowdDelegates.SetColor},
                {"face_4", CrowdDelegates.SetColor},
                {"face_5", CrowdDelegates.SetColor},
                {"face_6", CrowdDelegates.SetColor},
                {"face_7", CrowdDelegates.SetColor},

                {"opendoor", CrowdDelegates.OpenDoor},
                {"closedoor", CrowdDelegates.CloseDoor},

                {"teleto", CrowdDelegates.TeleToCrew},
                {"telecrew", CrowdDelegates.TeleCrew},
                {"teletobell", CrowdDelegates.TeleToBell},
                {"telecrewbell", CrowdDelegates.TeleCrewBell},

                {"killcrew", CrowdDelegates.KillCrew},

                {"event-hype-train", CrowdDelegates.SpawnHypeTrain }



            };
        }

        public bool isReady()
        {
            try
            {

                if (Player.localPlayer == null) return false;

                //if (SceneManager.GetActiveScene().name == "SurfaceScene") return false;
                //if (Player.localPlayer.data.isInDiveBell) return false;
                //if (!Level.currentLevel || !Level.currentLevel.levelIsReady) return false;


            }
            catch (Exception e)
            {
                TestMod.mls.LogError(e.ToString());
                return false;
            }

            return true;
        }

        public static void HideEffect(string code)
        {
            CrowdResponse res = new CrowdResponse(0, CrowdResponse.Status.STATUS_NOTVISIBLE);
            res.type = 1;
            res.code = code;
            res.Send(Socket);
        }

        public static void ShowEffect(string code)
        {
            CrowdResponse res = new CrowdResponse(0, CrowdResponse.Status.STATUS_VISIBLE);
            res.type = 1;
            res.code = code;
            res.Send(Socket);
        }

        public static void DisableEffect(string code)
        {
            CrowdResponse res = new CrowdResponse(0, CrowdResponse.Status.STATUS_NOTSELECTABLE);
            res.type = 1;
            res.code = code;
            res.Send(Socket);
        }

        public static void EnableEffect(string code)
        {
            CrowdResponse res = new CrowdResponse(0, CrowdResponse.Status.STATUS_SELECTABLE);
            res.type = 1;
            res.code = code;
            res.Send(Socket);
        }

        private void ClientLoop()
        {

            TestMod.mls.LogInfo("Connected to Crowd Control");

            var timer = new Timer(timeUpdate, null, 0, 200);

            try
            {
                while (Running)
                {
                    CrowdRequest req = CrowdRequest.Recieve(this, Socket);
                    if (req == null || req.IsKeepAlive()) continue;

                   
                    lock (Requests)
                        Requests.Enqueue(req);

                }
            }
            catch (Exception e)
            {
                TestMod.mls.LogInfo($"{e.ToString()}");
                TestMod.mls.LogInfo("Disconnected from Crowd Control");
                Socket.Close();
            }
        }

        public void timeUpdate(System.Object state)
        {
            inGame = true;

            if(!isReady()) inGame = false;

            if (!inGame)
            {
                TimedThread.addTime(200);
                paused = true;
            } else if(paused)
            {
                paused = false;
                TimedThread.unPause();
                TimedThread.tickTime(200);
            }  else
            {
                TimedThread.tickTime(200);
            }
        }

        public bool IsRunning() => Running;

        public void NetworkLoop()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            while (Running)
            {
                
                TestMod.mls.LogInfo("Attempting to connect to Crowd Control");

                try
                {
                    Socket = new Socket(Endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                    if (Socket.BeginConnect(Endpoint, null, null).AsyncWaitHandle.WaitOne(10000, true) && Socket.Connected)
                        ClientLoop();
                    else
                        TestMod.mls.LogInfo("Failed to connect to Crowd Control");
                    Socket.Close();
                }
                catch (Exception e)
                {
                    TestMod.mls.LogInfo(e.GetType().Name);
                    TestMod.mls.LogInfo("Failed to connect to Crowd Control");
                }

                Thread.Sleep(10000);
            }
        }

        public void RequestLoop()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            while (Running)
            {
                try
                {

                    CrowdRequest req = null;
                    lock (Requests)
                    {
                        if (Requests.Count == 0)
                            continue;
                        req = Requests.Dequeue();
                    }

                    string code = req.GetReqCode();


                    try
                    {
                        CrowdResponse res;
                        if (!isReady())
                            res = new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY);
                        else
                            res = Delegate[code](this, req);
                        if (res == null)
                        {
                            new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE, $"Request error for '{code}'").Send(Socket);
                        }

                        res.Send(Socket);
                    }
                    catch (KeyNotFoundException)
                    {
                        new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE, $"Request error for '{code}'").Send(Socket);
                    }
                }
                catch (Exception e)
                {
                    TestMod.mls.LogError(e.ToString());
                    TestMod.mls.LogInfo("Disconnected from Crowd Control");
                    Socket.Close();
                }
            }
        }

        public void Stop()
        {
            Running = false;
        }

    }
}
