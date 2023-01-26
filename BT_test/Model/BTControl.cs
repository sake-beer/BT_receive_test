using Microsoft.Win32;
using System;
using System.Management;
using System.Text;
using System.Text.RegularExpressions;

namespace BT_test.Model
{
    public class BTControl
    {
        // Property
        public string Device { get; private set; } = "";
        public string Com { get; private set; } = "";

        private TopLog _log;

        public BTControl(TopLog log)
        {
            _log = log;
        }

        public void Init()
        {
            // COM番号を抜き出すための準備
            Regex regexPortName = new Regex(@"(COM\d+)");
            ManagementObjectSearcher searchSerial = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity");  // デバイスマネージャーから情報を取得するためのオブジェクト

            // デバイスマネージャーの情報を列挙する
            foreach (ManagementObject obj in searchSerial.Get())
            {
                string name = obj["Name"] as string; // デバイスマネージャーに表示されている機器名
                string classGuid = obj["ClassGuid"] as string; // GUID
                string devicePass = obj["DeviceID"] as string; // デバイスインスタンスパス

                if (classGuid != null && devicePass != null)
                {
                    // デバイスインスタンスパスからBluetooth接続機器のみを抽出
                    // {4d36e978-e325-11ce-bfc1-08002be10318}はBluetooth接続機器を示す固定値
                    if (String.Equals(classGuid, "{4d36e978-e325-11ce-bfc1-08002be10318}",
              StringComparison.InvariantCulture))
                    {
                        // デバイスインスタンスパスからデバイスIDを2段階で抜き出す
                        string[] tokens = devicePass.Split('&');
                        string[] addressToken = tokens[4].Split('_');

                        string bluetoothAddress = addressToken[0];
                        Match m = regexPortName.Match(name);
                        string comPortNumber = "";
                        if (m.Success)
                        {
                            // COM番号を抜き出す
                            comPortNumber = m.Groups[1].ToString();
                            Com = comPortNumber;
                        }

                        if (Convert.ToUInt64(bluetoothAddress, 16) > 0)
                        {
                            string bluetoothName = GetBluetoothRegistryName(bluetoothAddress);
                            Device = bluetoothName;
                        }
                        // bluetoothNameが接続機器名
                        // comPortNumberが接続機器名のCOM番号
                    }
                }
            }
            _log.Log = "Device = " + Device;
            _log.Log = "COM port = " + Com;
        }


        /// <summary>機器名称取得</summary> 
        /// <param name="address">[in] アドレス</param> 
        /// <returns>[out] 機器名称</returns> 
        private string GetBluetoothRegistryName(string address)
        {
            string deviceName = "";
            // 以下のレジストリパスはどのPCでも共通
            string registryPath = @"SYSTEM\CurrentControlSet\Services\BTHPORT\Parameters\Devices";
            string devicePath = String.Format(@"{0}\{1}", registryPath, address);

            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(devicePath))
            {
                if (key != null)
                {
                    Object o = key.GetValue("Name");

                    byte[] raw = o as byte[];

                    if (raw != null)
                    {
                        // ASCII変換
                        deviceName = Encoding.ASCII.GetString(raw);
                    }
                }
            }
            // NULL文字をトリミングしてリターン
            return deviceName.TrimEnd('\0');
        }

    }
}
