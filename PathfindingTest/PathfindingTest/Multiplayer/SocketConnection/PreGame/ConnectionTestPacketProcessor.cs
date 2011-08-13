using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketLibrary.Packets;
using SocketLibrary.Protocol;
using PathfindingTest.Multiplayer.PreGame.SocketConnection;
using XNAInterfaceComponents.Components;
using PathfindingTest.UI.Menus;
using XNAInterfaceComponents.AbstractComponents;
using PathfindingTest.UI.Menus.Multiplayer;

namespace PathfindingTest.Multiplayer.SocketConnection.PreGame
{
    public class ConnectionTestPacketProcessor
    {
        public void DataReceived(Packet p)
        {
            switch (p.GetHeader())
            {
                case TestHeaders.STEADY_TEST:
                    {
                        ParentComponent menu = MenuManager.GetInstance().GetCurrentlyDisplayedMenu();
                        if (menu is TestConnectionMenu)
                        {
                            TestConnectionMenu testMenu = ((TestConnectionMenu)menu);

                            testMenu.steadyPacketTestBar.currentValue++;
                        }
                        break;
                    }
                case TestHeaders.BURST_TEST:
                    {
                        ParentComponent menu = MenuManager.GetInstance().GetCurrentlyDisplayedMenu();
                        if (menu is TestConnectionMenu)
                        {
                            TestConnectionMenu testMenu = ((TestConnectionMenu)menu);

                            testMenu.burstPacketTestBar.currentValue++;
                        }
                        break;
                    }
                case TestHeaders.MALFORM_TEST:
                    {
                        ParentComponent menu = MenuManager.GetInstance().GetCurrentlyDisplayedMenu();
                        if (menu is TestConnectionMenu)
                        {
                            TestConnectionMenu testMenu = ((TestConnectionMenu)menu);

                            String hash = PacketUtil.DecodePacketString(p, 0);
                            foreach (Packet toTest in testMenu.malformPacketsSent)
                            {
                                if (toTest == null)
                                {
                                    Console.Out.WriteLine("Unable to find packet with hash = " + hash);
                                    break;
                                }
                                if (PacketUtil.DecodePacketString(toTest, 0) == hash)
                                {
                                    testMenu.malformPacketTestBar.currentValue++;
                                    break;
                                }
                            }
                        }
                        break;
                    }
            }
        }
    }
}
