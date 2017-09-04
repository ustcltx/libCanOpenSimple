/*
    This file is part of libCanopenSimple.
    libCanopenSimple is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
    libCanopenSimple is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.
    You should have received a copy of the GNU General Public License
    along with libCanopenSimple.  If not, see <http://www.gnu.org/licenses/>.
 
    Copyright(c) 2017 Robin Cornelius <robin.cornelius@gmail.com>
*/

using System;
using libCanopenSimple;

namespace SimpleTest
{
    public class TaskTimer : System.Timers.Timer
    {
        private object client;

        public object Client
        {
            set { client = value; }
            get { return client; }
        }

        public TaskTimer()
            : base()
        {
        }
    }

    class SimpleTest
    {
        /// <summary>
        /// A simple test that uses libcanopensimple to open a can device, set up some COB specific callbacks
        /// then send a bus reset all nodes NMT command after 5 seconds
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {

            //Change these to load correct driver and connect it to correct bus  
            string driver = "can_qm_rs232_win32";
            string bus = "COM2";
            BUSSPEED bitrate = BUSSPEED.BUS_1Mbit;

            try
            {
                libCanopenSimple.libCanopenSimple lco = new libCanopenSimple.libCanopenSimple();

                lco.nmtevent += Lco_nmtevent;
                lco.nmtecevent += Lco_nmtecevent;
                lco.pdoevent += Lco_pdoevent;
                lco.sdoevent += Lco_sdoevent;

                int COM_ID = 1;
                if (!int.TryParse(bus.Substring(3), out COM_ID)) return;
                lco.open(COM_ID, bitrate, driver);

                Console.WriteLine("listening for any traffic");

                Console.WriteLine("Sending NMT reset all nodes in 5 seconds");

                System.Threading.Thread.Sleep(500);

                //lco.NMT_ResetNode(); //reset all

                //lco.NMT_start(1);

                //lco.SDOread(1, 0x1601, 0, null);
                //lco.SDOwrite(1, 0x1601, 0, 0x02, null);
                //lco.SDOread(1, 0x1601, 0, null);
                //lco.writePDO(0x0301, new byte[8] { 0x9c, 0xac, 0xed, 0x06, 0xe4, 0x01, 0, 0 });
                //lco.checkguard(1, new TimeSpan(0, 0, 1));
                Console.WriteLine("Press any key to exit test..");
                
                lco.dbglevel = debuglevel.DEBUG_ALL;
                //lco.SDOread(1, 0x1601, 0, null);
                TaskTimer t = new TaskTimer();
                t.Client = lco;
                t.Interval = 1000;
                t.Elapsed += new System.Timers.ElapsedEventHandler(t_Elapsed);
                t.Start();
                while (!Console.KeyAvailable)
                {
                    Console.WriteLine("CheckGuard Status: {0}", lco.CheckGuard(1, new TimeSpan(0, 0, 1)));
                    System.Threading.Thread.Sleep(1000);
                }

                lco.close();

            }
            catch(Exception e)
            {
                Console.WriteLine("That did not work out, exception message was \n" + e.ToString());
            }
        }

        private static void Lco_syncevent(canpacket p)
        {
            throw new NotImplementedException();
        }

        private static void t_Elapsed(object sender, EventArgs arg)
        {
            TaskTimer t = sender as TaskTimer;
            libCanopenSimple.libCanopenSimple lco = t.Client as libCanopenSimple.libCanopenSimple;
            lco.GuardRTRRequest(1);
        }

        private static void Lco_nmtecevent(canpacket p)
        {
            Console.WriteLine("NMTEC :" + p.ToString());
        }

        private static void Lco_sdoevent(libCanopenSimple.canpacket p)
        {
            Console.WriteLine("SDO :" + p.ToString());
        }

        private static void Lco_pdoevent(libCanopenSimple.canpacket[] ps)
        {
            foreach(canpacket p in ps)
                Console.WriteLine("PDO :" + p.ToString());
        }

        private static void Lco_nmtevent(libCanopenSimple.canpacket p)
        {
            Console.WriteLine("NMT :" + p.ToString());
        }
    }
}
