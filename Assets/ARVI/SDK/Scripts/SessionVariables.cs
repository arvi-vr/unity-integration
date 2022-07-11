namespace ARVI.SDK
{
    using System;
    using System.Text;

    public static class SessionVariables
    {
        public static bool InitializeVariables()
        {
            return API.SessionVariables_Initialize();
        }

        public static void FinalizeVariables()
        {
            API.SessionVariables_Finalize();
        }
    }

    public class SessionVariable
    {
        private readonly byte[] data;

        public SessionVariable(byte[] data)
        {
            this.data = data;
        }

        public byte[] GetBytes()
        {
            return data;
        }

        public bool GetBoolean()
        {
            return BitConverter.ToBoolean(data, 0);
        }

        public char GetChar()
        {
            return BitConverter.ToChar(data, 0);
        }

        public double GetDouble()
        {
            return BitConverter.ToDouble(data, 0);
        }

        public float GetFloat()
        {
            return BitConverter.ToSingle(data, 0);
        }

        public short GetInt16()
        {
            return BitConverter.ToInt16(data, 0);
        }

        public int GetInt32()
        {
            return BitConverter.ToInt32(data, 0);
        }

        public long GetInt64()
        {
            return BitConverter.ToInt64(data, 0);
        }

        public ushort GetUInt16()
        {
            return BitConverter.ToUInt16(data, 0);
        }

        public uint GetUInt32()
        {
            return BitConverter.ToUInt32(data, 0);
        }

        public ulong GetUInt64()
        {
            return BitConverter.ToUInt64(data, 0);
        }

        public string GetString()
        {
            return Encoding.UTF8.GetString(data);
        }
    }
}