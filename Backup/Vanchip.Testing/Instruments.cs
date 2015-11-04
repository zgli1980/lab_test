///     Reversion history log
///     Rev1.4      Add LTE  CDMA EVDO  Libery                                                   AceLi       2012-08-09

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vanchip.Common;
using NationalInstruments.NI4882;
using NationalInstruments.VisaNS;

namespace Vanchip.Testing
{
    #region *** Instruments class ***

    #region SigGen

    public class SigGen
    {
        HP8665B _HP8665B;
        E4438C _E4438C;
        E4432B _E4432B;

        public enum Type
        {
            HP8665B     = 1,
            E4438C      = 2,
            E4432B      = 3
        }

        public SigGen(Type SigGenType, byte GPIB_Address)
        {
            switch (SigGenType)
            {
                case Type.HP8665B:
                    {
                        _HP8665B = new HP8665B(GPIB_Address);
                        break;
                    }
                case Type.E4438C:
                    {
                        _E4438C = new E4438C(GPIB_Address);
                        break;
                    }
                case Type.E4432B:
                    {
                        _E4432B = new E4432B(GPIB_Address);
                        break;
                    }
            }
        }

        public bool Initialize(Type SigGenType)
        {
            switch (SigGenType)
            {
                case Type.HP8665B:
                    {
                        _HP8665B.Initialize();
                        break;
                    }
                case Type.E4438C:
                    {
                        _E4438C.Initialize();
                        break;
                    }
                case Type.E4432B:
                    {
                        _E4432B.Initialize();
                        break;
                    }
            }
            return (true);
        }

        public void SetFrequency(Type SigGenType, double dblValue_in_MHz)
        {
            switch (SigGenType)
            {
                case Type.HP8665B:
                    {
                        _HP8665B.SetFrequency(dblValue_in_MHz);
                        break;
                    }
                case Type.E4438C:
                    {
                        _E4438C.SetFrequency(dblValue_in_MHz);
                        break;
                    }
                case Type.E4432B:
                    {
                        _E4432B.SetFrequency(dblValue_in_MHz);
                        break;
                    }
            }
        }

        public void SetPower(Type SigGenType, double dblValue_in_dBm)
        {
            switch (SigGenType)
            {
                case Type.HP8665B:
                    {
                        _HP8665B.SetPower(dblValue_in_dBm);
                        break;
                    }
                case Type.E4438C:
                    {
                        _E4438C.SetPower(dblValue_in_dBm);
                        break;
                    }
                case Type.E4432B:
                    {
                        _E4432B.SetPower(dblValue_in_dBm);
                        break;
                    }
            }
        }

        public void SetOutput(Type SigGenType, Output Status)
        {
            switch (SigGenType)
            {
                case Type.HP8665B:
                    {
                        _HP8665B.SetOutput(Status);
                        break;
                    }
                case Type.E4438C:
                    {
                        _E4438C.SetOutput(Status);
                        break;
                    }
                case Type.E4432B:
                    {
                        _E4432B.SetOutput(Status);
                        break;
                    }
            }
        }

        public void Write(Type SigGenType, string strValue)
        {
            switch (SigGenType)
            {
                case Type.HP8665B:
                    {
                        _HP8665B.Write(strValue);
                        break;
                    }
                case Type.E4438C:
                    {
                        _E4438C.Write(strValue);
                        break;
                    }
                case Type.E4432B:
                    {
                        _E4432B.Write(strValue);
                        break;
                    }
            }
        }

        public string Read(Type SigGenType)
        {
            string strValue = "";
            switch (SigGenType)
            {
                case Type.HP8665B:
                    {
                        strValue = _HP8665B.Read();
                        break;
                    }
                case Type.E4438C:
                    {
                        strValue = _E4438C.Read();
                        break;
                    }
                case Type.E4432B:
                    {
                        strValue = _E4432B.Read();
                        break;
                    }
            }
            return strValue;
        }

        public void Dispose(Type SigGenType)
        {
            switch (SigGenType)
            {
                case Type.HP8665B:
                    {
                        _HP8665B.Dispose();
                        break;
                    }
                case Type.E4438C:
                    {
                        _E4438C.Dispose();
                        break;
                    }
                case Type.E4432B:
                    {
                        _E4432B.Dispose();
                        break;
                    }
            }
        }
    }

    #endregion

    #region *** HP8665B ***

    public class HP8665B
    {
        Util _Util = new Util();
        Device ESG_8665B;

        public HP8665B(byte GPIB_Address)
        {
            ESG_8665B = new Device(Instruments_address._00, GPIB_Address);
        }

        public void Initialize()
        {
            try
            {

                ESG_8665B.Write("*CLS");
                ESG_8665B.Write("*RST");
                ESG_8665B.Write("FREQ:CW 915 MHz");
                ESG_8665B.Write("AMPL -20 dBm");
                ESG_8665B.Write("AMPL:STAT ON");
                _Util.Wait(1000);
                ESG_8665B.Write("AMPL:STAT OFF");
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }

        public void SetFrequency(double dblValue_in_MHz)
        {
            try
            {
                StringBuilder sbValue = new StringBuilder(Convert.ToString(dblValue_in_MHz));

                ESG_8665B.Write("FREQ:CW " + sbValue.ToString() + " MHz");

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }

        public void SetPower(double dblValue_in_dBm)
        {
            try
            {
                StringBuilder sbValue = new StringBuilder(Convert.ToString(dblValue_in_dBm));

                ESG_8665B.Write("AMPL " + sbValue.ToString() + " dBm");

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }

        public void SetOutput(Output Status)
        {
            try
            {
                if (Status == Output.ON)
                    ESG_8665B.Write("AMPL:STAT ON");
                else if (Status == Output.OFF)
                    ESG_8665B.Write("AMPL:STAT OFF");
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }

        public void Write(string strValue)
        {
            try
            {
                ESG_8665B.Write(strValue);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public string Read()
        {
            try
            {
                return ESG_8665B.ReadString();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void Dispose()
        {
            try
            {
                ESG_8665B.Dispose();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

    }

    #endregion HP8665B

    #region *** E4432B ***

    public class E4432B
    {
        Device ESG_E4432B;

        public E4432B(byte GPIB_Address)
        {
            ESG_E4432B = new Device(Instruments_address._00, GPIB_Address);
        }

        public void Initialize()
        {
            try
            {

                ESG_E4432B.Write("*CLS");
                ESG_E4432B.Write("*RST");
                ESG_E4432B.Write("FREQ:CW 915 MHz");
                ESG_E4432B.Write("POW:AMPL -20 dBm");
                //ESG_E4432B.Write("OUTP:STAT ON");
                ESG_E4432B.Write("OUTP:MOD:STAT OFF");
                ESG_E4432B.Write("OUTP:STAT OFF");
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }

        public void SetFrequency(double dblValue_in_MHz)
        {
            try
            {
                StringBuilder sbValue = new StringBuilder(Convert.ToString(dblValue_in_MHz));

                ESG_E4432B.Write("FREQ:CW " + sbValue.ToString() + " MHz");
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }

        public void SetPower(double dblValue_in_dBm)
        {
            try
            {
                StringBuilder sbValue = new StringBuilder(Convert.ToString(dblValue_in_dBm));

                ESG_E4432B.Write("POW:AMPL " + sbValue.ToString() + " dBm");
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }

        public void SetOutput(Output Status)
        {
            try
            {
                if (Status == Output.ON)
                    ESG_E4432B.Write("OUTP:STAT ON");
                else if (Status == Output.OFF)
                    ESG_E4432B.Write("OUTP:STAT OFF");
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }

        public void Write(string strValue)
        {
            try
            {
                ESG_E4432B.Write(strValue);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }

        public string Read()
        {
            try
            {
                return ESG_E4432B.ReadString();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void Dispose()
        {
            try
            {
                ESG_E4432B.Dispose();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

    }

    #endregion E4432B

    #region *** E4438C ***

    public class E4438C
    {
        Device ESG_E4438C;

        public E4438C(byte GPIB_Address)
        {
            ESG_E4438C = new Device(Instruments_address._00, GPIB_Address);
        }

        public void Initialize()
        {
            try
            {
                //ESG_E4438C.Write(":SOUR:RAD:ARB:WAV?");
                ESG_E4438C.Write(":MEM:COPY \"NVWFM:" + Mod_Waveform_Name.WCDMA + "\",\"WFM1:" + Mod_Waveform_Name.WCDMA + "\"");
                ESG_E4438C.Write(":MEM:COPY \"NVWFM:" + Mod_Waveform_Name.TDSCDMA + "\",\"WFM1:" + Mod_Waveform_Name.TDSCDMA + "\"");
                ESG_E4438C.Write(":MEM:COPY \"NVWFM:" + Mod_Waveform_Name.TDHSDPA + "\",\"WFM1:" + Mod_Waveform_Name.TDHSDPA + "\"");
                ESG_E4438C.Write(":MEM:COPY \"NVWFM:" + Mod_Waveform_Name.EDGE + "\",\"WFM1:" + Mod_Waveform_Name.EDGE + "\"");
                ESG_E4438C.Write(":MEM:COPY \"NVWFM:" + Mod_Waveform_Name.LTETDD + "\",\"WFM1:" + Mod_Waveform_Name.LTETDD + "\"");
                ESG_E4438C.Write(":MEM:COPY \"NVWFM:" + Mod_Waveform_Name.LTETDD_FULL + "\",\"WFM1:" + Mod_Waveform_Name.LTETDD_FULL + "\"");
                ESG_E4438C.Write(":MEM:COPY \"NVWFM:" + Mod_Waveform_Name.LTEFDD + "\",\"WFM1:" + Mod_Waveform_Name.LTEFDD + "\"");
                ESG_E4438C.Write(":MEM:COPY \"NVWFM:" + Mod_Waveform_Name.LTEFDD_FULL + "\",\"WFM1:" + Mod_Waveform_Name.LTEFDD_FULL + "\"");
                ESG_E4438C.Write(":MEM:COPY \"NVWFM:" + Mod_Waveform_Name.CDMA_ACP + "\",\"WFM1:" + Mod_Waveform_Name.CDMA_ACP + "\"");
                ESG_E4438C.Write(":MEM:COPY \"NVWFM:" + Mod_Waveform_Name.CDMA_EVM + "\",\"WFM1:" + Mod_Waveform_Name.CDMA_EVM + "\"");
                ESG_E4438C.Write(":MEM:COPY \"NVWFM:" + Mod_Waveform_Name.EVDO_ACP + "\",\"WFM1:" + Mod_Waveform_Name.EVDO_ACP + "\"");
                ESG_E4438C.Write(":MEM:COPY \"NVWFM:" + Mod_Waveform_Name.EVDO_EVM + "\",\"WFM1:" + Mod_Waveform_Name.EVDO_EVM + "\"");

                ESG_E4438C.Write("*CLS");
                ESG_E4438C.Write("*CLS");
                ESG_E4438C.Write("*RST");
                ESG_E4438C.Write("FREQ:CW 915 MHz");
                ESG_E4438C.Write("POW:AMPL -20 dBm");
                ESG_E4438C.Write("OUTP:MOD:STAT OFF");
                ESG_E4438C.Write("OUTP:STAT OFF");
                Wait();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }

        public void Mode_Initialize(Modulation mode)
        {
            try
            {
                if (mode == Modulation.WCDMA)
                {
                    ESG_E4438C.Write(":MEM:COPY \"NVWFM:" + Mod_Waveform_Name.WCDMA + "\",\"WFM1:" + Mod_Waveform_Name.WCDMA + "\"");
                    string cmd = ":SOUR:RAD:ARB:WAV " + '"' + Mod_Waveform_Name.WCDMA + '"';
                    ESG_E4438C.Write(cmd);
                    ESG_E4438C.Write(":SOUR:RAD:ARB:TRIG:TYPE CONT");
                    ESG_E4438C.Write(":SOUR:RAD:ARB:TRIG:TYPE:CONT FREE");
                    ESG_E4438C.Write(":SOUR:RAD:ARB ON");
                    ESG_E4438C.Write("OUTP:MOD:STAT ON");
                    ESG_E4438C.Write("OUTP:STAT OFF");
                    Wait(); 
                }
                else if (mode == Modulation.TDSCDMA)
                {
                    ESG_E4438C.Write(":MEM:COPY \"NVWFM:" + Mod_Waveform_Name.TDSCDMA + "\",\"WFM1:" + Mod_Waveform_Name.TDSCDMA + "\"");
                    string cmd = ":SOUR:RAD:ARB:WAV " + '"' + Mod_Waveform_Name.TDSCDMA + '"';
                    ESG_E4438C.Write(cmd);
                    ESG_E4438C.Write(":SOUR:RAD:ARB:TRIG:EXT:SLOP POS");
                    ESG_E4438C.Write(":SOUR:RAD:ARB:TRIG:TYPE CONT");
                    ESG_E4438C.Write(":SOUR:RAD:ARB:TRIG:TYPE:CONT RES");
                    ESG_E4438C.Write(":SOUR:RAD:ARB ON");
                    ESG_E4438C.Write("OUTP:MOD:STAT ON");
                    ESG_E4438C.Write("OUTP:STAT OFF");
                    Wait();
                }
                else if (mode == Modulation.EDGE)
                {
                    ESG_E4438C.Write(":MEM:COPY \"NVWFM:" + Mod_Waveform_Name.EDGE + "\",\"WFM1:" + Mod_Waveform_Name.EDGE + "\"");
                    string cmd = ":SOUR:RAD:ARB:WAV " + '"' + Mod_Waveform_Name.EDGE + '"';
                    ESG_E4438C.Write(cmd);
                    ESG_E4438C.Write(":SOUR:RAD:ARB:TRIG:EXT:SLOP POS");
                    ESG_E4438C.Write(":SOUR:RAD:ARB:TRIG:TYPE CONT");
                    ESG_E4438C.Write(":SOUR:RAD:ARB:TRIG:TYPE:CONT FREE");
                    //ESG_E4438C.Write(":SOUR:RAD:ARB:TRIG:TYPE:CONT RES");
                    ESG_E4438C.Write(":SOUR:RAD:ARB ON");
                    ESG_E4438C.Write("OUTP:MOD:STAT ON");
                    ESG_E4438C.Write("OUTP:STAT OFF");
                    Wait();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }


        public void Mode_Initialize(Modulation mode ,double DelayTime_in_ms)
        {
            try
            {
                if (mode == Modulation.TDSCDMA)
                {
                    ESG_E4438C.Write(":MEM:COPY \"NVWFM:" + Mod_Waveform_Name.TDSCDMA + "\",\"WFM1:" + Mod_Waveform_Name.TDSCDMA + "\"");
                    string cmd = ":SOUR:RAD:ARB:WAV " + '"' + Mod_Waveform_Name.TDSCDMA + '"';
                    ESG_E4438C.Write(cmd);
                    ESG_E4438C.Write(":SOUR:RAD:ARB:TRIG:EXT:SLOP POS");
                    ESG_E4438C.Write(":SOUR:RAD:ARB:TRIG:TYPE CONT");
                    ESG_E4438C.Write(":SOUR:RAD:ARB:TRIG:TYPE:CONT RES");
                    ESG_E4438C.Write(":SOUR:RAD:ARB:TRIG:EXT:DEL " + DelayTime_in_ms + "ms");
                    ESG_E4438C.Write(":SOUR:RAD:ARB:TRIG:EXT:DEL:STAT OFF");
                    ESG_E4438C.Write(":SOUR:RAD:ARB ON");
                    ESG_E4438C.Write("OUTP:MOD:STAT ON");
                    ESG_E4438C.Write("OUTP:STAT OFF");
                    Wait();
                    ESG_E4438C.Write(":SOUR:RAD:ARB:TRIG:EXT:DEL:STAT ON");
                    Wait();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }

        public void Mode_Initialize(Modulation mode, bool HSDPA)
        {
            try
            {
                if (mode == Modulation.WCDMA)               
                {
                    #region Content
                    ESG_E4438C.Write(":MEM:COPY \"NVWFM:" + Mod_Waveform_Name.WCDMA + "\",\"WFM1:" + Mod_Waveform_Name.WCDMA + "\"");
                    string cmd = ":SOUR:RAD:ARB:WAV " + '"' + Mod_Waveform_Name.WCDMA + '"';
                    ESG_E4438C.Write(cmd);
                    ESG_E4438C.Write(":SOUR:RAD:ARB:TRIG:TYPE CONT");
                    ESG_E4438C.Write(":SOUR:RAD:ARB:TRIG:TYPE:CONT FREE");
                    ESG_E4438C.Write(":SOUR:RAD:ARB ON");
                    ESG_E4438C.Write("OUTP:MOD:STAT ON");
                    ESG_E4438C.Write("OUTP:STAT OFF");
                    Wait();
                    #endregion
                }
                else if (mode == Modulation.TDSCDMA)
                {
                    #region Content
                    if (HSDPA)
                    {
                        ESG_E4438C.Write(":MEM:COPY \"NVWFM:" + Mod_Waveform_Name.TDHSDPA + "\",\"WFM1:" + Mod_Waveform_Name.TDHSDPA + "\"");
                        string cmd = ":SOUR:RAD:ARB:WAV " + '"' + Mod_Waveform_Name.TDHSDPA + '"';
                        ESG_E4438C.Write(cmd);
                        ESG_E4438C.Write(":SOUR:RAD:ARB:TRIG:EXT:SLOP POS");
                        ESG_E4438C.Write(":SOUR:RAD:ARB:TRIG:TYPE CONT");
                        ESG_E4438C.Write(":SOUR:RAD:ARB:TRIG:TYPE:CONT RES");
                        ESG_E4438C.Write(":SOUR:RAD:ARB:TRIG:EXT:DEL 2ms");
                        ESG_E4438C.Write(":SOUR:RAD:ARB:TRIG:EXT:DEL:STAT OFF");
                        ESG_E4438C.Write(":SOUR:RAD:ARB ON");
                        ESG_E4438C.Write("OUTP:MOD:STAT ON");
                        ESG_E4438C.Write("OUTP:STAT OFF");
                        Wait();
                        ESG_E4438C.Write(":SOUR:RAD:ARB:TRIG:EXT:DEL:STAT ON");
                        Wait();
                    }
                    else
                    {
                        string cmd = ":SOUR:RAD:ARB:WAV " + '"' + Mod_Waveform_Name.TDSCDMA + '"';
                        ESG_E4438C.Write(cmd);
                        ESG_E4438C.Write(":SOUR:RAD:ARB:TRIG:EXT:SLOP POS");
                        ESG_E4438C.Write(":SOUR:RAD:ARB:TRIG:TYPE CONT");
                        ESG_E4438C.Write(":SOUR:RAD:ARB:TRIG:TYPE:CONT RES");
                        ESG_E4438C.Write(":SOUR:RAD:ARB ON");
                        ESG_E4438C.Write("OUTP:MOD:STAT ON");
                        ESG_E4438C.Write("OUTP:STAT OFF");
                        Wait();
                    }
                    #endregion
                }
                else if (mode == Modulation.EDGE)
                {
                    #region Content
                    ESG_E4438C.Write(":MEM:COPY \"NVWFM:" + Mod_Waveform_Name.EDGE + "\",\"WFM1:" + Mod_Waveform_Name.EDGE + "\"");
                    string cmd = ":SOUR:RAD:ARB:WAV " + '"' + Mod_Waveform_Name.EDGE + '"';
                    ESG_E4438C.Write(cmd);
                    ESG_E4438C.Write(":SOUR:RAD:ARB:TRIG:EXT:SLOP POS");
                    ESG_E4438C.Write(":SOUR:RAD:ARB:TRIG:TYPE CONT");
                    ESG_E4438C.Write(":SOUR:RAD:ARB:TRIG:TYPE:CONT FREE");
                    //ESG_E4438C.Write(":SOUR:RAD:ARB:TRIG:TYPE:CONT RES");
                    ESG_E4438C.Write(":SOUR:RAD:ARB ON");
                    ESG_E4438C.Write("OUTP:MOD:STAT ON");
                    ESG_E4438C.Write("OUTP:STAT OFF");
                    Wait();
                    #endregion Content
                }
                else if (mode == Modulation.LTETDD)
                {
                    #region Content
                    ESG_E4438C.Write(":MEM:COPY \"NVWFM:" + Mod_Waveform_Name.LTETDD + "\",\"WFM1:" + Mod_Waveform_Name.LTETDD + "\"");
                    string cmd = ":SOUR:RAD:ARB:WAV " + '"' + Mod_Waveform_Name.LTETDD + '"';
                    ESG_E4438C.Write(cmd);
                    ESG_E4438C.Write(":SOUR:RAD:ARB:TRIG:EXT:SLOP POS");
                    ESG_E4438C.Write(":SOUR:RAD:ARB:TRIG:EXT:DEL 3.05ms");
                    ESG_E4438C.Write(":SOUR:RAD:ARB:TRIG:EXT:DEL:STAT ON");
                    ESG_E4438C.Write(":SOUR:RAD:ARB:TRIG:TYPE CONT");
                    ESG_E4438C.Write(":SOUR:RAD:ARB:TRIG:TYPE:CONT RES");
                    ESG_E4438C.Write(":SOUR:RAD:ARB ON");
                    ESG_E4438C.Write("OUTP:MOD:STAT ON");
                    ESG_E4438C.Write("OUTP:STAT OFF");
                    Wait();
                    #endregion Content
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }

        public void Mode_Initialize(Modulation mode, string Waveform_Name)
        {
            ESG_E4438C.Write(":SOUR:RAD:ARB OFF");

            try
            {
                if (mode == Modulation.WCDMA)
                {
                    #region Content
                    ESG_E4438C.Write(":MEM:COPY \"NVWFM:" + Waveform_Name + "\",\"WFM1:" + Waveform_Name + "\"");
                    string cmd = ":SOUR:RAD:ARB:WAV " + '"' + Waveform_Name + '"';
                    ESG_E4438C.Write(cmd);
                    ESG_E4438C.Write(":SOUR:RAD:ARB:TRIG:TYPE CONT");
                    ESG_E4438C.Write(":SOUR:RAD:ARB:TRIG:TYPE:CONT FREE");
                    ESG_E4438C.Write(":SOUR:RAD:ARB ON");
                    ESG_E4438C.Write("OUTP:MOD:STAT ON");
                    ESG_E4438C.Write("OUTP:STAT OFF");
                    Wait();
                    #endregion
                }
                else if (mode == Modulation.TDSCDMA)
                {
                    #region Content
                    ESG_E4438C.Write(":MEM:COPY \"NVWFM:" + Waveform_Name  + "\",\"WFM1:"  + Waveform_Name + "\"");
                    string cmd = ":SOUR:RAD:ARB:WAV " + '"' + Waveform_Name + '"';
                    ESG_E4438C.Write(cmd);
                    ESG_E4438C.Write(":SOUR:RAD:ARB:TRIG:EXT:SLOP POS");
                    ESG_E4438C.Write(":SOUR:RAD:ARB:TRIG:TYPE CONT");
                    ESG_E4438C.Write(":SOUR:RAD:ARB:TRIG:TYPE:CONT TRIG");
                    ESG_E4438C.Write(":SOUR:RAD:ARB ON");
                    ESG_E4438C.Write("OUTP:MOD:STAT ON");
                    ESG_E4438C.Write("OUTP:STAT OFF");
                    Wait();
                    #endregion
                }
                else if (mode == Modulation.EDGE)
                {
                    #region Content
                    ESG_E4438C.Write(":MEM:COPY \"NVWFM:" + Waveform_Name + "\",\"WFM1:" + Waveform_Name + "\"");
                    string cmd = ":SOUR:RAD:ARB:WAV " + '"' + Waveform_Name + '"';
                    ESG_E4438C.Write(cmd);
                    ESG_E4438C.Write(":SOUR:RAD:ARB:TRIG:EXT:SLOP POS");
                    ESG_E4438C.Write(":SOUR:RAD:ARB:TRIG:TYPE CONT");
                    ESG_E4438C.Write(":SOUR:RAD:ARB:TRIG:TYPE:CONT FREE");
                    //ESG_E4438C.Write(":SOUR:RAD:ARB:TRIG:TYPE:CONT RES");
                    ESG_E4438C.Write(":SOUR:RAD:ARB ON");
                    ESG_E4438C.Write("OUTP:MOD:STAT ON");
                    ESG_E4438C.Write("OUTP:STAT OFF");
                    Wait();
                    #endregion Content
                }
                else if (mode == Modulation.LTETDD)
                {
                    #region Content
                    ESG_E4438C.Write(":MEM:COPY \"NVWFM:" + Waveform_Name + "\",\"WFM1:" + Waveform_Name + "\"");
                    string cmd = ":SOUR:RAD:ARB:WAV " + '"' + Waveform_Name + '"';
                    ESG_E4438C.Write(cmd);
                    ESG_E4438C.Write(":SOUR:RAD:ARB ON");
                    ESG_E4438C.Write(":SOUR:RAD:ARB:TRIG:EXT:SLOP POS");
                    ESG_E4438C.Write(":SOUR:RAD:ARB:TRIG:TYPE CONT");
                    ESG_E4438C.Write(":SOUR:RAD:ARB:TRIG:TYPE:CONT TRIG");
                    ESG_E4438C.Write(":SOUR:RAD:ARB:TRIG:EXT:DEL 3.00ms");
                    ESG_E4438C.Write(":SOUR:RAD:ARB:TRIG:EXT:DEL:STAT OFF");
                    ESG_E4438C.Write("OUTP:MOD:STAT ON");
                    ESG_E4438C.Write("OUTP:STAT OFF");
                    Wait();
                    ESG_E4438C.Write(":SOUR:RAD:ARB:TRIG:EXT:DEL:STAT ON");
                    Wait();
                    #endregion Content
                }
                else if (mode == Modulation.LTEFDD)
                {
                    #region Content
                    ESG_E4438C.Write(":MEM:COPY \"NVWFM:" + Waveform_Name + "\",\"WFM1:" + Waveform_Name + "\"");
                    string cmd = ":SOUR:RAD:ARB:WAV " + '"' + Waveform_Name + '"';
                    ESG_E4438C.Write(cmd);
                    ESG_E4438C.Write(":SOUR:RAD:ARB:TRIG:TYPE CONT");
                    ESG_E4438C.Write(":SOUR:RAD:ARB:TRIG:TYPE:CONT FREE");
                    ESG_E4438C.Write(":SOUR:RAD:ARB ON");
                    ESG_E4438C.Write("OUTP:MOD:STAT ON");
                    ESG_E4438C.Write("OUTP:STAT OFF");
                    Wait();
                    #endregion
                }
                else if (mode == Modulation.CDMA)
                {
                    #region Content
                    ESG_E4438C.Write(":MEM:COPY \"NVWFM:" + Waveform_Name + "\",\"WFM1:" + Waveform_Name + "\"");
                    string cmd = ":SOUR:RAD:ARB:WAV " + '"' + Waveform_Name + '"';
                    ESG_E4438C.Write(cmd);
                    ESG_E4438C.Write(":SOUR:RAD:ARB:TRIG:TYPE CONT");
                    ESG_E4438C.Write(":SOUR:RAD:ARB:TRIG:TYPE:CONT FREE");
                    ESG_E4438C.Write(":SOUR:RAD:ARB ON");
                    ESG_E4438C.Write("OUTP:MOD:STAT ON");
                    ESG_E4438C.Write("OUTP:STAT OFF");
                    Wait();
                    #endregion
                }
                else if (mode == Modulation.EVDO)
                {
                    #region Content
                    ESG_E4438C.Write(":MEM:COPY \"NVWFM:" + Waveform_Name + "\",\"WFM1:" + Waveform_Name + "\"");
                    string cmd = ":SOUR:RAD:ARB:WAV " + '"' + Waveform_Name + '"';
                    ESG_E4438C.Write(cmd);
                    ESG_E4438C.Write(":SOUR:RAD:ARB:TRIG:TYPE CONT");
                    ESG_E4438C.Write(":SOUR:RAD:ARB:TRIG:TYPE:CONT FREE");
                    ESG_E4438C.Write(":SOUR:RAD:ARB ON");
                    ESG_E4438C.Write("OUTP:MOD:STAT ON");
                    ESG_E4438C.Write("OUTP:STAT OFF");
                    Wait();
                    #endregion
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }

        public void SetArbTrig(Triger_Type TrigType, Triger_Source Trig_Src, double TrigDelay_in_ms)
        {
            //ESG_E4438C.Write(":SOUR:RAD:ARB:TRIG:EXT:SLOP POS");
            if (TrigType == Triger_Type.Continous_Free) // Free Run
            {
                ESG_E4438C.Write(":SOUR:RAD:ARB:TRIG:TYPE CONT");
                ESG_E4438C.Write(":SOUR:RAD:ARB:TRIG:TYPE:CONT FREE");
            }
            else if (TrigType == Triger_Type.Continous_Trig) // External Trig -- Triger and Run
            {
                ESG_E4438C.Write(":SOUR:RAD:ARB:TRIG:TYPE CONT");
                ESG_E4438C.Write(":SOUR:RAD:ARB:TRIG:TYPE:CONT TRIG");
            }
            else if (TrigType == Triger_Type.Continous_Reset) // External Trig -- Reset and Run
            {
                ESG_E4438C.Write(":SOUR:RAD:ARB:TRIG:TYPE CONT");
                ESG_E4438C.Write(":SOUR:RAD:ARB:TRIG:TYPE:CONT RES");
            }
            else
            {
                throw new Exception("Trig type not defined!");
            }

            //Triger Source
            if (Trig_Src == Triger_Source.Ext) // External(Patt 1)
            {
                ESG_E4438C.Write(":SOUR:RAD:ARB:TRIG:SOUR EXT");
            }
            else if (Trig_Src == Triger_Source.Bus) // Bus Trig 
            {
                ESG_E4438C.Write(":SOUR:RAD:ARB:TRIG:SOUR BUS");
            }
            else if (Trig_Src == Triger_Source.Manual) // Trigger Key 
            {
                ESG_E4438C.Write(":SOUR:RAD:ARB:TRIG:SOUR MAN");
            }
            else
            {
                throw new Exception("Trig type not defined!");
            }

            //Trgger Delay
            if (TrigDelay_in_ms == 0)
            {
                //ESG_E4438C.Write(":SOUR:RAD:ARB:TRIG:EXT:DEL " + TrigDelay_in_ms + "ms");
                ESG_E4438C.Write(":SOUR:RAD:ARB:TRIG:EXT:DEL:STAT OFF");
            }
            else
            {
                ESG_E4438C.Write(":SOUR:RAD:ARB:TRIG:EXT:DEL " + TrigDelay_in_ms + "ms");
                ESG_E4438C.Write(":SOUR:RAD:ARB:TRIG:EXT:DEL:STAT ON");
            }
        }

        public void TrigerBus()
        {
            ESG_E4438C.Write("*TRG");
        }

        public void SetFrequency(double dblValue)
        {
            try
            {
                StringBuilder sbValue = new StringBuilder(Convert.ToString(dblValue));

                ESG_E4438C.Write("FREQ:CW " + sbValue.ToString() + " MHz");

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }

        public void SetPower(double dblValue)
        {
            try
            {
                StringBuilder sbValue = new StringBuilder(Convert.ToString(dblValue));

                ESG_E4438C.Write("POW:AMPL " + sbValue.ToString() + " dBm");
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }

        public void SetOutput(Output Status)
        {
            try
            {
                if (Status == Output.ON)
                    ESG_E4438C.Write("OUTP:STAT ON");
                else if (Status == Output.OFF)
                    ESG_E4438C.Write("OUTP:STAT OFF");
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }

        public void SetModOutput(Output Status)
        {
            try
            {
                if (Status == Output.ON)
                    ESG_E4438C.Write("OUTP:MOD:STAT ON");
                else if (Status == Output.OFF)
                    ESG_E4438C.Write("OUTP:MOD:STAT OFF");
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }

        public void Write(string strValue)
        {
            try
            {
                ESG_E4438C.Write(strValue);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }

        public string Read()
        {
            try
            {
                return ESG_E4438C.ReadString();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void Dispose()
        {
            try
            {
                ESG_E4438C.Write("*CLS");
                ESG_E4438C.Write("*RST");
                ESG_E4438C.Dispose();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        private void Wait()
        {
            string strResult;
            ESG_E4438C.Write("*OPC?");
            strResult = ESG_E4438C.ReadString();
        }
    }

    #endregion E4438C

    #region *** 66332A ***

    public class PS_66332A
    {
        Util _Util = new Util();
        Device PowerSupply_66332A;

        public PS_66332A(byte GPIB_Address)
        {
            PowerSupply_66332A = new Device(Instruments_address._00, GPIB_Address);
        }

        public void Initialize()
        {
            try
            {
                PowerSupply_66332A.Write("*CLS");
                PowerSupply_66332A.Write("*RST");
                PowerSupply_66332A.Write("CURR 2.5"); //Set Max current to 3000mA
                PowerSupply_66332A.Write("VOLT 3.5"); //Set Voltage to 3.5
                //PowerSupply_66332A.Write("OUTP ON");
                PowerSupply_66332A.Write("OUTP OFF");
                //PowerSupply_66332A.Write("DISP OFF");
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }

        public void SetVoltage(double dblValue_in_Volts)
        {
            StringBuilder sbValue = new StringBuilder(Convert.ToString(dblValue_in_Volts));

            try
            {
                PowerSupply_66332A.Write("VOLT " + sbValue.ToString()); //Set Voltage to 3.5
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }

        public void SetCurrentRange(double dblValue_in_Amps)
        {
            StringBuilder sbValue = new StringBuilder(Convert.ToString(dblValue_in_Amps));
            try
            {
                PowerSupply_66332A.Write("CURR " + sbValue.ToString()); //Set Voltage to 3.5
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }
        
        public void SetOutput(Output Status)
        {
            try
            {
                if (Status == Output.ON)
                    PowerSupply_66332A.Write("OUTP ON");
                else if (Status == Output.OFF)
                    PowerSupply_66332A.Write("OUTP OFF");
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }

        public void Write(string strValue)
        {
            try
            {
                PowerSupply_66332A.Write(strValue);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void Read(ref string strValue)
        {
            try
            {
                strValue = PowerSupply_66332A.ReadString();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public double Max_Current()
        {
            double dblTemp = -99;
            try
            {
                PowerSupply_66332A.Write("MEAS:CURR:MAX?");
                string strReturn = PowerSupply_66332A.ReadString();
                return dblTemp = Convert.ToDouble(strReturn);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void High_Current_Set()
        {
            //double dblTemp = -99;
            try
            {

                PowerSupply_66332A.Write("MEAS:CURR:HIGH?");
                _Util.Wait(50);

                //string strReturn = PowerSupply_66332A.ReadString();

                //return dblTemp = Convert.ToDouble(strReturn);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public double High_Current_Read()
        {
            double dblTemp = -99;
            try
            {

                //PowerSupply_66332A.Write("MEAS:CURR:HIGH?");
                //_Util.Wait(50);

                string strReturn = PowerSupply_66332A.ReadString();

                return dblTemp = Convert.ToDouble(strReturn);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public double High_Current()
        {
            double dblTemp = -99;
            try
            {

                PowerSupply_66332A.Write("MEAS:CURR:HIGH?");
                _Util.Wait(50);

                string strReturn = PowerSupply_66332A.ReadString();

                return dblTemp = Convert.ToDouble(strReturn);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public double RMS_Current()
        {
            double dblTemp = -99;
            try
            {
                //PowerSupply_66332A.Write("SENS:SWE:TINT 5e-3");
                //PowerSupply_66332A.Write("SENS:SWE:POIN 256");
                //PowerSupply_66332A.Write("MEAS:CURR:MAX?");
                PowerSupply_66332A.Write("MEAS:CURR?");
                string strReturn = PowerSupply_66332A.ReadString();
                return dblTemp = Convert.ToDouble(strReturn);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public double[] Meas_Current_Trig(Triger_Source Trig_src, double dblTrigLevel_in_Amps, double dblTimeInterval_in_us, int intTotalPoints, int intOffsetPoints)
        {
            double[] dblResult = new double[5];
            double dblTimeInterval_in_Seconds = dblTimeInterval_in_us / 1000000;
            try
            {
                PowerSupply_66332A.Write("SENS:CURR:DET ACDC");
                PowerSupply_66332A.Write("SENS:CURR:RANG MAX");
                PowerSupply_66332A.Write("SENS:FUNC 'CURR'");
                PowerSupply_66332A.Write("SENS:SWE:TINT " + dblTimeInterval_in_Seconds.ToString());
                PowerSupply_66332A.Write("SENS:SWE:POIN " + intTotalPoints.ToString());
                PowerSupply_66332A.Write("SENS:SWE:OFFS:POIN " + intOffsetPoints.ToString());

                if (Trig_src == Triger_Source.Int)
                {
                    PowerSupply_66332A.Write("TRIG:ACQ:SOUR INT");
                    PowerSupply_66332A.Write("TRIG:ACQ:LEV:CURR " + dblTrigLevel_in_Amps.ToString());
                    PowerSupply_66332A.Write("TRIG:ACQ:SLOPE:CURR POS");
                    PowerSupply_66332A.Write("TRIG:ACQ:HYST:CURR 0.05");

                    _Util.Wait(10);
                    PowerSupply_66332A.Write("INIT:NAME ACQ");
                }
                else if (Trig_src == Triger_Source.Bus)
                {
                    PowerSupply_66332A.Write("TRIG:ACQ:SOUR BUS");

                    _Util.Wait(10);
                    PowerSupply_66332A.Write("INIT:NAME ACQ");
                    PowerSupply_66332A.Write("TRIG:ACQ:IMM");
                }
                else
                {
                    throw new Exception();
                }


                PowerSupply_66332A.Write("FETCH:ARRAY:CURR?");

                _Util.Wait(10);
                string strTemp = PowerSupply_66332A.ReadString();
                while (strTemp.Substring(strTemp.Length - 1) != "\n")
                {
                    strTemp += PowerSupply_66332A.ReadString();
                }

                string[] strReturn = strTemp.Split(',');


                PowerSupply_66332A.Write("FETCH:CURR:HIGH?");
                _Util.Wait(10);
                string strHigh = PowerSupply_66332A.ReadString();

                PowerSupply_66332A.Write("FETCH:CURR:LOW?");
                _Util.Wait(10);
                string strLow = PowerSupply_66332A.ReadString();

                PowerSupply_66332A.Write("FETCH:CURR:MIN?");
                _Util.Wait(10);
                string strMin = PowerSupply_66332A.ReadString();

                PowerSupply_66332A.Write("FETCH:CURR:MAX?");
                _Util.Wait(10);
                string strMax = PowerSupply_66332A.ReadString();

                PowerSupply_66332A.Write("FETCH:CURR:DC?");
                _Util.Wait(10);
                string strRms = PowerSupply_66332A.ReadString();

                //RMS Current
                dblResult[0] = double.Parse(strRms);
                //Low Current
                dblResult[1] = double.Parse(strLow);
                //High Current
                dblResult[2] = double.Parse(strHigh);
                //Min Current
                dblResult[3] = double.Parse(strMin);
                //Max Current
                dblResult[4] = double.Parse(strMax);


                double dblTemp = 0.0;
                int intAvgCount = 0;
                double[] dblResult1 = new double[4096];

                for (int i = 0; i < strReturn.Count() - 2; i++)
                {
                    dblResult1[i] = Double.Parse(strReturn[i]);

                    if (i >= 10 + Math.Abs(intOffsetPoints) && i <= strReturn.Count() - 1 - 10)
                    {
                        dblTemp += dblResult1[i];
                        intAvgCount++;
                    }
                }

                dblTemp = dblTemp / intAvgCount;
                if (Trig_src == Triger_Source.Int) dblResult[2] = dblTemp;
            }
            catch (Exception e)
            {
                //throw new Exception(e.Message);

                PowerSupply_66332A.Write("TRIG:ACQ:IMM");
                for (int i = 0; i <= 4; i++)
                {
                    dblResult[i] = -99;
                }
            }

            return dblResult;
        }

        public void Dispose()
        {
            try
            {
                PowerSupply_66332A.Write("*CLS");
                PowerSupply_66332A.Write("*RST");
                PowerSupply_66332A.Dispose();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }

    #endregion 66332A

    #region *** 66319B ***

    public class PS_66319B
    {
        Device PowerSupply_66319B;

        public PS_66319B(byte GPIB_Address)
        {
            PowerSupply_66319B = new Device(Instruments_address._00, GPIB_Address);
        }

        public void Initialize()
        {
            try
            {
                PowerSupply_66319B.Write("*CLS");
                PowerSupply_66319B.Write("*RST");
                PowerSupply_66319B.Write("CURR 2.5"); //Set Max current to 3000mA
                PowerSupply_66319B.Write("VOLT 3.5"); //Set Voltage to 3.5
                PowerSupply_66319B.Write("OUTP OFF");
                PowerSupply_66319B.Write("CURR2 2.5"); //Set Max current to 3000mA
                PowerSupply_66319B.Write("VOLT2 3.4"); //Set Voltage to 3.4
                PowerSupply_66319B.Write("OUTP2 OFF");
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }

        public void SetVoltage(PS_66319B_Channel ChannelList ,double dblValue_in_Volts)
        {
            StringBuilder sbValue = new StringBuilder(Convert.ToString(dblValue_in_Volts));

            try
            {

                if (ChannelList == PS_66319B_Channel.Channel_1)
                    PowerSupply_66319B.Write("VOLT " + sbValue.ToString());
                else
                    PowerSupply_66319B.Write("VOLT2 " + sbValue.ToString()); 

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }

        public void SetCurrentRange(PS_66319B_Channel ChannelList, double dblValue_in_Amps)
        {
            StringBuilder sbValue = new StringBuilder(Convert.ToString(dblValue_in_Amps));
            try
            {
                if (ChannelList == PS_66319B_Channel.Channel_1)
                    PowerSupply_66319B.Write("CURR " + sbValue.ToString());
                else
                    PowerSupply_66319B.Write("CURR2 " + sbValue.ToString());
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }

        public void SetOutput(PS_66319B_Channel ChannelList, Output Status)
        {
            try
            {
                if (Status == Output.ON)
                    if (ChannelList == PS_66319B_Channel.Channel_1)
                        PowerSupply_66319B.Write("OUTP ON");
                    else
                        PowerSupply_66319B.Write("OUTP2 ON");

                else if (Status == Output.OFF)
                    if (ChannelList == PS_66319B_Channel.Channel_1)
                        PowerSupply_66319B.Write("OUTP OFF");
                    else
                        PowerSupply_66319B.Write("OUTP2 OFF");
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }

        public void Write(string strValue)
        {
            try
            {
                PowerSupply_66319B.Write(strValue);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void Read(ref string strValue)
        {
            try
            {
                strValue = PowerSupply_66319B.ReadString();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public double Max_Current(PS_66319B_Channel ChannelList)
        {
            double dblTemp = -99;
            try
            {
                if (ChannelList == PS_66319B_Channel.Channel_1)
                    PowerSupply_66319B.Write("MEAS:CURR:MAX?");
                else
                    PowerSupply_66319B.Write("MEAS:CURR2:MAX?");

                string strReturn = PowerSupply_66319B.ReadString();
                return dblTemp = Convert.ToDouble(strReturn);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public double High_Current(PS_66319B_Channel ChannelList)
        {
            double dblTemp = -99;
            try
            {
                //PowerSupply_66332A.Write("SENS:CURR:DET ACDC");
                //PowerSupply_66332A.Write("SENS:CURR:RANG MAX");
                //PowerSupply_66332A.Write("TRIG:ACQ:SOUR INT");
                //PowerSupply_66332A.Write(@"SENS:FUNC ""CURR""");
                //PowerSupply_66332A.Write("TRIG:ACQ:LEV:CURR .2");
                //PowerSupply_66332A.Write("TRIG:ACQ:SLOPE:CURR POS");
                //PowerSupply_66332A.Write("TRIG:ACQ:HYST:CURR .05");
                //PowerSupply_66332A.Write("SENS:SWE:TINT 30E-6");
                //PowerSupply_66332A.Write("SENS:SWE:POIN 300");
                //PowerSupply_66332A.Write("SENS:SWE:OFFS:POIN -20");
                //PowerSupply_66332A.Write("INIT:NAME ACQ");
                //PowerSupply_66332A.Write("MEAS:ARRAY:CURR?");

                //string strReturn = PowerSupply_66332A.ReadString();

                if (ChannelList == PS_66319B_Channel.Channel_1)
                    PowerSupply_66319B.Write("MEAS:CURR:HIGH?");
                else
                    PowerSupply_66319B.Write("MEAS:CURR:HIGH?");

                string strReturn = PowerSupply_66319B.ReadString();

                return dblTemp = Convert.ToDouble(strReturn);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public double RMS_Current(PS_66319B_Channel ChannelList)
        {
            double dblTemp = -99;
            try
            {
                //PowerSupply_66332A.Write("SENS:SWE:TINT 5e-3");
                //PowerSupply_66332A.Write("SENS:SWE:POIN 256");
                //PowerSupply_66332A.Write("MEAS:CURR:MAX?");
                if (ChannelList == PS_66319B_Channel.Channel_1)
                    PowerSupply_66319B.Write("MEAS:CURR?");
                else
                    PowerSupply_66319B.Write("MEAS:CURR2?");

                string strReturn = PowerSupply_66319B.ReadString();
                return dblTemp = Convert.ToDouble(strReturn);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void Dispose()
        {
            try
            {
                PowerSupply_66319B.Write("*CLS");
                PowerSupply_66319B.Write("*RST");
                PowerSupply_66319B.Dispose();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }

    #endregion 66319B

    #region *** E3631A ***

    public class PS_E3631A
    {
        private bool isInit;
        Device PowerSupply_E3631A;

        public bool isInitialized
        {
            get { return isInit; }
            set { isInit = value; }
        }
        public PS_E3631A(byte GPIB_Address)
        {
            isInit = false;
            PowerSupply_E3631A = new Device(Instruments_address._00, GPIB_Address);
        }

        public void Initialize()
        {
            try
            {
                PowerSupply_E3631A.Write("*CLS");
                PowerSupply_E3631A.Write("*RST");
                PowerSupply_E3631A.Write("APPL P25V, 0.0, 1.0"); //Set 0.0 volts/1.0 amp to +25V output
                PowerSupply_E3631A.Write("APPL N25V, 0.0, 1.0"); //Set 0.0 volts/1.0 amp to -25V output
                PowerSupply_E3631A.Write("APPL P6V, 3.4, 1.0"); //Set 5.0 volts/1.0 amp to +6V output
                //PowerSupply_E3631A.Write("OUTP ON");
                PowerSupply_E3631A.Write("OUTP OFF");
                //PowerSupply_E3631A.Write("DISP OFF");

                isInit = true;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }

        public void SetVoltage_Current(E3631A_Channel Channel, double Voltage_in_Volts, double Current_in_Amps)
        {
            StringBuilder sbCmd = new StringBuilder();
            switch (Channel)
            {
                case E3631A_Channel.P6V:
                    {
                        sbCmd.Append("APPL P6V, ");
                        break;
                    }
                case E3631A_Channel.P25V:
                    {
                        sbCmd.Append("APPL P25V, ");
                        break;
                    }
                case E3631A_Channel.N25V:
                    {
                        sbCmd.Append("APPL N25V, ");
                        break;
                    }
            }
            sbCmd.Append(Voltage_in_Volts);
            sbCmd.Append(", ");
            sbCmd.Append(Current_in_Amps);

            try
            {
                PowerSupply_E3631A.Write(sbCmd.ToString()); //Set Voltage to 3.5
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }

        public void SetOutput(Output Status)
        {
            try
            {
                if (Status == Output.ON)
                    PowerSupply_E3631A.Write("OUTP ON");
                else if (Status == Output.OFF)
                    PowerSupply_E3631A.Write("OUTP OFF");
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }

        public void Write(string strValue)
        {
            try
            {
                PowerSupply_E3631A.Write(strValue);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void Read(ref string strValue)
        {
            try
            {
                strValue = PowerSupply_E3631A.ReadString();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public double Measure_Current(E3631A_Channel Channel)
        {
            double dblTemp = -99;
            StringBuilder sbCmd = new StringBuilder();
            switch (Channel)
            {
                case E3631A_Channel.P6V:
                    {
                        PowerSupply_E3631A.Write("MEAS:CURR? P6V");
                        break;
                    }
                case E3631A_Channel.P25V:
                    {
                        PowerSupply_E3631A.Write("MEAS:CURR? P25V");
                        break;
                    }
                case E3631A_Channel.N25V:
                    {
                        PowerSupply_E3631A.Write("MEAS:CURR? N25V");
                        break;
                    }
            }
            try
            {
                string strReturn = PowerSupply_E3631A.ReadString();
                return dblTemp = Convert.ToDouble(strReturn);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void Dispose()
        {
            try
            {
                PowerSupply_E3631A.Write("*CLS");
                PowerSupply_E3631A.Write("*RST");
                PowerSupply_E3631A.Dispose();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }

    #endregion E3631A

    #region *** 437B ***

    public class PM_437B
    {
        Util _Util = new Util();
        Device PowerMeter_437B;

        public PM_437B(byte GPIB_Address)
        {
            PowerMeter_437B = new Device(Instruments_address._00, GPIB_Address);
        }

        public void Initialize()
        {
            try
            {
                PowerMeter_437B.Write("*CLS");
                PowerMeter_437B.Write("*RST");

                //PowerMeter_437B.Write("ZE");        //Zeroing
                //PowerMeter_437B.Write("CL100%");   //Cal power sensor with 100% REF CAL FACTOR                
                //PowerMeter_437B.Write("RA");        //Set power meter to AUTO RANGE
                PowerMeter_437B.Write("RM3EN");     //Set  RANGE to 5, least sensitive (highest power mode)
                PowerMeter_437B.Write("RE3EN");     //Set  0.001dB Resolution
                PowerMeter_437B.Write("FR50MZ");   //Set Frequency to 50MHz
                PowerMeter_437B.Write("DY25%");     //Set Duty Cycle to 25%
                PowerMeter_437B.Write("DC0");       //"DC1" Duty Cycle on //"DC0" Duty cycle off 
                //PowerMeter_437B.Write("OS10EN");    //Set Offset to 10dB
                PowerMeter_437B.Write("OF0");       //"OF0"Disable Offset //"OF1" Enable Offset
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }

        public int CAL()
        {
            try
            {
                PowerMeter_437B.Write("ZE");            //Zeroing
                PowerMeter_437B.Write("CL100%");        //Cal power sensor with 100% REF CAL FACTOR  
                PowerMeter_437B.Write("*OPC?");
                _Util.Wait(20000);
                return 0;
                //string sdf = PowerMeter_437B.ReadString();
                //if (PowerMeter_437B.ReadString() == "1\n")    //Throw a error if zero fail
                //    return 0;
                //else
                //    return -99;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }

        public void Configure__CW_Power(double dblFrequncy_in_MHz, int intAverageCount, int intRangValue, int intDutyCycleValue)
        {
            try
            {
                PowerMeter_437B.Write("FR" + dblFrequncy_in_MHz.ToString() + "MZ");         //Set Frequency
                //Range 0   AutoRange
                //Range 1   Most sensitive (Low power mode)
                // ...
                //Range 5   Least sensitive (High power mode)
                PowerMeter_437B.Write("RM" + intRangValue.ToString() + "EN");               //Set Range
                PowerMeter_437B.Write("FM" + intAverageCount.ToString() + "EN");            //Set Average

                if (intDutyCycleValue != 1)
                {
                    PowerMeter_437B.Write("DC1");                                           //"DC1" Duty Cycle on //"DC0" Duty cycle off 
                    PowerMeter_437B.Write("DY" + intDutyCycleValue.ToString() + "%");       //Set Duty Cycle to 25%
                }
                else
                    PowerMeter_437B.Write("DC0");
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void SetFrequency(double dblValue_in_MHz)
        {
            StringBuilder sbValue = new StringBuilder(Convert.ToString(dblValue_in_MHz));
            try
            {
                PowerMeter_437B.Write("FR" + sbValue.ToString() + "MZ");   //Set Frequency to 915MHz
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void SetRange(int intValue)
        {
            //Range 0   AutoRange
            //Range 1   Most sensitive (Low power mode)
            // ...
            //Range 5   Least sensitive (High power mode)

            StringBuilder sbValue = new StringBuilder(Convert.ToString(intValue));

            try
            {
                PowerMeter_437B.Write("RM" + sbValue.ToString() + "EN");
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void SetResolution(int intValue)
        {
            //RES1      0.1dB
            //RES2      0.01dB         
            //RES3      0.001dB

            StringBuilder sbValue = new StringBuilder(Convert.ToString(intValue));

            try
            {
                PowerMeter_437B.Write("RE" + sbValue.ToString() + "EN");
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void SetDutyCycle(int intDutyCycleValue, bool Enable)
        {
            PowerMeter_437B.Write("DY" + intDutyCycleValue.ToString() + "%");     //Set Duty Cycle to 25%
            if (Enable)
                PowerMeter_437B.Write("DC1");       //"DC1" Duty Cycle on //"DC0" Duty cycle off 
            else
                PowerMeter_437B.Write("DC0");
        }

        public double GetPowerResult()
        {
            double dblTemp = -99;
            try
            {
                PowerMeter_437B.Write("GT1");   //"GT1" immediately trigger & "GT2" trigger delay
                string strReturn = PowerMeter_437B.ReadString();
                return dblTemp = Convert.ToDouble(strReturn);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void Dispose()
        {
            try
            {
                PowerMeter_437B.Dispose();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }

    #endregion 437B

    #region *** U2001A ***

    public class PM_U2001A
    {
        MessageBasedSession PowerMeter_U2001A = (MessageBasedSession)ResourceManager.GetLocalManager().Open(Instruments_VISA.PowerMeter_U2001A);
        //MessageBasedSession PowerMeter_U2001A = new MessageBasedSession(Instruments_VISA.PowerMeter_U2001A);
        Util _Util = new Util();

        public void Initialize()
        {
            try
            {
                PowerMeter_U2001A.Clear();
                PowerMeter_U2001A.Write("*CLS");
                PowerMeter_U2001A.Write("*RST");
                PowerMeter_U2001A.Write("SYST:PRES DEF");       //Configure to default(CW) mode
                PowerMeter_U2001A.Write("FREQ 50MHz");        //set frequency to 915MHz
                PowerMeter_U2001A.Write("AVER 1");              //Enable average
                //PowerMeter_U2001A.Write("POW:AC:RANG 0");       //Set low power mode
                PowerMeter_U2001A.Write("AVER:COUN 128");       //set average to 10
                PowerMeter_U2001A.Write("AVER:SDET OFF");        //Trun off step detection
                PowerMeter_U2001A.Write("INIT:CONT ON");        //Set trigger to free run mode
                PowerMeter_U2001A.Write("CAL:ZERO:TYPE INT");   //Set to internal zeroing
                //PowerMeter_U2001A.Write("CAL");                // Zeroing
                //PowerMeter_U2001A.Write("*OPC?"); 
                //util.Delay(30000);
                //if (PowerMeter_U2001A.ReadString() != "1\n")    //Throw a error if zero fail
                //{
                //    throw new Exception("Zeroing fail!");
                //}
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Clear()
        {
            PowerMeter_U2001A.Clear();
        }

        public int CAL(U2001_ZeroType Zeroing_Type)
        {
            try
            {
                if (Zeroing_Type == U2001_ZeroType.External)
                    PowerMeter_U2001A.Write("CAL:ZERO:TYPE EXT");   //Set to external zeroing
                else
                    PowerMeter_U2001A.Write("CAL:ZERO:TYPE INT");   //Set to internal zeroing

                PowerMeter_U2001A.Write("CAL?");                // Zeroing
                PowerMeter_U2001A.Write("*OPC?");
                _Util.Wait(30000);
                string temp = PowerMeter_U2001A.ReadString();

                if (PowerMeter_U2001A.ReadString() == "1\n")    //Throw a error if zero fail
                    return 0;
                else
                    return -99;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Configure__CW_Power(double dblFrequncy_in_MHz, int intAverageCount)
        {
            try
            {
                PowerMeter_U2001A.Write("*CLS");
                PowerMeter_U2001A.Write("*RST");

                if (intAverageCount == 0)
                    PowerMeter_U2001A.Write("AVER 0");
                else
                {
                    PowerMeter_U2001A.Write("AVER 1");
                    PowerMeter_U2001A.Write("AVER:COUN " + intAverageCount);
                }

                PowerMeter_U2001A.Write("SYST:PRES DEF");                           //Pre set to burst mode
                PowerMeter_U2001A.Write("FREQ " + dblFrequncy_in_MHz + "MHz");      //Set Freq  
                PowerMeter_U2001A.Write("TRIG:SOUR IMM");
                PowerMeter_U2001A.Write("INIT:CONT ON");                            //Set to free Run mode   
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public void Configure__Average_Modulated_Power(double dblFrequncy_in_MHz, int intAverageCount)
        {
            try
            {
                PowerMeter_U2001A.Write("*CLS");
                PowerMeter_U2001A.Write("*RST");

                if (intAverageCount == 0)
                    PowerMeter_U2001A.Write("AVER 0");
                else
                {
                    PowerMeter_U2001A.Write("AVER 1");
                    PowerMeter_U2001A.Write("AVER:COUN " + intAverageCount);
                }

                PowerMeter_U2001A.Write("SYST:PRES BURST");                         //Pre set to burst mode
                PowerMeter_U2001A.Write("FREQ " + dblFrequncy_in_MHz + "MHz");      //Set Freq
                PowerMeter_U2001A.Write("INIT:CONT ON");                            //Set to free Run mode
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Configure__Average_Pulse_Power_with_Duty_Cycle(double dblFrequncy_in_MHz,
                                                                   int intDutyCycle, int intAverageCount)
        {
            try
            {
                PowerMeter_U2001A.Write("*CLS");
                PowerMeter_U2001A.Write("*RST");

                if (intAverageCount == 0)
                    PowerMeter_U2001A.Write("AVER 0");
                else
                {
                    PowerMeter_U2001A.Write("AVER 1");
                    PowerMeter_U2001A.Write("AVER:COUN " + intAverageCount);
                }

                PowerMeter_U2001A.Write("SYST:PRES BURST");                         //Pre set to burst mode
                PowerMeter_U2001A.Write("FREQ " + dblFrequncy_in_MHz + "MHz");      //Set Freq
                PowerMeter_U2001A.Write("CORR:DCYC:STAT 1");                        //Enable DutyCycle
                PowerMeter_U2001A.Write("CORR:DCYC " + intDutyCycle);               //Set Duty Cycle
                PowerMeter_U2001A.Write("INIT:CONT ON");                            //Set to free Run mode
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Configure__Time_Gated_Burst_Power(double dblFrequncy_in_MHz, U2001_Trigger TriggerMode,
            double dblTriggerLevel_in_dBm, double dblSweepTime_in_MilliSecond, double dblSweepTime_Offset_in_MicroSecond)
        {
            try
            {
                PowerMeter_U2001A.Write("*CLS");
                PowerMeter_U2001A.Write("*RST");

                PowerMeter_U2001A.Write("SYST:PRES DEF");
                PowerMeter_U2001A.Write("FREQ " + dblFrequncy_in_MHz + "MHz");
                PowerMeter_U2001A.Write("DET:FUNC NORM");                                   //Set measurement mode to normal

                if (TriggerMode == U2001_Trigger.Internal)
                {
                    PowerMeter_U2001A.Write("TRIG:SOUR INT");
                }
                else if (TriggerMode == U2001_Trigger.External)
                    PowerMeter_U2001A.Write("TRIG:SOUR EXT");
                else
                    throw new Exception("Free Run is not allowed in this power mode");

                string SDF = PowerMeter_U2001A.Query("TRIG:LEV?");

                PowerMeter_U2001A.Write("TRIG:LEV " + dblTriggerLevel_in_dBm.ToString());   //Set trigger level
                //PowerMeter_U2001A.Write("TRIG:LEV -29.1");   //Set trigger level to lowest
                //PowerMeter_U2001A.Write("SWE:TIME " + (dblSweepTime_in_MilliSecond / 1000).ToString());  //Set Sweep time
                //PowerMeter_U2001A.Write("SWE:OFFS:TIME " + (dblSweepTime_Offset_in_MicroSecond / 1000000).ToString());   //Set Sweep time offset

                PowerMeter_U2001A.Write("SWE:TIME 0.001");
                PowerMeter_U2001A.Write("SWE:OFFS:TIME 0.0001");

                PowerMeter_U2001A.Write("CALC:FEED 'POW:AVER ON SWEEP'");   //Performs time-gated burst power measurement.
                PowerMeter_U2001A.Write("INIT:CONT ON");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Configure__Trace_Data(double dblFrequncy_in_MHz, U2001_Trigger TriggerMode,
            double dblTriggerLevel_in_dBm, double dblTraceTime_in_MilliSecond, double dblTraceTime_Offset_in_MicroSecond)
        {
            try
            {
                PowerMeter_U2001A.Write("*CLS");
                PowerMeter_U2001A.Write("*RST");

                //PowerMeter_U2001A.Write("SYST:PRES DEF");
                //PowerMeter_U2001A.Write("FREQ " + dblFrequncy_in_MHz + "MHz");
                PowerMeter_U2001A.Write("DET:FUNC NORM");                                   //Set measurement mode to normal

                //PowerMeter_U2001A.Write("TRIG:LEV " + dblTriggerLevel_in_dBm.ToString());   //Set trigger level

                if (TriggerMode == U2001_Trigger.Internal)
                    PowerMeter_U2001A.Write("TRIG:SOUR INT");
                else if (TriggerMode == U2001_Trigger.External)
                    PowerMeter_U2001A.Write("TRIG:SOUR EXT");
                else
                    throw new Exception("Free Run is not allowed in this power mode");

                PowerMeter_U2001A.Write("TRAC:STAT ON");
                PowerMeter_U2001A.Write("TRAC:TIME " + (dblTraceTime_in_MilliSecond / 1000).ToString());   //Set Sweep time
                PowerMeter_U2001A.Write("TRAC:OFFS:TIME " + (dblTraceTime_Offset_in_MicroSecond / 1000000).ToString());   //Set Sweep time offset           

                PowerMeter_U2001A.Write("TRAC:TIME 0.001");
                PowerMeter_U2001A.Write("TRAC:OFFS:TIME 0.0001");

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public double GetPowerResult()
        {
            try
            {
                PowerMeter_U2001A.Write("INIT:CONT ON");
                Wait();
                string strResult = PowerMeter_U2001A.Query("Fetch?");
                strResult = strResult.Substring(0, strResult.Length - 2);
                return Convert.ToDouble(strResult);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public string GetTraceData(U2001_TraceData_Resolution Resolution)
        {
            try
            {
                if (Resolution == U2001_TraceData_Resolution.Low_Resolution)
                {
                    PowerMeter_U2001A.Write("TRAC:DATA? LRES");
                    return PowerMeter_U2001A.ReadString();
                }
                else if (Resolution == U2001_TraceData_Resolution.Medium_Resolution)
                    return PowerMeter_U2001A.Query("TRAC:DATA? LRES");
                else
                {
                    string asd = PowerMeter_U2001A.Query("TRAC:DATA? HRES");

                    return asd;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Dispose()
        {
            try
            {
                PowerMeter_U2001A.Dispose();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        
        private void Wait()
        {
            string strResult;
            PowerMeter_U2001A.Write("*OPC?");
            strResult = PowerMeter_U2001A.ReadString();
        }
    }

    #endregion U2001A

    #region *** N1913A ***

    public class PM_N1913A
    {
        Util _Util = new Util();
        Device PowerMeter_N1913A;

        public PM_N1913A(byte GPIB_Address)
        {
            PowerMeter_N1913A = new Device(Instruments_address._00, GPIB_Address);
        }

        public bool Initialize(bool blnDisplay)
        {
            bool blnResult = false;
            try
            {
                PowerMeter_N1913A.Write("*CLS");
                PowerMeter_N1913A.Write("*RST");
                Wait();
                PowerMeter_N1913A.Write("SYST:PRES");
                Wait();
                PowerMeter_N1913A.Write("FREQ 50MHz");
                PowerMeter_N1913A.Write("MRAT DOUB");
                //PowerMeter_N1913A.Write("FORMAT REAL");
                PowerMeter_N1913A.Write("UNIT:POWER DBM");
                PowerMeter_N1913A.Write("AVER:COUN:AUTO ON");
                PowerMeter_N1913A.Write("AVER 1");
                //PowerMeter_N1913A.Write("POW:AC:RANG 1");
                Display_Enable(blnDisplay);
                Wait();
                blnResult = true;
            }
            catch (Exception ex)
            {
                throw new Exception("Power meter N1913A initialize fail. \r\n" + ex.Message);
            }

            return blnResult;
        }

        public void Display_Enable(bool enable)
        {
            if (!enable)
            {
                PowerMeter_N1913A.Write("DISP:ENAB OFF");
                PowerMeter_N1913A.Write("SERV:BACK:BRIG 15");
            }
            else
            {
                PowerMeter_N1913A.Write("DISP:ENAB ON");
                PowerMeter_N1913A.Write("SERV:BACK:BRIG 80");
            }
        }

        public int CAL()
        {
            int intResult = -99;
            string strResult = "";
            try
            {
                PowerMeter_N1913A.Write("CAL:ALL?");
                _Util.Wait(35 * 1000);
                strResult = PowerMeter_N1913A.ReadString();
                
                intResult = int.Parse(strResult);
                if (intResult == 1) intResult = -99;
            }
            catch (Exception ex)
            {
                throw new Exception("Power meter N1913A initialize fail. /r/n" + ex.Message);
            }

            return intResult;
        }

        public void Set_Power_Range(Power_Range range)
        {
            if (range == Power_Range.Lower)
                PowerMeter_N1913A.Write("POW:AC:RANG 0");
            else
                PowerMeter_N1913A.Write("POW:AC:RANG 1");

        }

        public void Configure__CW_Power(double dblFrequncy_in_MHz, int intAverageCount)
        {
            try
            {
                if (intAverageCount == 0)
                {
                    PowerMeter_N1913A.Write("AVER 0");
                }
                else if (intAverageCount == 1)
                {
                    PowerMeter_N1913A.Write("AVER:COUN:AUTO ON");
                    //PowerMeter_N1913A.Write("AVER 0");
                }
                else
                {
                    PowerMeter_N1913A.Write("AVER:COUN " + intAverageCount.ToString());
                    PowerMeter_N1913A.Write("AVER 1");
                }

                PowerMeter_N1913A.Write("FREQ " + dblFrequncy_in_MHz.ToString() + "MHz");
                PowerMeter_N1913A.Write("CORR:DCYC:STAT 0");
                PowerMeter_N1913A.Write("INIT:CONT ON");
                Wait();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Configure__Average_Pulse_Power_with_Duty_Cycle(double dblFrequncy_in_MHz,
                                                                   int intDutyCycle, int intAverageCount)
        {
            try
            {
                PowerMeter_N1913A.Write("AVER 0");
                PowerMeter_N1913A.Write("FREQ " + dblFrequncy_in_MHz.ToString() + "MHz");
                PowerMeter_N1913A.Write("CORR:DCYC:STAT 1");
                PowerMeter_N1913A.Write("CORR:DCYC " + intDutyCycle.ToString());

                if (intAverageCount == 0)
                {
                    PowerMeter_N1913A.Write("AVER 0");
                }
                else if (intAverageCount == 1)
                {
                    PowerMeter_N1913A.Write("AVER 1");
                    PowerMeter_N1913A.Write("AVER:COUN:AUTO ON");
                }
                else
                {
                    PowerMeter_N1913A.Write("AVER 1");
                    PowerMeter_N1913A.Write("AVER:COUN " + intAverageCount.ToString());
                }

                PowerMeter_N1913A.Write("SENS:POW:AC:RANG 1");
                PowerMeter_N1913A.Write("SENS:AVER:SDET ON");
                PowerMeter_N1913A.Write("INIT:CONT ON");
                Wait();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public double GetPowerResult()
        {
            string strResult;
            double dblResult;

            try
            {
                PowerMeter_N1913A.Write("FETCH?");
    
                strResult = PowerMeter_N1913A.ReadString();
                //strResult = strResult.Substring(0, strResult.Length - 1);
                dblResult = Convert.ToDouble(strResult);
                //return Math.Log10(dblResult / 0.001) * 10;
                return dblResult;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public double GetPowerResult_Byte()
        {
            double dblResult;

            try
            {
            //    PowerMeter_N1913A.Write("INIT");
            //    Wait();
                PowerMeter_N1913A.Write("FETCH?");
                Wait();
                byte[] Data = PowerMeter_N1913A.ReadByteArray();

                byte[] byteResult = Data.Reverse().ToArray();
                dblResult = BitConverter.ToDouble(byteResult, 1);
                return Math.Log10(dblResult / 0.001) * 10;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }        

        public void Dispose()
        {
            try
            {
                PowerMeter_N1913A.Write("*CLS");
                PowerMeter_N1913A.Write("*RST");
                Wait();
                PowerMeter_N1913A.Dispose();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void Wait()
        {
            string strResult;
            PowerMeter_N1913A.Write("*OPC?");
            strResult = PowerMeter_N1913A.ReadString();
        }
    }

    #endregion *** N1913A ***

    #region *** N9020A ***

    public class MXA_N9020A
    {
        Util _Util = new Util();
        Device _MXA_N9020A;

        public MXA_N9020A(byte GPIB_Address)
        {
            _MXA_N9020A = new Device(Instruments_address._00, GPIB_Address);
        }

        public void Initialize(bool Display_ON)
        {
            try
            {
                _MXA_N9020A.Write("*CLS");
                _MXA_N9020A.Write("*RST");
                Wait();
                //_MXA_N9020A.Write("*CAL?");
                //Util.Delay(20000);
                _MXA_N9020A.Write(":INST SA");               //Spectrum Analyzer mode      
                _MXA_N9020A.Write(":ROSC:SOUR:TYPE EXT");   //10MHz reference SENS / INT / EXT
                _MXA_N9020A.Write("FREQ:SPAN 0 Hz");         //Zero Span
                _MXA_N9020A.Write("TRIG:SOUR EXT2");         //Set trigger to external 1
                _MXA_N9020A.Write("TRIG:EXT2:SLOP POS");     //Trigger polarity
                _MXA_N9020A.Write("TRIG:EXT2:DEL 368 us");   //Trigger delay 100us
                _MXA_N9020A.Write("TRIG:EXT2:DEL:STAT ON");  //Trgger delay on
                //_MXA_N9020A.Write("DISP:ENAB OFF");          //Dispaly off
                //_MXA_N9020A.Write("DISP:BACK OFF");          //Backlight off
                //_MXA_N9020A.Write("DISP:BACK:INT 5");      // backlight
                //_MXA_N9020A.Write("FORM:DATA REAL,64");          
                _MXA_N9020A.Write("AVER:TYPE RMS");          //Average type
                _MXA_N9020A.Write("AVER:COUN 10");           //Average count 20
                _MXA_N9020A.Write("AVER ON");                //Turn on average
                _MXA_N9020A.Write("POW:ATT 0");              //Power attenuattor 0
                _MXA_N9020A.Write("BAND 30khz ");           //RBW 300kHz
                _MXA_N9020A.Write("SWE:TIME 0.7 ms");          //Sweep time 1ms
                _MXA_N9020A.Write(":INIT:CONT 1");           //puts analyzer in Single measurement operation. 1 for Continuous mode
                _MXA_N9020A.Write("FREQ:RF:CENT 915 MHz");   //Center frequency
                //_MXA_N9020A.Write(":INIT:CONT OFF");

                Display_Enable(Display_ON);
                Wait();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }

        public void SetFrequency(double dblValue_in_MHz)
        {
            StringBuilder sbValue = new StringBuilder(Convert.ToString(dblValue_in_MHz));
            string cmdString = "FREQ:RF:CENT " + sbValue.ToString() + " MHz";
            try
            {
                _MXA_N9020A.Write(cmdString);   //Center frequency
                Wait();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }

        public void SetFreqencySpan(double dblValue_in_MHz)
        {
            string cmdString = "FREQ:SPAN " + dblValue_in_MHz + " Hz";
            _MXA_N9020A.Write(cmdString);         //Zero Span
        }

        public void SetAttenuattor(double dblValue_in_dB)
        {
            StringBuilder sbValue = new StringBuilder(Convert.ToString(dblValue_in_dB));
            string cmdString = "POW:ATT " + sbValue.ToString();
            try
            {
                _MXA_N9020A.Write(cmdString);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }

        public void Display_Enable(bool Trun_On)
        {
            try {
                if (Trun_On)
                {
                    _MXA_N9020A.Write("DISP:ENAB ON");          //Dispaly off
                    _MXA_N9020A.Write("DISP:BACK ON");          //Backlight off
                    _MXA_N9020A.Write("DISP:BACK:INT 45");      // backlight
                }
                else
                {
                    _MXA_N9020A.Write("DISP:ENAB OFF");          //Dispaly off
                    _MXA_N9020A.Write("DISP:BACK OFF");          //Backlight off
                    _MXA_N9020A.Write("DISP:BACK:INT 5");      // backlight
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void Mod_Initialize(Modulation mode)
        {
            try
            {
                if (mode == Modulation.WCDMA)
                {
                    #region --- WCDMA ---
                    _MXA_N9020A.Write(":INST WCDMA");
                    _MXA_N9020A.Write(":INST:DEF");
                    _MXA_N9020A.Write(":SENS:RAD:STAN:DEV MS");
                    _MXA_N9020A.Write(":SENS:RAD:CONF:EHSP:STAT OFF");
                    _MXA_N9020A.Write(":SENS:RAD:CONF:HSDP:STAT OFF");
                    _MXA_N9020A.Write(":SENS:FREQ:RF:CENT 1920 MHz");
                    _MXA_N9020A.Write(":SENS:FREQ:CENT:STEP 5 MHz");
                    _MXA_N9020A.Write(":SENS:POW:ATT 20");
                    //_MXA_N9020A.Write("TRIG:SOUR EXT2");         //Set trigger to external 2
                    //_MXA_N9020A.Write("TRIG:EXT2:SLOP POS");     //Trigger polarity
                    //_MXA_N9020A.Write("TRIG:EXT2:DEL:STAT OFF");  //Trgger delay on

                    // ACP
                    _MXA_N9020A.Write(":CONF:ACP:NDEF");
                    _MXA_N9020A.Write(":SENS:ACP:AVER ON");
                    _MXA_N9020A.Write(":SENS:ACP:AVER:TCON REP");
                    _MXA_N9020A.Write(":SENS:ACP:AVER:COUN 15");
                    //_MXA_N9020A.Write(":SENS:ACP:BAND 30 kHz");
                    //_MXA_N9020A.Write(":SENS:ACP:BAND:VID 30 kHz");
                    _MXA_N9020A.Write(":SENS:ACP:BAND:SHAP GAUS");
                    _MXA_N9020A.Write(":ACP:SWE:TIME 25 ms");
                    _MXA_N9020A.Write(":TRAC:ACP:TYPE AVER");
                    _MXA_N9020A.Write(":TRIG:ACP:SOUR IMM");

                    // EVM
                    _MXA_N9020A.Write(":CONF:RHO:NDEF");
                    _MXA_N9020A.Write(":SENS:RHO:AVER ON");
                    _MXA_N9020A.Write(":SENS:RHO:AVER:TCON REP");
                    //_MXA_N9020A.Write(":SENSRHO:CAPT:TIME 1.0");
                    _MXA_N9020A.Write(":SENS:RHO:AVER:COUN 2");
                    //_MXA_N9020A.Write(":SENS:RHO:SWE:POIN 2560");
                    _MXA_N9020A.Write(":CALC:RHO:LIM:RMS 5");
                    _MXA_N9020A.Write(":CALC:RHO:LIM:FERR 300");
                    _MXA_N9020A.Write(":SENS:RHO:FILT ON");
                    _MXA_N9020A.Write(":SENS:RHO:FILT:ALPH 0.22");
                    _MXA_N9020A.Write(":SENS:RHO:CRAT 3.84 MHz");
                    _MXA_N9020A.Write(":TRIG:RHO:SOUR IMM");
                    //_MXA_N9020A.Write(":SENS:RHO:BAND 6 MHz");

                    // Channel Power
                    _MXA_N9020A.Write(":CONF:CHP");
                    _MXA_N9020A.Write(":SENS:CHP:FREQ:SPAN 7.5 MHz");
                    _MXA_N9020A.Write(":SENS:CHP:AVER ON");
                    _MXA_N9020A.Write(":SENS:CHP:AVER:TCON REP");
                    _MXA_N9020A.Write(":SENS:CHP:AVER:COUN 300");
                    _MXA_N9020A.Write(":SENS:CHP:BAND:INT 3.84MHz");
                    _MXA_N9020A.Write(":SENS:CHP:FILT OFF");
                    _MXA_N9020A.Write(":SENS:CHP:FILT:ALPH 0.22");
                    _MXA_N9020A.Write(":SENS:CHP:FILT:BAND 5MHz");
                    //_MXA_N9020A.Write(":SENS:CHP:BAND 30 kHz");
                    //_MXA_N9020A.Write(":SENS:CHP:BAND:VID 1 MHz");
                    _MXA_N9020A.Write(":SENS:CHP:BAND:SHAP GAUS");
                    _MXA_N9020A.Write(":CHP:SWE:TIME 1ms");
                    _MXA_N9020A.Write(":TRAC:CHP:TYPE AVER");
                    _MXA_N9020A.Write(":TRIG:CHP:SOUR IMM");


                    Wait();
                    #endregion --- WCDMA ---
                }
                else if (mode == Modulation.TDSCDMA)
                {
                    #region --- TDSCDMA ---
                    //Display_Enable(true);
                    _MXA_N9020A.Write(":INST TDSCDMA");
                    _MXA_N9020A.Write(":INST:DEF");
                    _MXA_N9020A.Write(":SENS:RAD:STAN:DEV MS");
                    _MXA_N9020A.Write(":SENS:SLOT TS0");
                    _MXA_N9020A.Write(":SENS:RAD:CONF:HSDP:STAT OFF");
                    _MXA_N9020A.Write(":TDEM:SCOD 0");
                    _MXA_N9020A.Write(":TDEM:UPTS 0");
                    _MXA_N9020A.Write(":TDEM:SYNC PIL");    // MID / TRI
                    _MXA_N9020A.Write(":TDEM:ULSP 1");      // Switch Point
                    _MXA_N9020A.Write(":TDEM:MXUS:TS0 16");  // Max User for slot, 16 is maxinum
                    _MXA_N9020A.Write(":TDEM:SREF MID");    // Slot freq ref PIL / MID
                    _MXA_N9020A.Write(":TDEM:CDCH:DET AUTO");   // Code channel dectection
                    //_MXA_N9020A.Write(":TDEM:MODS:AUTO ON");    // Mode Scheme
                    //_MXA_N9020A.Write(":TDEM:SCL 1");           // Channel configuration
                    _MXA_N9020A.Write(":TDEM:TREF UPTS");       // Timing Ref DPTS / UPTS / TRIG
                    _MXA_N9020A.Write(":TDEM:MCAR OFF");         // Multi-carrier demod

                    _MXA_N9020A.Write(":SENS:FREQ:RF:CENT 2025 MHz");
                    _MXA_N9020A.Write(":SENS:POW:ATT 10");
                    _MXA_N9020A.Write(":SENS:FREQ:CENT:STEP 1.6 MHz");
                    _MXA_N9020A.Write(":TRIG:EXT2:SLOP POS");     //Trigger polarity
                    _MXA_N9020A.Write(":TRIG:EXT2:DEL:STAT ON");  //Trgger delay on
                    _MXA_N9020A.Write(":TRIG:EXT2:DEL 4.616ms");  //Trgger delay on


                    //EVM
                    _MXA_N9020A.Write(":CONF:EVM:NDEF");
                    _MXA_N9020A.Write(":SENS:EVM:AVER ON");
                    _MXA_N9020A.Write(":SENS:EVM:AVER:TCON REP");
                    _MXA_N9020A.Write(":SENS:EVM:AVER:COUN 2");
                    _MXA_N9020A.Write(":CALC:EVM:LIM:RMS 5");
                    _MXA_N9020A.Write(":CALC:EVM:LIM:FERR 0.05");
                    _MXA_N9020A.Write(":SENS:EVM:RINT 1");
                    _MXA_N9020A.Write(":SENS:EVM:ANAL:SUBF 0");
                    _MXA_N9020A.Write(":TRIG:EVM:SOUR EXT1");

                    // Burst Power
                    _MXA_N9020A.Write(":CONF:TXP:NDEF");
                    _MXA_N9020A.Write("TXP:AVER:COUN 10");
                    _MXA_N9020A.Write("TXP:AVER ON");
                    _MXA_N9020A.Write("TXP:AVER:TCON REP");
                    _MXA_N9020A.Write("TXP:AVER:TYPE RMS");     // RMS / LOG
                    _MXA_N9020A.Write("TXP:METH THR");         // Meas method THReshold|BWIDth|SINGle
                    //_MXA_N9020A.Write("TXP:BURS:AUTO OFF");
                    //_MXA_N9020A.Write("TXP:BURS:WIDT 662.5us");
                    _MXA_N9020A.Write(":TRIG:TXP:SOUR EXT1");

                    // ACP
                    _MXA_N9020A.Write(":CONF:ACP:NDEF");
                    _MXA_N9020A.Write("ACP:BAND 75kHz");            //RBW
                    _MXA_N9020A.Write("ACP:BAND:VID 750kHz");       //VBW
                    _MXA_N9020A.Write("ACP:AVER ON");
                    _MXA_N9020A.Write("ACP:AVER:COUN 5");
                    _MXA_N9020A.Write("ACP:AVER:TCON REP");
                    _MXA_N9020A.Write("CALC:ACP:LIM:STAT OFF");
                    _MXA_N9020A.Write("ACP:SWE:TIME:AUTO ON");
                    _MXA_N9020A.Write("SWE:EGAT ON");           //Gate ON
                    _MXA_N9020A.Write("SWE:EGAT:VIEW OFF");           //Gate View OFF
                    _MXA_N9020A.Write("SWE:EGAT:DEL 4.616ms");      //Gate Delay 0ms
                    _MXA_N9020A.Write("SWE:EGAT:LENG 662.5us"); // Gate Length
                    _MXA_N9020A.Write("SWE:EGAT:SOUR EXT1");
                    _MXA_N9020A.Write("SWE:EGAT:CONT EDGE");
                    _MXA_N9020A.Write(":TRIG:ACP:SOUR EXT1");

                    Wait();
                    #endregion --- TDSCDMA ---
                }
                else if (mode == Modulation.EDGE)
                {
                    #region --- EDGE ---
                    //Display_Enable(true);
                    _MXA_N9020A.Write(":INST EDGEGSM");
                    _MXA_N9020A.Write(":INST:DEF");
                    _MXA_N9020A.Write("RAD:STAN:BAND EGSM");        // Band
                    _MXA_N9020A.Write("RAD:DEV MS");               // Device
                    _MXA_N9020A.Write("RAD:CARR CONT");             // BURSt|CONTinuous
                    _MXA_N9020A.Write("CHAN:SLOT 0");               // Demod Slot

                    _MXA_N9020A.Write(":SENS:FREQ:RF:CENT 915 MHz");
                    _MXA_N9020A.Write(":SENS:POW:ATT 10");
                    _MXA_N9020A.Write(":TRIG:EXT2:SLOP POS");     //Trigger polarity
                    _MXA_N9020A.Write(":TRIG:EXT2:DEL:STAT ON");  //Trgger delay on
                    _MXA_N9020A.Write(":TRIG:EXT2:DEL 0us");  //Trgger delay on

                    //EVM
                    _MXA_N9020A.Write(":CONF:EEVM:NDEF");
                    _MXA_N9020A.Write(":SENS:EEVM:AVER ON");
                    _MXA_N9020A.Write(":SENS:EEVM:AVER:TCON REP");
                    _MXA_N9020A.Write(":SENS:EEVM:AVER:COUN 10");
                    _MXA_N9020A.Write(":TRIG:EEVM:SOUR EXT2");
                    _MXA_N9020A.Write("EEVM:BSYNC:SOUR NONE");       // TSEQuence|RFBurst|PMODulation|NONE

                    // Burst Power
                    _MXA_N9020A.Write(":CONF:TXP:NDEF");
                    _MXA_N9020A.Write("TXP:BAND 510kHz");       // RBW
                    _MXA_N9020A.Write("TXP:AVER:COUN 5");
                    _MXA_N9020A.Write("TXP:AVER ON");
                    _MXA_N9020A.Write("TXP:AVER:TCON REP");
                    _MXA_N9020A.Write("TXP:AVER:TYPE RMS");     // RMS / LOG
                    _MXA_N9020A.Write("TXP:METH BWID");         // Meas method THReshold|BWIDth
                    _MXA_N9020A.Write("TXP:BURS:AUTO OFF");
                    _MXA_N9020A.Write("TXP:BURS:WIDT 1154us");
                    _MXA_N9020A.Write(":TRIG:TXP:SOUR EXT2");


                    // WaveForm Power
                    _MXA_N9020A.Write(":CONF:WAV:NDEF");
                    _MXA_N9020A.Write(":TRIG:WAV:SOUR EXT2");
                    //_MXA_N9020A.Write("WAV:SWE:TIME 562.5 us");
                    _MXA_N9020A.Write("WAV:SWE:TIME 1154 us");
                    _MXA_N9020A.Write("WAV:AVER:COUN 5");
                    _MXA_N9020A.Write("WAV:AVER ON");
                    _MXA_N9020A.Write("WAV:AVER:TCON REP");
                    _MXA_N9020A.Write("WAV:AVER:TYPE RMS");

                    // ORFS (ACP)
                    _MXA_N9020A.Write(":CONF:EORF:NDEF");
                    _MXA_N9020A.Write("EORF:AVER ON");
                    _MXA_N9020A.Write("EORF:AVER:COUN 15");
                    _MXA_N9020A.Write("EORF:TYPE MOD");
                    _MXA_N9020A.Write("EORF:AVER:FAST ON");
                    _MXA_N9020A.Write("EORF:AVER:MOD:TYPE RMS");
                    _MXA_N9020A.Write("EORF:CARR:PREF:TYPE REF");
                    _MXA_N9020A.Write("EORF:MEAS MULT");            // MULTiple|SINGle|SWEPt
                    _MXA_N9020A.Write("EORF:LIST:SEL SHOR");        // CUSTom|SHORt|STANdard
                    _MXA_N9020A.Write(":TRIG:EORF:SOUR EXT2");

                    Wait();
                    #endregion --- EDGE --- }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void Mod_Initialize(Modulation mode , bool HSDPA)
        {
            try
            {
                if (mode == Modulation.WCDMA)
                {
                    #region --- WCDMA ---
                    _MXA_N9020A.Write(":INST WCDMA");
                    _MXA_N9020A.Write(":INST:DEF");
                    _MXA_N9020A.Write(":SENS:RAD:STAN:DEV MS");
                    _MXA_N9020A.Write(":SENS:RAD:CONF:EHSP:STAT OFF");
                    _MXA_N9020A.Write(":SENS:RAD:CONF:HSDP:STAT OFF");
                    _MXA_N9020A.Write(":SENS:FREQ:RF:CENT 1920 MHz");
                    _MXA_N9020A.Write(":SENS:FREQ:CENT:STEP 5 MHz");
                    _MXA_N9020A.Write(":SENS:POW:ATT 10");
                    //_MXA_N9020A.Write("TRIG:SOUR EXT2");         //Set trigger to external 2
                    //_MXA_N9020A.Write("TRIG:EXT2:SLOP POS");     //Trigger polarity
                    //_MXA_N9020A.Write("TRIG:EXT2:DEL:STAT OFF");  //Trgger delay on

                    // ACP
                    _MXA_N9020A.Write(":CONF:ACP:NDEF");
                    _MXA_N9020A.Write(":SENS:ACP:AVER ON");
                    _MXA_N9020A.Write(":SENS:ACP:AVER:TCON REP");
                    _MXA_N9020A.Write(":SENS:ACP:AVER:COUN 15");
                    //_MXA_N9020A.Write(":SENS:ACP:BAND 30 kHz");
                    //_MXA_N9020A.Write(":SENS:ACP:BAND:VID 30 kHz");
                    _MXA_N9020A.Write(":SENS:ACP:BAND:SHAP GAUS");
                    _MXA_N9020A.Write(":ACP:SWE:TIME 25 ms");
                    _MXA_N9020A.Write(":TRAC:ACP:TYPE AVER");
                    _MXA_N9020A.Write(":TRIG:ACP:SOUR EXT1");

                    // EVM
                    _MXA_N9020A.Write(":CONF:RHO:NDEF");
                    _MXA_N9020A.Write(":SENS:RHO:AVER ON");
                    _MXA_N9020A.Write(":SENS:RHO:AVER:TCON REP");
                    //_MXA_N9020A.Write(":SENSRHO:CAPT:TIME 1.0");
                    _MXA_N9020A.Write(":SENS:RHO:AVER:COUN 2");
                    //_MXA_N9020A.Write(":SENS:RHO:SWE:POIN 2560");
                    _MXA_N9020A.Write(":CALC:RHO:LIM:RMS 5");
                    _MXA_N9020A.Write(":CALC:RHO:LIM:FERR 300");
                    _MXA_N9020A.Write(":SENS:RHO:FILT ON");
                    _MXA_N9020A.Write(":SENS:RHO:FILT:ALPH 0.22");
                    _MXA_N9020A.Write(":SENS:RHO:CRAT 3.84 MHz");
                    _MXA_N9020A.Write(":TRIG:RHO:SOUR EXT1");
                    //_MXA_N9020A.Write(":SENS:RHO:BAND 6 MHz");

                    // Channel Power
                    _MXA_N9020A.Write(":CONF:CHP:NDEF");
                    _MXA_N9020A.Write(":SENS:CHP:FREQ:SPAN 7.5 MHz");
                    _MXA_N9020A.Write(":SENS:CHP:AVER ON");
                    _MXA_N9020A.Write(":SENS:CHP:AVER:TCON REP");
                    _MXA_N9020A.Write(":SENS:CHP:AVER:COUN 30");
                    _MXA_N9020A.Write(":SENS:CHP:BAND:INT 3.84MHz");
                    _MXA_N9020A.Write(":SENS:CHP:FILT OFF");
                    _MXA_N9020A.Write(":SENS:CHP:FILT:ALPH 0.22");
                    _MXA_N9020A.Write(":SENS:CHP:FILT:BAND 3.84MHz");
                    //_MXA_N9020A.Write(":SENS:CHP:BAND 30 kHz");
                    //_MXA_N9020A.Write(":SENS:CHP:BAND:VID 1 MHz");
                    _MXA_N9020A.Write(":SENS:CHP:BAND:SHAP GAUS");
                    _MXA_N9020A.Write(":CHP:SWE:TIME 1ms");
                    _MXA_N9020A.Write(":TRAC:CHP:TYPE AVER");
                    _MXA_N9020A.Write(":TRIG:CHP:SOUR EXT1");


                    Wait();
                    #endregion --- WCDMA ---
                }
                else if (mode == Modulation.TDSCDMA)
                {
                    if (HSDPA)
                    {
                        #region --- HSDPA ---
                        //Display_Enable(true);
                        _MXA_N9020A.Write(":INST TDSCDMA");
                        _MXA_N9020A.Write(":INST:DEF");
                        _MXA_N9020A.Write(":SENS:RAD:STAN:DEV MS");
                        _MXA_N9020A.Write(":SENS:SLOT TS4");
                        _MXA_N9020A.Write(":SENS:RAD:CONF:HSDP:STAT ON");
                        _MXA_N9020A.Write(":TDEM:SCOD 0");
                        _MXA_N9020A.Write(":TDEM:UPTS 0");
                        _MXA_N9020A.Write(":TDEM:SYNC TRI");    // MID / TRI
                        _MXA_N9020A.Write(":TDEM:ULSP 1");      // Switch Point
                        _MXA_N9020A.Write(":TDEM:MXUS:TS0 16");  // Max User for slot, 16 is maxinum
                        _MXA_N9020A.Write(":TDEM:SREF MID");    // Slot freq ref PIL / MID
                        _MXA_N9020A.Write(":TDEM:CDCH:DET AUTO");   // Code channel dectection
                        //_MXA_N9020A.Write(":TDEM:MODS:AUTO ON");    // Mode Scheme
                        //_MXA_N9020A.Write(":TDEM:SCL 1");           // Channel configuration
                        _MXA_N9020A.Write(":TDEM:TREF TRIG");       // Timing Ref DPTS / UPTS / TRIG
                        _MXA_N9020A.Write(":TDEM:MCAR OFF");         // Multi-carrier demod

                        _MXA_N9020A.Write(":SENS:FREQ:RF:CENT 2025 MHz");
                        _MXA_N9020A.Write(":SENS:POW:ATT 10");
                        _MXA_N9020A.Write(":SENS:FREQ:CENT:STEP 1.6 MHz");
                        _MXA_N9020A.Write(":TRIG:EXT1:SLOP POS");     //Trigger polarity
                        //_MXA_N9020A.Write(":TRIG:EXT2:DEL:STAT ON");  //Trgger delay on
                        //_MXA_N9020A.Write(":TRIG:EXT2:DEL 4.616ms");  //Trgger delay on


                        //EVM
                        _MXA_N9020A.Write(":CONF:EVM:NDEF");
                        _MXA_N9020A.Write(":SENS:EVM:AVER ON");
                        _MXA_N9020A.Write(":SENS:EVM:AVER:TCON REP");
                        _MXA_N9020A.Write(":SENS:EVM:AVER:COUN 2");
                        _MXA_N9020A.Write(":CALC:EVM:LIM:RMS 5");
                        _MXA_N9020A.Write(":CALC:EVM:LIM:FERR 0.05");
                        _MXA_N9020A.Write(":SENS:EVM:RINT 1");
                        _MXA_N9020A.Write(":SENS:EVM:ANAL:SUBF 0");
                        _MXA_N9020A.Write(":TRIG:EVM:SOUR EXT1");

                        // Burst Power
                        _MXA_N9020A.Write(":CONF:TXP:NDEF");
                        _MXA_N9020A.Write("TXP:AVER:COUN 10");
                        _MXA_N9020A.Write("TXP:AVER ON");
                        _MXA_N9020A.Write("TXP:AVER:TCON REP");
                        _MXA_N9020A.Write("TXP:AVER:TYPE RMS");     // RMS / LOG
                        _MXA_N9020A.Write("TXP:METH THR");         // Meas method THReshold|BWIDth|SINGle
                        //_MXA_N9020A.Write("TXP:BURS:AUTO OFF");
                        //_MXA_N9020A.Write("TXP:BURS:WIDT 662.5us");
                        _MXA_N9020A.Write(":TRIG:TXP:SOUR EXT1");

                        // ACP
                        _MXA_N9020A.Write(":CONF:ACP:NDEF");
                        _MXA_N9020A.Write("ACP:BAND 75kHz");            //RBW
                        _MXA_N9020A.Write("ACP:BAND:VID 750kHz");       //VBW
                        _MXA_N9020A.Write("ACP:AVER ON");
                        _MXA_N9020A.Write("ACP:AVER:COUN 5");
                        _MXA_N9020A.Write("ACP:AVER:TCON REP");
                        _MXA_N9020A.Write("CALC:ACP:LIM:STAT OFF");
                        _MXA_N9020A.Write("ACP:SWE:TIME:AUTO ON");
                        _MXA_N9020A.Write("SWE:EGAT ON");           //Gate ON
                        _MXA_N9020A.Write("SWE:EGAT:VIEW OFF");           //Gate View OFF
                        _MXA_N9020A.Write("SWE:EGAT:DEL 360ms");      //Gate Delay 0ms
                        _MXA_N9020A.Write("SWE:EGAT:LENG 662.5us"); // Gate Length
                        _MXA_N9020A.Write("SWE:EGAT:SOUR EXT2");
                        _MXA_N9020A.Write("SWE:EGAT:CONT EDGE");
                        _MXA_N9020A.Write(":TRIG:ACP:SOUR EXT1");

                        Wait();
                        #endregion --- HSDPA ---
                    }
                    else
                    {
                        #region --- TDSCDMA ---
                        //Display_Enable(true);
                        _MXA_N9020A.Write(":INST TDSCDMA");
                        _MXA_N9020A.Write(":INST:DEF");
                        _MXA_N9020A.Write(":SENS:RAD:STAN:DEV MS");
                        _MXA_N9020A.Write(":SENS:SLOT TS0");
                        _MXA_N9020A.Write(":SENS:RAD:CONF:HSDP:STAT OFF");
                        _MXA_N9020A.Write(":TDEM:SCOD 0");
                        _MXA_N9020A.Write(":TDEM:UPTS 0");
                        _MXA_N9020A.Write(":TDEM:SYNC PIL");    // MID / TRI
                        _MXA_N9020A.Write(":TDEM:ULSP 1");      // Switch Point
                        _MXA_N9020A.Write(":TDEM:MXUS:TS0 16");  // Max User for slot, 16 is maxinum
                        _MXA_N9020A.Write(":TDEM:SREF MID");    // Slot freq ref PIL / MID
                        _MXA_N9020A.Write(":TDEM:CDCH:DET AUTO");   // Code channel dectection
                        //_MXA_N9020A.Write(":TDEM:MODS:AUTO ON");    // Mode Scheme
                        //_MXA_N9020A.Write(":TDEM:SCL 1");           // Channel configuration
                        _MXA_N9020A.Write(":TDEM:TREF UPTS");       // Timing Ref DPTS / UPTS / TRIG
                        _MXA_N9020A.Write(":TDEM:MCAR OFF");         // Multi-carrier demod

                        _MXA_N9020A.Write(":SENS:FREQ:RF:CENT 2025 MHz");
                        _MXA_N9020A.Write(":SENS:POW:ATT 10");
                        _MXA_N9020A.Write(":SENS:FREQ:CENT:STEP 1.6 MHz");
                        _MXA_N9020A.Write(":TRIG:EXT2:SLOP POS");     //Trigger polarity
                        _MXA_N9020A.Write(":TRIG:EXT2:DEL:STAT ON");  //Trgger delay on
                        _MXA_N9020A.Write(":TRIG:EXT2:DEL 4.616ms");  //Trgger delay on


                        //EVM
                        _MXA_N9020A.Write(":CONF:EVM:NDEF");
                        _MXA_N9020A.Write(":SENS:EVM:AVER ON");
                        _MXA_N9020A.Write(":SENS:EVM:AVER:TCON REP");
                        _MXA_N9020A.Write(":SENS:EVM:AVER:COUN 2");
                        _MXA_N9020A.Write(":CALC:EVM:LIM:RMS 5");
                        _MXA_N9020A.Write(":CALC:EVM:LIM:FERR 0.05");
                        _MXA_N9020A.Write(":SENS:EVM:RINT 1");
                        _MXA_N9020A.Write(":SENS:EVM:ANAL:SUBF 0");
                        _MXA_N9020A.Write(":TRIG:EVM:SOUR EXT1");

                        // Burst Power
                        _MXA_N9020A.Write(":CONF:TXP:NDEF");
                        _MXA_N9020A.Write("TXP:AVER:COUN 10");
                        _MXA_N9020A.Write("TXP:AVER ON");
                        _MXA_N9020A.Write("TXP:AVER:TCON REP");
                        _MXA_N9020A.Write("TXP:AVER:TYPE RMS");     // RMS / LOG
                        _MXA_N9020A.Write("TXP:METH THR");         // Meas method THReshold|BWIDth|SINGle
                        //_MXA_N9020A.Write("TXP:BURS:AUTO OFF");
                        //_MXA_N9020A.Write("TXP:BURS:WIDT 662.5us");
                        _MXA_N9020A.Write(":TRIG:TXP:SOUR EXT1");

                        // ACP
                        _MXA_N9020A.Write(":CONF:ACP:NDEF");
                        _MXA_N9020A.Write("ACP:BAND 75kHz");            //RBW
                        _MXA_N9020A.Write("ACP:BAND:VID 750kHz");       //VBW
                        _MXA_N9020A.Write("ACP:AVER ON");
                        _MXA_N9020A.Write("ACP:AVER:COUN 5");
                        _MXA_N9020A.Write("ACP:AVER:TCON REP");
                        _MXA_N9020A.Write("CALC:ACP:LIM:STAT OFF");
                        _MXA_N9020A.Write("ACP:SWE:TIME:AUTO ON");
                        _MXA_N9020A.Write("SWE:EGAT ON");           //Gate ON
                        _MXA_N9020A.Write("SWE:EGAT:VIEW OFF");           //Gate View OFF
                        _MXA_N9020A.Write("SWE:EGAT:DEL 4.616ms");      //Gate Delay 0ms
                        _MXA_N9020A.Write("SWE:EGAT:LENG 662.5us"); // Gate Length
                        _MXA_N9020A.Write("SWE:EGAT:SOUR EXT2");
                        _MXA_N9020A.Write("SWE:EGAT:CONT EDGE");
                        _MXA_N9020A.Write(":TRIG:ACP:SOUR EXT1");

                        Wait();
                        #endregion --- TDSCDMA --- }
                    }
                }//EDGE_CONTINOUS
                else if (mode == Modulation.EDGE_CONTINOUS)
                {
                    #region --- EDGE_CONTINOUS ---
                    //Display_Enable(true);
                    _MXA_N9020A.Write(":INST EDGEGSM");
                    _MXA_N9020A.Write(":INST:DEF");
                    _MXA_N9020A.Write("RAD:STAN:BAND EGSM");        // Band
                    _MXA_N9020A.Write("RAD:DEV MS");               // Device
                    _MXA_N9020A.Write("RAD:CARR CONT");             // BURSt|CONTinuous
                    _MXA_N9020A.Write("CHAN:SLOT 0");               // Demod Slot

                    _MXA_N9020A.Write(":SENS:FREQ:RF:CENT 915 MHz");
                    _MXA_N9020A.Write(":SENS:POW:ATT 10");
                    _MXA_N9020A.Write(":TRIG:EXT2:SLOP POS");     //Trigger polarity
                    _MXA_N9020A.Write(":TRIG:EXT2:DEL:STAT ON");  //Trgger delay on
                    _MXA_N9020A.Write(":TRIG:EXT2:DEL 0us");  //Trgger delay on

                    //EVM
                    _MXA_N9020A.Write(":CONF:EEVM:NDEF");
                    _MXA_N9020A.Write(":SENS:EEVM:AVER ON");
                    _MXA_N9020A.Write(":SENS:EEVM:AVER:TCON REP");
                    _MXA_N9020A.Write(":SENS:EEVM:AVER:COUN 10");
                    _MXA_N9020A.Write(":TRIG:EEVM:SOUR EXT2");
                    _MXA_N9020A.Write("EEVM:BSYNC:SOUR NONE");       // TSEQuence|RFBurst|PMODulation|NONE

                    // Burst Power
                    _MXA_N9020A.Write(":CONF:TXP:NDEF");
                    _MXA_N9020A.Write("TXP:BAND 510kHz");       // RBW
                    _MXA_N9020A.Write("TXP:AVER:COUN 3");
                    _MXA_N9020A.Write("TXP:AVER ON");
                    _MXA_N9020A.Write("TXP:AVER:TCON REP");
                    _MXA_N9020A.Write("TXP:AVER:TYPE RMS");     // RMS / LOG
                    _MXA_N9020A.Write("TXP:METH BWID");         // Meas method THReshold|BWIDth
                    _MXA_N9020A.Write("TXP:BURS:AUTO ON");
                    _MXA_N9020A.Write("TXP:BURS:WIDT 1154us");
                    _MXA_N9020A.Write(":TRIG:TXP:SOUR EXT2");


                    // WaveForm Power
                    _MXA_N9020A.Write(":CONF:WAV:NDEF");
                    _MXA_N9020A.Write(":TRIG:WAV:SOUR EXT2");
                    //_MXA_N9020A.Write("WAV:SWE:TIME 562.5 us");
                    _MXA_N9020A.Write("WAV:SWE:TIME 1154 us");
                    _MXA_N9020A.Write("WAV:AVER:COUN 5");
                    _MXA_N9020A.Write("WAV:AVER ON");
                    _MXA_N9020A.Write("WAV:AVER:TCON REP");
                    _MXA_N9020A.Write("WAV:AVER:TYPE RMS");

                    // ORFS (ACP)
                    _MXA_N9020A.Write(":CONF:EORF:NDEF");
                    _MXA_N9020A.Write("EORF:AVER ON");
                    _MXA_N9020A.Write("EORF:AVER:COUN 15");
                    _MXA_N9020A.Write("EORF:TYPE MOD");
                    _MXA_N9020A.Write("EORF:AVER:FAST ON");
                    _MXA_N9020A.Write("EORF:AVER:MOD:TYPE RMS");
                    _MXA_N9020A.Write("EORF:CARR:PREF:TYPE REF");
                    _MXA_N9020A.Write("EORF:MEAS MULT");            // MULTiple|SINGle|SWEPt
                    _MXA_N9020A.Write("EORF:LIST:SEL SHOR");        // CUSTom|SHORt|STANdard
                    _MXA_N9020A.Write(":TRIG:EORF:SOUR EXT2");

                    Wait();
                    #endregion --- EDGE_CONTINOUS --- }
                }
                else if (mode == Modulation.EDGE)
                {
                    #region --- EDGE ---
                    //Display_Enable(true);
                    _MXA_N9020A.Write(":INST EDGEGSM");
                    _MXA_N9020A.Write(":INST:DEF");
                    _MXA_N9020A.Write("RAD:STAN:BAND EGSM");        // Band
                    _MXA_N9020A.Write("RAD:DEV MS");               // Device
                    _MXA_N9020A.Write("RAD:CARR CONT");             // BURSt|CONTinuous
                    _MXA_N9020A.Write("CHAN:SLOT 0");               // Demod Slot

                    _MXA_N9020A.Write(":SENS:FREQ:RF:CENT 915 MHz");
                    _MXA_N9020A.Write(":SENS:POW:ATT 10");
                    _MXA_N9020A.Write(":TRIG:EXT1:SLOP POS");     //Trigger polarity
                    _MXA_N9020A.Write(":TRIG:EXT1:DEL:STAT ON");  //Trgger delay on
                    _MXA_N9020A.Write(":TRIG:EXT1:DEL 0us");  //Trgger delay on

                    //EVM
                    _MXA_N9020A.Write(":CONF:EEVM:NDEF");
                    _MXA_N9020A.Write(":SENS:EEVM:AVER ON");
                    _MXA_N9020A.Write(":SENS:EEVM:AVER:TCON REP");
                    _MXA_N9020A.Write(":SENS:EEVM:AVER:COUN 10");
                    _MXA_N9020A.Write(":TRIG:EEVM:SOUR EXT1");
                    _MXA_N9020A.Write("EEVM:BSYNC:SOUR NONE");       // TSEQuence|RFBurst|PMODulation|NONE

                    // Burst Power
                    _MXA_N9020A.Write(":CONF:TXP:NDEF");
                    _MXA_N9020A.Write("TXP:BAND 510kHz");       // RBW
                    _MXA_N9020A.Write("TXP:AVER:COUN 3");
                    _MXA_N9020A.Write("TXP:AVER ON");
                    _MXA_N9020A.Write("TXP:AVER:TCON REP");
                    _MXA_N9020A.Write("TXP:AVER:TYPE RMS");     // RMS / LOG
                    _MXA_N9020A.Write("TXP:METH BWID");         // Meas method THReshold|BWIDth
                    _MXA_N9020A.Write("TXP:BURS:AUTO ON");
                    _MXA_N9020A.Write("TXP:BURS:WIDT 577us");
                    _MXA_N9020A.Write(":TRIG:TXP:SOUR EXT1");


                    // WaveForm Power
                    _MXA_N9020A.Write(":CONF:WAV:NDEF");
                    _MXA_N9020A.Write(":TRIG:WAV:SOUR EXT1");
                    //_MXA_N9020A.Write("WAV:SWE:TIME 562.5 us");
                    _MXA_N9020A.Write("WAV:SWE:TIME 577 us");
                    _MXA_N9020A.Write("WAV:AVER:COUN 5");
                    _MXA_N9020A.Write("WAV:AVER ON");
                    _MXA_N9020A.Write("WAV:AVER:TCON REP");
                    _MXA_N9020A.Write("WAV:AVER:TYPE RMS");

                    // ORFS (ACP)
                    _MXA_N9020A.Write(":CONF:EORF:NDEF");
                    _MXA_N9020A.Write("EORF:AVER ON");
                    _MXA_N9020A.Write("EORF:AVER:COUN 15");
                    _MXA_N9020A.Write("EORF:TYPE MOD");
                    _MXA_N9020A.Write("EORF:AVER:FAST ON");
                    _MXA_N9020A.Write("EORF:AVER:MOD:TYPE RMS");
                    _MXA_N9020A.Write("EORF:CARR:PREF:TYPE REF");
                    _MXA_N9020A.Write("EORF:MEAS MULT");            // MULTiple|SINGle|SWEPt
                    _MXA_N9020A.Write("EORF:LIST:SEL SHOR");        // CUSTom|SHORt|STANdard
                    _MXA_N9020A.Write(":TRIG:EORF:SOUR EXT1");

                    Wait();
                    #endregion --- EDGE --- }
                }
                else if (mode == Modulation.LTETDD)
                {
                    #region --- LTETDD ---
                    //Display_Enable(true);
                    //_MXA_N9020A.Write(":INIT:CONT OFF");     
                    _MXA_N9020A.Write(":SENS:POW:ATT 10");
                    // Mode Setup
                    _MXA_N9020A.Write(":INST LTETDD");
                    _MXA_N9020A.Write(":INST:DEF");
                    _MXA_N9020A.Write(":RAD:STAN:DIR ULIN");            // Uplink ULIN/DLIN
                    _MXA_N9020A.Write(":RAD:STAN:ULDL CONF1");          // CONF1: Configuration 1 (DSUUDDSUUD)
                    _MXA_N9020A.Write(":RAD:STAN:PRES B10M");           // Preset to standard  B1M4|B3M|B5M|B10M|B15M|B20M
                    _MXA_N9020A.Write(":RAD:SLOT TS4");                 // Analysis slot TS4
                    _MXA_N9020A.Write(":RAD:MINT 4");                   // Measusre interval 2slots

                    // Channel Power Setup
                    _MXA_N9020A.Write(":CONF:CHP");
                    _MXA_N9020A.Write(":TRIG:CHP:SOUR EXT1");
                    _MXA_N9020A.Write(":SWE:EGAT:SOUR EXT2");
                    _MXA_N9020A.Write(":SWE:EGAT:DEL 5ms");      //Gate Delay 0ms
                    //_MXA_N9020A.Write(":CHP:AVER:COUN 5");              // Average count
                    _MXA_N9020A.Write(":CHP:AVER OFF");                  // Average OFF
                    _MXA_N9020A.Write(":CHP:SWE:TIME 60ms");

                    // ACPR E-ULTA
                    _MXA_N9020A.Write(":CONF:ACP");
                    _MXA_N9020A.Write(":TRIG:ACP:SOUR EXT1");
                    _MXA_N9020A.Write(":SWE:EGAT:SOUR EXT1");
                    _MXA_N9020A.Write(":SWE:EGAT:DEL 5ms");      //Gate Delay 0ms
                    _MXA_N9020A.Write(":ACP:SWE:TIME 60ms");                    // Sweep Time 60ms
                    _MXA_N9020A.Write(":ACP:AVER:TCON REP");                    // Average Mode EXP || REP                   
                    _MXA_N9020A.Write(":ACP:AVER:COUN 2");                      // Average count
                    _MXA_N9020A.Write(":ACP:AVER ON");                          // Average ON
                    _MXA_N9020A.Write(":ACP:OFFS2:LIST:STAT 1,0,0,0,0,0");      // Offset State
                    _MXA_N9020A.Write(":ACP:OFFS2:LIST 10MHz,0,0,0,0,0");       // Offset Frequecny
                    _MXA_N9020A.Write(":ACP:OFFS2:LIST:BAND 9MHz,9MHz,9MHz,9MHz,9MHz,9MHz");    // Offset Integ BW
                    _MXA_N9020A.Write(":ACP:OFFS2:LIST:SIDE BOTH");               // Offset Side NEG || BOTH || POS
                    _MXA_N9020A.Write(":ACP:OFFS2:LIST:FILT 0,0,0,0,0,0");       // Offset Method 1|ON = RRC Weighted, 0|OFF = Integ BW
                    _MXA_N9020A.Write(":ACP:OFFS2:LIST:FILT:ALPH 0.22, 0.22, 0.22, 0.22, 0.22, 0.22");  // Offset Filter Alpha                    
                    _MXA_N9020A.Write(":ACP:OFFS:TYPE CTOC");                   // Offset Frequency Define  CTOC || CTOE || ETOC || ETOE
                    _MXA_N9020A.Write(":ACP:FREQ:SPAN 30MHz");                  // Span 30MHz

                    // ACPR ULTA(TD-S)
                    _MXA_N9020A.Write(":CONF:ACP");
                    _MXA_N9020A.Write(":TRIG:ACP:SOUR EXT1");
                    _MXA_N9020A.Write(":SWE:EGAT:SOUR EXT1");
                    _MXA_N9020A.Write(":SWE:EGAT:DEL 5ms");      //Gate Delay 0ms
                    _MXA_N9020A.Write(":ACP:SWE:TIME 60ms");                    // Sweep Time 60ms
                    _MXA_N9020A.Write(":ACP:AVER:TCON REP");                    // Average Mode EXP || REP                   
                    _MXA_N9020A.Write(":ACP:AVER:COUN 2");                      // Average count
                    _MXA_N9020A.Write(":ACP:AVER ON");                          // Average ON
                    _MXA_N9020A.Write(":ACP:OFFS2:LIST:STAT 1,1,0,0,0,0");      // Offset State
                    _MXA_N9020A.Write(":ACP:OFFS2:LIST 5.8MHz,7.4MHz,0,0,0,0"); // Offset Frequecny
                    _MXA_N9020A.Write(":ACP:OFFS2:LIST:BAND 1.28MHz,1.28MHz,9MHz,9MHz,9MHz,9MHz");  // Offset Integ BW
                    _MXA_N9020A.Write(":ACP:OFFS2:LIST:SIDE NEG,BOTH");               // Offset Side NEG || BOTH || POS
                    _MXA_N9020A.Write(":ACP:OFFS2:LIST:FILT 1,1,0,0,0,0");       // Offset Method 1|ON = RRC Weighted, 0|OFF = Integ BW
                    _MXA_N9020A.Write(":ACP:OFFS2:LIST:FILT:ALPH 0.22, 0.22, 0.22, 0.22, 0.22, 0.22");  // Offset Filter Alpha
                    _MXA_N9020A.Write(":ACP:OFFS:TYPE CTOC");                   // Offset Frequency Define  CTOC || CTOE || ETOC || ETOE
                    _MXA_N9020A.Write(":ACP:FREQ:SPAN 17MHz");                  // Span 17MHz

                    // CEVM
                    _MXA_N9020A.Write(":CONF:CEVM");
                    _MXA_N9020A.Write(":TRIG:CEVM:SOUR EXT1");
                    _MXA_N9020A.Write(":SWE:EGAT:SOUR EXT1");

                    Wait();
                    #endregion --- LTETDD --- }
                }
                else if (mode == Modulation.LTEFDD)
                {
                    #region --- LTEFDD ---
                    //Display_Enable(true);
                    //_MXA_N9020A.Write(":INIT:CONT OFF");     
                    _MXA_N9020A.Write(":SENS:POW:ATT 10");
                    // Mode Setup
                    _MXA_N9020A.Write(":INST LTE");
                    _MXA_N9020A.Write(":INST:DEF");
                    _MXA_N9020A.Write(":RAD:STAN:PRES B10M");           // Preset to standard  B1M4|B3M|B5M|B10M|B15M|B20M
                    _MXA_N9020A.Write(":RAD:STAN:DIR ULIN");            // Uplink ULIN/DLIN

                    // Channel Power Setup
                    _MXA_N9020A.Write(":CONF:CHP");
                    _MXA_N9020A.Write(":TRIG:CHP:SOUR EXT1");
                    //_MXA_N9020A.Write(":CHP:AVER:COUN 5");              // Average count
                    _MXA_N9020A.Write(":CHP:AVER OFF");                  // Average OFF
                    _MXA_N9020A.Write(":CHP:SWE:TIME 60ms");

                    // ACPR E-ULTA
                    _MXA_N9020A.Write(":CONF:ACP");
                    _MXA_N9020A.Write(":TRIG:ACP:SOUR EXT1");
                    _MXA_N9020A.Write(":ACP:SWE:TIME 60ms");                    // Sweep Time 60ms
                    _MXA_N9020A.Write(":ACP:AVER:TCON REP");                    // Average Mode EXP || REP                   
                    _MXA_N9020A.Write(":ACP:AVER:COUN 2");                      // Average count
                    _MXA_N9020A.Write(":ACP:AVER ON");                          // Average ON
                    _MXA_N9020A.Write(":ACP:OFFS2:LIST:STAT 1,0,0,0,0,0");      // Offset State
                    _MXA_N9020A.Write(":ACP:OFFS2:LIST 10MHz,0,0,0,0,0");       // Offset Frequecny
                    _MXA_N9020A.Write(":ACP:OFFS2:LIST:BAND 9MHz,9MHz,9MHz,9MHz,9MHz,9MHz");    // Offset Integ BW
                    _MXA_N9020A.Write(":ACP:OFFS2:LIST:SIDE BOTH");               // Offset Side NEG || BOTH || POS
                    _MXA_N9020A.Write(":ACP:OFFS2:LIST:FILT 0,0,0,0,0,0");       // Offset Method 1|ON = RRC Weighted, 0|OFF = Integ BW
                    _MXA_N9020A.Write(":ACP:OFFS2:LIST:FILT:ALPH 0.22, 0.22, 0.22, 0.22, 0.22, 0.22");  // Offset Filter Alpha                    
                    _MXA_N9020A.Write(":ACP:OFFS:TYPE CTOC");                   // Offset Frequency Define  CTOC || CTOE || ETOC || ETOE
                    _MXA_N9020A.Write(":ACP:FREQ:SPAN 30MHz");                  // Span 30MHz

                    // ACPR ULTA(WCDMA)
                    _MXA_N9020A.Write(":CONF:ACP");
                    _MXA_N9020A.Write(":TRIG:ACP:SOUR EXT1");
                    _MXA_N9020A.Write(":ACP:SWE:TIME 60ms");                    // Sweep Time 60ms
                    _MXA_N9020A.Write(":ACP:AVER:TCON REP");                    // Average Mode EXP || REP                   
                    _MXA_N9020A.Write(":ACP:AVER:COUN 2");                      // Average count
                    _MXA_N9020A.Write(":ACP:AVER ON");                          // Average ON
                    _MXA_N9020A.Write(":ACP:OFFS2:LIST:STAT 1,1,0,0,0,0");      // Offset State
                    _MXA_N9020A.Write(":ACP:OFFS2:LIST 7.5MHz,12.5MHz,0,0,0,0"); // Offset Frequecny
                    _MXA_N9020A.Write(":ACP:OFFS2:LIST:BAND 3.84MHz,3.84MHz,9MHz,9MHz,9MHz,9MHz");  // Offset Integ BW
                    _MXA_N9020A.Write(":ACP:OFFS2:LIST:SIDE BOTH,BOTH");               // Offset Side NEG || BOTH || POS
                    _MXA_N9020A.Write(":ACP:OFFS2:LIST:FILT 1,1,0,0,0,0");       // Offset Method 1|ON = RRC Weighted, 0|OFF = Integ BW
                    _MXA_N9020A.Write(":ACP:OFFS2:LIST:FILT:ALPH 0.22, 0.22, 0.22, 0.22, 0.22, 0.22");  // Offset Filter Alpha
                    _MXA_N9020A.Write(":ACP:OFFS:TYPE CTOC");                   // Offset Frequency Define  CTOC || CTOE || ETOC || ETOE
                    _MXA_N9020A.Write(":ACP:FREQ:SPAN 17MHz");                  // Span 17MHz

                    // CEVM
                    _MXA_N9020A.Write(":CONF:CEVM");
                    _MXA_N9020A.Write(":TRIG:CEVM:SOUR EXT1");

                    Wait();
                    #endregion --- LTEFDD --- }
                }
                else if (mode == Modulation.CDMA)
                {
                    #region --- CDMA ---
                    _MXA_N9020A.Write(":INST CDMA2K");
                    _MXA_N9020A.Write(":INST:DEF");
                    _MXA_N9020A.Write(":SENS:RAD:STAN:DEV MS");
                    //_MXA_N9020A.Write(":SENS:RAD:CONF:EHSP:STAT OFF");
                    //_MXA_N9020A.Write(":SENS:RAD:CONF:HSDP:STAT OFF");
                    _MXA_N9020A.Write(":SENS:FREQ:RF:CENT 1920 MHz");
                    //_MXA_N9020A.Write(":SENS:FREQ:CENT:STEP 5 MHz");
                    _MXA_N9020A.Write(":SENS:POW:ATT 10");
                    _MXA_N9020A.Write("TRIG:SOUR EXT1");         //Set trigger to external 2
                    //_MXA_N9020A.Write("TRIG:EXT2:SLOP POS");     //Trigger polarity
                    //_MXA_N9020A.Write("TRIG:EXT2:DEL:STAT OFF");  //Trgger delay on

                    // ACP
                    _MXA_N9020A.Write(":CONF:ACP");
                    _MXA_N9020A.Write(":SENS:ACP:AVER OFF");
                    _MXA_N9020A.Write(":SENS:ACP:AVER:TCON REP");
                    _MXA_N9020A.Write(":SENS:ACP:AVER:COUN 64");
                    _MXA_N9020A.Write(":ACP:SWE:TIME 59.4 ms");
                    _MXA_N9020A.Write(":ACP:SWE:TIME:AUTO ON");
                    _MXA_N9020A.Write(":SENS:ACP:METH RBW");
                    _MXA_N9020A.Write(":TRAC:ACP:TYPE AVER");
                    _MXA_N9020A.Write(":TRIG:ACP:SOUR EXT1");

                    // EVM
                    _MXA_N9020A.Write(":CONF:RHO");
                    _MXA_N9020A.Write(":SENS:RHO:AVER ON");
                    _MXA_N9020A.Write(":SENS:RHO:AVER:TCON REP");
                    _MXA_N9020A.Write(":SENS:RHO:AVER:COUN 2");
                    _MXA_N9020A.Write(":TRIG:RHO:SOUR EXT1");

                    // Channel Power
                    _MXA_N9020A.Write(":CONF:CHP");
                    _MXA_N9020A.Write(":SENS:CHP:AVER ON");
                    _MXA_N9020A.Write(":SENS:CHP:AVER:TCON REP");
                    _MXA_N9020A.Write(":SENS:CHP:AVER:COUN 64");
                    _MXA_N9020A.Write(":CHP:SWE:TIME 5ms");
                    _MXA_N9020A.Write(":TRAC:CHP:TYPE AVER");
                    _MXA_N9020A.Write(":TRIG:CHP:SOUR EXT1");


                    Wait();
                    #endregion --- CDMA ---
                }
                else if (mode == Modulation.EVDO)
                {
                    #region --- EVDO ---
                    _MXA_N9020A.Write(":INST CDMA1XEV");
                    _MXA_N9020A.Write(":INST:DEF");
                    _MXA_N9020A.Write(":SENS:RAD:STAN:DEV MS");
                    _MXA_N9020A.Write(":SENS:FREQ:RF:CENT 1920 MHz");
                    _MXA_N9020A.Write(":SENS:POW:ATT 10");

                    // ACP
                    _MXA_N9020A.Write(":CONF:ACP");
                    _MXA_N9020A.Write(":SENS:ACP:AVER OFF");
                    _MXA_N9020A.Write(":SENS:ACP:AVER:TCON REP");
                    _MXA_N9020A.Write(":SENS:ACP:AVER:COUN 64");
                    _MXA_N9020A.Write(":ACP:SWE:TIME 59.4 ms");
                    _MXA_N9020A.Write(":SENS:ACP:METH RBW");
                    _MXA_N9020A.Write(":TRAC:ACP:TYPE AVER");
                    _MXA_N9020A.Write(":TRIG:ACP:SOUR EXT1");

                    // EVM
                    _MXA_N9020A.Write(":CONF:RHO:MS");
                    _MXA_N9020A.Write(":SENS:RHO:MS:AVER ON");
                    _MXA_N9020A.Write(":SENS:RHO:MS:AVER:TCON REP");
                    _MXA_N9020A.Write(":SENS:RHO:MS:AVER:COUN 2");
                    _MXA_N9020A.Write(":TRIG:RHO:MS:SOUR EXT1");

                    // Channel Power
                    _MXA_N9020A.Write(":CONF:CHP");
                    _MXA_N9020A.Write(":SENS:CHP:AVER ON");
                    _MXA_N9020A.Write(":SENS:CHP:AVER:TCON REP");
                    _MXA_N9020A.Write(":SENS:CHP:AVER:COUN 64");
                    _MXA_N9020A.Write(":CHP:SWE:TIME 10ms");
                    _MXA_N9020A.Write(":TRAC:CHP:TYPE AVER");
                    _MXA_N9020A.Write(":TRIG:CHP:SOUR EXT1");


                    Wait();
                    #endregion --- EVDO ---
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        #region --- CW Mode ---
        public void Config__CW_Power(double Freq_in_MHz, double Atten_in_dB, double ResBW_in_kHz, double TrigDelay_in_us, double SweepTime_in_ms, int Average_Count)
        {
            try
            {
                _MXA_N9020A.Write(":INST SA");                                  //Spectrum Analyzer mode 
                _MXA_N9020A.Write("CONF:SAN:NDEF");
                _MXA_N9020A.Write("FREQ:SPAN 0 Hz");                            //Zero Span
                _MXA_N9020A.Write("TRIG:SOUR EXT2");                            //Set trigger to external 1
                _MXA_N9020A.Write("TRIG:EXT2:SLOP POS");                        //Trigger polarity
                _MXA_N9020A.Write("TRIG:EXT2:DEL:STAT ON");                     //Trgger delay on
                _MXA_N9020A.Write("AVER:TYPE RMS");                             //Average type
                _MXA_N9020A.Write("AVER ON");                                   //Turn on average
                //_MXA_N9020A.Write("FORM:DATA REAL,64");          
                //_MXA_N9020A.Write(":INIT:CONT 1");                            //puts analyzer in Single measurement operation. 1 for Continuous mode
                _MXA_N9020A.Write("FREQ:RF:CENT " + Freq_in_MHz + " MHz");      //Center frequency
                _MXA_N9020A.Write("POW:ATT " + Atten_in_dB.ToString());             //Power attenuattor 0
                _MXA_N9020A.Write("BAND " + ResBW_in_kHz + " khz ");                //RBW 30kHz
                _MXA_N9020A.Write("TRIG:EXT2:DEL " + TrigDelay_in_us + " us");      //Trigger delay 100us
                _MXA_N9020A.Write("SWE:TIME " + SweepTime_in_ms + " ms");           //Sweep time 1ms
                _MXA_N9020A.Write("AVER:COUN " + Average_Count);                    //Average count 20

                Wait();
                //_MXA_N9020A.Write(":INIT:SAN");
                //_MXA_N9020A.Write(":FETCH:SAN?");
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void Config__CW_Power_FreeRun(double Freq_in_MHz, double ResBW_in_kHz, double TrigDelay_in_us, double SweepTime_in_ms, int Average_Count)
        {
            try
            {
                _MXA_N9020A.Write(":INST SA");                                  //Spectrum Analyzer mode 
                _MXA_N9020A.Write("CONF:SAN:NDEF");
                _MXA_N9020A.Write("FREQ:SPAN 0 Hz");                            //Zero Span
                _MXA_N9020A.Write("TRIG:SOUR IMM");                            //Set trigger to external 1
                _MXA_N9020A.Write("TRIG:EXT2:SLOP POS");                        //Trigger polarity
                _MXA_N9020A.Write("TRIG:EXT2:DEL:STAT ON");                     //Trgger delay on
                _MXA_N9020A.Write("AVER:TYPE RMS");                             //Average type
                _MXA_N9020A.Write("AVER ON");                                   //Turn on average
                //_MXA_N9020A.Write("FORM:DATA REAL,64");          
                //_MXA_N9020A.Write(":INIT:CONT 1");                            //puts analyzer in Single measurement operation. 1 for Continuous mode
                _MXA_N9020A.Write("FREQ:RF:CENT " + Freq_in_MHz + " MHz");      //Center frequency
                //_MXA_N9020A.Write("POW:ATT " + Atten_in_dB.ToString());             //Power attenuattor 0
                _MXA_N9020A.Write("BAND " + ResBW_in_kHz + " khz ");                //RBW 30kHz
                _MXA_N9020A.Write("TRIG:EXT2:DEL " + TrigDelay_in_us + " us");      //Trigger delay 100us
                _MXA_N9020A.Write("SWE:TIME " + SweepTime_in_ms + " ms");           //Sweep time 1ms
                _MXA_N9020A.Write("AVER:COUN " + Average_Count);                    //Average count 20

                Wait();
                //_MXA_N9020A.Write(":INIT:SAN");
                //_MXA_N9020A.Write(":FETCH:SAN?");
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public double Get_CW_PowerResult()
        {
            double dblTemp = 0;
            try
            {
                //_MXA_N9020A.Write("CONF:SAN:NDEF");  
                _MXA_N9020A.Write(":INIT:SAN");
                Wait();
                _MXA_N9020A.Write("TRAC:MATH:MEAN? TRACE1");

                string strReturn = _MXA_N9020A.ReadString();
                dblTemp = double.Parse(strReturn);

                return dblTemp;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        #endregion --- CW Mode ---

        #region --- WCDMA ---
        public void Config__WCDMA_EVM(double dblValue_in_MHz)
        {
            try
            {
                _MXA_N9020A.Write(":INST WCDMA");
                Wait();
                _MXA_N9020A.Write(":CONF:RHO");
                _MXA_N9020A.Write(":SENS:POW:ATT 20");
                _MXA_N9020A.Write(":SENS:RHO:AVER ON");
                _MXA_N9020A.Write(":SENS:RHO:AVER:TCON REP");
                //_MXA_N9020A.Write(":SENS:RHO:CAPT:TIME 1.0");
                //_MXA_N9020A.Write(":SENS:RHO:SWE:POIN 2560");
                //_MXA_N9020A.Write(":SENS:RHO:BAND 6 MHz");
                _MXA_N9020A.Write(":SENS:RHO:AVER:COUN 2");
                _MXA_N9020A.Write(":CALC:RHO:LIM:RMS 5");
                _MXA_N9020A.Write(":CALC:RHO:LIM:FERR 300");
                _MXA_N9020A.Write(":SENS:RHO:FILT ON");
                _MXA_N9020A.Write(":SENS:RHO:FILT:ALPH 0.22");
                _MXA_N9020A.Write(":SENS:RHO:CRAT 3.84 MHz");
                _MXA_N9020A.Write(":TRIG:RHO:SOUR EXT1");
                _MXA_N9020A.Write(":INIT:CONT OFF");
                _MXA_N9020A.Write(":SENS:FREQ:RF:CENT " + dblValue_in_MHz + " MHz");

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public double Get_WCDMA_EVM_Result()
        {
            double dblResult = 0;
            try
            {
                _MXA_N9020A.Write(":INIT:RHO");
                Wait();
                _MXA_N9020A.Write(":FETCH:RHO?");

                string strTemp = _MXA_N9020A.ReadString();

                string[] strReturn = strTemp.Split(',');
                dblResult = Double.Parse(strReturn[0]);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return dblResult;
        }

        public void Config__WCDMA_CHP(double dblValue_in_MHz)
        {
            try
            {
                _MXA_N9020A.Write(":INST WCDMA");
                Wait();
                _MXA_N9020A.Write(":CONF:CHP");
                _MXA_N9020A.Write(":SENS:POW:ATT 20");
                _MXA_N9020A.Write(":SENS:CHP:FREQ:SPAN 7.5 MHz");
                _MXA_N9020A.Write(":SENS:CHP:AVER ON");
                _MXA_N9020A.Write(":SENS:CHP:AVER:TCON REP");
                _MXA_N9020A.Write(":SENS:CHP:AVER:COUN 20");
                _MXA_N9020A.Write(":SENS:CHP:BAND:INT 3.84MHz");
                _MXA_N9020A.Write(":SENS:CHP:FILT OFF");
                _MXA_N9020A.Write(":SENS:CHP:FILT:ALPH 0.22");
                _MXA_N9020A.Write(":SENS:CHP:FILT:BAND 5MHz");
                //_MXA_N9020A.Write(":SENS:CHP:BAND 30 kHz");
                //_MXA_N9020A.Write(":SENS:CHP:BAND:VID 1 MHz");
                _MXA_N9020A.Write(":SENS:CHP:BAND:SHAP GAUS");
                _MXA_N9020A.Write(":CHP:SWE:TIME 20ms");
                _MXA_N9020A.Write(":TRAC:CHP:TYPE AVER");
                _MXA_N9020A.Write(":TRIG:CHP:SOUR EXT1");
                _MXA_N9020A.Write(":SENS:FREQ:RF:CENT " + dblValue_in_MHz + " MHz");
                _MXA_N9020A.Write(":INIT:CONT OFF");

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public double Get_WCDMA_CHP_Result()
        {
            double dblResult = 0;
            try
            {
                _MXA_N9020A.Write(":INIT:CHP");
                Wait();
                _MXA_N9020A.Write(":FETCH:CHP?");

                string strTemp = _MXA_N9020A.ReadString();

                string[] strReturn = strTemp.Split(',');
                dblResult = Double.Parse(strReturn[0]);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return dblResult;
        }

        public void Config__WCDMA_ACP(double dblValue_in_MHz)
        {
            try
            {
                _MXA_N9020A.Write(":INST WCDMA");
                Wait();
                _MXA_N9020A.Write(":CONF:ACP");
                _MXA_N9020A.Write(":SENS:ACP:AVER ON");
                _MXA_N9020A.Write(":SENS:ACP:AVER:TCON REP");
                _MXA_N9020A.Write(":SENS:ACP:AVER:COUN 15");
                //_MXA_N9020A.Write(":SENS:ACP:BAND 30 kHz");
                //_MXA_N9020A.Write(":SENS:ACP:BAND:VID 30 kHz");
                _MXA_N9020A.Write(":SENS:ACP:BAND:SHAP GAUS");
                _MXA_N9020A.Write(":ACP:SWE:TIME 25 ms");
                _MXA_N9020A.Write(":TRAC:ACP:TYPE AVER");
                _MXA_N9020A.Write(":TRIG:ACP:SOUR EXT1");
                _MXA_N9020A.Write(":SENS:POW:ATT 20");
                _MXA_N9020A.Write(":INIT:CONT OFF");
                _MXA_N9020A.Write(":SENS:FREQ:RF:CENT " + dblValue_in_MHz + " MHz");

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public double[] Get_WCDMA_ACP_Result()
        {
            double[] dblResult = new double[5];
            try
            {
                _MXA_N9020A.Write(":INIT:ACP");
                Wait();
                _MXA_N9020A.Write(":FETCH:ACP?");

                string strTemp = _MXA_N9020A.ReadString();

                string[] strReturn = strTemp.Split(',');
                dblResult[0] = Double.Parse(strReturn[1]);
                dblResult[1] = Double.Parse(strReturn[4]);
                dblResult[2] = Double.Parse(strReturn[6]);
                dblResult[3] = Double.Parse(strReturn[8]);
                dblResult[4] = Double.Parse(strReturn[10]);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return dblResult;
        }
        #endregion --- WCDMA ---

        #region --- TDSCDMA ---
        public void Config__TDSCDMA_EVM(double dblValue_in_MHz)
        {
            try
            {
                _MXA_N9020A.Write(":INST TDSCDMA");
                Wait();
                _MXA_N9020A.Write(":CONF:EVM:NDEF");
                _MXA_N9020A.Write(":INIT:CONT OFF");
                _MXA_N9020A.Write(":SENS:FREQ:RF:CENT " + dblValue_in_MHz + " MHz");

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public double Get_TDSCDMA_EVM_Result()
        {
            double dblResult = 0;
            try
            {
                _MXA_N9020A.Write(":INIT:EVM");
                Wait();
                _MXA_N9020A.Write(":FETCH:EVM?");

                string strTemp = _MXA_N9020A.ReadString();

                string[] strReturn = strTemp.Split(',');
                dblResult = Double.Parse(strReturn[2]);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return dblResult;
        }

        public void Config__TDSCDMA_CHP(double dblValue_in_MHz)
        {
            try
            {
                _MXA_N9020A.Write(":INST TDSCDMA");
                Wait();
                _MXA_N9020A.Write(":CONF:TXP:NDEF");
                _MXA_N9020A.Write(":INIT:CONT OFF");
                _MXA_N9020A.Write(":SENS:FREQ:RF:CENT " + dblValue_in_MHz + " MHz");

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public double Get_TDSCDMA_CHP_Result()
        {
            double dblResult = 0;
            try
            {
                _MXA_N9020A.Write(":INIT:TXP");
                Wait();
                //_Util.Wait(5);
                _MXA_N9020A.Write(":FETCH:TXP?");

                string strTemp = _MXA_N9020A.ReadString();

                string[] strReturn = strTemp.Split(',');
                dblResult = Double.Parse(strReturn[1]);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return dblResult;
        }

        public void Config__TDSCDMA_ACP(double dblValue_in_MHz, double Trigger_Delay_in_ms)
        {
            try
            {
                _MXA_N9020A.Write(":INST TDSCDMA");
                Wait();
                _MXA_N9020A.Write(":CONF:ACP:NDEF");
                _MXA_N9020A.Write("SWE:EGAT:VIEW OFF");           //Gate View OFF
                _MXA_N9020A.Write("SWE:EGAT:DEL " + Trigger_Delay_in_ms + "ms");      //Gate Delay 0ms
                _MXA_N9020A.Write("SWE:EGAT:LENG 662.5us"); // Gate Length
                _MXA_N9020A.Write(":INIT:CONT OFF");
                _MXA_N9020A.Write(":SENS:FREQ:RF:CENT " + dblValue_in_MHz + " MHz");

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public double[] Get_TDSCDMA_ACP_Result()
        {
            double[] dblResult = new double[5];
            try
            {
                _MXA_N9020A.Write(":INIT:ACP");
                Wait();
                _MXA_N9020A.Write(":FETCH:ACP?");

                string strTemp = _MXA_N9020A.ReadString();

                string[] strReturn = strTemp.Split(',');
                dblResult[0] = Double.Parse(strReturn[1]);
                dblResult[1] = Double.Parse(strReturn[4]);
                dblResult[2] = Double.Parse(strReturn[6]);
                dblResult[3] = Double.Parse(strReturn[8]);
                dblResult[4] = Double.Parse(strReturn[10]);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return dblResult;
        }
        #endregion --- TDSCDMA ---

        #region --- EDGE ---
        public void Config__EDGE_Burst_Power(double dblValue_in_MHz)
        {
            try
            {
                _MXA_N9020A.Write(":INST EDGEGSM");
                Wait();
                //_MXA_N9020A.Write(":CONF:TXP:NDEF");
                _MXA_N9020A.Write(":CONF:TXP");
                _MXA_N9020A.Write("TXP:BAND 510kHz");       // RBW
                _MXA_N9020A.Write("TXP:AVER:COUN 3");
                _MXA_N9020A.Write("TXP:AVER ON");
                _MXA_N9020A.Write("TXP:SWE:TIME 1");
                _MXA_N9020A.Write("TXP:AVER:TCON REP");
                _MXA_N9020A.Write("TXP:AVER:TYPE RMS");     // RMS / LOG
                _MXA_N9020A.Write("TXP:METH BWID");         // Meas method THReshold|BWIDth
                _MXA_N9020A.Write("TXP:BURS:AUTO ON");
                _MXA_N9020A.Write("TXP:BURS:WIDT 577us");
                _MXA_N9020A.Write(":TRIG:TXP:SOUR EXT1");
                _MXA_N9020A.Write(":SENS:FREQ:RF:CENT " + dblValue_in_MHz + " MHz");
                _MXA_N9020A.Write(":INIT:CONT OFF");

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public double Get_EDGE_Burst_Power_Result()
        {
            double dblResult = 0;
            try
            {
                _MXA_N9020A.Write(":INIT:TXP");
                Wait();
                _MXA_N9020A.Write(":FETCH:TXP?");

                string strTemp = _MXA_N9020A.ReadString();

                string[] strReturn = strTemp.Split(',');
                dblResult = Double.Parse(strReturn[2]);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return dblResult;
        }

        public double Get_EDGE_Burst_Power_CW_Result()
        {
            double dblResult = 0;
            try
            {
                _MXA_N9020A.Write(":INIT:TXP");
                Wait();
                _MXA_N9020A.Write(":FETCH:TXP?");

                string strTemp = _MXA_N9020A.ReadString();

                string[] strReturn = strTemp.Split(',');
                dblResult = Double.Parse(strReturn[2]) - Power_Mod_CW_Delta.EDGE;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return dblResult;
        }

        public void Config__EDGE_ORFS(double dblValue_in_MHz)
        {
            try
            {
                _MXA_N9020A.Write(":INST EDGEGSM");
                Wait();
                _MXA_N9020A.Write(":CONF:EORF:NDEF");
                _MXA_N9020A.Write(":CHAN:SLOT 0");
                _MXA_N9020A.Write(":CHAN:SLOT:AUTO ON");
                _MXA_N9020A.Write(":TRIG:EORF:SOUR EXT1");
                _MXA_N9020A.Write(":INIT:CONT OFF");
                _MXA_N9020A.Write(":SENS:FREQ:RF:CENT " + dblValue_in_MHz + " MHz");

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public double[] Get_EDGE_ORFS_Result()
        {
            double[] dblResult = new double[10];
            try
            {
                _MXA_N9020A.Write(":INIT:EORF");
                Wait();
                _MXA_N9020A.Write(":FETCH:EORF?");

                string strTemp = _MXA_N9020A.ReadString();

                string[] strReturn = strTemp.Split(',');
                dblResult[0] = Double.Parse(strReturn[1]);      //Ref Power
                dblResult[1] = Double.Parse(strReturn[3]);          //Ref Power
                dblResult[2] = Double.Parse(strReturn[4]);      //-200kHz
                dblResult[3] = Double.Parse(strReturn[6]);          //+200kHz
                dblResult[4] = Double.Parse(strReturn[8]);      //-250kHz
                dblResult[5] = Double.Parse(strReturn[10]);         //+250kHz
                dblResult[6] = Double.Parse(strReturn[12]);     //-400kHz
                dblResult[7] = Double.Parse(strReturn[14]);         //+400kHz
                dblResult[8] = Double.Parse(strReturn[16]);     //-600kHz
                dblResult[9] = Double.Parse(strReturn[18]);         //+600kHz

                strTemp = _MXA_N9020A.ReadString();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return dblResult;
        }

        public void Config__EDGE_ACP(double dblValue_in_MHz)
        {
            try
            {
                _MXA_N9020A.Write(":INST SA");
                Wait();
                //_MXA_N9020A.Write(":CONF:ACP:NDEF");
                _MXA_N9020A.Write(":SENS:POW:ATT 10");
                _MXA_N9020A.Write(":CONF:ACP");
                _MXA_N9020A.Write(":ACP:AVER:COUN 15");
                _MXA_N9020A.Write(":ACP:AVER:TCON EXP");
                _MXA_N9020A.Write(":ACP:CARR1:LIST:WIDT 200kHz");   // Spaceing
                _MXA_N9020A.Write(":ACP:CARR1:LIST:BAND 30kHz");    //Integ BW
                _MXA_N9020A.Write(":ACP:OFFS1:LIST 400kHz,0,0,0,0,0");    //Freq Offset
                _MXA_N9020A.Write(":ACP:OFFS1:LIST:BAND:INT 30kHz,0,0,0,0,0");    //Integ BW
                //_MXA_N9020A.Write(":ACP:OFFS1:STAT 1,0,0,0,0,0");    //Offset state
                _MXA_N9020A.Write(":ACP:METH IBWR");    //Meas method
                _MXA_N9020A.Write(":ACP:FREQ:SPAN 1MHz");
                _MXA_N9020A.Write(":SWE:EGAT:STAT ON");
                _MXA_N9020A.Write(":SWE:EGAT:SOUR EXT2");
                _MXA_N9020A.Write(":SWE:EGAT:TIME 2ms");
                _MXA_N9020A.Write(":SWE:EGAT:DEL 4.616ms");    // bypass the fisrt slot
                _MXA_N9020A.Write(":SWE:EGAT:LENG 1.154ms");
                _MXA_N9020A.Write(":SENS:FREQ:RF:CENT " + dblValue_in_MHz + " MHz");
                _MXA_N9020A.Write(":INIT:CONT OFF");

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public double[] Get_EDGE_ACP_Result()
        {
            double[] dblResult = new double[10];
            try
            {
                _MXA_N9020A.Write(":INIT:ACP");
                Wait();
                _MXA_N9020A.Write(":FETCH:ACP?");

                string strTemp = _MXA_N9020A.ReadString();

                string[] strReturn = strTemp.Split(',');
                dblResult[0] = Double.Parse(strReturn[0]);      //Ref Power
                //dblResult[1] = Double.Parse(strReturn[3]);          //Ref Power
                //dblResult[2] = Double.Parse(strReturn[4]);      //-200kHz
                //dblResult[3] = Double.Parse(strReturn[6]);          //+200kHz
                //dblResult[4] = Double.Parse(strReturn[8]);      //-250kHz
                //dblResult[5] = Double.Parse(strReturn[10]);         //+250kHz
                dblResult[6] = Double.Parse(strReturn[1]);     //-400kHz
                dblResult[7] = Double.Parse(strReturn[2]);         //+400kHz
                //dblResult[8] = Double.Parse(strReturn[16]);     //-600kHz
                //dblResult[9] = Double.Parse(strReturn[18]);         //+600kHz
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return dblResult;
        }

        public void Config__EDGE_EVM(double dblValue_in_MHz)
        {
            try
            {
                _MXA_N9020A.Write(":INST EDGEGSM");
                Wait();
                //_MXA_N9020A.Write(":CONF:EEVM:NDEF");
                _MXA_N9020A.Write(":CONF:EEVM");
                _MXA_N9020A.Write(":SENS:EEVM:AVER ON");
                _MXA_N9020A.Write(":SENS:EEVM:AVER:TCON REP");
                _MXA_N9020A.Write(":SENS:EEVM:AVER:COUN 10");
                _MXA_N9020A.Write(":TRIG:EEVM:SOUR EXT1");
                _MXA_N9020A.Write("EEVM:BSYNC:SOUR TSEQ");       // TSEQuence|RFBurst|PMODulation|NONE
                _MXA_N9020A.Write(":SENS:FREQ:RF:CENT " + dblValue_in_MHz + " MHz");
                _MXA_N9020A.Write(":INIT:CONT OFF");

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public double Get_EDGE_EVM_Result()
        {
            double dblResult = 0;
            try
            {
                _MXA_N9020A.Write(":INIT:EEVM");
                Wait();
                _MXA_N9020A.Write(":FETCH:EEVM?");

                string strTemp = _MXA_N9020A.ReadString();

                string[] strReturn = strTemp.Split(',');
                dblResult = Double.Parse(strReturn[1]);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return dblResult;
        }

        #endregion --- EDGE ---

        #region --- EDGE_CONTINOUS ---
        public void Config__EDGE_CONTINOUS_Burst_Power(double dblValue_in_MHz)
        {
            try
            {
                //_MXA_N9020A.Write(":INST EDGEGSM");
                //Wait();
                //// WaveForm Power
                //_MXA_N9020A.Write(":CONF:WAV:NDEF");
                //_MXA_N9020A.Write(":TRIG:WAV:SOUR EXT2");
                ////_MXA_N9020A.Write("WAV:SWE:TIME 562.5 us");
                //_MXA_N9020A.Write("WAV:SWE:TIME 1154 us");
                //_MXA_N9020A.Write("WAV:AVER:COUN 100");
                //_MXA_N9020A.Write("WAV:AVER ON");
                //_MXA_N9020A.Write("WAV:AVER:TCON REP");
                //_MXA_N9020A.Write("WAV:AVER:TYPE RMS");
                
                _MXA_N9020A.Write(":INST EDGEGSM");
                Wait();
                //_MXA_N9020A.Write(":CONF:TXP:NDEF");
                _MXA_N9020A.Write(":CONF:TXP");
                _MXA_N9020A.Write("TXP:BAND 510kHz");       // RBW
                _MXA_N9020A.Write("TXP:AVER:COUN 5");
                _MXA_N9020A.Write("TXP:AVER ON");
                _MXA_N9020A.Write("TXP:SWE:TIME 2");
                _MXA_N9020A.Write("TXP:AVER:TCON REP");
                _MXA_N9020A.Write("TXP:AVER:TYPE RMS");     // RMS / LOG
                _MXA_N9020A.Write("TXP:METH BWID");         // Meas method THReshold|BWIDth
                _MXA_N9020A.Write("TXP:BURS:AUTO OFF");
                _MXA_N9020A.Write("TXP:BURS:WIDT 1154us");
                _MXA_N9020A.Write("TRIG:TXP:SOUR EXT1");
                _MXA_N9020A.Write("TRIG:EXT1:DEL:STAT ON");                     //Trgger delay on
                _MXA_N9020A.Write("TRIG:EXT1:DEL 4.616ms");                     //Trgger delay 4.616ms, bypass first slot
                _MXA_N9020A.Write(":SENS:FREQ:RF:CENT " + dblValue_in_MHz + " MHz");
                _MXA_N9020A.Write(":INIT:CONT OFF");
                
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public double Get_EDGE_CONTINOUS_Burst_Power_Result()
        {
            double dblResult = 0;
            try
            {
                _MXA_N9020A.Write(":INIT:TXP");
                Wait();
                _MXA_N9020A.Write(":FETCH:TXP?");

                string strTemp = _MXA_N9020A.ReadString();

                string[] strReturn = strTemp.Split(',');
                dblResult = Double.Parse(strReturn[2]);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return dblResult;
        }

        public void Config__EDGE_CONTINOUS_ORFS(double dblValue_in_MHz)
        {
            try
            {
                _MXA_N9020A.Write(":INST EDGEGSM");
                Wait();
                _MXA_N9020A.Write(":CONF:EORF:NDEF");
                _MXA_N9020A.Write("EORF:AVER ON");
                _MXA_N9020A.Write("EORF:AVER:COUN 10");
                _MXA_N9020A.Write("EORF:TYPE MOD");
                _MXA_N9020A.Write("EORF:AVER:FAST ON");
                _MXA_N9020A.Write("EORF:AVER:MOD:TYPE RMS");
                _MXA_N9020A.Write("EORF:CARR:PREF:TYPE REF");
                _MXA_N9020A.Write("EORF:MEAS MULT");            // MULTiple|SINGle|SWEPt
                _MXA_N9020A.Write("EORF:LIST:SEL SHOR");        // CUSTom|SHORt|STANdard
                _MXA_N9020A.Write(":TRIG:EORF:SOUR EXT1");
                _MXA_N9020A.Write(":SENS:FREQ:RF:CENT " + dblValue_in_MHz + " MHz");

                _MXA_N9020A.Write(":INIT:CONT OFF");


            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public double[] Get_EDGE_CONTINOUS_ORFS_Result()
        {
            double[] dblResult = new double[10];
            try
            {
                _MXA_N9020A.Write(":INIT:EORF");
                Wait();
                _MXA_N9020A.Write(":FETCH:EORF?");

                string strTemp = _MXA_N9020A.ReadString();

                string[] strReturn = strTemp.Split(',');
                dblResult[0] = Double.Parse(strReturn[1]);      //Ref Power
                dblResult[1] = Double.Parse(strReturn[3]);          //Ref Power
                dblResult[2] = Double.Parse(strReturn[4]);      //-200kHz
                dblResult[3] = Double.Parse(strReturn[6]);          //+200kHz
                dblResult[4] = Double.Parse(strReturn[8]);      //-250kHz
                dblResult[5] = Double.Parse(strReturn[10]);         //+250kHz
                dblResult[6] = Double.Parse(strReturn[12]);     //-400kHz
                dblResult[7] = Double.Parse(strReturn[14]);         //+400kHz
                dblResult[8] = Double.Parse(strReturn[16]);     //-600kHz
                dblResult[9] = Double.Parse(strReturn[18]);         //+600kHz

                strTemp = _MXA_N9020A.ReadString();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return dblResult;
        }

        public void Config__EDGE_CONTINOUS_ACP(double dblValue_in_MHz)
        {
            try
            {
                _MXA_N9020A.Write(":INST SA");
                Wait();
                //_MXA_N9020A.Write(":CONF:ACP:NDEF");
                _MXA_N9020A.Write(":SENS:POW:ATT 10");
                _MXA_N9020A.Write(":CONF:ACP");
                _MXA_N9020A.Write(":ACP:AVER:COUN 15");
                _MXA_N9020A.Write(":ACP:AVER:TCON EXP");
                _MXA_N9020A.Write(":ACP:CARR1:LIST:WIDT 200kHz");   // Spaceing
                _MXA_N9020A.Write(":ACP:CARR1:LIST:BAND 30kHz");    //Integ BW
                _MXA_N9020A.Write(":ACP:OFFS1:LIST 400kHz,0,0,0,0,0");    //Freq Offset
                _MXA_N9020A.Write(":ACP:OFFS1:LIST:BAND:INT 30kHz,0,0,0,0,0");    //Integ BW
                //_MXA_N9020A.Write(":ACP:OFFS1:STAT 1,0,0,0,0,0");    //Offset state
                _MXA_N9020A.Write(":ACP:METH IBWR");    //Meas method
                _MXA_N9020A.Write(":ACP:FREQ:SPAN 1MHz");
                _MXA_N9020A.Write(":SWE:EGAT:STAT ON");
                _MXA_N9020A.Write(":SWE:EGAT:SOUR EXT1");
                _MXA_N9020A.Write(":SWE:EGAT:TIME 2ms");
                _MXA_N9020A.Write(":SWE:EGAT:DEL 4.616ms");    // bypass the fisrt slot
                _MXA_N9020A.Write(":SWE:EGAT:LENG 1.154ms");
                _MXA_N9020A.Write(":SENS:FREQ:RF:CENT " + dblValue_in_MHz + " MHz");
                _MXA_N9020A.Write(":INIT:CONT OFF");

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public double[] Get_EDGE_CONTINOUS_ACP_Result()
        {
            double[] dblResult = new double[10];
            try
            {
                _MXA_N9020A.Write(":INIT:ACP");
                Wait();
                _MXA_N9020A.Write(":FETCH:ACP?");

                string strTemp = _MXA_N9020A.ReadString();

                string[] strReturn = strTemp.Split(',');
                dblResult[0] = Double.Parse(strReturn[0]);      //Ref Power
                //dblResult[1] = Double.Parse(strReturn[3]);          //Ref Power
                //dblResult[2] = Double.Parse(strReturn[4]);      //-200kHz
                //dblResult[3] = Double.Parse(strReturn[6]);          //+200kHz
                //dblResult[4] = Double.Parse(strReturn[8]);      //-250kHz
                //dblResult[5] = Double.Parse(strReturn[10]);         //+250kHz
                dblResult[6] = Double.Parse(strReturn[1]);     //-400kHz
                dblResult[7] = Double.Parse(strReturn[2]);         //+400kHz
                //dblResult[8] = Double.Parse(strReturn[16]);     //-600kHz
                //dblResult[9] = Double.Parse(strReturn[18]);         //+600kHz
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return dblResult;
        }

        public void Config__EDGE_CONTINOUS_EVM(double dblValue_in_MHz)
        {
            try
            {
                _MXA_N9020A.Write(":INST EDGEGSM");
                Wait();
                //_MXA_N9020A.Write(":CONF:EEVM:NDEF");
                _MXA_N9020A.Write(":CONF:EEVM");
                _MXA_N9020A.Write(":SENS:EEVM:AVER ON");
                _MXA_N9020A.Write(":SENS:EEVM:AVER:TCON REP");
                _MXA_N9020A.Write(":SENS:EEVM:AVER:COUN 10");
                _MXA_N9020A.Write(":TRIG:EEVM:SOUR EXT1");
                _MXA_N9020A.Write("EEVM:BSYNC:SOUR NONE");       // TSEQuence|RFBurst|PMODulation|NONE
                _MXA_N9020A.Write(":SENS:FREQ:RF:CENT " + dblValue_in_MHz + " MHz");
                _MXA_N9020A.Write(":INIT:CONT OFF");

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public double Get_EDGE_CONTINOUS_EVM_Result()
        {
            double dblResult = 0;
            try
            {
                _MXA_N9020A.Write(":INIT:EEVM");
                Wait();
                _MXA_N9020A.Write(":FETCH:EEVM?");

                string strTemp = _MXA_N9020A.ReadString();

                string[] strReturn = strTemp.Split(',');
                dblResult = Double.Parse(strReturn[1]);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return dblResult;
        }

        #endregion --- EDGE_CONTINOUS ---

        #region --- LTETDD ---
        public void Config__LTETDD_EVM(double dblValue_in_MHz)
        {
            try
            {
                _MXA_N9020A.Write(":INST LTETDD");
                Wait();
                // CEVM
                _MXA_N9020A.Write(":CONF:CEVM");
                _MXA_N9020A.Write(":TRIG:CEVM:SOUR EXT1");
                _MXA_N9020A.Write(":SWE:EGAT:SOUR EXT1");
                _MXA_N9020A.Write(":SWE:EGAT:DEL 12ms");      //Gate Delay 0ms
                _MXA_N9020A.Write(":SENS:POW:ATT 20");
                _MXA_N9020A.Write(":INIT:CONT OFF");
                _MXA_N9020A.Write(":SENS:FREQ:RF:CENT " + dblValue_in_MHz + " MHz");

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public double Get_LTETDD_EVM_Result()
        {
            double dblResult = 0;
            try
            {
                _MXA_N9020A.Write(":INIT:CEVM");
                Wait();
                _MXA_N9020A.Write(":FETCH:CEVM?");

                string strTemp = _MXA_N9020A.ReadString();

                string[] strReturn = strTemp.Split(',');
                dblResult = Double.Parse(strReturn[0]);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return dblResult;
        }

        public void Config__LTETDD_CHP(double dblValue_in_MHz)
        {
            try
            {
                _MXA_N9020A.Write(":INST LTETDD");
                Wait();

                _MXA_N9020A.Write(":CONF:CHP");
                _MXA_N9020A.Write(":TRIG:CHP:SOUR EXT1");
                _MXA_N9020A.Write(":SWE:EGAT:SOUR EXT1");
                _MXA_N9020A.Write(":SWE:EGAT:DEL 12ms");      //Gate Delay 0ms
                //_MXA_N9020A.Write(":CHP:AVER:COUN 5");              // Average count
                _MXA_N9020A.Write(":CHP:AVER OFF");                  // Average OFF
                _MXA_N9020A.Write(":CHP:SWE:TIME 60ms");
                _MXA_N9020A.Write(":SENS:POW:ATT 20");
                _MXA_N9020A.Write(":SENS:FREQ:RF:CENT " + dblValue_in_MHz + " MHz");
                _MXA_N9020A.Write(":INIT:CONT OFF");

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public double Get_LTETDD_CHP_Result()
        {
            double dblResult = 0;
            try
            {
                _MXA_N9020A.Write(":INIT:CHP");
                Wait();
                _MXA_N9020A.Write(":FETCH:CHP?");

                string strTemp = _MXA_N9020A.ReadString();

                string[] strReturn = strTemp.Split(',');
                dblResult = Double.Parse(strReturn[0]);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return dblResult;
        }

        public void Config__LTETDD_ACP_EULTRA(double dblValue_in_MHz)
        {
            try
            {
                _MXA_N9020A.Write(":INST LTETDD");
                Wait();
                // ACPR E-ULTA
                _MXA_N9020A.Write(":CONF:ACP");
                _MXA_N9020A.Write(":TRIG:ACP:SOUR EXT1");
                _MXA_N9020A.Write(":SWE:EGAT:SOUR EXT1");
                _MXA_N9020A.Write(":SWE:EGAT:DEL 12ms");      //Gate Delay 0ms
                _MXA_N9020A.Write(":ACP:SWE:TIME 60ms");                    // Sweep Time 60ms
                _MXA_N9020A.Write(":ACP:AVER:TCON REP");                    // Average Mode EXP || REP                   
                _MXA_N9020A.Write(":ACP:AVER:COUN 2");                      // Average count
                _MXA_N9020A.Write(":ACP:AVER ON");                          // Average ON
                _MXA_N9020A.Write(":ACP:OFFS2:LIST:STAT 1,0,0,0,0,0");      // Offset State
                _MXA_N9020A.Write(":ACP:OFFS2:LIST 10MHz,0,0,0,0,0");       // Offset Frequecny
                _MXA_N9020A.Write(":ACP:OFFS2:LIST:BAND 9MHz,9MHz,9MHz,9MHz,9MHz,9MHz");    // Offset Integ BW
                _MXA_N9020A.Write(":ACP:OFFS2:LIST:SIDE BOTH");               // Offset Side NEG || BOTH || POS
                _MXA_N9020A.Write(":ACP:OFFS2:LIST:FILT 0,0,0,0,0,0");       // Offset Method 1|ON = RRC Weighted, 0|OFF = Integ BW
                _MXA_N9020A.Write(":ACP:OFFS2:LIST:FILT:ALPH 0.22, 0.22, 0.22, 0.22, 0.22, 0.22");  // Offset Filter Alpha                    
                _MXA_N9020A.Write(":ACP:OFFS:TYPE CTOC");                   // Offset Frequency Define  CTOC || CTOE || ETOC || ETOE
                _MXA_N9020A.Write(":ACP:FREQ:SPAN 30MHz");                  // Span 30MHz    
                _MXA_N9020A.Write(":SENS:POW:ATT 20");
                _MXA_N9020A.Write(":INIT:CONT OFF");
                _MXA_N9020A.Write(":SENS:FREQ:RF:CENT " + dblValue_in_MHz + " MHz");

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public double[] Get_LTETDD_ACP_EULTRA_Result()
        {
            double[] dblResult = new double[5];
            try
            {
                _MXA_N9020A.Write(":INIT:ACP");
                Wait();
                _MXA_N9020A.Write(":FETCH:ACP?");

                string strTemp = _MXA_N9020A.ReadString();

                string[] strReturn = strTemp.Split(',');
                dblResult[0] = Double.Parse(strReturn[1]);
                dblResult[1] = Double.Parse(strReturn[4]);
                dblResult[2] = Double.Parse(strReturn[6]);
                dblResult[3] = Double.Parse(strReturn[8]);
                dblResult[4] = Double.Parse(strReturn[10]);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return dblResult;
        }

        public void Config__LTETDD_ACP_ULTRA(double dblValue_in_MHz)
        {
            try
            {
                _MXA_N9020A.Write(":INST LTETDD");
                Wait();
                // ACPR ULTRA
                _MXA_N9020A.Write(":CONF:ACP");
                _MXA_N9020A.Write(":TRIG:ACP:SOUR EXT1");
                _MXA_N9020A.Write(":SWE:EGAT:SOUR EXT1");
                _MXA_N9020A.Write(":SWE:EGAT:DEL 12ms");      //Gate Delay 0ms
                _MXA_N9020A.Write(":ACP:SWE:TIME 60ms");                    // Sweep Time 60ms
                _MXA_N9020A.Write(":ACP:AVER:TCON REP");                    // Average Mode EXP || REP                   
                _MXA_N9020A.Write(":ACP:AVER:COUN 2");                      // Average count
                _MXA_N9020A.Write(":ACP:AVER ON");                          // Average ON
                _MXA_N9020A.Write(":ACP:OFFS2:LIST:STAT 1,1,0,0,0,0");      // Offset State
                _MXA_N9020A.Write(":ACP:OFFS2:LIST 5.8MHz,7.4MHz,0,0,0,0"); // Offset Frequecny
                _MXA_N9020A.Write(":ACP:OFFS2:LIST:BAND 1.28MHz,1.28MHz,9MHz,9MHz,9MHz,9MHz");  // Offset Integ BW
                _MXA_N9020A.Write(":ACP:OFFS2:LIST:SIDE BOTH,BOTH");               // Offset Side NEG || BOTH || POS
                _MXA_N9020A.Write(":ACP:OFFS2:LIST:FILT 1,1,0,0,0,0");       // Offset Method 1|ON = RRC Weighted, 0|OFF = Integ BW
                _MXA_N9020A.Write(":ACP:OFFS2:LIST:FILT:ALPH 0.22, 0.22, 0.22, 0.22, 0.22, 0.22");  // Offset Filter Alpha
                _MXA_N9020A.Write(":ACP:OFFS:TYPE CTOC");                   // Offset Frequency Define  CTOC || CTOE || ETOC || ETOE
                _MXA_N9020A.Write(":ACP:FREQ:SPAN 17MHz");                  // Span 17MHz  
                _MXA_N9020A.Write(":SENS:POW:ATT 10");
                _MXA_N9020A.Write(":INIT:CONT OFF");
                _MXA_N9020A.Write(":SENS:FREQ:RF:CENT " + dblValue_in_MHz + " MHz");

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public double[] Get_LTETDD_ACP_ULTRA_Result()
        {
            double[] dblResult = new double[5];
            try
            {
                _MXA_N9020A.Write(":INIT:ACP");
                Wait();
                _MXA_N9020A.Write(":FETCH:ACP?");

                string strTemp = _MXA_N9020A.ReadString();

                string[] strReturn = strTemp.Split(',');
                dblResult[0] = Double.Parse(strReturn[1]);
                dblResult[1] = Double.Parse(strReturn[4]);
                dblResult[2] = Double.Parse(strReturn[6]);
                dblResult[3] = Double.Parse(strReturn[8]);
                dblResult[4] = Double.Parse(strReturn[10]);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return dblResult;
        }
        #endregion --- LTETDD ---

        #region --- LTEFDD ---
        public void Config__LTEFDD_EVM(double dblValue_in_MHz)
        {
            try
            {
                _MXA_N9020A.Write(":INST LTE");
                Wait();
                // CEVM
                _MXA_N9020A.Write(":CONF:CEVM");
                _MXA_N9020A.Write(":TRIG:CEVM:SOUR EXT1");
                _MXA_N9020A.Write(":SENS:POW:ATT 10");
                _MXA_N9020A.Write(":INIT:CONT OFF");
                _MXA_N9020A.Write(":SENS:FREQ:RF:CENT " + dblValue_in_MHz + " MHz");

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public double Get_LTEFDD_EVM_Result()
        {
            double dblResult = 0;
            try
            {
                _MXA_N9020A.Write(":INIT:CEVM");
                Wait();
                _MXA_N9020A.Write(":FETCH:CEVM?");

                string strTemp = _MXA_N9020A.ReadString();

                string[] strReturn = strTemp.Split(',');
                dblResult = Double.Parse(strReturn[0]);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return dblResult;
        }

        public void Config__LTEFDD_CHP(double dblValue_in_MHz)
        {
            try
            {
                _MXA_N9020A.Write(":INST LTE");
                Wait();

                _MXA_N9020A.Write(":CONF:CHP");
                _MXA_N9020A.Write(":TRIG:CHP:SOUR EXT1");
                _MXA_N9020A.Write(":CHP:AVER:COUN 10");              // Average count
                _MXA_N9020A.Write(":CHP:AVER ON");                  // Average OFF
                _MXA_N9020A.Write(":CHP:SWE:TIME 60ms");
                _MXA_N9020A.Write(":SENS:POW:ATT 10");
                _MXA_N9020A.Write(":SENS:FREQ:RF:CENT " + dblValue_in_MHz + " MHz");
                _MXA_N9020A.Write(":INIT:CONT OFF");

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public double Get_LTEFDD_CHP_Result()
        {
            double dblResult = 0;
            try
            {
                _MXA_N9020A.Write(":INIT:CHP");
                Wait();
                _MXA_N9020A.Write(":FETCH:CHP?");

                string strTemp = _MXA_N9020A.ReadString();

                string[] strReturn = strTemp.Split(',');
                dblResult = Double.Parse(strReturn[0]);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return dblResult;
        }

        public void Config__LTEFDD_ACP_EULTRA(double dblValue_in_MHz)
        {
            try
            {
                _MXA_N9020A.Write(":INST LTE");
                Wait();
                // ACPR E-ULTA
                _MXA_N9020A.Write(":CONF:ACP");
                _MXA_N9020A.Write(":TRIG:ACP:SOUR EXT1");
                _MXA_N9020A.Write(":ACP:SWE:TIME 60ms");                    // Sweep Time 60ms
                _MXA_N9020A.Write(":ACP:AVER:TCON REP");                    // Average Mode EXP || REP                   
                _MXA_N9020A.Write(":ACP:AVER:COUN 5");                      // Average count
                _MXA_N9020A.Write(":ACP:AVER ON");                          // Average ON
                _MXA_N9020A.Write(":ACP:OFFS2:LIST:STAT 1,0,0,0,0,0");      // Offset State
                _MXA_N9020A.Write(":ACP:OFFS2:LIST 10MHz,0,0,0,0,0");       // Offset Frequecny
                _MXA_N9020A.Write(":ACP:OFFS2:LIST:BAND 9MHz,9MHz,9MHz,9MHz,9MHz,9MHz");    // Offset Integ BW
                _MXA_N9020A.Write(":ACP:OFFS2:LIST:SIDE BOTH");               // Offset Side NEG || BOTH || POS
                _MXA_N9020A.Write(":ACP:OFFS2:LIST:FILT 0,0,0,0,0,0");       // Offset Method 1|ON = RRC Weighted, 0|OFF = Integ BW
                _MXA_N9020A.Write(":ACP:OFFS2:LIST:FILT:ALPH 0.22, 0.22, 0.22, 0.22, 0.22, 0.22");  // Offset Filter Alpha                    
                _MXA_N9020A.Write(":ACP:OFFS:TYPE CTOC");                   // Offset Frequency Define  CTOC || CTOE || ETOC || ETOE
                _MXA_N9020A.Write(":ACP:FREQ:SPAN 30MHz");                  // Span 30MHz
                _MXA_N9020A.Write(":SENS:POW:ATT 10");
                _MXA_N9020A.Write(":INIT:CONT OFF");
                _MXA_N9020A.Write(":SENS:FREQ:RF:CENT " + dblValue_in_MHz + " MHz");

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public double[] Get_LTEFDD_ACP_EULTRA_Result()
        {
            double[] dblResult = new double[5];
            try
            {
                _MXA_N9020A.Write(":INIT:ACP");
                Wait();
                _MXA_N9020A.Write(":FETCH:ACP?");

                string strTemp = _MXA_N9020A.ReadString();

                string[] strReturn = strTemp.Split(',');
                dblResult[0] = Double.Parse(strReturn[1]);
                dblResult[1] = Double.Parse(strReturn[4]);
                dblResult[2] = Double.Parse(strReturn[6]);
                dblResult[3] = Double.Parse(strReturn[8]);
                dblResult[4] = Double.Parse(strReturn[10]);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return dblResult;
        }

        public void Config__LTEFDD_ACP_ULTRA(double dblValue_in_MHz)
        {
            try
            {
                _MXA_N9020A.Write(":INST LTE");
                Wait();
                // ACPR ULTRA
                _MXA_N9020A.Write(":CONF:ACP");
                _MXA_N9020A.Write(":TRIG:ACP:SOUR EXT1");
                _MXA_N9020A.Write(":ACP:SWE:TIME 60ms");                    // Sweep Time 60ms
                _MXA_N9020A.Write(":ACP:AVER:TCON REP");                    // Average Mode EXP || REP                   
                _MXA_N9020A.Write(":ACP:AVER:COUN 5");                      // Average count
                _MXA_N9020A.Write(":ACP:AVER ON");                          // Average ON
                _MXA_N9020A.Write(":ACP:OFFS2:LIST:STAT 1,1,0,0,0,0");      // Offset State
                _MXA_N9020A.Write(":ACP:OFFS2:LIST 7.5MHz,12.5MHz,0,0,0,0"); // Offset Frequecny
                _MXA_N9020A.Write(":ACP:OFFS2:LIST:BAND 3.84MHz,3.84MHz,9MHz,9MHz,9MHz,9MHz");  // Offset Integ BW
                _MXA_N9020A.Write(":ACP:OFFS2:LIST:SIDE BOTH,BOTH");               // Offset Side NEG || BOTH || POS
                _MXA_N9020A.Write(":ACP:OFFS2:LIST:FILT 1,1,0,0,0,0");       // Offset Method 1|ON = RRC Weighted, 0|OFF = Integ BW
                _MXA_N9020A.Write(":ACP:OFFS2:LIST:FILT:ALPH 0.22, 0.22, 0.22, 0.22, 0.22, 0.22");  // Offset Filter Alpha
                _MXA_N9020A.Write(":ACP:OFFS:TYPE CTOC");                   // Offset Frequency Define  CTOC || CTOE || ETOC || ETOE
                _MXA_N9020A.Write(":ACP:FREQ:SPAN 17MHz");                  // Span 17MHz
                _MXA_N9020A.Write(":SENS:POW:ATT 10");
                _MXA_N9020A.Write(":INIT:CONT OFF");
                _MXA_N9020A.Write(":SENS:FREQ:RF:CENT " + dblValue_in_MHz + " MHz");

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public double[] Get_LTEFDD_ACP_ULTRA_Result()
        {
            double[] dblResult = new double[5];
            try
            {
                _MXA_N9020A.Write(":INIT:ACP");
                Wait();
                _MXA_N9020A.Write(":FETCH:ACP?");

                string strTemp = _MXA_N9020A.ReadString();

                string[] strReturn = strTemp.Split(',');
                dblResult[0] = Double.Parse(strReturn[1]);
                dblResult[1] = Double.Parse(strReturn[4]);
                dblResult[2] = Double.Parse(strReturn[6]);
                dblResult[3] = Double.Parse(strReturn[8]);
                dblResult[4] = Double.Parse(strReturn[10]);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return dblResult;
        }
        #endregion --- LTETDD ---

        #region --- CDMA ---
        public void Config__CDMA_EVM(double dblValue_in_MHz)
        {
            try
            {
                _MXA_N9020A.Write(":INST CDMA2K");
                Wait();
                _MXA_N9020A.Write(":CONF:RHO");
                _MXA_N9020A.Write(":SENS:RHO:AVER ON");
                _MXA_N9020A.Write(":SENS:RHO:AVER:TCON REP");
                _MXA_N9020A.Write(":SENS:RHO:AVER:COUN 2");
                _MXA_N9020A.Write(":TRIG:RHO:SOUR EXT1");

                _MXA_N9020A.Write(":INIT:CONT OFF");
                _MXA_N9020A.Write(":SENS:FREQ:RF:CENT " + dblValue_in_MHz + " MHz");

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public double Get_CDMA_EVM_Result()
        {
            double dblResult = 0;
            try
            {
                _MXA_N9020A.Write(":INIT:RHO");
                Wait();
                _MXA_N9020A.Write(":FETCH:RHO?");

                string strTemp = _MXA_N9020A.ReadString();

                string[] strReturn = strTemp.Split(',');
                dblResult = Double.Parse(strReturn[0]);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return dblResult;
        }

        public void Config__CDMA_CHP(double dblValue_in_MHz)
        {
            try
            {
                _MXA_N9020A.Write(":INST CDMA2K");
                Wait();
                _MXA_N9020A.Write(":CONF:CHP");
                _MXA_N9020A.Write(":SENS:CHP:AVER ON");
                _MXA_N9020A.Write(":SENS:CHP:AVER:TCON REP");
                _MXA_N9020A.Write(":SENS:CHP:AVER:COUN 64");
                _MXA_N9020A.Write(":CHP:SWE:TIME 5ms");
                _MXA_N9020A.Write(":TRAC:CHP:TYPE AVER");
                _MXA_N9020A.Write(":TRIG:CHP:SOUR EXT1");

                _MXA_N9020A.Write(":SENS:FREQ:RF:CENT " + dblValue_in_MHz + " MHz");
                _MXA_N9020A.Write(":INIT:CONT OFF");

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public double Get_CDMA_CHP_Result()
        {
            double dblResult = 0;
            try
            {
                _MXA_N9020A.Write(":INIT:CHP");
                Wait();
                _MXA_N9020A.Write(":FETCH:CHP?");

                string strTemp = _MXA_N9020A.ReadString();

                string[] strReturn = strTemp.Split(',');
                dblResult = Double.Parse(strReturn[0]);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return dblResult;
        }

        public void Config__CDMA_ACP(double dblValue_in_MHz)
        {
            try
            {
                _MXA_N9020A.Write(":INST CDMA2K");
                Wait();
                _MXA_N9020A.Write(":CONF:ACP");
                _MXA_N9020A.Write(":SENS:ACP:AVER OFF");
                _MXA_N9020A.Write(":SENS:ACP:AVER:TCON REP");
                _MXA_N9020A.Write(":SENS:ACP:AVER:COUN 64");
                _MXA_N9020A.Write(":ACP:SWE:TIME 59.4 ms");
                _MXA_N9020A.Write(":ACP:SWE:TIME:AUTO ON");
                _MXA_N9020A.Write(":SENS:ACP:METH RBW");
                _MXA_N9020A.Write(":TRAC:ACP:TYPE AVER");
                _MXA_N9020A.Write(":TRIG:ACP:SOUR EXT1");

                _MXA_N9020A.Write(":SENS:POW:ATT 10");
                _MXA_N9020A.Write(":INIT:CONT OFF");
                _MXA_N9020A.Write(":SENS:FREQ:RF:CENT " + dblValue_in_MHz + " MHz");

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public double[] Get_CDMA_ACP_Result()
        {
            double[] dblResult = new double[5];
            try
            {
                _MXA_N9020A.Write(":INIT:ACP");
                Wait();
                _MXA_N9020A.Write(":FETCH:ACP?");

                string strTemp = _MXA_N9020A.ReadString();

                string[] strReturn = strTemp.Split(',');
                dblResult[0] = Double.Parse(strReturn[1]);
                dblResult[1] = Double.Parse(strReturn[4]);
                dblResult[2] = Double.Parse(strReturn[6]);
                dblResult[3] = Double.Parse(strReturn[8]);
                dblResult[4] = Double.Parse(strReturn[10]);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return dblResult;
        }
        #endregion --- CDMA ---

        #region --- EVDO ---
        public void Config__EVDO_EVM(double dblValue_in_MHz)
        {
            try
            {
                _MXA_N9020A.Write(":INST CDMA1XEV");
                Wait();
                _MXA_N9020A.Write(":CONF:RHO:MS");
                _MXA_N9020A.Write(":SENS:RHO:MS:AVER ON");
                _MXA_N9020A.Write(":SENS:RHO:MS:AVER:TCON REP");
                _MXA_N9020A.Write(":SENS:RHO:MS:AVER:COUN 2");
                _MXA_N9020A.Write(":TRIG:RHO:MS:SOUR EXT1");

                _MXA_N9020A.Write(":INIT:CONT OFF");
                _MXA_N9020A.Write(":SENS:FREQ:RF:CENT " + dblValue_in_MHz + " MHz");

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public double Get_EVDO_EVM_Result()
        {
            double dblResult = 0;
            try
            {
                _MXA_N9020A.Write(":INIT:RHO:MS");
                Wait();
                _MXA_N9020A.Write(":FETCH:RHO:MS?");

                string strTemp = _MXA_N9020A.ReadString();

                string[] strReturn = strTemp.Split(',');
                dblResult = Double.Parse(strReturn[0]);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return dblResult;
        }

        public void Config__EVDO_CHP(double dblValue_in_MHz)
        {
            try
            {
                _MXA_N9020A.Write(":INST CDMA1XEV");
                Wait();

                _MXA_N9020A.Write(":CONF:CHP");
                _MXA_N9020A.Write(":SENS:CHP:AVER ON");
                _MXA_N9020A.Write(":SENS:CHP:AVER:TCON REP");
                _MXA_N9020A.Write(":SENS:CHP:AVER:COUN 64");
                _MXA_N9020A.Write(":CHP:SWE:TIME 10ms");
                _MXA_N9020A.Write(":TRAC:CHP:TYPE AVER");
                _MXA_N9020A.Write(":TRIG:CHP:SOUR EXT1");

                _MXA_N9020A.Write(":SENS:FREQ:RF:CENT " + dblValue_in_MHz + " MHz");
                _MXA_N9020A.Write(":INIT:CONT OFF");

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public double Get_EVDO_CHP_Result()
        {
            double dblResult = 0;
            try
            {
                _MXA_N9020A.Write(":INIT:CHP");
                Wait();
                _MXA_N9020A.Write(":FETCH:CHP?");

                string strTemp = _MXA_N9020A.ReadString();

                string[] strReturn = strTemp.Split(',');
                dblResult = Double.Parse(strReturn[0]);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return dblResult;
        }

        public void Config__EVDO_ACP(double dblValue_in_MHz)
        {
            try
            {
                _MXA_N9020A.Write(":INST CDMA1XEV");
                Wait();
                _MXA_N9020A.Write(":CONF:ACP");
                _MXA_N9020A.Write(":SENS:ACP:AVER OFF");
                _MXA_N9020A.Write(":SENS:ACP:AVER:TCON REP");
                _MXA_N9020A.Write(":SENS:ACP:AVER:COUN 64");
                _MXA_N9020A.Write(":ACP:SWE:TIME 59.4 ms");
                _MXA_N9020A.Write(":SENS:ACP:METH RBW");
                _MXA_N9020A.Write(":TRAC:ACP:TYPE AVER");
                _MXA_N9020A.Write(":TRIG:ACP:SOUR EXT1");

                _MXA_N9020A.Write(":SENS:POW:ATT 10");
                _MXA_N9020A.Write(":INIT:CONT OFF");
                _MXA_N9020A.Write(":SENS:FREQ:RF:CENT " + dblValue_in_MHz + " MHz");

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public double[] Get_EVDO_ACP_Result()
        {
            double[] dblResult = new double[5];
            try
            {
                _MXA_N9020A.Write(":INIT:ACP");
                Wait();
                _MXA_N9020A.Write(":FETCH:ACP?");

                string strTemp = _MXA_N9020A.ReadString();

                string[] strReturn = strTemp.Split(',');
                dblResult[0] = Double.Parse(strReturn[1]);
                dblResult[1] = Double.Parse(strReturn[4]);
                dblResult[2] = Double.Parse(strReturn[6]);
                dblResult[3] = Double.Parse(strReturn[8]);
                dblResult[4] = Double.Parse(strReturn[10]);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return dblResult;
        }
        #endregion --- EVDO ---

        public void Dispose()
        {
            try
            {
                _MXA_N9020A.Write("DISP:BACK:INT 50");      // backlight
                _MXA_N9020A.Write("DISP:BACK ON");          //Backlight on
                _MXA_N9020A.Write("DISP:ENAB ON");          //Dispaly on
                _MXA_N9020A.Write("*CLS");
                _MXA_N9020A.Write("*RST");
                Wait();
                Wait();
                _MXA_N9020A.Dispose();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        private void Wait()
        {
            string strResult;
            _MXA_N9020A.Write("*OPC?");
            strResult = _MXA_N9020A.ReadString();
        }
    }

    #endregion N9020A

    #region *** 33120A ***

    public class Arb_33120A
    {
        Util _Util = new Util();
        Device AWG_33120A;

        public Arb_33120A(byte GPIB_Address)
        {
            AWG_33120A = new Device(Instruments_address._00, GPIB_Address);
        }

        public void Initialize()
        {
            try
            {
                AWG_33120A.Write("*CLS");
                AWG_33120A.Write("*RST");
                //AWG_33120A.Write("DISP:TEXT 'VC5268'");
                AWG_33120A.Write("VOLT:UNIT VPP");              //Set Amplitude Unit
                AWG_33120A.Write("OUTP:LOAD INF");                 //LOAD
                AWG_33120A.Write("APPL:DC DEF, DEF, 1.8");            //Frequency, Amplitude, DC Offset
                //AWG_33120A.Write("PHAS 20");                    //Set intial phase
                //AWG_33120A.Write("PULS:DCYC 25");               //Set pulse duty cycle
                //AWG_33120A.Write("OUTP ON");                    //Source Out
                //AWG_33120A.Write("VOLT:HIGH 1.5");              //Set high voltage
                //AWG_33120A.Write("VOLT:LOW 0");                 //Set low woltage
                AWG_33120A.Write("OUTP:SYNC ON");                   //SYNC Out
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }

        public void SetHighVoltage(double dblHighVoltage_in_Volt)
        {
            try
            {
                AWG_33120A.Write("VOLT:HIGH  " + dblHighVoltage_in_Volt.ToString());
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void SetArbOut(string Arb_Waveform_Type, double dblFreq_in_KHz, double dblVoltate_in_Volt, double dblDCOffset_in_Volt)
        {
            try
            {
                AWG_33120A.Write("VOLT:UNIT VPP");              //Set Amplitude Unit

                StringBuilder sbCommand = new StringBuilder("APPL:");
                sbCommand.Append(Arb_Waveform_Type + " ");
                sbCommand.Append(dblFreq_in_KHz.ToString() + ",");
                sbCommand.Append(dblVoltate_in_Volt.ToString() + ",");
                if (dblDCOffset_in_Volt != 0)
                    sbCommand.Append(dblDCOffset_in_Volt.ToString());

                AWG_33120A.Write(sbCommand.ToString());
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void SYNC_OUT(bool SyncOut)
        {
            try
            {
                if (SyncOut)
                    AWG_33120A.Write("OUTP:SYNC ON");
                else
                    AWG_33120A.Write("OUTP:SYNC OFF");
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void Dispose()
        {
            try
            {
                AWG_33120A.Dispose();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

    }

    #endregion 33120A

    #region *** DG1022 ***

    public class Arb_DG1022
    {
        //Device DG1022 = new Device(Instruments_address.Address_00, Instruments_address.Arb_DG1022);
        MessageBasedSession DG1022 = (MessageBasedSession)ResourceManager.GetLocalManager().Open(Instruments_VISA.Arb_DG1022);
        Util _Util = new Util();
        int intdelayTime = 100;

        public void Initialize(double dblFrequency_in_KHz)
        {
            try
            {
                DG1022.Query("*IDN?");
                _Util.Wait(intdelayTime);
                DG1022.Write("VOLT:UNIT VPP");              //Set Amplitude Unit
                _Util.Wait(intdelayTime);
                DG1022.Write("VOLT:UNIT:CH2 VPP");
                _Util.Wait(intdelayTime);
                //Util.Delay(intdelayTime);
                //DG1022.Write("VOLT:LOW 0");                 //Set Low Voltage
                //Util.Delay(intdelayTime);
                //DG1022.Write("VOLT:LOW:CH2 0");
                //Util.Delay(intdelayTime);
                DG1022.Write("APPL:PULS:CH2 " + dblFrequency_in_KHz.ToString() + ",1.8,0.9");      //Frequency , Amplitude, DC Offset
                _Util.Wait(intdelayTime);
                DG1022.Write("APPL:PULS " + dblFrequency_in_KHz.ToString() + ",1.25,0.625");
                _Util.Wait(intdelayTime);
                //DG1022.Write("PHAS 20");                    //Set intial phase
                //DG1022.Write("PHAS:CH2 20");
                DG1022.Write("PULS:DCYC 26");               //Set pulse duty cycle
                _Util.Wait(intdelayTime);
                DG1022.Write("PULS:DCYC:CH2 25");
                _Util.Wait(intdelayTime);
                //DG1022.Write("VOLT:LOW 0");                 //Set low woltage
                //Util.Delay(intdelayTime);
                //DG1022.Write("VOLT:LOW:CH2 0");
                //Util.Delay(intdelayTime);
                //DG1022.Write("VOLT:HIGH 1.25");              //Set high voltage
                //Util.Delay(intdelayTime);
                //DG1022.Write("VOLT:HIGH:CH2 1.8");
                //Util.Delay(intdelayTime);
                DG1022.Write("OUTP:SYNC ON");                    //SYNC Out
                _Util.Wait(intdelayTime);
                DG1022.Write("OUTP ON");                    //Source Out
                _Util.Wait(intdelayTime);
                DG1022.Write("OUTP:CH2 ON");
                _Util.Wait(intdelayTime);
                DG1022.Write("PHAS:ALIGN");                 //2 channel phase synchronize
                _Util.Wait(intdelayTime);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }

        public void SetHighVoltage(double dblHighVoltage_in_Volt, Arb_Channel ArbChannel)
        {
            try
            {
                if (ArbChannel == Arb_Channel.Channel_1)
                {
                    DG1022.Write("VOLT:HIGH " + dblHighVoltage_in_Volt.ToString());
                    _Util.Wait(intdelayTime);
                }
                else
                {
                    DG1022.Write("VOLT:HIGH:CH2 " + dblHighVoltage_in_Volt.ToString());
                    _Util.Wait(intdelayTime);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void SetArbOut(string Arb_Waveform_Type, Arb_Channel Channel,
            double dblFreq_in_KHz, double dblVoltate_in_Volt, double dblDCOffset_in_Volt, int intDutyCycle)
        {
            try
            {
                DG1022.Write("VOLT:UNIT VPP");              //Set Amplitude Unit
                _Util.Wait(intdelayTime);
                DG1022.Write("VOLT:UNIT:CH2 VPP");
                _Util.Wait(intdelayTime);

                StringBuilder sbCommand = new StringBuilder("APPL:");
                sbCommand.Append(Arb_Waveform_Type);
                if (Channel == Arb_Channel.Channel_2)
                {
                    sbCommand.Append(":CH2");
                    DG1022.Write("PULS:DCYC:CH2 " + intDutyCycle.ToString());
                    _Util.Wait(intdelayTime);
                }
                else
                {
                    DG1022.Write("PULS:DCYC " + intDutyCycle.ToString());
                    _Util.Wait(intdelayTime);
                }
                sbCommand.Append(" " + dblFreq_in_KHz.ToString() + ",");
                sbCommand.Append(dblVoltate_in_Volt.ToString() + ",");
                if (dblDCOffset_in_Volt != 0)
                    sbCommand.Append(dblDCOffset_in_Volt.ToString());

                DG1022.Write(sbCommand.ToString());
                _Util.Wait(intdelayTime);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void SetOutput(bool Status, Arb_Channel Channel)
        {

            if (Status)
            {
                if (Channel == Arb_Channel.Channel_1)
                    DG1022.Write("OUTP ON");                    //Source Out
                else
                    DG1022.Write("OUTP:CH2 ON");
            }
            else
            {
                if (Channel == Arb_Channel.Channel_1)
                    DG1022.Write("OUTP OFF");                    //Source Out
                else
                    DG1022.Write("OUTP:CH2 OFF");
            }

            _Util.Wait(intdelayTime);

        }

        public void PhaseAlign()
        {
            try
            {
                DG1022.Write("PHAS:ALIGN");
                _Util.Wait(intdelayTime);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void Dispose()
        {
            try
            {
                DG1022.Dispose();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }

    #endregion DG1022

    #region *** 33220A ***

    public class Arb_33220A
    {
        Util _Util = new Util();
        Device AWG_33220A;

        public Arb_33220A(byte GPIB_Address)
        {
            AWG_33220A = new Device(Instruments_address._00, GPIB_Address);
        }

        public void Initialize()
        {
            try
            {
                AWG_33220A.Write("*CLS");
                AWG_33220A.Write("*RST");
                AWG_33220A.Write("VOLT:UNIT VPP");              //Set Amplitude Unit
                AWG_33220A.Write("OUTP:LOAD INF");                 //LOAD
                AWG_33220A.Write("APPL:DC DEF,DEF,1.8");      //Frequency, Amplitude, DC Offset
                //AWG_33120A.Write("PHAS 20");                    //Set intial phase
                //AWG_33120A.Write("PULS:DCYC 25");               //Set pulse duty cycle
                //AWG_33120A.Write("OUTP ON");                    //Source Out
                //AWG_33120A.Write("VOLT:HIGH 1.5");              //Set high voltage
                //AWG_33120A.Write("VOLT:LOW 0");                 //Set low woltage
                AWG_33220A.Write("OUTP:SYNC ON");                 //SYNC Out
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }

        public void SetDCLevel(double dblDCVoltage_in_Volt)
        {
            try
            {
                AWG_33220A.Write("VOLT:UNIT VPP");              //Set Amplitude Unit
                AWG_33220A.Write("APPL:DC DEF,DEF," + dblDCVoltage_in_Volt.ToString());      //Frequency, Amplitude, DC Offset
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }
        public void SetHighVoltage(double dblHighVoltage_in_Volt)
        {
            try
            {
                AWG_33220A.Write("VOLT:HIGH  " + dblHighVoltage_in_Volt.ToString());
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void SetArbOut(string Arb_Waveform_Type, double dblFreq_in_KHz, double dblVoltate_in_Volt, double dblDCOffset_in_Volt)
        {
            try
            {
                AWG_33220A.Write("VOLT:UNIT VPP");              //Set Amplitude Unit

                StringBuilder sbCommand = new StringBuilder("APPL:");
                sbCommand.Append(Arb_Waveform_Type + " ");
                sbCommand.Append(dblFreq_in_KHz.ToString() + ",");
                sbCommand.Append(dblVoltate_in_Volt.ToString() + ",");
                if (dblDCOffset_in_Volt != 0)
                    sbCommand.Append(dblDCOffset_in_Volt.ToString());

                AWG_33220A.Write(sbCommand.ToString());
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void SYNC_OUT(Output Status)
        {
            try
            {
                if (Status == Output.ON)
                    AWG_33220A.Write("OUTP:SYNC ON");
                else if (Status == Output.OFF)
                    AWG_33220A.Write("OUTP:SYNC OFF");
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void Dispose()
        {
            try
            {
                AWG_33220A.Write("*CLS");
                AWG_33220A.Write("*RST");
                AWG_33220A.Dispose();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }

    #endregion 33220A

    #region *** 33522A ***

    public class Arb_33522A
    {
        Util _Util = new Util();
        Device AWG_33522A;

        public Arb_33522A(byte GPIB_Address)
        {
            AWG_33522A = new Device(Instruments_address._00, GPIB_Address);
        }


        public void Initialize(double dblFrequency_in_Hz)
        {
            try
            {
                AWG_33522A.Write("*CLS");
                AWG_33522A.Write("*RST");
                _Util.Wait(50);
                AWG_33522A.Write("SOUR1:VOLT:UNIT VPP");                //Set Amplitude Unit
                AWG_33522A.Write("SOUR2:VOLT:UNIT VPP");
                AWG_33522A.Write("OUTP1:LOAD INF");                 //Load {<ohms>|INFinity|MINimum|MAXimum}
                AWG_33522A.Write("OUTP2:LOAD INF");                 //Load {<ohms>|INFinity|MINimum|MAXimum}
                AWG_33522A.Write("SOUR1:APPL:PULS " + dblFrequency_in_Hz.ToString() + " , 1.25 , 0.625");      //Frequency, Amplitude, DC Offset
                AWG_33522A.Write("SOUR2:APPL:PULS " + dblFrequency_in_Hz.ToString() + " , 1.8 , 0.9");
                //AWG_33522A.Write("SOUR1:PHAS 20");                    //Set intial phase
                //AWG_33522A.Write("SOUR2:PHAS 20");
                AWG_33522A.Write("SOUR1:FUNC:PULS:DCYC 26");                 //Set pulse duty cycle
                AWG_33522A.Write("SOUR2:FUNC:PULS:DCYC 25");
                //AWG_33522A.Write("SOUR1:VOLT:LOW 0");                   //Set low woltage
                //AWG_33522A.Write("SOUR2:VOLT:LOW 0");
                //AWG_33522A.Write("SOUR1:VOLT:HIGH 1.25");               //Set high voltage
                //AWG_33522A.Write("SOUR2:VOLT:HIGH 1.8");                
                AWG_33522A.Write("OUTP:SYNC ON");                    //Sync Out
                AWG_33522A.Write("OUTP1 ON");                           //Source Out
                AWG_33522A.Write("OUTP2 ON");
                AWG_33522A.Write("PHAS:SYNC");                    //Sync PHASE

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            _Util.Wait(20);
        }

        public void SetHighVoltage(double dblHighVoltage_in_Volt, Arb_Channel ArbChannel)
        {
            try
            {
                if (ArbChannel == Arb_Channel.Channel_1)
                    AWG_33522A.Write("SOUR1:VOLT:HIGH " + dblHighVoltage_in_Volt.ToString());
                else
                    AWG_33522A.Write("SOUR2:VOLT:HIGH " + dblHighVoltage_in_Volt.ToString());

                AWG_33522A.Write("PHAS:SYNC");                    //Sync PHASE
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void SetArbOut(string Arb_Waveform_Type, Arb_Channel Channel,
            double dblFreq_in_KHz, double dblHighVoltate_in_Volt, double dblLowVoltage_in_Volt, int intDutyCycle)
        {
            try
            {
                if (Channel == Arb_Channel.Channel_1)
                {
                    AWG_33522A.Write("OUTP1:LOAD INF");
                    AWG_33522A.Write("SOUR1:VOLT:UNIT VPP");              //Set Amplitude Unit
                    AWG_33522A.Write("SOUR1:FUNC SQU");
                    AWG_33522A.Write("SOUR1:FUNC:SQU:DCYC " + intDutyCycle);
                    AWG_33522A.Write("SOUR1:FREQ " + dblFreq_in_KHz + "Hz");
                    AWG_33522A.Write("SOUR1:VOLT:LOW " + dblLowVoltage_in_Volt);
                    AWG_33522A.Write("SOUR1:VOLT:HIGH " + dblHighVoltate_in_Volt);
                    //AWG_33522A.Write("SOUR1:VOLT " + dblVoltate_in_Volt);
                    //AWG_33522A.Write("SOUR1:VOLT:OFFS " + dblDCOffset_in_Volt);
                    
                }
                else if (Channel == Arb_Channel.Channel_2)
                {
                    AWG_33522A.Write("OUTP2:LOAD INF");
                    AWG_33522A.Write("SOUR2:VOLT:UNIT VPP");              //Set Amplitude Unit
                    AWG_33522A.Write("SOUR2:FUNC SQU");
                    AWG_33522A.Write("SOUR2:FUNC:SQU:DCYC " + intDutyCycle);
                    AWG_33522A.Write("SOUR2:FREQ " + dblFreq_in_KHz + "Hz");
                    AWG_33522A.Write("SOUR2:VOLT:LOW " + dblLowVoltage_in_Volt);
                    AWG_33522A.Write("SOUR2:VOLT:HIGH " + dblHighVoltate_in_Volt);
                    //AWG_33522A.Write("SOUR2:VOLT " + dblVoltate_in_Volt);
                    //AWG_33522A.Write("SOUR2:VOLT:OFFS " + dblDCOffset_in_Volt);
                }
                AWG_33522A.Write("PHAS:SYNC");                    //Sync PHASE
                SetOutput(Output.ON, Channel);

                #region ---# APPLy #---
                //AWG_33522A.Write("SOUR1:VOLT:UNIT VPP");              //Set Amplitude Unit
                //AWG_33522A.Write("SOUR2:VOLT:UNIT VPP");

                //StringBuilder sbCommand = new StringBuilder("SOUR");

                //if (Channel == Arb_Channel.Channel_1)
                //{
                //    sbCommand.Append("1:APPL:");
                //    AWG_33522A.Write("SOUR1:FUNC:PULS:DCYC " + intDutyCycle.ToString());
                //}
                //else
                //{
                //    sbCommand.Append("2:APPL:");
                //    AWG_33522A.Write("SOUR2:FUNC:PULS:DCYC " + intDutyCycle.ToString());
                //}

                //sbCommand.Append(Arb_Waveform_Type + " ");
                //sbCommand.Append(dblFreq_in_KHz.ToString() + " , ");
                //sbCommand.Append(dblVoltate_in_Volt.ToString() + " , ");
                //if (dblDCOffset_in_Volt != 0)
                //    sbCommand.Append(dblDCOffset_in_Volt.ToString());

                //AWG_33522A.Write(sbCommand.ToString());

                ////AWG_33522A.Write("PHAS:SYNC");                    //Sync PHASE               
                #endregion
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void SetArbOut(string Arb_Waveform_Type, Arb_Channel Channel,
            double dblFreq_in_KHz, double dblHighVoltate_in_Volt, double dblLowVoltage_in_Volt, int intDutyCycle, bool SyncPhase)
        {
            try
            {
                if (Channel == Arb_Channel.Channel_1)
                {
                    AWG_33522A.Write("OUTP1:LOAD INF");
                    AWG_33522A.Write("SOUR1:VOLT:UNIT VPP");              //Set Amplitude Unit
                    AWG_33522A.Write("SOUR1:FUNC SQU");
                    AWG_33522A.Write("SOUR1:FUNC:SQU:DCYC " + intDutyCycle);
                    AWG_33522A.Write("SOUR1:FREQ " + dblFreq_in_KHz + "Hz");
                    AWG_33522A.Write("SOUR1:VOLT:LOW " + dblLowVoltage_in_Volt);
                    AWG_33522A.Write("SOUR1:VOLT:HIGH " + dblHighVoltate_in_Volt);
                    //AWG_33522A.Write("SOUR1:VOLT " + dblVoltate_in_Volt);
                    //AWG_33522A.Write("SOUR1:VOLT:OFFS " + dblDCOffset_in_Volt);

                }
                else if (Channel == Arb_Channel.Channel_2)
                {
                    AWG_33522A.Write("OUTP2:LOAD INF");
                    AWG_33522A.Write("SOUR2:VOLT:UNIT VPP");              //Set Amplitude Unit
                    AWG_33522A.Write("SOUR2:FUNC SQU");
                    AWG_33522A.Write("SOUR2:FUNC:SQU:DCYC " + intDutyCycle);
                    AWG_33522A.Write("SOUR2:FREQ " + dblFreq_in_KHz + "Hz");
                    AWG_33522A.Write("SOUR2:VOLT:LOW " + dblLowVoltage_in_Volt);
                    AWG_33522A.Write("SOUR2:VOLT:HIGH " + dblHighVoltate_in_Volt);
                    //AWG_33522A.Write("SOUR2:VOLT " + dblVoltate_in_Volt);
                    //AWG_33522A.Write("SOUR2:VOLT:OFFS " + dblDCOffset_in_Volt);
                }
                if (SyncPhase)
                {
                    AWG_33522A.Write("PHAS:SYNC");                    //Sync PHASE
                }
                SetOutput(Output.ON, Channel);

                #region ---# APPLy #---
                //AWG_33522A.Write("SOUR1:VOLT:UNIT VPP");              //Set Amplitude Unit
                //AWG_33522A.Write("SOUR2:VOLT:UNIT VPP");

                //StringBuilder sbCommand = new StringBuilder("SOUR");

                //if (Channel == Arb_Channel.Channel_1)
                //{
                //    sbCommand.Append("1:APPL:");
                //    AWG_33522A.Write("SOUR1:FUNC:PULS:DCYC " + intDutyCycle.ToString());
                //}
                //else
                //{
                //    sbCommand.Append("2:APPL:");
                //    AWG_33522A.Write("SOUR2:FUNC:PULS:DCYC " + intDutyCycle.ToString());
                //}

                //sbCommand.Append(Arb_Waveform_Type + " ");
                //sbCommand.Append(dblFreq_in_KHz.ToString() + " , ");
                //sbCommand.Append(dblVoltate_in_Volt.ToString() + " , ");
                //if (dblDCOffset_in_Volt != 0)
                //    sbCommand.Append(dblDCOffset_in_Volt.ToString());

                //AWG_33522A.Write(sbCommand.ToString());

                ////AWG_33522A.Write("PHAS:SYNC");                    //Sync PHASE               
                #endregion
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void SetBurstTrig(Arb_Channel Channel , double dblTrigDelay_in_ms , int intCycle)
        {
            try
            {
                if (Channel == Arb_Channel.Channel_1)
                {
                    AWG_33522A.Write("SOUR1:BURS:MODE TRIG");
                    if (intCycle == 0)
                    { 
                        AWG_33522A.Write("SOUR1:BURS:NCYC INF"); 
                    }
                    else
                    {
                        AWG_33522A.Write("SOUR1:BURS:NCYC " + intCycle.ToString());
                    }
                    AWG_33522A.Write("SOUR1:BURS:PHAS 0");
                    AWG_33522A.Write("TRIG1:SOUR EXT");
                    AWG_33522A.Write("TRIG1:SLOP POS");
                    AWG_33522A.Write("TRIG1:DEL " + dblTrigDelay_in_ms.ToString() + "ms");
                    AWG_33522A.Write("SOUR1:BURS:STAT ON");
                }
                else if (Channel == Arb_Channel.Channel_2)
                {
                    AWG_33522A.Write("SOUR2:BURS:MODE TRIG");
                    if (intCycle == 0)
                    { 
                        AWG_33522A.Write("SOUR1:BURS:NCYC INF");
                    }
                    else
                    {
                        AWG_33522A.Write("SOUR1:BURS:NCYC " + intCycle.ToString());
                    }
                    AWG_33522A.Write("SOUR2:BURS:PHAS 0");
                    AWG_33522A.Write("TRIG2:SOUR EXT");
                    AWG_33522A.Write("TRIG2:SLOP POS");
                    AWG_33522A.Write("TRIG2:DEL " + dblTrigDelay_in_ms.ToString() + "ms");
                    AWG_33522A.Write("SOUR2:BURS:STAT ON");
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void SetBurstModeOFF()
        {
            try
            {
                AWG_33522A.Write("SOUR1:BURS:STAT OFF");
                AWG_33522A.Write("SOUR2:BURS:STAT OFF");
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void SetDC(Arb_Channel Channel ,double dblVoltage_in_Volts)
        {
            try
            {
                if (Channel == Arb_Channel.Channel_1)
                {
                    AWG_33522A.Write("SOUR1:APPL:DC DEF,DEF," + dblVoltage_in_Volts.ToString());
                }
                else if (Channel == Arb_Channel.Channel_2)
                {
                    AWG_33522A.Write("SOUR2:APPL:DC DEF,DEF," + dblVoltage_in_Volts.ToString());
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void SYNC_OUT(Arb_Channel Channel, Output Status)
        {
            try
            {
                if (Channel == Arb_Channel.Channel_1)
                    AWG_33522A.Write("OUTP:SYNC:SOUR CH1");
                else if (Channel == Arb_Channel.Channel_2)
                    AWG_33522A.Write("OUTP:SYNC:SOUR CH2");

                if (Status == Output.ON)
                    AWG_33522A.Write("OUTP:SYNC ON");
                else if (Status == Output.OFF)
                    AWG_33522A.Write("OUTP:SYNC OFF");
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void SetOutput(Output Status, Arb_Channel Channel)
        {
            if (Channel == Arb_Channel.Channel_1)
            {
                if (Status == Output.ON)
                {
                    AWG_33522A.Write("OUTP1 ON");
                }
                else
                    AWG_33522A.Write("OUTP1 OFF");
            }
            else
            {
                if (Status == Output.ON)
                {
                    AWG_33522A.Write("OUTP2 ON");
                }
                else
                    AWG_33522A.Write("OUTP2 OFF");
            }
        }

        public void Dispose()
        {
            try
            {
                AWG_33522A.Write("*CLS");
                AWG_33522A.Write("*RST");
                _Util.Wait(50);
                AWG_33522A.Dispose();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }

    #endregion 33522A

    #region *** 33522A USB ***

    public class Arb_33522A_USB
    {
        Util _Util = new Util();
        //Device AWG_33522A;
        MessageBasedSession AWG_33522A_USB;

        public Arb_33522A_USB(string VISA_Address_String)
        {
            AWG_33522A_USB = (MessageBasedSession)ResourceManager.GetLocalManager().Open(VISA_Address_String);
        }


        public void Initialize(double dblFrequency_in_Hz)
        {
            try
            {
                AWG_33522A_USB.Write("*CLS");
                AWG_33522A_USB.Write("*RST");
                _Util.Wait(50);
                AWG_33522A_USB.Write("SOUR1:VOLT:UNIT VPP");                //Set Amplitude Unit
                AWG_33522A_USB.Write("SOUR2:VOLT:UNIT VPP");
                AWG_33522A_USB.Write("OUTP1:LOAD INF");                 //Load {<ohms>|INFinity|MINimum|MAXimum}
                AWG_33522A_USB.Write("OUTP2:LOAD INF");                 //Load {<ohms>|INFinity|MINimum|MAXimum}
                AWG_33522A_USB.Write("SOUR1:APPL:PULS " + dblFrequency_in_Hz.ToString() + " , 1.25 , 0.625");      //Frequency, Amplitude, DC Offset
                AWG_33522A_USB.Write("SOUR2:APPL:PULS " + dblFrequency_in_Hz.ToString() + " , 1.8 , 0.9");
                //AWG_33522A.Write("SOUR1:PHAS 20");                    //Set intial phase
                //AWG_33522A.Write("SOUR2:PHAS 20");
                AWG_33522A_USB.Write("SOUR1:FUNC:PULS:DCYC 26");                 //Set pulse duty cycle
                AWG_33522A_USB.Write("SOUR2:FUNC:PULS:DCYC 25");
                //AWG_33522A.Write("SOUR1:VOLT:LOW 0");                   //Set low woltage
                //AWG_33522A.Write("SOUR2:VOLT:LOW 0");
                //AWG_33522A.Write("SOUR1:VOLT:HIGH 1.25");               //Set high voltage
                //AWG_33522A.Write("SOUR2:VOLT:HIGH 1.8");                
                AWG_33522A_USB.Write("OUTP:SYNC ON");                    //Sync Out
                AWG_33522A_USB.Write("OUTP1 ON");                           //Source Out
                AWG_33522A_USB.Write("OUTP2 ON");
                AWG_33522A_USB.Write("PHAS:SYNC");                    //Sync PHASE

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            _Util.Wait(20);
        }

        public void SetHighVoltage(double dblHighVoltage_in_Volt, Arb_Channel Channel)
        {
            try
            {
                if (Channel == Arb_Channel.Channel_1)
                    AWG_33522A_USB.Write("SOUR1:VOLT:HIGH " + dblHighVoltage_in_Volt.ToString());
                else
                    AWG_33522A_USB.Write("SOUR2:VOLT:HIGH " + dblHighVoltage_in_Volt.ToString());

                AWG_33522A_USB.Write("PHAS:SYNC");                    //Sync PHASE
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void SetArbOut(string Arb_Waveform_Type, Arb_Channel Channel,
            double dblFreq_in_Hz, double dblHighVoltate_in_Volt, double dblLowVoltage_in_Volt, int intDutyCycle)
        {
            try
            {
                if (Channel == Arb_Channel.Channel_1)
                {
                    AWG_33522A_USB.Write("OUTP1:LOAD INF");
                    AWG_33522A_USB.Write("SOUR1:VOLT:UNIT VPP");              //Set Amplitude Unit
                    AWG_33522A_USB.Write("SOUR1:FUNC SQU");
                    AWG_33522A_USB.Write("SOUR1:FUNC:SQU:DCYC " + intDutyCycle);
                    AWG_33522A_USB.Write("SOUR1:FREQ " + dblFreq_in_Hz + "Hz");
                    AWG_33522A_USB.Write("SOUR1:VOLT:LOW " + dblLowVoltage_in_Volt);
                    AWG_33522A_USB.Write("SOUR1:VOLT:HIGH " + dblHighVoltate_in_Volt);
                    //AWG_33522A_USB.Write("SOUR1:VOLT " + dblVoltate_in_Volt);
                    //AWG_33522A_USB.Write("SOUR1:VOLT:OFFS " + dblDCOffset_in_Volt);
                }
                else if (Channel == Arb_Channel.Channel_2)
                {
                    AWG_33522A_USB.Write("OUTP2:LOAD INF");
                    AWG_33522A_USB.Write("SOUR2:VOLT:UNIT VPP");              //Set Amplitude Unit
                    AWG_33522A_USB.Write("SOUR2:FUNC SQU");
                    AWG_33522A_USB.Write("SOUR2:FUNC:SQU:DCYC " + intDutyCycle);
                    AWG_33522A_USB.Write("SOUR2:FREQ " + dblFreq_in_Hz + "Hz");
                    AWG_33522A_USB.Write("SOUR2:VOLT:LOW " + dblLowVoltage_in_Volt);
                    AWG_33522A_USB.Write("SOUR2:VOLT:HIGH " + dblHighVoltate_in_Volt);
                    //AWG_33522A_USB.Write("SOUR2:VOLT " + dblVoltate_in_Volt);
                    //AWG_33522A_USB.Write("SOUR2:VOLT:OFFS " + dblDCOffset_in_Volt);
                }

                AWG_33522A_USB.Write("PHAS:SYNC");                    //Sync PHASE
                SetOutput(Output.ON, Channel);

                #region ---# APPLy #----
                //StringBuilder sbCommand = new StringBuilder("SOUR");

                //if (Channel == Arb_Channel.Channel_1)
                //{
                //    sbCommand.Append("1:APPL:");
                //    AWG_33522A_USB.Write("SOUR1:FUNC:PULS:DCYC " + intDutyCycle.ToString());
                //}
                //else
                //{
                //    sbCommand.Append("2:APPL:");
                //    AWG_33522A_USB.Write("SOUR2:FUNC:PULS:DCYC " + intDutyCycle.ToString());
                //}

                //sbCommand.Append(Arb_Waveform_Type + " ");
                //sbCommand.Append(dblFreq_in_KHz.ToString() + " , ");
                //sbCommand.Append(dblVoltate_in_Volt.ToString() + " , ");
                //if (dblDCOffset_in_Volt != 0)
                //    sbCommand.Append(dblDCOffset_in_Volt.ToString());

                //AWG_33522A_USB.Write(sbCommand.ToString());
                #endregion

                //AWG_33522A_USB.Write("PHAS:SYNC");                    //Sync PHASE               

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void SetArbOut(string Arb_Waveform_Type, Arb_Channel Channel,
            double dblFreq_in_KHz, double dblHighVoltate_in_Volt, double dblLowVoltage_in_Volt, int intDutyCycle, bool SyncPhase)
        {
            try
            {
                if (Channel == Arb_Channel.Channel_1)
                {
                    AWG_33522A_USB.Write("OUTP1:LOAD INF");
                    AWG_33522A_USB.Write("SOUR1:VOLT:UNIT VPP");              //Set Amplitude Unit
                    AWG_33522A_USB.Write("SOUR1:FUNC SQU");
                    AWG_33522A_USB.Write("SOUR1:FUNC:SQU:DCYC " + intDutyCycle);
                    AWG_33522A_USB.Write("SOUR1:FREQ " + dblFreq_in_KHz + "Hz");
                    AWG_33522A_USB.Write("SOUR1:VOLT:LOW " + dblLowVoltage_in_Volt);
                    AWG_33522A_USB.Write("SOUR1:VOLT:HIGH " + dblHighVoltate_in_Volt);
                    //AWG_33522A_USB.Write("SOUR1:VOLT " + dblVoltate_in_Volt);
                    //AWG_33522A_USB.Write("SOUR1:VOLT:OFFS " + dblDCOffset_in_Volt);
                }
                else if (Channel == Arb_Channel.Channel_2)
                {
                    AWG_33522A_USB.Write("OUTP2:LOAD INF");
                    AWG_33522A_USB.Write("SOUR2:VOLT:UNIT VPP");              //Set Amplitude Unit
                    AWG_33522A_USB.Write("SOUR2:FUNC SQU");
                    AWG_33522A_USB.Write("SOUR2:FUNC:SQU:DCYC " + intDutyCycle);
                    AWG_33522A_USB.Write("SOUR2:FREQ " + dblFreq_in_KHz + "Hz");
                    AWG_33522A_USB.Write("SOUR2:VOLT:LOW " + dblLowVoltage_in_Volt);
                    AWG_33522A_USB.Write("SOUR2:VOLT:HIGH " + dblHighVoltate_in_Volt);
                    //AWG_33522A_USB.Write("SOUR2:VOLT " + dblVoltate_in_Volt);
                    //AWG_33522A_USB.Write("SOUR2:VOLT:OFFS " + dblDCOffset_in_Volt);
                }

                if (SyncPhase)
                {
                    AWG_33522A_USB.Write("PHAS:SYNC");                    //Sync PHASE
                }
                SetOutput(Output.ON, Channel);

                #region ---# APPLy #----
                //StringBuilder sbCommand = new StringBuilder("SOUR");

                //if (Channel == Arb_Channel.Channel_1)
                //{
                //    sbCommand.Append("1:APPL:");
                //    AWG_33522A_USB.Write("SOUR1:FUNC:PULS:DCYC " + intDutyCycle.ToString());
                //}
                //else
                //{
                //    sbCommand.Append("2:APPL:");
                //    AWG_33522A_USB.Write("SOUR2:FUNC:PULS:DCYC " + intDutyCycle.ToString());
                //}

                //sbCommand.Append(Arb_Waveform_Type + " ");
                //sbCommand.Append(dblFreq_in_KHz.ToString() + " , ");
                //sbCommand.Append(dblVoltate_in_Volt.ToString() + " , ");
                //if (dblDCOffset_in_Volt != 0)
                //    sbCommand.Append(dblDCOffset_in_Volt.ToString());

                //AWG_33522A_USB.Write(sbCommand.ToString());
                #endregion

                //AWG_33522A_USB.Write("PHAS:SYNC");                    //Sync PHASE               

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void SetBurstTrig(Arb_Channel Channel, double dblTrigDelay_in_ms, int intCycle)
        {
            try
            {
                if (Channel == Arb_Channel.Channel_1)
                {
                    AWG_33522A_USB.Write("SOUR1:BURS:MODE TRIG");
                    if (intCycle == 0)
                    { AWG_33522A_USB.Write("SOUR1:BURS:NCYC INF"); }
                    else
                    {
                        AWG_33522A_USB.Write("SOUR1:BURS:NCYC " + intCycle.ToString());
                    }
                    AWG_33522A_USB.Write("SOUR1:BURS:PHAS 0");
                    AWG_33522A_USB.Write("TRIG1:SOUR EXT");
                    AWG_33522A_USB.Write("TRIG1:SLOP POS");
                    AWG_33522A_USB.Write("TRIG1:DEL " + dblTrigDelay_in_ms.ToString() + "ms");
                    AWG_33522A_USB.Write("SOUR1:BURS:STAT ON");
                }
                else if (Channel == Arb_Channel.Channel_2)
                {
                    AWG_33522A_USB.Write("SOUR2:BURS:MODE TRIG");
                    AWG_33522A_USB.Write("SOUR2:BURS:NCYC " + intCycle.ToString());
                    AWG_33522A_USB.Write("SOUR2:BURS:PHAS 0");
                    AWG_33522A_USB.Write("TRIG2:SOUR EXT");
                    AWG_33522A_USB.Write("TRIG2:SLOP POS");
                    AWG_33522A_USB.Write("TRIG2:DEL " + dblTrigDelay_in_ms.ToString() + "ms");
                    AWG_33522A_USB.Write("SOUR2:BURS:STAT ON");
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void SetDC(Arb_Channel Channel, double dblVoltage_in_Volts)
        {
            try
            {
                if (Channel == Arb_Channel.Channel_1)
                {
                    AWG_33522A_USB.Write("SOUR1:APPL:DC DEF,DEF," + dblVoltage_in_Volts.ToString());
                }
                else if (Channel == Arb_Channel.Channel_2)
                {
                    AWG_33522A_USB.Write("SOUR2:APPL:DC DEF,DEF," + dblVoltage_in_Volts.ToString());
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void SYNC_OUT(Arb_Channel Channel, Output Status)
        {
            try
            {
                if (Channel == Arb_Channel.Channel_1)
                    AWG_33522A_USB.Write("OUTP:SYNC:SOUR CH1");
                else if (Channel == Arb_Channel.Channel_2)
                    AWG_33522A_USB.Write("OUTP:SYNC:SOUR CH2");

                if (Status == Output.ON)
                    AWG_33522A_USB.Write("OUTP:SYNC ON");
                else if (Status == Output.OFF)
                    AWG_33522A_USB.Write("OUTP:SYNC OFF");
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void SetOutput(Output Status, Arb_Channel Channel)
        {
            if (Channel == Arb_Channel.Channel_1)
            {
                if (Status == Output.ON)
                {
                    AWG_33522A_USB.Write("OUTP1 ON");
                }
                else
                    AWG_33522A_USB.Write("OUTP1 OFF");
            }
            else
            {
                if (Status == Output.ON)
                {
                    AWG_33522A_USB.Write("OUTP2 ON");
                }
                else
                    AWG_33522A_USB.Write("OUTP2 OFF");
            }
        }

        public void Dispose()
        {
            try
            {
                AWG_33522A_USB.Write("*CLS");
                AWG_33522A_USB.Write("*RST");
                _Util.Wait(50);
                AWG_33522A_USB.Dispose();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }

    #endregion 33522A

    #endregion Instruments class

    #region *** Static parameter define ***

    public class Mod_Waveform_Name
    {
        public const string EDGE = "EDGE_TS0_BASIC";
        public const string EDGE_CONTINOUS = "EDGE_CONTINUOUS_BASIC";
        public const string WCDMA = "WCDMA_UL_REL7";
        public const string TDSCDMA = "TDSCDMA_UL_TS0_BASIC";
        public const string TDHSDPA = "TD-SCDMA_HSDPA_TS4_DL";
        public const string LTETDD = "LTETD_R9_10MHz_12RB_UL";
        public const string LTETDD_FULL = "LTETD_R9_10MHz_UL";
        public const string LTEFDD = "LTE_R9_10MHz_12RB_UL";
        public const string LTEFDD_FULL = "LTE_R9_10MHz_UL";
        public const string CDMA_ACP = "CDMA2000_ACP_Reverse";
        public const string CDMA_EVM = "CDMA2000_EVM_Reverse";
        public const string EVDO_ACP = "EVDO_ACP_Reverse";
        public const string EVDO_EVM = "EVDO_EVM_Reverse";
    }

    public class Arb_Waveform_Type
    {
        public const string Sinusoid = "SIN";
        public const string Square = "SQU";
        public const string Ramp = "RAMP";
        public const string Pulse = "PULS";
        public const string Noise = "NOIS";
        public const string DC = "DC";
        public const string User = "USR";

    }

    public class Instruments_address
    {
        public const int _00 = 0;
        public const int _01 = 1;
        public const int _02 = 2;
        public const int _03 = 3;
        public const int _04 = 4;
        public const int _05 = 5;
        public const int _06 = 6;
        public const int _07 = 7;
        public const int _08 = 8;
        public const int _09 = 9;
        public const int _10 = 10;
        public const int _11 = 11;
        public const int _12 = 12;
        public const int _13 = 13;
        public const int _14 = 14;
        public const int _15 = 15;
        public const int _16 = 16;
        public const int _17 = 17;
        public const int _18 = 18;
        public const int _19 = 19;
        public const int _20 = 20;
        public const int _21 = 21;
        public const int _22 = 22;
        public const int _23 = 23;
        public const int _24 = 24;
        public const int _25 = 25;
        public const int _26 = 26;
        public const int _27 = 27;
        public const int _28 = 28;
        public const int _29 = 29;
        public const int _30 = 30;

        public const int GpibBoardNumber = 0;

        public const int PS_66332A = 5;
        public const int PS_E3631A = 6;

        public const int SigGen_8665B = 11;
        public const int SigGen_E4438C_BJ = 12;
        public const int SigGen_E4438C = 19;
        public const int SigGen_N5182A = 14;

        public const int Arb_33120A = 16;
        public const int Arb_DG1022 = 17;
        public const int Arb_33220A = 1;
        public const int Arb_33522A = 10;

        public const int MXA_N9020A = 18;
        public const int MXA_N9020A_BJ = 18;
        public const int PowerMeter_437B = 23;
        public const int PowerMeter_N1913A = 9;
    }

    public class Instruments_VISA
    {
        public const string PowerMeter_U2001A = "USB0::0x0957::0x2B18::MY51290009::INSTR";
        public const string Arb_DG1022 = "USB0::0x1AB1::0x0588::DG1D124705228::INSTR";
        public const string Arb_33522A = "ARB_33522A";
        public const string Arb_33522A_BJ1 = "USB0::0x0957::0x2307::MY50002524::INSTR";
        public const string Arb_33522A_SH2 = "USB0::0x0957::0x2307::MY50004982::INSTR";
    }

    public class Power_Mod_CW_Delta
    {
        public const double EDGE = 0.5;
        public const double WCDMA = 0.5;
        public const double TDSCDMA = 0.5;
    }

    #endregion Static parameter define

    #region *** eNum ***

    public enum Power_Range
    {
        Lower = 1,
        Upper
    }

    public enum Arb_Channel
    {
        Channel_1 = 1,
        Channel_2
    }

    public enum Triger_Source
    {
        Ext = 1,
        Bus = 2,
        Manual = 3,
        Int = 4
    }

    public enum Triger_Type
    {
        Continous_Free = 1,
        Continous_Trig = 2,
        Continous_Reset
    }
    public enum PS_66319B_Channel
    {
        Channel_1 = 1,
        Channel_2
    }

    public enum E3631A_Channel
    {
        P6V = 1,
        P25V,
        N25V
    }

    public enum U2001_ZeroType
    {
        Internal = 1,
        External
    }

    public enum U2001_Trigger
    {
        FreeRun = 1,
        Internal,
        External
    }

    public enum U2001_TraceData_Resolution
    {
        Low_Resolution = 1,
        Medium_Resolution,
        High_Resolution
    }

    public enum Modulation
    {
        EDGE = 1,       //1
        WCDMA,          //2
        TDSCDMA,        //3
        LTETDD,         //4
        LTEFDD,         //5
        CDMA,           //6
        EVDO,           //7
        EDGE_CONTINOUS  //8
    }

    public enum Output
    {
        ON = 1,       //1
        OFF,          //2
    }

    public enum GPCTRL_ARB_TYPE
    {
        GPIB = 1,       //1
        USB = 2,        //2
        MANUAL,         //3
    }

    public enum POWER_SUPPLY_TYPE
    {
        _66319B = 1,   //1
        _66332A        //2
    }

    public enum SIGGEN_MAX_FREQ
    {
        _6G = 1,   //1
        _3G        //2
    }
    
    #endregion *** eNum ***
}


