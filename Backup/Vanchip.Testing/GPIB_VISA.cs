using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NationalInstruments.NI4882;
using NationalInstruments.VisaNS;


namespace Vanchip.Testing
{
    public class GPIB
    {
        Board GPIB_Board;
        static bool is_Init = false;
        string strErr = "";

        public GPIB()
        {
            if (is_Init)
            {
                throw new Exception();
            }
            try
            {
                GPIB_Board = new Board(0);
                GPIB_Board.SendInterfaceClear();
                GPIB_Board.BusTiming = BusTimingType.Normal;
                GPIB_Board.ParallelPollTimeoutValue = ParallelPollTimeoutValue.Standard;

                is_Init = true;
            }
            catch (Exception e)
            {
                strErr = e.Message.ToString();
            }
        }

        ~GPIB()
        {
            try
            {
                if (GPIB_Board != null) GPIB_Board.Dispose();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            { }
        }

        public bool isInit
        {
            get { return is_Init; }
            set { isInit = is_Init; }
        }

        public void Send(int intaddress, string strCommand)
        {
            try
            {
                GPIB_Board.Write(new Address(Convert.ToByte(intaddress)), strCommand);   // Write to a Device                   
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void Read(int intaddress, ref List<double> RtnList)
        {
            string strReturn = "0";
            RtnList.Clear();

            try
            {
                strReturn = GPIB_Board.ReadString(new Address(Convert.ToByte(intaddress)));

                string[] RtnArray = strReturn.Split(new string[] { "," }, StringSplitOptions.None);

                double dblTemp = 0;
                int i = 0; int z = RtnArray.GetUpperBound(0);

                for (i = 0; i <= z; i++)
                {
                    dblTemp = Convert.ToDouble(RtnArray[i]);
                    RtnList.Add(dblTemp);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void Read(int address, ref string RtnValue)
        {
            try
            {
                RtnValue = GPIB_Board.ReadString(new Address(Convert.ToByte(address)));
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }

    public class VISA
    {
        //VISA Device 
        MessageBasedSession myVISA;
        static bool is_Init = false;
        string strErr = "";

        public VISA(string VISA_Address_String)
        {
            if (is_Init)
            {
                throw new Exception();
            }
            try
            {
                myVISA = (MessageBasedSession)ResourceManager.GetLocalManager().Open(VISA_Address_String);

                is_Init = true;
            }
            catch (Exception e)
            {
                strErr = e.Message.ToString();
            }
        }

        
        ~VISA()
        {
            try
            {
                if (myVISA != null) myVISA.Dispose();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public bool isInit
        {
            get { return is_Init; }
            set { isInit = is_Init; }
        }

        public void Send(string strCommand)
        {
            try
            {
                myVISA.Write(strCommand);   // Write to a Device                   
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void Read(ref List<double> RtnList)
        {
            string strReturn = "0";
            RtnList.Clear();

            try
            {
                strReturn = myVISA.ReadString();

                string[] RtnArray = strReturn.Split(new string[] { "," }, StringSplitOptions.None);

                double dblTemp = 0;
                int i = 0; int z = RtnArray.GetUpperBound(0);

                for (i = 0; i <= z; i++)
                {
                    dblTemp = Convert.ToDouble(RtnArray[i]);
                    RtnList.Add(dblTemp);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void Read(ref string RtnValue)
        {
            try
            {
                RtnValue = myVISA.ReadString();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }



}
