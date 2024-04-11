using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Assertions.Must;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
using Steamworks;
using Steamworks.Data;
using System.Collections;
using System.Security.AccessControl;
using BepInEx.Configuration;
using System.Reflection;
using static System.Net.Mime.MediaTypeNames;
using Steamworks.Ugc;
using System.Threading;
using BepinControl;
using System.Runtime.InteropServices;
using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography;
using DefaultNamespace;
using Photon.Realtime;
using Photon.Pun;
using ExitGames.Client.Photon;

namespace BepinControl
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class TestMod : BaseUnityPlugin
    {
        // Mod Details
        private const string modGUID = "WarpWorld.CrowdControl";
        private const string modName = "Crowd Control";
        private const string modVersion = "1.0.0.0";

        private readonly Harmony harmony = new Harmony(modGUID);

        public static ManualLogSource mls;

        internal static TestMod Instance = null;
        private ControlClient client = null;

        public static RoundSpawner spawner = null;
        public static List<IBudgetCost> spawns = new List<IBudgetCost>();

        void Awake()
        {
            

            Instance = this;
            mls = BepInEx.Logging.Logger.CreateLogSource("Crowd Control");

            mls.LogInfo($"Loaded {modGUID}. Patching.");
            harmony.PatchAll(typeof(TestMod));

            mls.LogInfo($"Initializing Crowd Control");

            try
            {
                client = new ControlClient();
                new Thread(new ThreadStart(client.NetworkLoop)).Start();
                new Thread(new ThreadStart(client.RequestLoop)).Start();
            }
            catch (Exception e)
            {
                mls.LogInfo($"CC Init Error: {e.ToString()}");
            }

            mls.LogInfo($"Crowd Control Initialized");


            mls = Logger;
        }
        
        void Update()
        {
            if (ActionQueue.Count > 0)
            {
                Action action = ActionQueue.Dequeue();
                action.Invoke();
            }

            lock (TimedThread.threads)
            {
                foreach (var thread in TimedThread.threads)
                {
                    if (!thread.paused)
                        thread.effect.tick();
                }
            }
        }

        const int MSG_CC= 33;

        public static void SendPlayerStats(Player player)
        {
            RaiseEventOptions val = new RaiseEventOptions
            {
                Receivers = (ReceiverGroup)1
            };
            PhotonNetwork.RaiseEvent(MSG_CC, new object[] { "stats", Player.localPlayer.refs.view.ViewID, Player.localPlayer.data.remainingOxygen, Player.localPlayer.data.health }, val, SendOptions.SendReliable);


        }

        void OnEvent(EventData photonEvent)
        {
            if (photonEvent.Code == MSG_CC)
            {
                object[] array = (object[])photonEvent.CustomData;
                switch (array[0])
                {
                    case "stats":

                        for(int i=0; i < PlayerHandler.instance.players.Count; i++)
                        {
                            Player player = PlayerHandler.instance.players[i];
                            if (player.refs.view.ViewID == (int)array[1]) {
                                player.data.remainingOxygen = (float)array[2];
                                player.data.health = (float)array[3];
                                break;
                            }
                        }

                        break;
                }
            }
        }


        public static Queue<Action> ActionQueue = new Queue<Action>();

        //attach this to some game class with a function that runs every frame like the player's Update()
        [HarmonyPatch(typeof(Player), "FixedUpdate")]
        [HarmonyPrefix]
        static void RunEffects()
        {


        }

        [HarmonyPatch(typeof(RoundSpawner), "SpawnRound")]
        [HarmonyPrefix]
        static void Spawning(RoundSpawner __instance)
        {
            spawner = __instance;

            foreach(GameObject monster in __instance.possibleSpawns)

            TestMod.mls.LogInfo($"Monster Type: {monster.name}");

        }

    }
        
}
