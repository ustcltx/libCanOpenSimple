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
using System.Threading.Tasks;
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
            //driver = "can_usb_win32";
            // driver = "can_uvccm_win32";
            string bus = "COM4";
            BUSSPEED bitrate = BUSSPEED.BUS_1Mbit;

            try
            {
                libCanopenSimple.libCanopenSimple lco = new libCanopenSimple.libCanopenSimple();

                lco.nmtevent += Lco_nmtevent;
                lco.nmtecevent += Lco_nmtecevent;
                lco.pdoevent += Lco_pdoevent;
                lco.sdoevent += Lco_sdoevent;

                lco.open(1, bitrate, driver);

                Console.WriteLine("listening for any traffic");

                Console.WriteLine("Sending NMT reset all nodes in 5 seconds");

                //System.Threading.Thread.Sleep(1500);

                //lco.NMT_ResetNode(); //reset all

                Console.WriteLine("Press any key to exit test..");

                var task1 = Task.Factory.StartNew(() => { while (true) { lco.writePDO(0x1001, new byte[] { 1, 2, 3, 4, 5 }); System.Threading.Thread.Sleep(1); } });
                var task2 = Task.Factory.StartNew(() => { while (true) { lco.writePDO(0x2002, new byte[] { 1, 2, 3, 4, 5 }); System.Threading.Thread.Sleep(1); } });
                /*var task3 = Task.Factory.StartNew(() => { while (true) { lco.writePDO(3, new byte[] { 1, 2, 3, 4, 5 }); System.Threading.Thread.Sleep(1); } });
                var task4 = Task.Factory.StartNew(() => { while (true) { lco.writePDO(4, new byte[] { 1, 2, 3, 4, 5 }); System.Threading.Thread.Sleep(1); } });
                var task5 = Task.Factory.StartNew(() => { while (true) { lco.writePDO(5, new byte[] { 1, 2, 3, 4, 5 }); System.Threading.Thread.Sleep(1); } });
                var task6 = Task.Factory.StartNew(() => { while (true) { lco.writePDO(6, new byte[] { 1, 2, 3, 4, 5 }); System.Threading.Thread.Sleep(1); } });
                var task7 = Task.Factory.StartNew(() => { while (true) { lco.writePDO(7, new byte[] { 1, 2, 3, 4, 5 }); System.Threading.Thread.Sleep(1); } });
                var task8 = Task.Factory.StartNew(() => { while (true) { lco.writePDO(8, new byte[] { 1, 2, 3, 4, 5 }); System.Threading.Thread.Sleep(1); } });
                var task9 = Task.Factory.StartNew(() => { while (true) { lco.writePDO(9, new byte[] { 1, 2, 3, 4, 5 }); System.Threading.Thread.Sleep(1); } });
                var task10 = Task.Factory.StartNew(() => { while (true) { lco.writePDO(10, new byte[] { 1, 2, 3, 4, 5 }); System.Threading.Thread.Sleep(1); } });
                var task11 = Task.Factory.StartNew(() => { while (true) { lco.writePDO(11, new byte[] { 1, 2, 3, 4, 5 }); System.Threading.Thread.Sleep(1); } });
                var task12 = Task.Factory.StartNew(() => { while (true) { lco.writePDO(12, new byte[] { 1, 2, 3, 4, 5 }); System.Threading.Thread.Sleep(1); } });
                var task13 = Task.Factory.StartNew(() => { while (true) { lco.writePDO(13, new byte[] { 1, 2, 3, 4, 5 }); System.Threading.Thread.Sleep(1); } });
                */
                //Task.WaitAll(task1, task2, task3, task4, task5, task6, task7, task8, task9, task10, task11, task12, task13);
                System.Threading.Thread.Sleep(100);
                Task.WaitAll(task1);
                while (!Console.KeyAvailable)
                {

                }

                lco.close();

            }
            catch (Exception e)
            {
                Console.WriteLine("That did not work out, exception message was \n" + e.ToString());
            }
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
