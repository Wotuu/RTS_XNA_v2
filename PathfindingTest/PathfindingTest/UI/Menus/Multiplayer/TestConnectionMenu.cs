using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XNAInterfaceComponents.Components;
using Microsoft.Xna.Framework;
using XNAInterfaceComponents.ChildComponents;
using XNAInterfaceComponents.AbstractComponents;
using System.Threading;
using SocketLibrary.Protocol;
using SocketLibrary.Packets;
using PathfindingTest.Multiplayer.PreGame.SocketConnection;
using SocketLibrary.Users;
using System.Security.Cryptography;

namespace PathfindingTest.UI.Menus.Multiplayer
{
    public class TestConnectionMenu : XNAPanel
    {

        public XNALabel ipLbl { get; set; }
        public XNATextField ipTF { get; set; }
        public XNAButton startBtn { get; set; }

        public XNALabel serverResponseLbl { get; set; }
        public XNAProgressBar serverResponseBar { get; set; }

        public XNALabel steadyPacketTestLbl { get; set; }
        public XNAProgressBar steadyPacketTestBar { get; set; }

        public XNALabel burstPacketTestLbl { get; set; }
        public XNAProgressBar burstPacketTestBar { get; set; }

        public XNALabel malformPacketTestLbl { get; set; }
        public XNAProgressBar malformPacketTestBar { get; set; }

        public Packet[] malformPacketsSent { get; set; }



        public int steadyTestMaxCount = 2500;
        public int burstTestMaxCount = 2500;
        public int burstCount = 100;

        public int malformTestMaxCount = 2500;



        public XNALabel statusLbl { get; set; }
        public XNAButton backBtn { get; set; }

        public TestConnectionMenu()
            : base(null,
                new Rectangle((Game1.GetInstance().graphics.PreferredBackBufferWidth / 2) - 200,
                (Game1.GetInstance().graphics.PreferredBackBufferHeight / 2) - 200, 400, 400))
        {

            ipLbl = new XNALabel(this, new Rectangle(10, 10, 100, 35), "IP");
            ipLbl.border = null;
            ipLbl.textAlign = XNALabel.TextAlign.RIGHT;
            ipLbl.font = MenuManager.BIG_TEXTFIELD_FONT;

            ipTF = new XNATextField(this, new Rectangle(120, 10, 100, 35), 1);
            ipTF.font = MenuManager.BIG_TEXTFIELD_FONT;
            ipTF.text = "localhost";

            startBtn = new XNAButton(this, new Rectangle(240, 10, 100, 35), "Start");
            startBtn.onClickListeners += this.TestConnection;



            serverResponseLbl = new XNALabel(this, new Rectangle(10, 60, 100, 20), "Server connection:");
            serverResponseLbl.border = null;

            serverResponseBar = new XNAProgressBar(this, new Rectangle(150, 60, 220, 20), 100);
            serverResponseBar.progressDisplayLabel.font = MenuManager.PROGRESSBAR_FONT;
            serverResponseBar.progressDisplayLabel.fontColor = Color.White;


            steadyPacketTestLbl = new XNALabel(this, new Rectangle(10, 90, 100, 20), "Steady packets received: ");
            steadyPacketTestLbl.border = null;

            steadyPacketTestBar = new XNAProgressBar(this, new Rectangle(150, 90, 220, 20), steadyTestMaxCount);
            steadyPacketTestBar.progressDisplayLabel.font = MenuManager.PROGRESSBAR_FONT;
            steadyPacketTestBar.progressDisplayLabel.fontColor = Color.White;


            burstPacketTestLbl = new XNALabel(this, new Rectangle(10, 120, 100, 20), "Burst packets received: ");
            burstPacketTestLbl.border = null;

            burstPacketTestBar = new XNAProgressBar(this, new Rectangle(150, 120, 220, 20), 100);
            burstPacketTestBar.progressDisplayLabel.font = MenuManager.PROGRESSBAR_FONT;
            burstPacketTestBar.progressDisplayLabel.fontColor = Color.White;


            malformPacketTestLbl = new XNALabel(this, new Rectangle(10, 150, 100, 20), "Malform packets correct: ");
            malformPacketTestLbl.border = null;

            malformPacketTestBar = new XNAProgressBar(this, new Rectangle(150, 150, 220, 20), malformTestMaxCount);
            malformPacketTestBar.progressDisplayLabel.font = MenuManager.PROGRESSBAR_FONT;
            malformPacketTestBar.progressDisplayLabel.fontColor = Color.White;

            malformPacketsSent = new Packet[malformTestMaxCount];






            statusLbl = new XNALabel(this, new Rectangle(this.bounds.Width / 2 - 50, this.bounds.Height - 80, 100, 30), "Status");
            statusLbl.border = null;
            statusLbl.textAlign = XNALabel.TextAlign.CENTER;

            backBtn = new XNAButton(this, new Rectangle(this.bounds.Width / 2 - 50, this.bounds.Height - 40, 100, 30), "Back");
            backBtn.onClickListeners += this.OnBackBtnClick;
        }

        /// <summary>
        /// Starts the connection to the server, and tests the connection.
        /// </summary>
        public void TestConnection(XNAButton source)
        {
            ChatServerConnectionManager.GetInstance().useRandomUsername = true;
            ChatServerConnectionManager.GetInstance().serverLocation = this.ipTF.text;

            ChatServerConnectionManager.GetInstance().user = new User(new Random(DateTime.Now.Millisecond).Next() + "");
            ChatServerConnectionManager.GetInstance().ConnectToServer();
            this.statusLbl.text = "Connecting..";

            Thread.Sleep(50);

            if (ChatServerConnectionManager.GetInstance().connection == null)
            {
                // Not connected
                this.statusLbl.text = "Failed to connect to server. Check IP, and make sure the server is online.";
            }
            else
            {
                this.serverResponseBar.currentValue = this.serverResponseBar.maxValue;
                this.StartSteadyTest();
            }
        }

        /// <summary>
        /// Sets up and starts the steady test.
        /// </summary>
        public void StartSteadyTest()
        {
            this.steadyPacketTestBar.currentValue = 0;
            this.steadyPacketTestBar.maxValue = steadyTestMaxCount;
            new Thread(this.RunSteadyTest).Start();
        }

        /// <summary>
        /// Runs the steady test. Don't just call this, it blocks in a while loop.
        /// Only call in a new thread (run StartSteadyTest();)
        /// </summary>
        public void RunSteadyTest()
        {
            int count = 0;
            while (count < steadyTestMaxCount)
            {
                this.statusLbl.text = "Running steady test " + (count + 1) + " / " + steadyTestMaxCount;
                Packet p = new Packet(TestHeaders.STEADY_TEST);
                ChatServerConnectionManager.GetInstance().SendPacket(p);

                Thread.Sleep(5);
                count++;
            }

            StartBurstTest();
        }

        /// <summary>
        /// Starts the burst test.
        /// </summary>
        public void StartBurstTest()
        {
            this.burstPacketTestBar.currentValue = 0;
            this.burstPacketTestBar.maxValue = burstTestMaxCount;
            new Thread(this.RunBurstTest).Start();
        }

        /// <summary>
        /// Runs the burst test.
        /// </summary>
        public void RunBurstTest()
        {
            int count = 0;
            while (count < burstTestMaxCount)
            {
                this.statusLbl.text = "Running burst test " + (count + 1) + " / " + burstTestMaxCount;
                Packet p = new Packet(TestHeaders.BURST_TEST);
                ChatServerConnectionManager.GetInstance().SendPacket(p);

                if (count % this.burstCount == 0) Thread.Sleep(500);
                count++;
            }
            StartMalformTest();
        }

        /// <summary>
        /// Starts the malform test.
        /// </summary>
        public void StartMalformTest()
        {
            this.malformPacketTestBar.currentValue = 0;
            this.malformPacketTestBar.maxValue = malformTestMaxCount;
            new Thread(this.RunMalformTest).Start();
        }

        /// <summary>
        /// Runs the malform test.
        /// </summary>
        public void RunMalformTest()
        {
            int count = 0;
            while (count < malformTestMaxCount)
            {
                this.statusLbl.text = "Running burst test " + (count + 1) + " / " + steadyTestMaxCount;
                Packet p = new Packet(TestHeaders.MALFORM_TEST);
                p.AddString(this.MD5Encode(new Random(DateTime.Now.Millisecond).Next() + ""));
                ChatServerConnectionManager.GetInstance().SendPacket(p);
                malformPacketsSent[count] = p;

                Thread.Sleep(10);
                count++;
            }
        }

        public String MD5Encode(String originalPassword)
        {
            //Declarations
            Byte[] originalBytes;
            Byte[] encodedBytes;
            MD5 md5;

            //Instantiate MD5CryptoServiceProvider, get bytes for original password and compute hash (encoded password)
            md5 = new MD5CryptoServiceProvider();
            originalBytes = ASCIIEncoding.Default.GetBytes(originalPassword);
            encodedBytes = md5.ComputeHash(originalBytes);

            //Convert encoded bytes back to a 'readable' string
            return BitConverter.ToString(encodedBytes);
        }

        /// <summary>
        /// When the back button was clicked by the user.
        /// </summary>
        /// <param name="source"></param>
        public void OnBackBtnClick(XNAButton source)
        {
            ChatServerConnectionManager.GetInstance().DisconnectFromServer();
            MenuManager.GetInstance().ShowMenu(MenuManager.Menu.MultiplayerLogin);
        }
    }
}
