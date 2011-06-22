using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketLibrary.Packets;

namespace SocketLibrary.Protocol
{
    public class Logger
    {

        public LinkedList<LogMessage> messageLog = new LinkedList<LogMessage>();

        public struct LogMessage
        {
            public String message;
            public Boolean received;

            public LogMessage(String message, Boolean received)
            {
                this.message = message;
                this.received = received;
            }
        }

        /// <summary>
        /// Logs the data to the logger.
        /// </summary>
        /// <param name="data"></param>
        public void Log(Packet p, Boolean isReceived)
        {
            String currTime = System.DateTime.Now.ToLongTimeString() + "," + System.DateTime.Now.Millisecond + " ";
            switch (p.GetHeader())
            {
                case Headers.HANDSHAKE_1:
                    {
                        if (isReceived) this.messageLog.AddLast(new LogMessage(currTime + "HANDSHAKE_1 Received handshake request (1)", isReceived));
                        else this.messageLog.AddLast(new LogMessage(currTime + "HANDSHAKE_1 Sent handshake request (1)", isReceived));
                        break;
                    }
                case Headers.HANDSHAKE_2:
                    {
                        if (isReceived) this.messageLog.AddLast(new LogMessage(currTime + "HANDSHAKE_2 Received handshake acknowledge (2)", isReceived));
                        else this.messageLog.AddLast(new LogMessage(currTime + "HANDSHAKE_2 Sent handshake acknowledge (2)", isReceived));
                        break;
                    }
                case Headers.HANDSHAKE_3:
                    {
                        if (isReceived) this.messageLog.AddLast(new LogMessage(currTime + "HANDSHAKE_3 Received handshake confirmation (3)", isReceived));
                        else this.messageLog.AddLast(new LogMessage(currTime + "HANDSHAKE_3 Sent handshake confirmation (3)", isReceived));
                        break;
                    }
                case Headers.KEEP_ALIVE:
                    {
                        if (isReceived) this.messageLog.AddLast(new LogMessage(currTime + "KEEP_ALIVE User is still connected (still alive)", isReceived));
                        else this.messageLog.AddLast(new LogMessage(currTime + "KEEP_ALIVE Asking client if he's still alive ", isReceived));
                        break;
                    }
                case Headers.CHAT_MESSAGE:
                    {
                        if (isReceived)
                            this.messageLog.AddLast(new LogMessage(currTime + "CHAT_MESSAGE Received sent a chat message in channel " + PacketUtil.DecodePacketInt(p, 0) +
                                ": " + PacketUtil.DecodePacketString(p, 4), isReceived));
                        else this.messageLog.AddLast(new LogMessage(currTime + "CHAT_MESSAGE Sent chat message to all in channel " + PacketUtil.DecodePacketInt(p, 0) +
                                ": " + PacketUtil.DecodePacketString(p, 4), isReceived));
                        break;
                    }
                case Headers.CLIENT_DISCONNECT:
                    {
                        if (isReceived) this.messageLog.AddLast(new LogMessage(currTime + "CLIENT_DISCONNECT Client disconnected", isReceived));
                        else this.messageLog.AddLast(new LogMessage(currTime + "CLIENT_DISCONNECT Disconnected this client", isReceived));
                        break;
                    }
                case Headers.SERVER_DISCONNECT:
                    {
                        if (isReceived) this.messageLog.AddLast(new LogMessage(currTime + "SERVER_DISCONNECT Server disconnected", isReceived));
                        else this.messageLog.AddLast(new LogMessage(currTime + "SERVER_DISCONNECT Disconnected from server", isReceived));
                        break;
                    }
                case Headers.CLIENT_USERNAME:
                    {
                        if (isReceived) this.messageLog.AddLast(new LogMessage(currTime + "CLIENT_USERNAME Requested a username: " +
                            PacketUtil.DecodePacketString(p, 0), isReceived));
                        else this.messageLog.AddLast(new LogMessage(currTime + "CLIENT_USERNAME Confirming requested username: " + PacketUtil.DecodePacketString(p, 0), isReceived));
                        break;
                    }
                case Headers.CLIENT_USER_ID:
                    {
                        if (isReceived) this.messageLog.AddLast(new LogMessage(currTime + "CLIENT_USER_ID Requested a userid: " +
                            PacketUtil.DecodePacketInt(p, 0), isReceived));
                        else this.messageLog.AddLast(new LogMessage(currTime + "CLIENT_USER_ID Confirming requested userid: " + PacketUtil.DecodePacketInt(p, 0), isReceived));
                        break;
                    }
                case Headers.CLIENT_CHANNEL:
                    {
                        if (isReceived) this.messageLog.AddLast(new LogMessage(currTime + "CLIENT_CHANNEL Received change channel request to " +
                            PacketUtil.DecodePacketInt(p, 0), isReceived));
                        else this.messageLog.AddLast(new LogMessage(currTime + "CLIENT_CHANNEL Sent change channel to " +
                            PacketUtil.DecodePacketInt(p, 0), isReceived));
                        break;
                    }
                case Headers.NEW_USER:
                    {
                        if (isReceived) this.messageLog.AddLast(new LogMessage(currTime + "NEW_USER Received new user: " +
                            PacketUtil.DecodePacketString(p, 4), isReceived));
                        else this.messageLog.AddLast(new LogMessage(currTime + "NEW_USER Sent new user: " +
                            PacketUtil.DecodePacketString(p, 4), isReceived));
                        break;
                    }
                case Headers.USER_LEFT:
                    {
                        if (isReceived) this.messageLog.AddLast(new LogMessage(currTime + "USER_LEFT Received user has left: " +
                            PacketUtil.DecodePacketString(p, 4), isReceived));
                        else this.messageLog.AddLast(new LogMessage(currTime + "USER_LEFT Sent user has left: " +
                            PacketUtil.DecodePacketString(p, 4), isReceived));
                        break;
                    }
                case Headers.CLIENT_CREATE_GAME:
                    {
                        if (isReceived) this.messageLog.AddLast(new LogMessage(currTime + "CLIENT_CREATE_GAME Received game creation request: userid = " +
                            PacketUtil.DecodePacketInt(p, 0) + ", gamename = " +
                            PacketUtil.DecodePacketString(p, 4), isReceived));
                        else this.messageLog.AddLast(new LogMessage(currTime + "CLIENT_CREATE_GAME Sent game creation request: userid = " +
                            PacketUtil.DecodePacketInt(p, 0) + ", gamename = " +
                            PacketUtil.DecodePacketString(p, 4), isReceived));
                        break;
                    }
                case Headers.SERVER_CREATE_GAME:
                    {
                        if (isReceived) this.messageLog.AddLast(new LogMessage(currTime + "SERVER_CREATE_GAME Received server game creation request: " +
                            "gameid = " + PacketUtil.DecodePacketInt(p, 0) +
                            ", gamename = " + PacketUtil.DecodePacketString(p, 4), isReceived));
                        else this.messageLog.AddLast(new LogMessage(currTime + "SERVER_CREATE_GAME Sent server game creation request: " +
                            "gameid = " + PacketUtil.DecodePacketInt(p, 0) +
                            ", gamename = " + PacketUtil.DecodePacketString(p, 4), isReceived));
                        break;
                    }
                case Headers.GAME_ID:
                    {
                        if (isReceived) this.messageLog.AddLast(new LogMessage(currTime + "GAME_ID Received a game ID: " +
                            PacketUtil.DecodePacketInt(p, 0), isReceived));
                        else this.messageLog.AddLast(new LogMessage(currTime + "GAME_ID Sent a game ID to use: " +
                           PacketUtil.DecodePacketInt(p, 0), isReceived));
                        break;
                    }
                case Headers.GAME_MAP_CHANGED:
                    {
                        if (isReceived) this.messageLog.AddLast(new LogMessage(currTime + "GAME_MAP_CHANGED Received game map of game id = " +
                            PacketUtil.DecodePacketInt(p, 0) + " has changed to -> " +
                            PacketUtil.DecodePacketString(p, 4), isReceived));
                        else this.messageLog.AddLast(new LogMessage(currTime + "GAME_MAP_CHANGED Sent that game map of game id = " +
                           PacketUtil.DecodePacketInt(p, 0) + " has changed to -> " +
                           PacketUtil.DecodePacketString(p, 4), isReceived));
                        break;
                    }
                case Headers.CLIENT_DESTROY_GAME:
                    {
                        if (isReceived) this.messageLog.AddLast(new LogMessage(currTime + "CLIENT_DESTROY_GAME Received destroy game request by client!", isReceived));
                        else this.messageLog.AddLast(new LogMessage(currTime + "CLIENT_DESTROY_GAME Sent destroy game request.", isReceived));
                        break;
                    }
                case Headers.SERVER_DESTROY_GAME:
                    {
                        if (isReceived) this.messageLog.AddLast(new LogMessage(currTime + "SERVER_DESTROY_GAME Received destroy game request of game " +
                           PacketUtil.DecodePacketInt(p, 0), isReceived));
                        else this.messageLog.AddLast(new LogMessage(currTime + "SERVER_DESTROY_GAME Sent destroy game request of game " +
                           PacketUtil.DecodePacketInt(p, 0) + " to all clients.", isReceived));
                        break;
                    }
                case Headers.CLIENT_REQUEST_JOIN:
                    {
                        if (isReceived) this.messageLog.AddLast(new LogMessage(currTime + "CLIENT_REQUEST_JOIN Received request to join game " +
                            PacketUtil.DecodePacketInt(p, 0), isReceived));
                        else this.messageLog.AddLast(new LogMessage(currTime + "CLIENT_REQUEST_JOIN Sent request to join game " +
                            PacketUtil.DecodePacketInt(p, 0), isReceived));
                        break;
                    }
                case Headers.SERVER_REQUEST_JOIN:
                    {
                        if (isReceived) this.messageLog.AddLast(new LogMessage(currTime + "SERVER_REQUEST_JOIN Received request if user " +
                            PacketUtil.DecodePacketInt(p, 4) + " can join my game " +
                            PacketUtil.DecodePacketInt(p, 0), isReceived));
                        else this.messageLog.AddLast(new LogMessage(currTime + "SERVER_REQUEST_JOIN Sent request if user " +
                            PacketUtil.DecodePacketInt(p, 4) + " can join my game " +
                            PacketUtil.DecodePacketInt(p, 0), isReceived));
                        break;
                    }
                case Headers.CLIENT_OK_JOIN:
                    {
                        if (isReceived) this.messageLog.AddLast(new LogMessage(currTime + "CLIENT_OK_JOIN Received OK to join game. ", isReceived));
                        else this.messageLog.AddLast(new LogMessage(currTime + "CLIENT_OK_JOIN Sent OK to join game. ", isReceived));
                        break;
                    }
                case Headers.CLIENT_GAME_FULL:
                    {
                        if (isReceived) this.messageLog.AddLast(new LogMessage(currTime + "CLIENT_GAME_FULL Received game full message. ", isReceived));
                        else this.messageLog.AddLast(new LogMessage(currTime + "CLIENT_GAME_FULL Sent game full message.", isReceived));
                        break;
                    }
                case Headers.CLIENT_LEFT_GAME:
                    {
                        if (isReceived) this.messageLog.AddLast(new LogMessage(currTime + "CLIENT_LEFT_GAME Received client has left game message. ", isReceived));
                        else this.messageLog.AddLast(new LogMessage(currTime + "CLIENT_LEFT_GAME Sent client has left message.", isReceived));
                        break;
                    }
                case Headers.GAME_COLOR_CHANGED:
                    {
                        if (isReceived) this.messageLog.AddLast(new LogMessage(currTime + "GAME_COLOR_CHANGED Received client has changed color message: color = " +
                            PacketUtil.DecodePacketInt(p, 8), isReceived));
                        else this.messageLog.AddLast(new LogMessage(currTime + "GAME_COLOR_CHANGED Sent client has changed color message: color = " +
                            PacketUtil.DecodePacketInt(p, 8), isReceived));
                        break;
                    }
                case Headers.GAME_TEAM_CHANGED:
                    {
                        if (isReceived) this.messageLog.AddLast(new LogMessage(currTime + "GAME_TEAM_CHANGED Received client has changed team message: team = " +
                            PacketUtil.DecodePacketInt(p, 8), isReceived));
                        else this.messageLog.AddLast(new LogMessage(currTime + "GAME_TEAM_CHANGED Sent client has changed team message: team = " +
                            PacketUtil.DecodePacketInt(p, 8), isReceived));
                        break;
                    }
                case Headers.GAME_READY_CHANGED:
                    {
                        if (isReceived) this.messageLog.AddLast(new LogMessage(currTime + "GAME_READY_CHANGED Received client has changed ready state: " +
                            PacketUtil.DecodePacketInt(p, 8), isReceived));
                        else this.messageLog.AddLast(new LogMessage(currTime + "GAME_READY_CHANGED Sent client has changed ready state: " +
                            PacketUtil.DecodePacketInt(p, 8), isReceived));
                        break;
                    }
                case Headers.GAME_KICK_CLIENT:
                    {
                        if (isReceived) this.messageLog.AddLast(new LogMessage(currTime + "GAME_KICK_CLIENT Received client kick message: " +
                            PacketUtil.DecodePacketInt(p, 0), isReceived));
                        else this.messageLog.AddLast(new LogMessage(currTime + "GAME_KICK_CLIENT Sent client kick message: " +
                            PacketUtil.DecodePacketInt(p, 0), isReceived));
                        break;
                    }
                case Headers.CLIENT_GAME_START:
                    {
                        if (isReceived) this.messageLog.AddLast(new LogMessage(currTime + "CLIENT_GAME_START Received client game start message: ", isReceived));
                        else this.messageLog.AddLast(new LogMessage(currTime + "CLIENT_GAME_START Sent client game start message: ", isReceived));
                        break;
                    }
                case Headers.SERVER_GAME_START:
                    {
                        if (isReceived) this.messageLog.AddLast(new LogMessage(currTime + "SERVER_GAME_START Received server game start message: " +
                            PacketUtil.DecodePacketInt(p, 0), isReceived));
                        else this.messageLog.AddLast(new LogMessage(currTime + "SERVER_GAME_START Sent server game start message: " +
                            PacketUtil.DecodePacketInt(p, 0), isReceived));
                        break;
                    }
                case Headers.GAME_REQUEST_OBJECT_ID:
                    {
                        if (isReceived) this.messageLog.AddLast(new LogMessage(currTime + "GAME_REQUEST_OBJECT_ID Received client wants object ID message: local = " +
                            PacketUtil.DecodePacketInt(p, 0), isReceived));
                        else this.messageLog.AddLast(new LogMessage(currTime + "GAME_REQUEST_OBJECT_ID Sent server ID package: local = " +
                            PacketUtil.DecodePacketInt(p, 0), isReceived));
                        break;
                    }
                case Headers.GAME_OBJECT_ID:
                    {
                        if (isReceived) this.messageLog.AddLast(new LogMessage(currTime + "GAME_OBJECT_ID Received client wants object ID message: local = " +
                            PacketUtil.DecodePacketInt(p, 0) + ", serverID = " +
                            PacketUtil.DecodePacketInt(p, 4), isReceived));
                        else this.messageLog.AddLast(new LogMessage(currTime + "GAME_OBJECT_ID Sent server ID package: local = " +
                            PacketUtil.DecodePacketInt(p, 0) + ", serverID = " +
                            PacketUtil.DecodePacketInt(p, 4), isReceived));
                        break;
                    }
                case UnitHeaders.GAME_UNIT_LOCATION:
                    {
                        if (isReceived) this.messageLog.AddLast(new LogMessage(currTime + "GAME_UNIT_LOCATION Received client sync unit request (" +
                        PacketUtil.DecodePacketInt(p, 0) + ") target(" +
                        PacketUtil.DecodePacketInt(p, 4) + ", " +
                        PacketUtil.DecodePacketInt(p, 8) + "), current("+
                        PacketUtil.DecodePacketInt(p, 12) + ", " +
                        PacketUtil.DecodePacketInt(p, 16) + ")", isReceived));
                        else this.messageLog.AddLast(new LogMessage(currTime + "GAME_UNIT_LOCATION Sent client sync unit request (" +
                        PacketUtil.DecodePacketInt(p, 0) + ") target(" +
                        PacketUtil.DecodePacketInt(p, 4) + ", " +
                        PacketUtil.DecodePacketInt(p, 8) + "), current("+
                        PacketUtil.DecodePacketInt(p, 12) + ", " +
                        PacketUtil.DecodePacketInt(p, 16) + ")", isReceived));
                        break;
                    }
                case UnitHeaders.GAME_NEW_UNIT:
                    {
                        if (isReceived) this.messageLog.AddLast(new LogMessage(currTime + "GAME_NEW_UNIT Received client wants to create new unit: player = " +
                            PacketUtil.DecodePacketInt(p, 0) + ", serverID = " + +
                            PacketUtil.DecodePacketInt(p, 4) + ", type = " + +
                            PacketUtil.DecodePacketInt(p, 8), isReceived));
                        else this.messageLog.AddLast(new LogMessage(currTime + "GAME_NEW_UNIT Sent create new unit: player = " +
                            PacketUtil.DecodePacketInt(p, 0) + ", serverID = " +
                            PacketUtil.DecodePacketInt(p, 4) + ", type = " +
                            PacketUtil.DecodePacketInt(p, 8), isReceived));
                        break;
                    }
                case UnitHeaders.GAME_REQUEST_UNIT_DATA:
                    {
                        if (isReceived) this.messageLog.AddLast(new LogMessage(currTime + "GAME_REQUEST_UNIT_DATA Received client wants info about a unit: player = " +
                            PacketUtil.DecodePacketInt(p, 0) + ", serverID = " + +
                            PacketUtil.DecodePacketInt(p, 4), isReceived));
                        else this.messageLog.AddLast(new LogMessage(currTime + "GAME_REQUEST_UNIT_DATA Sent info about a unit: player = " +
                            PacketUtil.DecodePacketInt(p, 0) + ", serverID = " +
                            PacketUtil.DecodePacketInt(p, 4), isReceived));
                        break;
                    }
                case UnitHeaders.GAME_SEND_UNIT_DATA:
                    {
                        if (isReceived) this.messageLog.AddLast(new LogMessage(currTime + "GAME_SEND_UNIT_DATA Received info about a unit: targetPlayer = " +
                            PacketUtil.DecodePacketInt(p, 0) + ", ownerID = " +
                            PacketUtil.DecodePacketInt(p, 4) + ", serverID = " +
                            PacketUtil.DecodePacketInt(p, 8) + ", type = " +
                            PacketUtil.DecodePacketInt(p, 12), isReceived));
                        else this.messageLog.AddLast(new LogMessage(currTime + "GAME_SEND_UNIT_DATA Sent data about a unit: targetPlayer = " +
                            PacketUtil.DecodePacketInt(p, 0) + ", ownerID = " +
                            PacketUtil.DecodePacketInt(p, 4) + ", serverID = " +
                            PacketUtil.DecodePacketInt(p, 8) + ", type = " +
                            PacketUtil.DecodePacketInt(p, 12), isReceived));
                        break;
                    }
                case BuildingHeaders.GAME_NEW_BUILDING:
                    {
                        if (isReceived) this.messageLog.AddLast(new LogMessage(currTime + "GAME_NEW_BUILDING Received client wants to create a new building: ownerID = " +
                            PacketUtil.DecodePacketInt(p, 0) + ", serverID = " +
                            PacketUtil.DecodePacketInt(p, 4) + ", type = " +
                            PacketUtil.DecodePacketInt(p, 8) + ", by = " +
                            PacketUtil.DecodePacketInt(p, 12), isReceived));
                        else this.messageLog.AddLast(new LogMessage(currTime + "GAME_NEW_BUILDING Sent create building request: ownerID = " +
                            PacketUtil.DecodePacketInt(p, 0) + ", serverID = " +
                            PacketUtil.DecodePacketInt(p, 4) + ", type = " +
                            PacketUtil.DecodePacketInt(p, 8) + ", by = " +
                            PacketUtil.DecodePacketInt(p, 12), isReceived));
                        break;
                    }
                case BuildingHeaders.GAME_BUILDING_LOCATION:
                    {
                        if (isReceived) this.messageLog.AddLast(new LogMessage(currTime + "GAME_BUILDING_LOCATION Received building location update request: serverID = " +
                            PacketUtil.DecodePacketInt(p, 0) + ", (" +
                            PacketUtil.DecodePacketInt(p, 4) + ", " +
                            PacketUtil.DecodePacketInt(p, 8) + ")", isReceived));
                        else this.messageLog.AddLast(new LogMessage(currTime + "GAME_BUILDING_LOCATION Sent building location request: serverID = " +
                            PacketUtil.DecodePacketInt(p, 0) + ", (" +
                            PacketUtil.DecodePacketInt(p, 4) + ", " +
                            PacketUtil.DecodePacketInt(p, 8) + ")", isReceived));
                        break;
                    }
                case UnitHeaders.GAME_UNIT_MELEE_DAMAGE:
                    {
                        if (isReceived) this.messageLog.AddLast(new LogMessage(currTime + "GAME_UNIT_MELEE_DAMAGE Received unit damage done request: typeSource = " +
                            PacketUtil.DecodePacketInt(p, 0) + ", targetID = " +
                            PacketUtil.DecodePacketInt(p, 4) + ", fromID =  " +
                            PacketUtil.DecodePacketInt(p, 8), isReceived));
                        else this.messageLog.AddLast(new LogMessage(currTime + "GAME_UNIT_MELEE_DAMAGE Sent unit damage done request: typeSource = " +
                            PacketUtil.DecodePacketInt(p, 0) + ", targetID = " +
                            PacketUtil.DecodePacketInt(p, 4) + ", fromID =  " +
                            PacketUtil.DecodePacketInt(p, 8), isReceived));
                        break;
                    }
                case UnitHeaders.GAME_UNIT_RANGED_DAMAGE:
                    {
                        if (isReceived) this.messageLog.AddLast(new LogMessage(currTime + "GAME_UNIT_RANGED_DAMAGE Received unit ranged damage done request: projectileID = " +
                            PacketUtil.DecodePacketInt(p, 0) + ", targetID = " +
                            PacketUtil.DecodePacketInt(p, 4) + ", fromID =  " +
                            PacketUtil.DecodePacketInt(p, 8), isReceived));
                        else this.messageLog.AddLast(new LogMessage(currTime + "GAME_UNIT_RANGED_DAMAGE Sent unit ranged damage done request: projectileID = " +
                            PacketUtil.DecodePacketInt(p, 0) + ", targetID = " +
                            PacketUtil.DecodePacketInt(p, 4) + ", fromID =  " +
                            PacketUtil.DecodePacketInt(p, 8), isReceived));
                        break;
                    }
                case UnitHeaders.GAME_UNIT_RANGED_SHOT:
                    {
                        if (isReceived) this.messageLog.AddLast(new LogMessage(currTime + "GAME_UNIT_RANGED_SHOT Received unit ranged damage done request: arrowID = " +
                            PacketUtil.DecodePacketInt(p, 0) + ", sourceID = " +
                            PacketUtil.DecodePacketInt(p, 4) + ", targetID = " +
                            PacketUtil.DecodePacketInt(p, 8), isReceived));
                        else this.messageLog.AddLast(new LogMessage(currTime + "GAME_UNIT_RANGED_SHOT Sent unit ranged damage done request: arrowID = " +
                            PacketUtil.DecodePacketInt(p, 0) + ", sourceID = " +
                            PacketUtil.DecodePacketInt(p, 4) + ", targetID = " +
                            PacketUtil.DecodePacketInt(p, 8), isReceived));
                        break;
                    }

                /*
                 * 
                    if (isReceived) this.messageLog.AddLast(new LogMessage( currTime + " ",isReceived ));
                    else this.messageLog.AddLast(new LogMessage( currTime + " ",isReceived ));
                 *
                 */
                default:
                    {
                        if (isReceived) this.messageLog.AddLast(new LogMessage(currTime + " Received an unknown request (" + p.GetHeader() + ") "
                            + "(or have you forgotten to add the header to the log?)", isReceived));
                        else this.messageLog.AddLast(new LogMessage(currTime + " Sent unknown request (" + p.GetHeader() + ") "
                            + "(or have you forgotten to add the header to the log?)", isReceived));
                        break;
                    }
            }
        }
    }
}
