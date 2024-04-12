using DefaultNamespace;
using DG.Tweening;
using Lean.Pool;
using MyBox;
using Newtonsoft.Json.Linq;
using Photon.Pun;
using Steamworks.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Xml.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BepinControl
{
    public delegate CrowdResponse CrowdDelegate(ControlClient client, CrowdRequest req);

    public class CrowdDelegates
    {
        public static System.Random rnd = new System.Random();


        public static CrowdResponse Money100(ControlClient client, CrowdRequest req)
        {

            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            try
            {
               

                TestMod.ActionQueue.Enqueue(() =>
                {
                    PerlinShake perlin = (PerlinShake)getProperty(GamefeelHandler.instance, "perlin");
                    perlin.AddShake(15f, 0.2f, 15f);
                });


            }
            catch (Exception e)
            {
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);

        }

        public static CrowdResponse GiveMoney(ControlClient client, CrowdRequest req)
        {

            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            string code = req.code;
            code = code.Split('_')[1];

            int num = int.Parse(code);

            if (num < 0)
            {
                if(SurfaceNetworkHandler.RoomStats.Money + num < 0) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            }

            try
            {


                TestMod.ActionQueue.Enqueue(() =>
                {
                    SurfaceNetworkHandler.RoomStats.AddMoney(num);
                    Singleton<UserInterface>.Instance.moneyAddedUI.Show("Crowd Control", "$" + num, num>0);
                    TestMod.SendRoundStats();
                    if(num>0)TestMod.comments.Add("Yo, free money! Thanks Crowd Control!");
                });


            }
            catch (Exception e)
            {
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);

        }
        public static CrowdResponse SetColor(ControlClient client, CrowdRequest req)
        {

            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            string code = req.code;
            code = code.Split('_')[1];

            int num = int.Parse(code);

            PlayerCustomizer playerCustomizer = UnityEngine.Object.FindObjectOfType<PlayerCustomizer>();
            if (playerCustomizer == null) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            try
            {


                TestMod.ActionQueue.Enqueue(() =>
                {

                    playerCustomizer.RPCA_PickColor(num);
                    playerCustomizer.RPCA_PlayerLeftTerminal(true);

                });


            }
            catch (Exception e)
            {
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);

        }

        public static CrowdResponse GiveViews(ControlClient client, CrowdRequest req)
        {

            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            string code = req.code;
            code = code.Split('_')[1];

            int num = int.Parse(code);

            if (num == -100)
            {
                num = -SurfaceNetworkHandler.RoomStats.CurrentQuota;
                if(num==0) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            } else 
                num = (SurfaceNetworkHandler.RoomStats.QuotaToReach * num) / 100;

            if (num < 0)
            {
                if (SurfaceNetworkHandler.RoomStats.CurrentQuota + num < 0) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            }

            try
            {


                TestMod.ActionQueue.Enqueue(() =>
                {
                    setProperty(SurfaceNetworkHandler.RoomStats, "currentQuoutaInternal", SurfaceNetworkHandler.RoomStats.CurrentQuota + num);
                    callFunc(SurfaceNetworkHandler.RoomStats, "OnStatsUpdated", null);
                    Singleton<UserInterface>.Instance.moneyAddedUI.Show("Crowd Control", num + "0 Views", num > 0);
                    TestMod.SendRoundStats();
                    if (num > 0) TestMod.comments.Add("I watched this before it was even uploaded!");
                });


            }
            catch (Exception e)
            {
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);

        }


        public static CrowdResponse ShakeScreen(ControlClient client, CrowdRequest req)
        {

            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            if (Player.localPlayer.data.isInDiveBell) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (Player.localPlayer.data.dead) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE, "Player is dead.");

            try
            {


                TestMod.ActionQueue.Enqueue(() =>
                {
                    PerlinShake perlin = (PerlinShake)getProperty(GamefeelHandler.instance, "perlin");
                    perlin.AddShake(15f, 0.2f, 15f);
                });


            }
            catch (Exception e)
            {
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);

        }

        public static CrowdResponse TeleToCrew(ControlClient client, CrowdRequest req)
        {

            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            if (SceneManager.GetActiveScene().name == "SurfaceScene") return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE, "Player is not in the old world.");
            if (Player.localPlayer.data.isInDiveBell) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            Player player = null;
            List<Player> list = new List<Player>();


            foreach (Player p in PlayerHandler.instance.playerAlive)
            {
                if (p != Player.localPlayer)
                    list.Add(p);
            }

            if(list.Count==0) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            int r = rnd.Next(list.Count);
            player = list[r];

            try
            {


                TestMod.ActionQueue.Enqueue(() =>
                {
                    try
                    {
                        callFunc(Player.localPlayer.refs.ragdoll, "MoveAllRigsInDirection", player.data.groundPos - Player.localPlayer.data.groundPos);
 
                    }
                    catch (Exception e)
                    {
                        TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
                    }
                });


            }
            catch (Exception e)
            {
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);

        }

        public static CrowdResponse TeleToBell(ControlClient client, CrowdRequest req)
        {

            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            if (SceneManager.GetActiveScene().name == "SurfaceScene") return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE, "Player is not in the old world.");

            try
            {


                TestMod.ActionQueue.Enqueue(() =>
                {
                    try
                    {
                        Transform spawnPoint = (Transform)CrowdDelegates.callAndReturnFunc(SpawnHandler.Instance, "GetSpawnPoint", Spawns.DiveBell);
                        callFunc(Player.localPlayer.refs.ragdoll, "MoveAllRigsInDirection", spawnPoint.position - Player.localPlayer.data.groundPos);
 
                    }
                    catch (Exception e)
                    {
                        TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
                    }
                });


            }
            catch (Exception e)
            {
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);

        }

        public static CrowdResponse TeleCrew(ControlClient client, CrowdRequest req)
        {

            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            if (SceneManager.GetActiveScene().name == "SurfaceScene") return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE, "Player is not in the old world.");

            Player player = null;
            List<Player> list = new List<Player>();

            foreach (Player p in PlayerHandler.instance.playerAlive)
            {
                if (p != Player.localPlayer)
                    list.Add(p);
            }

            if (list.Count == 0) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            int r = rnd.Next(list.Count);
            player = list[r];

            TestMod.SendTele(player, Player.localPlayer);

            return new CrowdResponse(req.GetReqID(), status, message);

        }

        public static CrowdResponse TeleCrewBell(ControlClient client, CrowdRequest req)
        {

            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            if (SceneManager.GetActiveScene().name == "SurfaceScene") return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE, "Player is not in the old world.");

            Player player = null;
            List<Player> list = new List<Player>();

            foreach (Player p in PlayerHandler.instance.playerAlive)
            {
                if (p != Player.localPlayer)
                    list.Add(p);
            }

            if (list.Count == 0) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            int r = rnd.Next(list.Count);
            player = list[r];

            TestMod.SendTeleBell(player);

            return new CrowdResponse(req.GetReqID(), status, message);

        }

        public static CrowdResponse KillCrew(ControlClient client, CrowdRequest req)
        {

            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            if (SceneManager.GetActiveScene().name == "SurfaceScene") return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE, "Player is not in the old world.");

            Player player = null;
            List<Player> list = new List<Player>();

            foreach (Player p in PlayerHandler.instance.playerAlive)
            {
                if (p != Player.localPlayer)
                    list.Add(p);
            }

            if (list.Count == 0) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            int r = rnd.Next(list.Count);
            player = list[r];

            TestMod.SendKill(player);
            TestMod.comments.Add($"Was {player.data.player.name} killed by Crowd Control?");

            return new CrowdResponse(req.GetReqID(), status, message);

        }

        public static CrowdResponse GiveCrewItem(ControlClient client, CrowdRequest req)
        {

            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            string code = req.code;
            code = code.Split('_')[1];

            byte id = byte.Parse(code);

            Player player = null;
            List<Player> list = new List<Player>();

            foreach (Player p in PlayerHandler.instance.playerAlive)
            {
                if (p != Player.localPlayer)
                {

                    InventorySlot slot;

                    PlayerInventory playerInventory;
                    if (!Player.localPlayer.TryGetInventory(out playerInventory))continue;

                    if (!playerInventory.TryGetFeeSlot(out slot)) continue;

                    list.Add(p);
                }
            }

            if (list.Count == 0) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            int r = rnd.Next(list.Count);
            player = list[r];

            TestMod.SendGiveItem(player, id);
            TestMod.comments.Add($"I gave {player.data.player.name} a item!");

            return new CrowdResponse(req.GetReqID(), status, message);

        }

        public static CrowdResponse OpenDoor(ControlClient client, CrowdRequest req)
        {

            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            if (SceneManager.GetActiveScene().name == "SurfaceScene") return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE, "Player is not in the old world.");
            
            DivingBell bell = UnityEngine.Object.FindObjectOfType<DivingBell>();
            if(bell == null) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            bool moving = (bool)getProperty(bell, "m_isMovingDoor");

            if (moving || bell.opened) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            try
            {


                TestMod.ActionQueue.Enqueue(() =>
                {
                    bell.AttemptSetOpen(true);
                    TestMod.comments.Add("The doors are opening on their own!");
                });


            }
            catch (Exception e)
            {
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);

        }

        public static CrowdResponse CloseDoor(ControlClient client, CrowdRequest req)
        {

            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            if (SceneManager.GetActiveScene().name == "SurfaceScene") return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE, "Player is not in the old world.");

            DivingBell bell = UnityEngine.Object.FindObjectOfType<DivingBell>();
            if (bell == null) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            bool moving = (bool)getProperty(bell, "m_isMovingDoor");

            if (moving || !bell.opened) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            try
            {


                TestMod.ActionQueue.Enqueue(() =>
                {
                    TestMod.comments.Add("The doors are closing on their own!");
                    bell.AttemptSetOpen(false);
                });


            }
            catch (Exception e)
            {
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);

        }

        public static CrowdResponse ShakeScreenBig(ControlClient client, CrowdRequest req)
        {

            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";


            if (SceneManager.GetActiveScene().name == "SurfaceScene") return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE, "Player is not in the old world.");
            if (Player.localPlayer.data.dead) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE, "Player is dead.");
            if (Player.localPlayer.data.isInDiveBell) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");


            try
            {


                TestMod.ActionQueue.Enqueue(() =>
                {
                    PerlinShake perlin = (PerlinShake)getProperty(GamefeelHandler.instance, "perlin");
                    perlin.AddShake(25f, 3.5f, 25f);
                });


            }
            catch (Exception e)
            {
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);

        }




        public static CrowdResponse SpawnMonster(ControlClient client, CrowdRequest req)
        {

            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            if (Player.localPlayer.data.dead) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE, "Player is dead.");
            if (SceneManager.GetActiveScene().name == "SurfaceScene") return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE, "Player is not in the old world.");
            if (Player.localPlayer.data.isInDiveBell) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            string code = req.code;
            code = code.Split('_')[1];

            if (code == "Whisk") code = "Toolkit_Wisk";

            try
            {

                TestMod.ActionQueue.Enqueue(() =>
                {
                    try
                    {
                        float dist = 15.0f;
                        RaycastHit raycastHit = HelperFunctions.LineCheck(MainCamera.instance.transform.position, MainCamera.instance.transform.position + MainCamera.instance.transform.forward * dist, HelperFunctions.LayerType.TerrainProp, 0f);
                        Vector3 vector = MainCamera.instance.transform.position + MainCamera.instance.transform.forward * dist;
                        if (raycastHit.collider != null)
                        {
                            vector = raycastHit.point;
                        }
                        vector = HelperFunctions.GetGroundPos(vector + Vector3.up * 1f, HelperFunctions.LayerType.TerrainProp, 0f);
                        PhotonNetwork.Instantiate(code, vector, quaternion.identity, 0, null);
                        TestMod.comments.Add("Where did that monster come from? Is that Crowd Control?");

                    }
                    catch (Exception e)
                    {
                        TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
                    }
                });


            }
            catch (Exception e)
            {
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse SpawnMonsterCrew(ControlClient client, CrowdRequest req)
        {

            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";


            if (SceneManager.GetActiveScene().name == "SurfaceScene") return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE, "Player is not in the old world.");


            string code = req.code;
            code = code.Split('_')[1];

            if (code == "Whisk") code = "Toolkit_Wisk";

            Player player = null;
            List<Player> list = new List<Player>();

            foreach (Player p in PlayerHandler.instance.playerAlive)
            {
                if (p != Player.localPlayer && !p.data.dead && !p.data.isInDiveBell)
                    list.Add(p);
            }

            if (list.Count == 0) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            int r = rnd.Next(list.Count);
            player = list[r];

            try
            {

                TestMod.SendSpawn(player, code);
                TestMod.comments.Add("Where did that monster come from? Is that Crowd Control?");

            }
            catch (Exception e)
            {
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse GiveItem(ControlClient client, CrowdRequest req)
        {

            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            if (Player.localPlayer.data.dead) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE, "Player is dead.");

            string code = req.code;
            code = code.Split('_')[1];

            byte id = byte.Parse(code);

            try
            {
                InventorySlot slot;

                PlayerInventory playerInventory;
                if (!Player.localPlayer.TryGetInventory(out playerInventory)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

                if (!playerInventory.TryGetFeeSlot(out slot)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

                if (!PhotonNetwork.IsMasterClient)
                {
                    TestMod.SendGiveItem(Player.localPlayer, id);
                    return new CrowdResponse(req.GetReqID(), status, message);
                }

                byte itemid = id;
                Item item;

                ItemInstanceData pickupdata = new ItemInstanceData(Guid.NewGuid());

                Vector3 pos = Player.localPlayer.transform.position;

                TestMod.ActionQueue.Enqueue(() =>
                {
                    try
                    {
                        Pickup pickup = PickupHandler.CreatePickup(itemid, pickupdata, pos, UnityEngine.Random.rotation, (Vector3.up + UnityEngine.Random.onUnitSphere) * 2f, UnityEngine.Random.onUnitSphere * 5f);

                        if (ItemDatabase.TryGetItemFromID(itemid, out item))
                        {
                            ItemInstanceData instanceData = (ItemInstanceData)getProperty(pickup, "instanceData");

                            ItemInstanceData data = instanceData.Copy();


                            InventorySlot inventorySlot;
                            if (playerInventory.TryAddItem(new ItemDescriptor(item, data), out inventorySlot))
                            {
                                Player.localPlayer.refs.view.RPC("RPC_SelectSlot", Player.localPlayer.refs.view.Owner, new object[]
                                {
                                inventorySlot.SlotID
                                });
                                pickup.m_photonView.RPC("RPC_Remove", RpcTarget.MasterClient, Array.Empty<object>());

                                playerInventory.SyncInventoryToOthers();
                                TestMod.comments.Add($"You're welcome for the item {Player.localPlayer.data.player.name}!");
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
                    }
                });


            }
            catch (Exception e)
            {
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);

        }

        public static CrowdResponse DropItem(ControlClient client, CrowdRequest req)
        {

            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            if(Player.localPlayer.data.dead) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE, "Player is dead.");
            if (Player.localPlayer.data.isInDiveBell) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (Player.localPlayer.data.currentItem == null)return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if(Player.localPlayer.data.selectedItemSlot == -1) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            

            try
            {
               

                TestMod.ActionQueue.Enqueue(() =>
                {
                    try
                    {
                        Player.localPlayer.refs.items.DropItem(Player.localPlayer.data.selectedItemSlot);
                    }
                    catch (Exception e)
                    {
                        TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
                    }
                });


            }
            catch (Exception e)
            {
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);

        }

        public static CrowdResponse TakeItem(ControlClient client, CrowdRequest req)
        {

            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            
            if (Player.localPlayer.data.dead) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE, "Player is dead.");
            if (Player.localPlayer.data.isInDiveBell) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE, "Player is safe in Diving Bell");
            if (Player.localPlayer.data.currentItem == null) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (Player.localPlayer.data.selectedItemSlot == -1) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            PlayerInventory playerInventory;
            if (!Player.localPlayer.TryGetInventory(out playerInventory)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            try
            {


                TestMod.ActionQueue.Enqueue(() =>
                {
                    try
                    {
                        ItemDescriptor item;
                        playerInventory.TryRemoveItemFromSlot(Player.localPlayer.data.selectedItemSlot, out item);
                        TestMod.comments.Add("Where'd my item go?");
                    }
                    catch (Exception e)
                    {
                        TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
                    }
                });


            }
            catch (Exception e)
            {
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);

        }


        public static CrowdResponse InfStam(ControlClient client, CrowdRequest req)
        {

            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            if (Player.localPlayer.data.dead) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE, "Player is dead.");

            int dur = 30;
            if (req.duration > 0) dur = req.duration / 1000;

            if (TimedThread.isRunning(TimedType.INF_STAM)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (TimedThread.isRunning(TimedType.NO_STAM)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");


            new Thread(new TimedThread(req.GetReqID(), TimedType.INF_STAM, dur * 1000).Run).Start();
            return new TimedResponse(req.GetReqID(), dur * 1000, CrowdResponse.Status.STATUS_SUCCESS);
        }

        public static CrowdResponse Invul(ControlClient client, CrowdRequest req)
        {

            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            if (Player.localPlayer.data.dead) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE, "Player is dead.");

            int dur = 30;
            if (req.duration > 0) dur = req.duration / 1000;

            if (TimedThread.isRunning(TimedType.INVUL)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (TimedThread.isRunning(TimedType.OHKO)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");


            new Thread(new TimedThread(req.GetReqID(), TimedType.INVUL, dur * 1000).Run).Start();
            return new TimedResponse(req.GetReqID(), dur * 1000, CrowdResponse.Status.STATUS_SUCCESS);
        }

        public static CrowdResponse OHKO(ControlClient client, CrowdRequest req)
        {

            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";
            
            if (Player.localPlayer.data.dead) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE, "Player is dead.");

            int dur = 30;
            if (req.duration > 0) dur = req.duration / 1000;

            if (TimedThread.isRunning(TimedType.INF_STAM)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (TimedThread.isRunning(TimedType.OHKO)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");


            new Thread(new TimedThread(req.GetReqID(), TimedType.OHKO, dur * 1000).Run).Start();
            return new TimedResponse(req.GetReqID(), dur * 1000, CrowdResponse.Status.STATUS_SUCCESS);
        }

        public static CrowdResponse NoStam(ControlClient client, CrowdRequest req)
        {

            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            if (Player.localPlayer.data.dead) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE, "Player is dead.");

            int dur = 30;
            if (req.duration > 0) dur = req.duration / 1000;

            if (TimedThread.isRunning(TimedType.INF_STAM)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (TimedThread.isRunning(TimedType.NO_STAM)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");


            new Thread(new TimedThread(req.GetReqID(), TimedType.NO_STAM, dur * 1000).Run).Start();
            return new TimedResponse(req.GetReqID(), dur * 1000, CrowdResponse.Status.STATUS_SUCCESS);
        }

        public static CrowdResponse FlipCamera(ControlClient client, CrowdRequest req)
        {

            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";
          
            if (Player.localPlayer.data.dead) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE, "Player is dead.");
            if (Player.localPlayer.data.isInDiveBell) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE, "Player is safe in Diving Bell.");

            int dur = 30;
            if (req.duration > 0) dur = req.duration / 1000;

            if (TimedThread.isRunning(TimedType.FLIP_CAMERA)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            TestMod.comments.Add("Oh god, the camera!");

            new Thread(new TimedThread(req.GetReqID(), TimedType.FLIP_CAMERA, dur * 1000).Run).Start();
            return new TimedResponse(req.GetReqID(), dur * 1000, CrowdResponse.Status.STATUS_SUCCESS);
        }

        public static CrowdResponse WideCamera(ControlClient client, CrowdRequest req)
        {

            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            if (Player.localPlayer.data.dead) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE, "Player is dead.");
            if (Player.localPlayer.data.isInDiveBell) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE, "Player is safe in Diving Bell.");

            int dur = 30;
            if (req.duration > 0) dur = req.duration / 1000;

            if (TimedThread.isRunning(TimedType.WIDE_CAMERA)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (TimedThread.isRunning(TimedType.NARROW_CAMERA)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");


            new Thread(new TimedThread(req.GetReqID(), TimedType.WIDE_CAMERA, dur * 1000).Run).Start();
            return new TimedResponse(req.GetReqID(), dur * 1000, CrowdResponse.Status.STATUS_SUCCESS);
        }

        public static CrowdResponse NarrowCamera(ControlClient client, CrowdRequest req)
        {

            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";
          
            if (Player.localPlayer.data.dead) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE, "Player is dead.");
            if (Player.localPlayer.data.isInDiveBell) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE, "Player is safe in Diving Bell.");

            int dur = 30;
            if (req.duration > 0) dur = req.duration / 1000;

            if (TimedThread.isRunning(TimedType.WIDE_CAMERA)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (TimedThread.isRunning(TimedType.NARROW_CAMERA)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");


            new Thread(new TimedThread(req.GetReqID(), TimedType.NARROW_CAMERA, dur * 1000).Run).Start();
            return new TimedResponse(req.GetReqID(), dur * 1000, CrowdResponse.Status.STATUS_SUCCESS);
        }


        public static CrowdResponse UltraSlow(ControlClient client, CrowdRequest req)
        {

            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            if (Player.localPlayer.data.dead) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE, "Player is dead.");
            if (Player.localPlayer.data.isInDiveBell) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            int dur = 30;
            if (req.duration > 0) dur = req.duration / 1000;

            if (TimedThread.isRunning(TimedType.PLAYER_ULTRA_SLOW)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (TimedThread.isRunning(TimedType.PLAYER_SLOW)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (TimedThread.isRunning(TimedType.PLAYER_FAST)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (TimedThread.isRunning(TimedType.PLAYER_ULTRA_FAST)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (TimedThread.isRunning(TimedType.PLAYER_FREEZE)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");


            new Thread(new TimedThread(req.GetReqID(), TimedType.PLAYER_ULTRA_SLOW, dur * 1000).Run).Start();
            return new TimedResponse(req.GetReqID(), dur * 1000, CrowdResponse.Status.STATUS_SUCCESS);
        }

        public static CrowdResponse Slow(ControlClient client, CrowdRequest req)
        {

            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            if (Player.localPlayer.data.dead) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE, "Player is dead.");
            if (Player.localPlayer.data.isInDiveBell) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            int dur = 30;
            if (req.duration > 0) dur = req.duration / 1000;

            if (TimedThread.isRunning(TimedType.PLAYER_ULTRA_SLOW)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (TimedThread.isRunning(TimedType.PLAYER_SLOW)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (TimedThread.isRunning(TimedType.PLAYER_FAST)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (TimedThread.isRunning(TimedType.PLAYER_ULTRA_FAST)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (TimedThread.isRunning(TimedType.PLAYER_FREEZE)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");


            new Thread(new TimedThread(req.GetReqID(), TimedType.PLAYER_SLOW, dur * 1000).Run).Start();
            return new TimedResponse(req.GetReqID(), dur * 1000, CrowdResponse.Status.STATUS_SUCCESS);
        }

        public static CrowdResponse Fast(ControlClient client, CrowdRequest req)
        {

            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            if (Player.localPlayer.data.dead) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE, "Player is dead.");
            if (Player.localPlayer.data.isInDiveBell) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            int dur = 30;
            if (req.duration > 0) dur = req.duration / 1000;

            if (TimedThread.isRunning(TimedType.PLAYER_ULTRA_SLOW)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (TimedThread.isRunning(TimedType.PLAYER_SLOW)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (TimedThread.isRunning(TimedType.PLAYER_FAST)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (TimedThread.isRunning(TimedType.PLAYER_ULTRA_FAST)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (TimedThread.isRunning(TimedType.PLAYER_FREEZE)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");


            new Thread(new TimedThread(req.GetReqID(), TimedType.PLAYER_FAST, dur * 1000).Run).Start();
            return new TimedResponse(req.GetReqID(), dur * 1000, CrowdResponse.Status.STATUS_SUCCESS);
        }

        public static CrowdResponse UltraFast(ControlClient client, CrowdRequest req)
        {

            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            if (Player.localPlayer.data.dead) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE, "Player is dead.");
            if (Player.localPlayer.data.isInDiveBell) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            int dur = 30;
            if (req.duration > 0) dur = req.duration / 1000;

            if (TimedThread.isRunning(TimedType.PLAYER_ULTRA_SLOW)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (TimedThread.isRunning(TimedType.PLAYER_SLOW)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (TimedThread.isRunning(TimedType.PLAYER_FAST)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (TimedThread.isRunning(TimedType.PLAYER_ULTRA_FAST)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (TimedThread.isRunning(TimedType.PLAYER_FREEZE)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            TestMod.comments.Add("I'm so fast!");

            new Thread(new TimedThread(req.GetReqID(), TimedType.PLAYER_ULTRA_FAST, dur * 1000).Run).Start();
            return new TimedResponse(req.GetReqID(), dur * 1000, CrowdResponse.Status.STATUS_SUCCESS);
        }

        public static CrowdResponse Freeze(ControlClient client, CrowdRequest req)
        {

            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            if (Player.localPlayer.data.dead) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE, "Player is dead.");
            if (Player.localPlayer.data.isInDiveBell) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            int dur = 30;
            if (req.duration > 0) dur = req.duration / 1000;

            if (TimedThread.isRunning(TimedType.PLAYER_ULTRA_SLOW)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (TimedThread.isRunning(TimedType.PLAYER_SLOW)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (TimedThread.isRunning(TimedType.PLAYER_FAST)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (TimedThread.isRunning(TimedType.PLAYER_ULTRA_FAST)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (TimedThread.isRunning(TimedType.PLAYER_FREEZE)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            TestMod.comments.Add($"I froze {Player.localPlayer.data.player.name} with Crowd Control! hahahaha");

            new Thread(new TimedThread(req.GetReqID(), TimedType.PLAYER_FREEZE, dur * 1000).Run).Start();
            return new TimedResponse(req.GetReqID(), dur * 1000, CrowdResponse.Status.STATUS_SUCCESS);
        }

        public static CrowdResponse LowJump(ControlClient client, CrowdRequest req)
        {

            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            if (Player.localPlayer.data.dead) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE, "Player is dead.");
            if (Player.localPlayer.data.isInDiveBell) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            int dur = 30;
            if (req.duration > 0) dur = req.duration / 1000;

            if (TimedThread.isRunning(TimedType.JUMP_LOW)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (TimedThread.isRunning(TimedType.JUMP_HIGH)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (TimedThread.isRunning(TimedType.JUMP_ULTRA)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");


            new Thread(new TimedThread(req.GetReqID(), TimedType.JUMP_LOW, dur * 1000).Run).Start();
            return new TimedResponse(req.GetReqID(), dur * 1000, CrowdResponse.Status.STATUS_SUCCESS);
        }

        public static CrowdResponse HighJump(ControlClient client, CrowdRequest req)
        {

            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            if (Player.localPlayer.data.dead) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE, "Player is dead.");
            if (Player.localPlayer.data.isInDiveBell) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            int dur = 30;
            if (req.duration > 0) dur = req.duration / 1000;

            if (TimedThread.isRunning(TimedType.JUMP_LOW)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (TimedThread.isRunning(TimedType.JUMP_HIGH)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (TimedThread.isRunning(TimedType.JUMP_ULTRA)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");


            new Thread(new TimedThread(req.GetReqID(), TimedType.JUMP_HIGH, dur * 1000).Run).Start();
            return new TimedResponse(req.GetReqID(), dur * 1000, CrowdResponse.Status.STATUS_SUCCESS);
        }

        public static CrowdResponse UltraJump(ControlClient client, CrowdRequest req)
        {

            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            if (Player.localPlayer.data.dead) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE, "Player is dead.");
            if (Player.localPlayer.data.isInDiveBell) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            int dur = 30;
            if (req.duration > 0) dur = req.duration / 1000;

            if (TimedThread.isRunning(TimedType.JUMP_LOW)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (TimedThread.isRunning(TimedType.JUMP_HIGH)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (TimedThread.isRunning(TimedType.JUMP_ULTRA)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");


            new Thread(new TimedThread(req.GetReqID(), TimedType.JUMP_ULTRA, dur * 1000).Run).Start();
            return new TimedResponse(req.GetReqID(), dur * 1000, CrowdResponse.Status.STATUS_SUCCESS);
        }

        public static CrowdResponse TakeOxygen10(ControlClient client, CrowdRequest req)
        {

            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            if (Player.localPlayer.data.dead) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE, "Player is dead.");
            if (Player.localPlayer.data.isInDiveBell) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            if (Player.localPlayer.data.remainingOxygen <= 50.0f)
                return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, message);

            try
            {
                TestMod.ActionQueue.Enqueue(() =>
                {
                    try
                    {
                        TakeDamagePost.instance.TakeDamageFeedback();
                        UI_Feedback.instance.TakeDamage(false);
                        Player.localPlayer.data.remainingOxygen -= 50.0f;
                        TestMod.SendPlayerStats(Player.localPlayer);
                    }
                    catch (Exception e)
                    {
                        TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
                    }
                });

            }
            catch (Exception e)
            {
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse Jump(ControlClient client, CrowdRequest req)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            if (Player.localPlayer.data.dead) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE, "Player is dead.");
            if (Player.localPlayer.data.isInDiveBell) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            if (Player.localPlayer.data.sinceGrounded < 0.5f && Player.localPlayer.data.sinceJump > 0.6f) { 

            } else  {
                return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, message);
            }

            try
            {
                TestMod.ActionQueue.Enqueue(() =>
                {
                    try
                    {
                        Player.localPlayer.refs.controller.TryJump();
                    }
                    catch (Exception e)
                    {
                        TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
                    }
                });

            }
            catch (Exception e)
            {
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse TakeOxygen30(ControlClient client, CrowdRequest req)
        {

            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            if (Player.localPlayer.data.dead) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE, "Player is dead.");
            if (Player.localPlayer.data.isInDiveBell) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            if (Player.localPlayer.data.remainingOxygen <= 150.0f)
                return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, message);

            try
            {
                TestMod.ActionQueue.Enqueue(() =>
                {
                    try
                    {
                        TakeDamagePost.instance.TakeDamageFeedback();
                        UI_Feedback.instance.TakeDamage(false);
                        Player.localPlayer.data.remainingOxygen -= 150.0f;
                        TestMod.SendPlayerStats(Player.localPlayer);
                    }
                    catch (Exception e)
                    {
                        TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
                    }
                });

            }
            catch (Exception e)
            {
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse TakeOxygen100(ControlClient client, CrowdRequest req)
        {

            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            if (Player.localPlayer.data.dead) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE, "Player is dead.");
            if (Player.localPlayer.data.isInDiveBell) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            if (Player.localPlayer.data.remainingOxygen <= 50.0f)
                return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, message);

            try
            {
                TestMod.ActionQueue.Enqueue(() =>
                {
                    try
                    {
                        TakeDamagePost.instance.TakeDamageFeedback();
                        UI_Feedback.instance.TakeDamage(false);
                        Player.localPlayer.data.remainingOxygen = 0;
                        TestMod.SendPlayerStats(Player.localPlayer);
                    }
                    catch (Exception e)
                    {
                        TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
                    }
                });

            }
            catch (Exception e)
            {
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse GiveOxygen10(ControlClient client, CrowdRequest req)
        {

            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            if (Player.localPlayer.data.dead) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE, "Player is dead.");

            if (Player.localPlayer.data.remainingOxygen >= Player.localPlayer.data.maxOxygen)
                return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, message);

            try
            {
                TestMod.ActionQueue.Enqueue(() =>
                {
                    try
                    {
                        UI_Feedback.instance.HealFeedback();

                        Player.localPlayer.data.remainingOxygen += 50.0f;
                        if (Player.localPlayer.data.remainingOxygen > Player.localPlayer.data.maxOxygen)
                            Player.localPlayer.data.remainingOxygen = Player.localPlayer.data.maxOxygen;
                        TestMod.SendPlayerStats(Player.localPlayer);
                    }
                    catch (Exception e)
                    {
                        TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
                    }
                });

            }
            catch (Exception e)
            {
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse GiveOxygen30(ControlClient client, CrowdRequest req)
        {

            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            if (Player.localPlayer.data.dead) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE, "Player is dead.");

            if (Player.localPlayer.data.remainingOxygen >= Player.localPlayer.data.maxOxygen)
                return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, message);

            try
            {
                TestMod.ActionQueue.Enqueue(() =>
                {
                    try
                    {
                        UI_Feedback.instance.HealFeedback();

                        Player.localPlayer.data.remainingOxygen += 150.0f;
                        if (Player.localPlayer.data.remainingOxygen > Player.localPlayer.data.maxOxygen)
                            Player.localPlayer.data.remainingOxygen = Player.localPlayer.data.maxOxygen;
                        TestMod.SendPlayerStats(Player.localPlayer);
                    }
                    catch (Exception e)
                    {
                        TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
                    }
                });

            }
            catch (Exception e)
            {
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse GiveOxygen100(ControlClient client, CrowdRequest req)
        {

            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            if (Player.localPlayer.data.dead) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE, "Player is dead.");

            if (Player.localPlayer.data.remainingOxygen >= Player.localPlayer.data.maxOxygen)
                return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, message);

            try
            {
                TestMod.ActionQueue.Enqueue(() =>
                {
                    try
                    {
                        UI_Feedback.instance.HealFeedback();
                        Player.localPlayer.data.remainingOxygen = Player.localPlayer.data.maxOxygen;
                        TestMod.SendPlayerStats(Player.localPlayer);
                    }
                    catch (Exception e)
                    {
                        TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
                    }
                });

            }
            catch (Exception e)
            {
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse FillStamina(ControlClient client, CrowdRequest req)
        {

            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            if (Player.localPlayer.data.dead) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE, "Player is dead.");

            if (Player.localPlayer.data.currentStamina >= 10f)
                return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, message);

            try
            {
                TestMod.ActionQueue.Enqueue(() =>
                {
                    try
                    {
                        UI_Feedback.instance.HealFeedback();
                        Player.localPlayer.data.currentStamina = 10f;
                    }
                    catch (Exception e)
                    {
                        TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
                    }
                });

            }
            catch (Exception e)
            {
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse EmptyStamina(ControlClient client, CrowdRequest req)
        {

            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            if (Player.localPlayer.data.dead) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE, "Player is dead.");
            if (Player.localPlayer.data.isInDiveBell) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            if (Player.localPlayer.data.currentStamina <= 0)
                return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, message);

            try
            {
                TestMod.ActionQueue.Enqueue(() =>
                {
                    try
                    {
                        TakeDamagePost.instance.TakeDamageFeedback();
                        UI_Feedback.instance.TakeDamage(false);
                        Player.localPlayer.data.currentStamina = 0;
                    }
                    catch (Exception e)
                    {
                        TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
                    }
                });

            }
            catch (Exception e)
            {
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse ShoveForward(ControlClient client, CrowdRequest req)
        {

            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            if (Player.localPlayer.data.dead) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE, "Player is dead.");
            if (Player.localPlayer.data.isInDiveBell) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            try
            {
                TestMod.ActionQueue.Enqueue(() =>
                {
                    try
                    {
                        UnityEngine.Vector3 force = MainCamera.instance.transform.forward * 50f;
                        callFunc(Player.localPlayer, "CallTakeDamageAndAddForceAndFall", new object[] { 0, force, 0 });

                        //Modal.ShowError("CC", msg);
                    }
                    catch (Exception e)
                    {
                        TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
                    }
                });

            }
            catch (Exception e)
            {
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse YankBackwards(ControlClient client, CrowdRequest req)
        {

            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            if (Player.localPlayer.data.dead) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE, "Player is dead.");
            if (Player.localPlayer.data.isInDiveBell) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            try
            {
                TestMod.ActionQueue.Enqueue(() =>
                {
                    try
                    {
                        UnityEngine.Vector3 force = MainCamera.instance.transform.forward * -50f;
                        callFunc(Player.localPlayer, "CallTakeDamageAndAddForceAndFall", new object[] { 0, force, 0 });

                        //Modal.ShowError("CC", msg);
                    }
                    catch (Exception e)
                    {
                        TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
                    }
                });

            }
            catch (Exception e)
            {
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse MegaLaunch(ControlClient client, CrowdRequest req)
        {

            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            if (Player.localPlayer.data.dead) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE, "Player is dead.");
            if (Player.localPlayer.data.isInDiveBell) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            try
            {
                TestMod.ActionQueue.Enqueue(() =>
                {
                    try
                    {
                        UnityEngine.Vector3 force = new UnityEngine.Vector3(0, 250.0f, 0);
                        callFunc(Player.localPlayer, "CallTakeDamageAndAddForceAndFall", new object[] { 0, force, 0 });
                        TestMod.comments.Add("They just flew up into the air!");
                        //Modal.ShowError("CC", msg);
                    }
                    catch (Exception e)
                    {
                        TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
                    }
                });

            }
            catch (Exception e)
            {
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse Ragdoll(ControlClient client, CrowdRequest req)
        {

            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            if (Player.localPlayer.data.dead) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE, "Player is dead.");
            if (Player.localPlayer.data.isInDiveBell) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            try
            {
                TestMod.ActionQueue.Enqueue(() =>
                {
                    try
                    {
                        UnityEngine.Vector3 force = new UnityEngine.Vector3(0, 0, 0);
                        callFunc(Player.localPlayer, "CallTakeDamageAndAddForceAndFall", new object[] { 0, force, 4.0f });

                        //Modal.ShowError("CC", msg);
                    }
                    catch (Exception e)
                    {
                        TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
                    }
                });

            }
            catch (Exception e)
            {
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse Launch(ControlClient client, CrowdRequest req)
        {

            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            if (Player.localPlayer.data.dead) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE, "Player is dead.");
            if (Player.localPlayer.data.isInDiveBell) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            try
            {
                TestMod.ActionQueue.Enqueue(() =>
                {
                    try
                    {
                        UnityEngine.Vector3 force = new UnityEngine.Vector3(0, 100.0f, 0);
                        callFunc(Player.localPlayer, "CallTakeDamageAndAddForceAndFall", new object[] { 0, force, 0 });
                        TestMod.comments.Add("They just flew up into the air!");
                        //Modal.ShowError("CC", msg);
                    }
                    catch (Exception e)
                    {
                        TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
                    }
                });

            }
            catch (Exception e)
            {
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse Damage10(ControlClient client, CrowdRequest req)
        {

            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            if (Player.localPlayer.data.dead) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE, "Player is dead.");
            if (Player.localPlayer.data.isInDiveBell) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            if (Player.localPlayer.data.health <= 10.0f)
                return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, message);

            try
            {
                TestMod.ActionQueue.Enqueue(() =>
                {
                    try
                    {
                        UnityEngine.Vector3 force = new UnityEngine.Vector3(0, 0, 0);
                        callFunc(Player.localPlayer, "CallTakeDamageAndAddForceAndFall", new object[] { 10.0f, force, 0 });
                        TestMod.SendPlayerStats(Player.localPlayer);

                        //Modal.ShowError("CC", msg);
                    }
                    catch (Exception e)
                    {
                        TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
                    }
                });

            }
            catch (Exception e)
            {
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse Damage30(ControlClient client, CrowdRequest req)
        {

            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            if (Player.localPlayer.data.dead) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE, "Player is dead.");
            if (Player.localPlayer.data.isInDiveBell) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            if (Player.localPlayer.data.health <= 30.0f)
                return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, message);

            try
            {
                TestMod.ActionQueue.Enqueue(() =>
                {
                    try
                    {
                        UnityEngine.Vector3 force = new UnityEngine.Vector3(0, 0, 0);
                        callFunc(Player.localPlayer, "CallTakeDamageAndAddForceAndFall", new object[] { 30.0f, force, 0 });
                        TestMod.SendPlayerStats(Player.localPlayer);
                        TestMod.comments.Add("Something hurt me out of nowhere!");
                        //Modal.ShowError("CC", msg);
                    }
                    catch (Exception e)
                    {
                        TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
                    }
                });

            }
            catch (Exception e)
            {
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse Heal10(ControlClient client, CrowdRequest req)
        {

            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            if (Player.localPlayer.data.dead) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE, "Player is dead.");

            if (Player.localPlayer.data.health >= Player.PlayerData.maxHealth)
                return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, message);

            try
            {
                TestMod.ActionQueue.Enqueue(() =>
                {
                    try
                    {
                        callFunc(Player.localPlayer, "Heal", new object[] { 10.0f });
                        TestMod.SendPlayerStats(Player.localPlayer);

                        //Modal.ShowError("CC", msg);
                    }
                    catch (Exception e)
                    {
                        TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
                    }
                });

            }
            catch (Exception e)
            {
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse Heal30(ControlClient client, CrowdRequest req)
        {

            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            if (Player.localPlayer.data.dead) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE, "Player is dead.");

            if (Player.localPlayer.data.health >= Player.PlayerData.maxHealth)
                return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, message);

            try
            {
                TestMod.ActionQueue.Enqueue(() =>
                {
                    try
                    {
                        callFunc(Player.localPlayer, "Heal", new object[] { 30.0f });
                        TestMod.SendPlayerStats(Player.localPlayer);

                        //Modal.ShowError("CC", msg);
                    }
                    catch (Exception e)
                    {
                        TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
                    }
                });

            }
            catch (Exception e)
            {
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse HealFull(ControlClient client, CrowdRequest req)
        {

            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            if (Player.localPlayer.data.dead) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE, "Player is dead.");

            if (Player.localPlayer.data.health >= Player.PlayerData.maxHealth)
                return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, message);

            try
            {
                TestMod.ActionQueue.Enqueue(() =>
                {
                    try
                    {
                        callFunc(Player.localPlayer, "Heal", new object[] { Player.PlayerData.maxHealth });
                        TestMod.SendPlayerStats(Player.localPlayer);
                        TestMod.comments.Add("Thanks for the heal, chat!");
                        //Modal.ShowError("CC", msg);
                    }
                    catch (Exception e)
                    {
                        TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
                    }
                });

            }
            catch (Exception e)
            {
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse Kill(ControlClient client, CrowdRequest req)
        {

            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            if (Player.localPlayer.data.dead) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE, "Player is dead.");
            if (Player.localPlayer.data.isInDiveBell) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            try
            {
                TestMod.ActionQueue.Enqueue(() =>
                {
                    try
                    {
                        callFunc(Player.localPlayer, "CallDie", null);
                        TestMod.comments.Add($"I just killed {Player.localPlayer.data.player.name} with Crowd Control!");
                        //Modal.ShowError("CC", msg);
                    }
                    catch (Exception e)
                    {
                        TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
                    }
                });

            }
            catch (Exception e)
            {
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse MakeSound(ControlClient client, CrowdRequest req)
        {

            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            if (Player.localPlayer.data.dead) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE, "Player is dead.");
            if (Player.localPlayer.data.isInDiveBell) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            try
            {
                TestMod.ActionQueue.Enqueue(() =>
                {
                    try
                    {
                        callFunc(Player.localPlayer, "CallMakeSound", 0);

                        //Modal.ShowError("CC", msg);
                    }
                    catch (Exception e)
                    {
                        TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
                    }
                });

            }
            catch (Exception e)
            {
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }
        /*
                public static CrowdResponse SpeedUltraSlow(ControlClient client, CrowdRequest req)
                {
                    int dur = 30;
                    if (req.duration > 0) dur = req.duration / 1000;

                    if(!Singleton<StoreStatus>.Instance.IsOpen) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

                    if (TimedThread.isRunning(TimedType.GAME_ULTRA_SLOW)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
                    if (TimedThread.isRunning(TimedType.GAME_SLOW)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
                    if (TimedThread.isRunning(TimedType.GAME_FAST)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
                    if (TimedThread.isRunning(TimedType.GAME_ULTRA_FAST)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

                    new Thread(new TimedThread(req.GetReqID(), TimedType.GAME_ULTRA_SLOW, dur * 1000).Run).Start();
                    return new TimedResponse(req.GetReqID(), dur * 1000, CrowdResponse.Status.STATUS_SUCCESS);
                }
          */

        public static void setProperty(System.Object a, string prop, System.Object val)
        {
            var f = a.GetType().GetField(prop, BindingFlags.Instance | BindingFlags.NonPublic);
            f.SetValue(a, val);
        }

        public static System.Object getProperty(System.Object a, string prop)
        {
            var f = a.GetType().GetField(prop, BindingFlags.Instance | BindingFlags.NonPublic);
            return f.GetValue(a);
        }

        public static void setSubProperty(System.Object a, string prop, string prop2, System.Object val)
        {
            var f = a.GetType().GetField(prop, BindingFlags.Instance | BindingFlags.NonPublic);
            var f2 = f.GetType().GetField(prop, BindingFlags.Instance | BindingFlags.NonPublic);
            f2.SetValue(f, val);
        }

        public static void callSubFunc(System.Object a, string prop, string func, System.Object val)
        {
            callSubFunc(a, prop, func, new object[] { val });
        }

        public static void callSubFunc(System.Object a, string prop, string func, System.Object[] vals)
        {
            var f = a.GetType().GetField(prop, BindingFlags.Instance | BindingFlags.NonPublic);


            var p = f.GetType().GetMethod(func, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy);
            p.Invoke(f, vals);

        }

        public static void callFunc(System.Object a, string func, System.Object val)
        {
            callFunc(a, func, new object[] { val });
        }

        public static void callFunc(System.Object a, string func, System.Object[] vals)
        {
            var p = a.GetType().GetMethod(func, BindingFlags.Instance | BindingFlags.NonPublic);
            p.Invoke(a, vals);

        }

        public static System.Object callAndReturnFunc(System.Object a, string func, System.Object val)
        {
            return callAndReturnFunc(a, func, new object[] { val });
        }

        public static System.Object callAndReturnFunc(System.Object a, string func, System.Object[] vals)
        {
            var p = a.GetType().GetMethod(func, BindingFlags.Instance | BindingFlags.NonPublic);
            return p.Invoke(a, vals);

        }

    }
}
