using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

// based on https://gist.github.com/ahawker/9715872

namespace BatteryInfo
{
    [Flags]
    public enum PowerStates
    {
        PowerOnline = 0x00000001,
        Discharging = 0x00000002,
        Charging = 0x00000004,
        Critical = 0x00000008
    }

    public class BatteryInformation
    {
        public BatteryInformation(int index,
                                  bool isSystemPowerBattery,
                                  string deviceName,
                                  string manufactureName,
                                  DateTime manufactureDate,
                                  string chemistry,
                                  int designedMaxCapacity,
                                  int fullChargeCapacity,
                                  uint currentCapacity,
                                  uint voltage,
                                  TimeSpan estimatedTime,
                                  int dischargeRate,
                                  int cycleCount,
                                  int defaultAlert1,
                                  int defaultAlert2,
                                  int criticalBias,
                                  PowerStates powerState,
                                  double temperature)
        {
            Index = index;
            IsSystemPowerBattery = isSystemPowerBattery;
            DeviceName = deviceName;
            ManufactureName = manufactureName;
            ManufactureDate = manufactureDate;
            Chemistry = chemistry;
            DesignedMaxCapacity = designedMaxCapacity;
            FullChargeCapacity = fullChargeCapacity;
            CurrentCapacity = currentCapacity;
            Voltage = voltage;
            EstimatedTime = estimatedTime;
            DischargeRate = dischargeRate;
            CycleCount = cycleCount;
            DefaultAlert1 = defaultAlert1;
            DefaultAlert2 = defaultAlert2;
            CriticalBias = criticalBias;
            PowerState = powerState;
            Temperature = temperature;
        }

        public int Index { get; }
        public bool IsSystemPowerBattery { get; }
        public string DeviceName { get; }
        public string ManufactureName { get; }
        public DateTime ManufactureDate { get; }
        public uint CurrentCapacity { get; }
        public int DesignedMaxCapacity { get; }
        public int FullChargeCapacity { get; }
        public uint Voltage { get; }
        public TimeSpan EstimatedTime { get; }
        public int DischargeRate { get; }
        public int CycleCount { get; }
        public int DefaultAlert1 { get; }
        public int DefaultAlert2 { get; }
        public int CriticalBias { get; }
        public PowerStates PowerState { get; }
        public double Temperature { get; }
        public string Chemistry { get; }
    }

    public static class BatteryInfo
    {
        public static List<BatteryInformation> GetBatteryData()
        {
            var batteries = new List<BatteryInformation>();

            IntPtr deviceHandle = SetupDiGetClassDevs(NativeMethods.GUID_DEVCLASS_BATTERY,
                                                      NativeMethods.DEVICE_GET_CLASS_FLAGS.DIGCF_PRESENT | NativeMethods.DEVICE_GET_CLASS_FLAGS.DIGCF_DEVICEINTERFACE);

            int batteryIndex = 0;

            for (; ; )
            {
                var batteryInfo = GetOneBatteryData(deviceHandle, batteryIndex);

                if (batteryInfo == null)
                {
                    break;
                }

                if (batteryInfo.IsSystemPowerBattery)
                {
                    batteries.Add(batteryInfo);
                }

                batteryIndex++;
            }

            NativeMethods.SetupDiDestroyDeviceInfoList(deviceHandle);

            return batteries;
        }

        private static BatteryInformation GetOneBatteryData(IntPtr deviceHandle, int batteryIndex)
        {
            IntPtr batteryHandle = IntPtr.Zero;
            bool isSystemPowerBatterie = false;

            var deviceInterfaceData = new NativeMethods.SP_DEVICE_INTERFACE_DATA();
            deviceInterfaceData.CbSize = Marshal.SizeOf(deviceInterfaceData);

            if (!SetupDiEnumDeviceInterfaces(deviceHandle, NativeMethods.GUID_DEVCLASS_BATTERY, batteryIndex, ref deviceInterfaceData))
            {
                return null;
            }

            var deviceDetailData = new NativeMethods.SP_DEVICE_INTERFACE_DETAIL_DATA();
            deviceDetailData.CbSize = (IntPtr.Size == 8) ? 8 : 4 + Marshal.SystemDefaultCharSize;
            SetupDiGetDeviceInterfaceDetail(deviceHandle, ref deviceInterfaceData, ref deviceDetailData, NativeMethods.DEVICE_INTERFACE_BUFFER_SIZE);
            batteryHandle = CreateFile(deviceDetailData.DevicePath, FileAccess.ReadWrite, FileShare.ReadWrite, FileMode.Open, NativeMethods.FILE_ATTRIBUTES.Normal);

            // get the battery tag
            uint batteryTag = batteryTag = GetBatteryTag(batteryHandle);

            var batteryInformation = GetBatteryInformation(batteryHandle, batteryTag);

            // detect none-UPS: http://msdn.microsoft.com/en-us/library/windows/desktop/bb204769%28v=vs.85%29.aspx

            if ((batteryInformation.Capabilities & NativeMethods.BATTERY_CAPABILITIES.BATTERY_SYSTEM_BATTERY) != 0
                && (batteryInformation.Capabilities & NativeMethods.BATTERY_CAPABILITIES.BATTERY_IS_SHORT_TERM) == 0)
            {
                isSystemPowerBatterie = true;
            }

            var deviceName = GetBatteryDeviceName(batteryHandle, batteryTag);
            var manufactureName = GetBatteryManufactureName(batteryHandle, batteryTag);
            var manufactureDate = GetBatteryManufactureDate(batteryHandle, batteryTag);
            var estimatedTime = GetBatteryEstimatedTime(batteryHandle, batteryTag);
            var temperature = GetBatteryTemperature(batteryHandle, batteryTag);
            var batterieStatus = GetBatteryStatus(batteryHandle, batteryTag);

            NativeMethods.CloseHandle(batteryHandle);

            return new BatteryInformation(
                batteryIndex,
                isSystemPowerBatterie,
                deviceName,
                manufactureName,
                manufactureDate,
                ConvertToString(batteryInformation.Chemistry),
                batteryInformation.DesignedCapacity,
                batteryInformation.FullChargedCapacity,
                batterieStatus.Capacity,
                batterieStatus.Voltage,
                estimatedTime,
                batterieStatus.Rate,
                batteryInformation.CycleCount,
                batteryInformation.DefaultAlert1,
                batteryInformation.DefaultAlert2,
                batteryInformation.CriticalBias,
                (PowerStates)batterieStatus.PowerState,
                temperature);
        }

        private static string ConvertToString(byte[] bytes)
        {
            string str = string.Empty;

            foreach (byte b in bytes)
            {
                if (b == 0)
                {
                    break;
                }

                str += (char)b;
            }

            return str;
        }

        private static uint GetBatteryTag(IntPtr batteryHandle)
        {
            // https://learn.microsoft.com/de-de/windows/win32/power/battery-information

            uint batteryTag = 0;
            DeviceIoControl(batteryHandle, NativeMethods.IOCTL_BATTERY_QUERY_TAG, ref batteryTag);
            return batteryTag;
        }

        private static string GetBatteryManufactureName(IntPtr batteryHandle, uint batteryTag)
        {
            // https://learn.microsoft.com/de-de/windows/win32/power/ioctl-battery-query-information

            var queryInformation = new NativeMethods.BATTERY_QUERY_INFORMATION();
            queryInformation.BatteryTag = batteryTag;
            queryInformation.InformationLevel = NativeMethods.BATTERY_QUERY_INFORMATION_LEVEL.BatteryManufactureName;

            int queryInfoSize = Marshal.SizeOf(queryInformation);
            IntPtr queryInfoPointer = Marshal.AllocHGlobal(queryInfoSize);
            Marshal.StructureToPtr(queryInformation, queryInfoPointer, false);

            int nameSize = 500;
            IntPtr namePointer = Marshal.AllocHGlobal(nameSize);

            DeviceIoControl(batteryHandle, NativeMethods.IOCTL_BATTERY_QUERY_INFORMATION, queryInfoPointer, queryInfoSize, namePointer, nameSize);

            string name = Marshal.PtrToStringUni(namePointer);

            Marshal.FreeHGlobal(queryInfoPointer);
            Marshal.FreeHGlobal(namePointer);

            return name;
        }

        private static string GetBatteryDeviceName(IntPtr batteryHandle, uint batteryTag)
        {
            var queryInformation = new NativeMethods.BATTERY_QUERY_INFORMATION();
            queryInformation.BatteryTag = batteryTag;
            queryInformation.InformationLevel = NativeMethods.BATTERY_QUERY_INFORMATION_LEVEL.BatteryDeviceName;

            int queryInfoSize = Marshal.SizeOf(queryInformation);
            IntPtr queryInfoPointer = Marshal.AllocHGlobal(queryInfoSize);
            Marshal.StructureToPtr(queryInformation, queryInfoPointer, false);

            int nameSize = 500;
            IntPtr namePointer = Marshal.AllocHGlobal(nameSize);

            DeviceIoControl(batteryHandle, NativeMethods.IOCTL_BATTERY_QUERY_INFORMATION, queryInfoPointer, queryInfoSize, namePointer, nameSize);

            string name = Marshal.PtrToStringUni(namePointer);

            Marshal.FreeHGlobal(queryInfoPointer);
            Marshal.FreeHGlobal(namePointer);

            return name;
        }

        private static TimeSpan GetBatteryEstimatedTime(IntPtr batteryHandle, uint batteryTag)
        {
            var queryInformation = new NativeMethods.BATTERY_QUERY_INFORMATION();
            queryInformation.BatteryTag = batteryTag;
            queryInformation.InformationLevel = NativeMethods.BATTERY_QUERY_INFORMATION_LEVEL.BatteryEstimatedTime;
            queryInformation.AtRate = 0;

            int queryInfoSize = Marshal.SizeOf(queryInformation);
            IntPtr queryInfoPointer = Marshal.AllocHGlobal(queryInfoSize);
            Marshal.StructureToPtr(queryInformation, queryInfoPointer, false);

            var timeSize = Marshal.SizeOf<uint>();
            IntPtr timePointer = Marshal.AllocHGlobal(timeSize);

            DeviceIoControl(batteryHandle, NativeMethods.IOCTL_BATTERY_QUERY_INFORMATION, queryInfoPointer, queryInfoSize, timePointer, timeSize);

            var seconds = Marshal.PtrToStructure<uint>(timePointer);

            Marshal.FreeHGlobal(queryInfoPointer);
            Marshal.FreeHGlobal(timePointer);

            TimeSpan estimatedTime;

            if (seconds == NativeMethods.BATTERY_UNKNOWN_TIME)
            {
                estimatedTime = TimeSpan.Zero;
            }
            else
            {
                estimatedTime = TimeSpan.FromSeconds(seconds);
            }

            return estimatedTime;
        }

        private static double GetBatteryTemperature(IntPtr batteryHandle, uint batteryTag)
        {
            var queryInformation = new NativeMethods.BATTERY_QUERY_INFORMATION();
            queryInformation.BatteryTag = batteryTag;
            queryInformation.InformationLevel = NativeMethods.BATTERY_QUERY_INFORMATION_LEVEL.BatteryTemperature;
            queryInformation.AtRate = 0;

            int queryInfoSize = Marshal.SizeOf(queryInformation);
            IntPtr queryInfoPointer = Marshal.AllocHGlobal(queryInfoSize);
            Marshal.StructureToPtr(queryInformation, queryInfoPointer, false);

            var temperatureSize = Marshal.SizeOf<uint>();
            IntPtr temperaturePointer = Marshal.AllocHGlobal(temperatureSize);

            if (!DeviceIoControl(batteryHandle, NativeMethods.IOCTL_BATTERY_QUERY_INFORMATION, queryInfoPointer, queryInfoSize, temperaturePointer, temperatureSize))
            {
                return double.NaN;
            }

            uint temperature = Marshal.PtrToStructure<uint>(temperaturePointer);

            Marshal.FreeHGlobal(queryInfoPointer);
            Marshal.FreeHGlobal(temperaturePointer);

            return temperature / 10.0;
        }

        private static DateTime GetBatteryManufactureDate(IntPtr batteryHandle, uint batteryTag)
        {
            var queryInformation = new NativeMethods.BATTERY_QUERY_INFORMATION();
            queryInformation.BatteryTag = batteryTag;
            queryInformation.InformationLevel = NativeMethods.BATTERY_QUERY_INFORMATION_LEVEL.BatteryManufactureDate;
            queryInformation.AtRate = 0;

            int queryInfoSize = Marshal.SizeOf(queryInformation);
            IntPtr queryInfoPointer = Marshal.AllocHGlobal(queryInfoSize);
            Marshal.StructureToPtr(queryInformation, queryInfoPointer, false);

            var dateSize = 500; //Marshal.SizeOf<Win32.BATTERY_MANUFACTURE_DATE>();
            IntPtr datePointer = Marshal.AllocHGlobal(dateSize);

            if (!DeviceIoControl(batteryHandle, NativeMethods.IOCTL_BATTERY_QUERY_INFORMATION, queryInfoPointer, queryInfoSize, datePointer, dateSize))
            {
                return DateTime.MinValue;
            }

            var date = Marshal.PtrToStructure<NativeMethods.BATTERY_MANUFACTURE_DATE>(datePointer);

            Marshal.FreeHGlobal(queryInfoPointer);
            Marshal.FreeHGlobal(datePointer);

            return new DateTime(date.Year, date.Month, date.Day);
        }

        private static NativeMethods.BATTERY_INFORMATION GetBatteryInformation(IntPtr batteryHandle, uint batteryTag)
        {
            var queryInformation = new NativeMethods.BATTERY_QUERY_INFORMATION();
            queryInformation.BatteryTag = batteryTag;
            queryInformation.InformationLevel = NativeMethods.BATTERY_QUERY_INFORMATION_LEVEL.BatteryInformation;

            int queryInfoSize = Marshal.SizeOf(queryInformation);
            IntPtr queryInfoPointer = Marshal.AllocHGlobal(queryInfoSize);
            Marshal.StructureToPtr(queryInformation, queryInfoPointer, false);

            int batteryInfoSize = Marshal.SizeOf<NativeMethods.BATTERY_INFORMATION>();
            IntPtr batteryInfoPointer = Marshal.AllocHGlobal(batteryInfoSize);

            DeviceIoControl(batteryHandle, NativeMethods.IOCTL_BATTERY_QUERY_INFORMATION, queryInfoPointer, queryInfoSize, batteryInfoPointer, batteryInfoSize);

            var batteryInformation = Marshal.PtrToStructure<NativeMethods.BATTERY_INFORMATION>(batteryInfoPointer);

            Marshal.FreeHGlobal(queryInfoPointer);
            Marshal.FreeHGlobal(batteryInfoPointer);

            return batteryInformation;
        }

        private static NativeMethods.BATTERY_STATUS GetBatteryStatus(IntPtr batteryHandle, uint batteryTag)
        {
            var batteryWaitStatus = new NativeMethods.BATTERY_WAIT_STATUS();
            batteryWaitStatus.BatteryTag = batteryTag;

            int waitStatusSize = Marshal.SizeOf(batteryWaitStatus);
            IntPtr batteryWaitStatusPointer = Marshal.AllocHGlobal(waitStatusSize);
            Marshal.StructureToPtr(batteryWaitStatus, batteryWaitStatusPointer, false);

            int batteryStatusSize = Marshal.SizeOf<NativeMethods.BATTERY_STATUS>();
            IntPtr batteryStatusPointer = Marshal.AllocHGlobal(batteryStatusSize);

            DeviceIoControl(batteryHandle, NativeMethods.IOCTL_BATTERY_QUERY_STATUS, batteryWaitStatusPointer, waitStatusSize, batteryStatusPointer, batteryStatusSize);

            var batteryStatus = Marshal.PtrToStructure<NativeMethods.BATTERY_STATUS>(batteryStatusPointer);

            Marshal.FreeHGlobal(batteryStatusPointer);
            Marshal.FreeHGlobal(batteryWaitStatusPointer);

            return batteryStatus;
        }

        private static bool DeviceIoControl(IntPtr deviceHandle, uint controlCode, ref uint output)
        {
            uint bytesReturned;
            uint junkInput = 0;
            bool retval = NativeMethods.DeviceIoControl(deviceHandle, controlCode, ref junkInput, 0, ref output, (uint)Marshal.SizeOf(output), out bytesReturned, IntPtr.Zero);

            if (!retval)
            {
                int errorCode = Marshal.GetLastWin32Error();

                if (errorCode != 0)
                    throw Marshal.GetExceptionForHR(errorCode);
                else
                    throw new Exception("DeviceIoControl call failed but Win32 didn't catch an error.");
            }

            return retval;
        }

        private static bool DeviceIoControl(IntPtr deviceHandle, uint controlCode, IntPtr input, int inputSize, IntPtr output, int outputSize)
        {
            uint bytesReturned;
            bool retval = NativeMethods.DeviceIoControl(deviceHandle, controlCode, input, (uint)inputSize, output, (uint)outputSize, out bytesReturned, IntPtr.Zero);

            if (!retval)
            {
                int errorCode = Marshal.GetLastWin32Error();

                if (errorCode == NativeMethods.ERROR_INVALID_FUNCTION)
                {
                    return false;
                }

                if (errorCode != 0)
                    throw Marshal.GetExceptionForHR(errorCode);
                else
                    throw new Exception("DeviceIoControl call failed but Win32 didn't catch an error.");
            }

            return retval;
        }

        private static IntPtr SetupDiGetClassDevs(Guid guid, NativeMethods.DEVICE_GET_CLASS_FLAGS flags)
        {
            IntPtr handle = NativeMethods.SetupDiGetClassDevs(ref guid, null, IntPtr.Zero, flags);

            if (handle == IntPtr.Zero || handle.ToInt64() == -1)
            {
                int errorCode = Marshal.GetLastWin32Error();

                if (errorCode != 0)
                    throw Marshal.GetExceptionForHR(errorCode);
                else
                    throw new Exception("SetupDiGetClassDev call returned a bad handle.");
            }

            return handle;
        }

        private static bool SetupDiEnumDeviceInterfaces(IntPtr deviceInfoSet,
                                                        Guid guid,
                                                        int memberIndex,
                                                        ref NativeMethods.SP_DEVICE_INTERFACE_DATA deviceInterfaceData)
        {
            bool retval = NativeMethods.SetupDiEnumDeviceInterfaces(deviceInfoSet, IntPtr.Zero, ref guid, (uint)memberIndex, ref deviceInterfaceData);

            if (!retval)
            {
                int errorCode = Marshal.GetLastWin32Error();

                if (errorCode != 0)
                {
                    if (errorCode == NativeMethods.ERROR_NO_MORE_ITEMS)
                    {
                        return false;
                    }

                    throw Marshal.GetExceptionForHR(errorCode);
                }
                else
                    throw new Exception("SetupDeviceInfoEnumerateDeviceInterfaces call failed but Win32 didn't catch an error.");
            }

            return retval;
        }

        private static bool SetupDiDestroyDeviceInfoList(IntPtr deviceInfoSet)
        {
            bool retval = NativeMethods.SetupDiDestroyDeviceInfoList(deviceInfoSet);

            if (!retval)
            {
                int errorCode = Marshal.GetLastWin32Error();

                if (errorCode != 0)
                    throw Marshal.GetExceptionForHR(errorCode);
                else
                    throw new Exception("SetupDiDestroyDeviceInfoList call failed but Win32 didn't catch an error.");
            }

            return retval;
        }

        private static bool SetupDiGetDeviceInterfaceDetail(IntPtr deviceInfoSet,
                                                            ref NativeMethods.SP_DEVICE_INTERFACE_DATA deviceInterfaceData,
                                                            ref NativeMethods.SP_DEVICE_INTERFACE_DETAIL_DATA deviceInterfaceDetailData,
                                                            int deviceInterfaceDetailSize)
        {
            uint reqSize;
            bool retval = NativeMethods.SetupDiGetDeviceInterfaceDetail(deviceInfoSet,
                                                                ref deviceInterfaceData,
                                                                ref deviceInterfaceDetailData,
                                                                (uint)deviceInterfaceDetailSize,
                                                                out reqSize,
                                                                IntPtr.Zero);

            if (!retval)
            {
                int errorCode = Marshal.GetLastWin32Error();

                if (errorCode != 0)
                    throw Marshal.GetExceptionForHR(errorCode);
                else
                    throw new Exception("SetupDiGetDeviceInterfaceDetail call failed but Win32 didn't catch an error.");
            }

            return retval;
        }

        private static IntPtr CreateFile(string filename,
                                         FileAccess access,
                                         FileShare shareMode,
                                         FileMode creation,
                                         NativeMethods.FILE_ATTRIBUTES flags)
        {
            IntPtr handle = NativeMethods.CreateFile(filename, access, shareMode, IntPtr.Zero, creation, flags, IntPtr.Zero);

            if (handle == IntPtr.Zero || handle.ToInt64() == -1)
            {
                int errorCode = Marshal.GetLastWin32Error();

                if (errorCode != 0)
                    Marshal.ThrowExceptionForHR(errorCode);
                else
                    throw new Exception("SetupDiGetDeviceInterfaceDetail call failed but Win32 didn't catch an error.");
            }

            return handle;
        }
    }
}