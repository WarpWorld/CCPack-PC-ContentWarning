using System;
using System.Collections.Generic;
using CrowdControl.Common;
using ConnectorType = CrowdControl.Common.ConnectorType;

namespace CrowdControl.Games.Packs.ContentWarning
{

    public class ContentWarning : SimpleTCPPack
    {
        public override string Host => "127.0.0.1";

        public override ushort Port => 51337;

        public override ISimpleTCPPack.MessageFormat MessageFormat => ISimpleTCPPack.MessageFormat.CrowdControlLegacy;

        public ContentWarning(UserRecord player, Func<CrowdControlBlock, bool> responseHandler, Action<object> statusUpdateHandler) : base(player, responseHandler, statusUpdateHandler) { }

        public override Game Game { get; } = new("Content Warning", "ContentWarning", "PC", ConnectorType.SimpleTCPServerConnector);

        public override EffectList Effects => new List<Effect>
        {
                new Effect("Damage Player 10%", "damage") { Category = "Health"},
                new Effect("Damage Player 30%", "damageb") { Category = "Health"},
                new Effect("Kill Player", "kill") { Category = "Health"},
                new Effect("Kill a Teammate", "killcrew") { Category = "Health"},
                new Effect("Heal Player 10%", "heal") { Category = "Health"},
                new Effect("Heal Player 30%", "healb") { Category = "Health"},
                new Effect("Full Heal Player", "healf") { Category = "Health"},
                new Effect("Invulnerable", "invul") { Category = "Health", Duration = 30},
                new Effect("One Hit KO", "ohko") { Category = "Health", Duration = 30},

                /*
                new Effect("Change Face to 0", "face_0") { Category = "Face"},
                new Effect("Change Face to 1", "face_1") { Category = "Face"},
                new Effect("Change Face to 2", "face_2") { Category = "Face"},
                new Effect("Change Face to 3", "face_3") { Category = "Face"},
                new Effect("Change Face to 4", "face_4") { Category = "Face"},
                new Effect("Change Face to 5", "face_5") { Category = "Face"},
                new Effect("Change Face to 6", "face_6") { Category = "Face"},
                new Effect("Change Face to 7", "face_7") { Category = "Face"},
                */

                new Effect("Restore Oxygen 10%", "giveo210") { Category = "Oxygen"},
                new Effect("Restore Oxygen 30%", "giveo230") { Category = "Oxygen"},
                new Effect("Restore Oxygen Full", "giveo2100") { Category = "Oxygen"},
                new Effect("Drain Oxygen 10%", "takeo210") { Category = "Oxygen"},
                new Effect("Drain Oxygen 30%", "takeo230") { Category = "Oxygen"},
                //new Effect("Drain Oxygen Full", "takeo2100") { Category = "Oxygen"},

                new Effect("Fill Stamina", "fillstam") { Category = "Stamina"},
                new Effect("Empty Stamina", "emptystam") { Category = "Stamina"},
                new Effect("Infinite Stamina", "infstam") { Category = "Stamina", Duration = 30},
                new Effect("Disable Stamina", "nostam") { Category = "Stamina", Duration = 30},

                new Effect("Make Sound", "sound"),

                new Effect("Launch Player", "launch") { Category = "Movement"},
                new Effect("Mega Launch Player", "megalaunch") { Category = "Movement"},
                new Effect("Shove Player Forward", "forward") { Category = "Movement"},
                new Effect("Yank Player Backward", "backward") { Category = "Movement"},
                new Effect("Ragdoll Player", "ragdoll") { Category = "Movement"},

                new Effect("Ultra Slow Player", "ultraslow") { Category = "Movement", Duration = 15},
                new Effect("Slow Player", "slow") { Category = "Movement", Duration = 30},
                new Effect("Fast Player", "fast") { Category = "Movement", Duration = 30},
                new Effect("Ultra Fast Player", "ultrafast") { Category = "Movement", Duration = 15},
                new Effect("Freeze Player", "freeze") { Category = "Movement", Duration = 5},
                new Effect("Teleport to a Teammate", "teleto") { Category = "Movement"},
                new Effect("Teleport a Teammate to Player", "telecrew") { Category = "Movement"},
                new Effect("Teleport Player to Dive Bell", "teletobell") { Category = "Movement"},
                new Effect("Teleport a Teammate to Dive Bell", "telecrewbell") { Category = "Movement"},

                new Effect("Low Jump", "lowjump") { Category = "Jumping", Duration = 30},
                new Effect("High Jump", "highjump") { Category = "Jumping", Duration = 30},
                new Effect("Ultra Jump", "ultrajump") { Category = "Jumping", Duration = 30},
                new Effect("Trigger Jump", "jump") { Category = "Jumping"},

                new Effect("Give Boom Mic", "giveitem_0") { Category = "Items"},
                new Effect("Give Camera", "giveitem_1") { Category = "Items"},
                new Effect("Give Clapper", "giveitem_3") { Category = "Items"},
                new Effect("Give Defibrilator", "giveitem_4") { Category = "Items"},
                new Effect("Give Flare", "giveitem_7") { Category = "Items"},
                new Effect("Give Gooball", "giveitem_8") { Category = "Items"},
                new Effect("Give Hugger", "giveitem_10") { Category = "Items"},
                new Effect("Give Long Flashlight", "giveitem_12") { Category = "Items"},
                new Effect("Give Party Popper", "giveitem_17") { Category = "Items"},
                new Effect("Give Shockstick", "giveitem_19") { Category = "Items"},
                new Effect("Give Sound Player", "giveitem_20") { Category = "Items"},
                new Effect("Give Chorby", "giveitem_30") { Category = "Items"},
                new Effect("Give Brain on a Stick", "giveitem_29") { Category = "Items"},
                new Effect("Give Radio", "giveitem_33") { Category = "Items"},
                new Effect("Give Skull", "giveitem_35") { Category = "Items"},
                
                /*
                new Effect("Give Item 0", "giveitem_0") { Category = "Items"},
                new Effect("Give Item 1", "giveitem_1") { Category = "Items"},
                new Effect("Give Item 2", "giveitem_2") { Category = "Items"},
                new Effect("Give Item 3", "giveitem_3") { Category = "Items"},
                new Effect("Give Item 4", "giveitem_4") { Category = "Items"},
                new Effect("Give Item 5", "giveitem_5") { Category = "Items"},
                new Effect("Give Item 6", "giveitem_6") { Category = "Items"},
                new Effect("Give Item 7", "giveitem_7") { Category = "Items"},
                new Effect("Give Item 8", "giveitem_8") { Category = "Items"},
                new Effect("Give Item 9", "giveitem_9") { Category = "Items"},
                new Effect("Give Item 10", "giveitem_10") { Category = "Items"},
                new Effect("Give Item 11", "giveitem_11") { Category = "Items"},
                new Effect("Give Item 12", "giveitem_12") { Category = "Items"},
                new Effect("Give Item 13", "giveitem_13") { Category = "Items"},
                new Effect("Give Item 14", "giveitem_14") { Category = "Items"},
                new Effect("Give Item 15", "giveitem_15") { Category = "Items"},
                new Effect("Give Item 16", "giveitem_16") { Category = "Items"},
                new Effect("Give Item 17", "giveitem_17") { Category = "Items"},
                new Effect("Give Item 18", "giveitem_18") { Category = "Items"},
                new Effect("Give Item 19", "giveitem_19") { Category = "Items"},
                new Effect("Give Item 20", "giveitem_20") { Category = "Items"},
                new Effect("Give Item 21", "giveitem_21") { Category = "Items"},
                new Effect("Give Item 22", "giveitem_22") { Category = "Items"},
                new Effect("Give Item 23", "giveitem_23") { Category = "Items"},
                new Effect("Give Item 24", "giveitem_24") { Category = "Items"},
                new Effect("Give Item 25", "giveitem_25") { Category = "Items"},
                new Effect("Give Item 26", "giveitem_26") { Category = "Items"},
                new Effect("Give Item 27", "giveitem_27") { Category = "Items"},
                new Effect("Give Item 28", "giveitem_28") { Category = "Items"},
                new Effect("Give Item 29", "giveitem_29") { Category = "Items"},
                new Effect("Give Item 30", "giveitem_30") { Category = "Items"},
                new Effect("Give Item 31", "giveitem_31") { Category = "Items"},
                new Effect("Give Item 32", "giveitem_32") { Category = "Items"},
                new Effect("Give Item 33", "giveitem_33") { Category = "Items"},
                new Effect("Give Item 34", "giveitem_34") { Category = "Items"},
                new Effect("Give Item 35", "giveitem_35") { Category = "Items"},
                new Effect("Give Item 36", "giveitem_36") { Category = "Items"},
                new Effect("Give Item 37", "giveitem_37") { Category = "Items"},
                new Effect("Give Item 38", "giveitem_38") { Category = "Items"},
                new Effect("Give Item 39", "giveitem_39") { Category = "Items"},
                new Effect("Give Item 40", "giveitem_40") { Category = "Items"},
                */

                new Effect("Give Teammate Boom Mic", "cgiveitem_0") { Category = "Items"},
                new Effect("Give Teammate Camera", "cgiveitem_1") { Category = "Items"},
                new Effect("Give Teammate Clapper", "cgiveitem_3") { Category = "Items"},
                new Effect("Give Teammate Defibrilator", "cgiveitem_4") { Category = "Items"},
                new Effect("Give Teammate Flare", "cgiveitem_7") { Category = "Items"},
                new Effect("Give Teammate Gooball", "cgiveitem_8") { Category = "Items"},
                new Effect("Give Teammate Hugger", "cgiveitem_10") { Category = "Items"},
                new Effect("Give Teammate Long Flashlight", "cgiveitem_12") { Category = "Items"},
                new Effect("Give Teammate Party Popper", "cgiveitem_17") { Category = "Items"},
                new Effect("Give Teammate Shockstick", "cgiveitem_19") { Category = "Items"},
                new Effect("Give Teammate Sound Player", "cgiveitem_20") { Category = "Items"},
                new Effect("Give Teammate Chorby", "cgiveitem_30") { Category = "Items"},
                new Effect("Give Teammate Brain on a Stick", "cgiveitem_29") { Category = "Items"},
                new Effect("Give Teammate Radio", "cgiveitem_33") { Category = "Items"},
                new Effect("Give Teammate Skull", "cgiveitem_35") { Category = "Items"},

                new Effect("Drop Item", "dropitem") { Category = "Items"},
                new Effect("Take Item", "takeitem") { Category = "Items"},

                new Effect("Spawn Jello", "spawn_Jello") { Category = "Monsters"},
                new Effect("Spawn Zombie", "spawn_Zombe") { Category = "Monsters"},
                new Effect("Spawn Spider", "spawn_Spider") { Category = "Monsters"},
                new Effect("Spawn Snatcho", "spawn_Snatcho") { Category = "Monsters"},
                new Effect("Spawn Bombs", "spawn_Bombs") { Category = "Monsters"},
                new Effect("Spawn Barnacle Ball", "spawn_BarnacleBall") { Category = "Monsters"},
                new Effect("Spawn Dog", "spawn_Dog") { Category = "Monsters"},
                new Effect("Spawn Ear", "spawn_Ear") { Category = "Monsters"},
                new Effect("Spawn Big Slap", "spawn_BigSlap") { Category = "Monsters"},
                new Effect("Spawn Eye Guy", "spawn_EyeGuy") { Category = "Monsters"},
                new Effect("Spawn Flicker", "spawn_Flicker") { Category = "Monsters"},
                new Effect("Spawn Harpoon", "spawn_Harpoon") { Category = "Monsters"},
                new Effect("Spawn Mouthe", "spawn_Mouthe") { Category = "Monsters"},
                new Effect("Spawn Whisk", "spawn_Whisk") { Category = "Monsters"},
                new Effect("Spawn Weeping", "spawn_Weeping") { Category = "Monsters"},

                new Effect("Spawn Jello at Teammate", "cspawn_Jello") { Category = "Monsters"},
                new Effect("Spawn Zombie at Teammate", "cspawn_Zombe") { Category = "Monsters"},
                new Effect("Spawn Spider at Teammate", "cspawn_Spider") { Category = "Monsters"},
                new Effect("Spawn Snatcho at Teammate", "cspawn_Snatcho") { Category = "Monsters"},
                new Effect("Spawn Bombs at Teammate", "cspawn_Bombs") { Category = "Monsters"},
                new Effect("Spawn Barnacle Ball at Teammate", "cspawn_BarnacleBall") { Category = "Monsters"},
                new Effect("Spawn Dog at Teammate", "cspawn_Dog") { Category = "Monsters"},
                new Effect("Spawn Ear at Teammate", "cspawn_Ear") { Category = "Monsters"},
                new Effect("Spawn Big Slap at Teammate", "cspawn_BigSlap") { Category = "Monsters"},
                new Effect("Spawn Eye Guy at Teammate", "cspawn_EyeGuy") { Category = "Monsters"},
                new Effect("Spawn Flicker at Teammate", "cspawn_Flicker") { Category = "Monsters"},
                new Effect("Spawn Harpoon at Teammate", "cspawn_Harpoon") { Category = "Monsters"},
                new Effect("Spawn Mouthe at Teammate", "cspawn_Mouthe") { Category = "Monsters"},
                new Effect("Spawn Whisk at Teammate", "cspawn_Whisk") { Category = "Monsters"},
                new Effect("Spawn Weeping at Teammate", "cspawn_Weeping") { Category = "Monsters"},

                new Effect("Shake Screen", "shake") { Category = "Camera", Duration = 30},
                new Effect("Shake Screen - Long", "shakebig") { Category = "Camera", Duration = 30},
                new Effect("Wide Camera", "widecam") { Category = "Camera", Duration = 30},
                new Effect("Narrow Camera", "narrowcam") { Category = "Camera", Duration = 30},
                //new Effect("Invert Camera", "flipcam") { Category = "Camera", Duration = 30},

                new Effect("Give $10", "money_10") { Category = "Money"},
                new Effect("Give $100", "money_100") { Category = "Money"},
                new Effect("Give $1000", "money_1000") { Category = "Money"},
                new Effect("Take $10", "money_-10") { Category = "Money"},
                new Effect("Take $100", "money_-100") { Category = "Money"},
                new Effect("Take $1000", "money_-1000") { Category = "Money"},

                new Effect("Give 10% View Quota", "views_10") { Category = "Views"},
                new Effect("Give 30% View Quota", "views_30") { Category = "Views"},
                new Effect("Give 100% View Quota", "views_100") { Category = "Views"},
                new Effect("Take 10% View Quota", "views_-10") { Category = "Views"},
                new Effect("Take 30% View Quota", "views_-30") { Category = "Views"},
                new Effect("Take 100% View Quota", "views_-100") { Category = "Views"},

                new Effect("Open Dive Bell", "opendoor"),
                new Effect("Close Dive Bell", "closedoor"),
        };
    }
}
