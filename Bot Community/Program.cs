using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QlikSenseEasy;

namespace Bot_Community
{
    class Program
    {
        // This is the default Qlik Sense app for new users
        static string user = "qliksvc";
        static string cntqsAppName = "";
        static string cntqsAppId = "62905416-3e48-4267-aafe-797014fe2675";
        static string cntqsServer = "https://40.74.246.240:4747";
        static bool cntqsServerSSL = true;
        static string cntqsServerVirtualProxy = "telegram";

        // The components used to build the single object links to apps, sheets, charts, etc.
        static string cntqsSingleServer = "https://qlik.cygrp.com";
        static string cntqsSingleApp = cntqsAppId;
        static string cntqsAlternativeStreams = "";

        // The QS object to connect to Qlik Sense
        static QSApp QS;

        
        // Here is where we insert the Bot Token
        static string cntBotToken = "498627711:AAEMx_mcDF4DMYjJcaYQ64LvMGL9z7wipzk";
        //498627711:AAEMx_mcDF4DMYjJcaYQ64LvMGL9z7wipzk -- cygrp-qlik-test
        //467810720:AAFJ4Ib_mrmCaFCaVlsqYdHYc4wPu1R3mYI --Agent

        // The Telegram Bot object
        static TelegramBot MyBot;


        static void Main(string[] args)
        {

            // The QS object to connect to Qlik Sense at the begining, to check the connection
            // This QS object will also be used as a template for new objects for every user session
            QSApp InitQS = new QSApp();
            CheckConnection(InitQS);

            if (InitQS.IsConnected)
                Console.WriteLine("Qlik Sense seems to be working :-)");
            else
            {
                Console.WriteLine("Something went wrong with Qlik Sense :-(.\nPress a key to exit.");
                Console.ReadKey();
                return;
            }

            // Telegram
            MyBot = new TelegramBot(cntBotToken, InitQS);

            Console.WriteLine("Telegram seems to be working :-)");

            // Wait forever (or until someone writes close)
            // During this time, the Bot functions will be receiving the messages
            bool go = true;
            while (go)
            {
                string c = Console.ReadLine();
                string[] parm = c.Split(' ');

                if (c.ToLower() == "close")
                {
                    go = false;
                }
            }

            // The End
            Console.WriteLine("Everything stopped. Bye.");
            MyBot.CloseBot();
        }

        private static void CheckConnection(QSApp TheQS)
        {   // Connects a QSApp object with the default values
            TheQS.qsAppName = cntqsAppName;
            TheQS.qsAppId = cntqsAppId;
            TheQS.qsServer = cntqsServer;
            TheQS.qsSingleServer = cntqsSingleServer;
            TheQS.qsSingleApp = cntqsSingleApp;
            TheQS.qsAlternativeStreams = cntqsAlternativeStreams;

            try
            {
                // Create or use the Telegram UserID as the Qlik Sense UserID
                // With the bot virtual proxy and user directory
                // The Virtual Proxy has to be created as "Header authentication static user directory"
                // The user directory is defined in the Virtual Proxy, in "Header authentication static user directory"
                // For this example:
                //
                // Qlik Sense Virtual Proxy
                // ------------------------
                //
                // Description: Telegram
                // Prefix: telegram
                // Timeout: 30
                // Session cookie header name: X-Qlik-Session-telegram
                // Anonymous access mode: No anonymous user
                // Authentication method: Header authentication static user directory
                // Header authentication header name: X-Qlik-HeaderAuthTelegram
                // Header authentication static user directory: TELEGRAM
                // 
                // PD.- Do not forget to add the server node, link the proxy and include the server name in the white list

                TheQS.QSConnectServerHeader(user, "Qlik Sense Header Auth is Here", cntqsServerVirtualProxy, cntqsServerSSL, true);


                if (TheQS.IsConnected)
                {
                    TheQS.QSOpenApp();
                    if (TheQS.AppIsOpen)
                    {
                        Console.WriteLine(string.Format("Opened the Qlik Sense app: {0} for user {1} ({2})", cntqsAppId, "User1", cntqsServerVirtualProxy));

                        string Val = TheQS.GetExpression("Sum([Sales Amount])");
                        Console.WriteLine("Value: " + Val);
                    }
                }


            }
            catch (Exception e)
            {
                Console.WriteLine(string.Format("Error opening the Qlik Sense app: {0} for user {1} ({2}): {3}", cntqsAppId, "User1", cntqsServerVirtualProxy, e));
            }

        }

        static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            Console.WriteLine("exit");
        }

    }
}
