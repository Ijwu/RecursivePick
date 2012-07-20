using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hooks;
using TShockAPI;
using Terraria;

namespace RecursivePick
{
    [APIVersion( 1,12)]
    public class RecursiveMain : TerrariaPlugin
    {
        bool[] players;

        public override string Author
        {
            get { return "Ijwu"; }
        }

        public override string Description
        {
            get { return "Recursive Tile Breaking"; }
        }

        public override string Name
        {
            get { return "Recursive Pick"; }
        }

        public override Version Version
        {
            get { return new Version(1, 0, 0, 0); }
        }

        public RecursiveMain(Main game)
            : base(game)
        {
            players = new bool[255];
            for( int i = 0; i < players.Length; i++ )
            {
                players[i] = false;
            }
        }

        public override void Initialize()
        {
            Hooks.ServerHooks.Leave += OnLeave;
            GetDataHandlers.TileEdit += OnTileEdit;
            Commands.ChatCommands.Add(new Command("recursivepick", RecurveToggle, "recursive"));
        }

        private void OnLeave( int who )
        {
            players[who] = false;
        }

        private void OnTileEdit( object sender, GetDataHandlers.TileEditEventArgs args )
        {
            if (players[args.Player.Index])
            {
                switch (args.EditType)
                {
                    case 0:
                    case 4:
                        {
                            Recursive re = new Recursive();
                            List<Vector2> del = re.RecursiveEdit(args.X, args.Y, args.EditType);
                            foreach (Vector2 tip in del)
                            {
                                WorldGen.KillTile(Convert.ToInt16(tip.X), Convert.ToInt16(tip.Y));
                                TSPlayer.All.SendTileSquare(Convert.ToInt16(tip.X), Convert.ToInt16(tip.Y), 1);
                            }
                            break;
                        }
                    case 2:
                        {
                            Recursive re = new Recursive();
                            List<Vector2> del = re.RecursiveEdit(args.X, args.Y, args.EditType);
                            foreach (Vector2 tip in del)
                            {
                                WorldGen.KillWall(Convert.ToInt16(tip.X), Convert.ToInt16(tip.Y));
                                TSPlayer.All.SendTileSquare(Convert.ToInt16(tip.X), Convert.ToInt16(tip.Y), 1);
                            }
                            break;
                        }
                    case 6:
                        {
                            Recursive re = new Recursive();
                            List<Vector2> del = re.RecursiveEdit(args.X, args.Y, args.EditType);
                            foreach (Vector2 tip in del)
                            {
                                WorldGen.KillWire(Convert.ToInt16(tip.X), Convert.ToInt16(tip.Y));
                                TSPlayer.All.SendTileSquare(Convert.ToInt16(tip.X), Convert.ToInt16(tip.Y), 1);
                            }
                            break;
                        }
                }
            }
        }

        private void RecurveToggle(CommandArgs args)
        {
            players[args.Player.Index] = (!players[args.Player.Index]);
            args.Player.SendMessage(String.Format("You have {0} recursivepick.", (players[args.Player.Index] ? "enabled" : "disabled")), Color.Aqua);
        }
       
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServerHooks.Leave -= OnLeave;
                GetDataHandlers.TileEdit -= OnTileEdit;
            }

            base.Dispose(disposing);
        }
    }
}
