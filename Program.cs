using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using HidApi;

namespace SC2SmoothBrainifier
{
    // ==========================================================
    // 1. libVIIPER NATIVE INTEROP DEFINITIONS
    // ==========================================================
    enum VIIPERLogLevel { Debug = -4, Info = 0, Warn = 4, Error = 8 }

    [StructLayout(LayoutKind.Sequential)]
    struct USBServerConfig
    {
        [MarshalAs(UnmanagedType.LPStr)] public string? addr;
        public ulong connection_timeout_ms;
        public ulong device_handler_connect_timeout_ms;
        public uint write_batch_flush_interval_ms;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct Xbox360DeviceState
    {
        public uint Buttons;
        public byte LT;
        public byte RT;
        public short LX;
        public short LY;
        public short RX;
        public short RY;
        public byte Reserved0, Reserved1, Reserved2, Reserved3, Reserved4, Reserved5;
    }
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    delegate void Xbox360RumbleCallbackDelegate(nuint handle, byte leftMotor, byte rightMotor);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    delegate void VIIPERLogCallbackDelegate(VIIPERLogLevel level, [MarshalAs(UnmanagedType.LPStr)] string message);

    static class LibVIIPER
    {
        const string Lib = "libVIIPER";

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool NewUSBServer([In] ref USBServerConfig config, out nuint outHandle, VIIPERLogCallbackDelegate? logCallback); [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool CloseUSBServer(nuint handle); [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool CreateUSBBus(nuint handle, ref uint busID);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool CreateXbox360Device(nuint serverHandle, out nuint outDeviceHandle, uint busID, [MarshalAs(UnmanagedType.I1)] bool autoAttachLocalhost, ushort idVendor, ushort idProduct, byte xinputSubType);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool SetXbox360DeviceState(nuint deviceHandle, Xbox360DeviceState state); [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool SetXbox360RumbleCallback(nuint deviceHandle, Xbox360RumbleCallbackDelegate? callback);
    }

    // ==========================================================
    // 2. MAIN APPLICATION CONTEXT
    // ==========================================================
    static class Program
    {
        [STAThread]
        static void Main()
        {
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);

            // Runs hidden in the system tray
            System.Windows.Forms.Application.Run(new TrayApplicationContext());
        }
    }

    public class TrayApplicationContext : ApplicationContext
    {
        private readonly NotifyIcon _trayIcon;
        private readonly CancellationTokenSource _cts;
        private readonly Task _backgroundTask;

        // XInput Button bitmasks
        private const uint XINPUT_DPAD_UP = 0x0001;
        private const uint XINPUT_DPAD_DOWN = 0x0002;
        private const uint XINPUT_DPAD_LEFT = 0x0004;
        private const uint XINPUT_DPAD_RIGHT = 0x0008;
        private const uint XINPUT_START = 0x0010;
        private const uint XINPUT_BACK = 0x0020;
        private const uint XINPUT_LEFT_THUMB = 0x0040;
        private const uint XINPUT_RIGHT_THUMB = 0x0080;
        private const uint XINPUT_LEFT_SHOULDER = 0x0100;
        private const uint XINPUT_RIGHT_SHOULDER = 0x0200;
        private const uint XINPUT_GUIDE = 0x0400;
        private const uint XINPUT_A = 0x1000;
        private const uint XINPUT_B = 0x2000;
        private const uint XINPUT_X = 0x4000;
        private const uint XINPUT_Y = 0x8000;

        // Valve Vendor ID & Triton Puck PID
        private const ushort VALVE_VID = 0x28DE;
        private const ushort PUCK_PID = 0x1304; // Update post launch

        private readonly VIIPERLogCallbackDelegate _logCb;
        private readonly Xbox360RumbleCallbackDelegate _rumbleCb;

        public TrayApplicationContext()
        {
            _logCb = LogCallback;
            _rumbleCb = RumbleCallback;

            // Configure the System Tray Icon
            _trayIcon = new NotifyIcon
            {
                Icon = SystemIcons.Application,
                Text = "SC2SmoothBrainifier",
                Visible = true,
                ContextMenuStrip = new ContextMenuStrip()
            };
            _trayIcon.ContextMenuStrip.Items.Add("Exit", null, OnExit);

            _cts = new CancellationTokenSource();
            _backgroundTask = Task.Run(() => RunControllerLoop(_cts.Token));
        }

        private void OnExit(object? sender, EventArgs e)
        {
            _trayIcon.Visible = false;
            _cts.Cancel();

            Task.WaitAny(_backgroundTask, Task.Delay(1500));
            System.Windows.Forms.Application.Exit();
        }

        private void LogCallback(VIIPERLogLevel level, string message)
        {
            Debug.WriteLine($"libVIIPER [{level}]: {message}");
        }

        private void RumbleCallback(nuint handle, byte leftMotor, byte rightMotor)
        {
            // Optional: Implement Steam Controller 2 rumble parsing here in the future
        }

        private async Task RunControllerLoop(CancellationToken token)
        {
            Debug.WriteLine("Warming up the iron...");
            nuint serverHandle = 0;

            try
            {
                USBServerConfig conf = new() { addr = "localhost:3245" };

                if (!LibVIIPER.NewUSBServer(ref conf, out serverHandle, _logCb))
                {
                    Debug.WriteLine("Fatal Error: Failed to start native libVIIPER server.");
                    return;
                }

                uint busID = 0;
                if (!LibVIIPER.CreateUSBBus(serverHandle, ref busID))
                {
                    Debug.WriteLine("Fatal Error: Failed to create USB bus.");
                    return;
                }

                if (!LibVIIPER.CreateXbox360Device(serverHandle, out nuint deviceHandle, busID, true, 0, 0, 0))
                {
                    Debug.WriteLine("Fatal Error: Failed to create Xbox 360 virtual device.");
                    return;
                }

                LibVIIPER.SetXbox360RumbleCallback(deviceHandle, _rumbleCb);
                Debug.WriteLine("Embedded virtual controller connected successfully.");

                Hid.Init();

                try
                {
                    while (!token.IsCancellationRequested)
                    {
                        Debug.WriteLine("Waiting for Steam Controller 2 Puck...");

                        Device? puckDevice = null;
                        while (puckDevice == null && !token.IsCancellationRequested)
                        {
                            // Pass NO arguments to Enumerate. Get absolutely everything on the PC.
                            foreach (var devInfo in Hid.Enumerate())
                            {
                                // Manually filter for Valve's Vendor ID
                                if (devInfo.VendorId == VALVE_VID)
                                {
                                    // Print out every single Valve endpoint it finds
                                    Debug.WriteLine($"Found Valve Device -> PID: 0x{devInfo.ProductId:X4}, Path: {devInfo.Path}, UsagePage: 0x{devInfo.UsagePage:X4}, Usage: 0x{devInfo.Usage:X2}");

                                    // If it's our Triton Puck
                                    if (devInfo.ProductId == PUCK_PID)
                                    {
                                        // Skip the protected Keyboard (0x06) and Mouse (0x02) endpoints
                                        if (devInfo.UsagePage == 0x0001 && (devInfo.Usage == 0x0002 || devInfo.Usage == 0x0006))
                                        {
                                            continue;
                                        }

                                        try
                                        {
                                            var tempDevice = new Device(devInfo.Path);

                                            try
                                            {
                                                // The 10ms dummy read test to make sure Windows doesn't block it
                                                tempDevice.ReadTimeout(new byte[64], 10);

                                                puckDevice = tempDevice;
                                                Debug.WriteLine($"SUCCESS: Hooked unblocked data interface!");
                                                break;
                                            }
                                            catch
                                            {
                                                // Windows blocked this interface. Dispose and try the next one.
                                                tempDevice.Dispose();
                                            }
                                        }
                                        catch
                                        {
                                            // Failed to open handle. Ignore.
                                        }
                                    }
                                }
                            }

                            if (puckDevice == null)
                            {
                                await Task.Delay(1000, token);
                            }
                        }

                        if (token.IsCancellationRequested || puckDevice == null)
                            break;

                        // Initial Watchdog feed (not quiet, so we see it connect)
                        DisableLizardMode(puckDevice, quiet: false);

                        byte[] buffer = new byte[64];
                        Stopwatch lizardWatchdog = Stopwatch.StartNew();

                        try
                        {
                            while (!token.IsCancellationRequested)
                            {
                                // Feed the watchdog every 2 seconds
                                if (lizardWatchdog.ElapsedMilliseconds > 2000)
                                {
                                    DisableLizardMode(puckDevice, quiet: true);
                                    lizardWatchdog.Restart();
                                }

                                int bytesRead = puckDevice.ReadTimeout(buffer, 250);

                                if (bytesRead < 0)
                                {
                                    Debug.WriteLine("Puck disconnected.");
                                    break;
                                }

                                if (bytesRead > 0)
                                {
                                    // WE ARE BACK TO 0x42!
                                    if (buffer[0] == 0x42)
                                    {
                                        uint buttons = BitConverter.ToUInt32(buffer, 2);
                                        uint xboxButtons = 0;

                                        if ((buttons & 0x00000001) != 0) xboxButtons |= XINPUT_A;
                                        if ((buttons & 0x00000002) != 0) xboxButtons |= XINPUT_B;
                                        if ((buttons & 0x00000004) != 0) xboxButtons |= XINPUT_X;
                                        if ((buttons & 0x00000008) != 0) xboxButtons |= XINPUT_Y;

                                        if ((buttons & 0x00000200) != 0) xboxButtons |= XINPUT_RIGHT_SHOULDER;
                                        if ((buttons & 0x00080000) != 0) xboxButtons |= XINPUT_LEFT_SHOULDER;
                                        if ((buttons & 0x00000020) != 0) xboxButtons |= XINPUT_RIGHT_THUMB;
                                        if ((buttons & 0x00008000) != 0) xboxButtons |= XINPUT_LEFT_THUMB;

                                        if ((buttons & 0x00000400) != 0) xboxButtons |= XINPUT_DPAD_DOWN;
                                        if ((buttons & 0x00000800) != 0) xboxButtons |= XINPUT_DPAD_RIGHT;
                                        if ((buttons & 0x00001000) != 0) xboxButtons |= XINPUT_DPAD_LEFT;
                                        if ((buttons & 0x00002000) != 0) xboxButtons |= XINPUT_DPAD_UP;

                                        if ((buttons & 0x00000040) != 0) xboxButtons |= XINPUT_START;
                                        if ((buttons & 0x00004000) != 0) xboxButtons |= XINPUT_BACK;
                                        if ((buttons & 0x00010000) != 0) xboxButtons |= XINPUT_GUIDE;

                                        short rawL2 = BitConverter.ToInt16(buffer, 6);
                                        short rawR2 = BitConverter.ToInt16(buffer, 8);

                                        var state = new Xbox360DeviceState
                                        {
                                            Buttons = xboxButtons,
                                            LT = (byte)Math.Clamp(rawL2 / 128, 0, 255),
                                            RT = (byte)Math.Clamp(rawR2 / 128, 0, 255),
                                            LX = BitConverter.ToInt16(buffer, 10),
                                            LY = (short)BitConverter.ToInt16(buffer, 12),
                                            RX = BitConverter.ToInt16(buffer, 14),
                                            RY = (short)BitConverter.ToInt16(buffer, 16)
                                        };

                                        LibVIIPER.SetXbox360DeviceState(deviceHandle, state);
                                    }
                                    // Ignore the heartbeat (0x7B) and the new sensor packet (0x43)
                                    else if (buffer[0] == 0x7B || buffer[0] == 0x43)
                                    {
                                        // Silently ignore
                                    }
                                    else
                                    {
                                        Debug.WriteLine($"Unknown packet received! Report ID: 0x{buffer[0]:X2}");
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"Connection lost: {ex.Message}");
                        }
                        finally
                        {
                            puckDevice.Dispose();
                        }
                    }
                }
                finally
                {
                    Debug.WriteLine("Shutting down HID API...");
                    Hid.Exit();
                }
            }
            catch (TaskCanceledException)
            {
                Debug.WriteLine("Graceful shutdown requested.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Fatal background error: {ex.Message}");
            }
            finally
            {
                if (serverHandle != 0)
                {
                    Debug.WriteLine("Tearing down native libVIIPER server...");
                    LibVIIPER.CloseUSBServer(serverHandle);
                }
            }
        }

        private static void DisableLizardMode(Device puckDevice, bool quiet = false)
        {
            if (!quiet) Debug.WriteLine("Ironing (Disabling Lizard Mode)...");

            byte[] featureReport = new byte[65];

            // 1. Valve Feature Report ID
            featureReport[0] = 0x01;

            // 2. ID_CLEAR_DIGITAL_MAPPINGS (The modern Valve Lizard-Mode Kill-Switch!)
            featureReport[1] = 0x81;

            // 3. Payload Length is 0 because this command takes no additional arguments
            featureReport[2] = 0x00;

            try
            {
                puckDevice.SendFeatureReport(featureReport);
                if (!quiet) Debug.WriteLine("Brain smoothing complete.");
            }
            catch (Exception ex)
            {
                if (!quiet) Debug.WriteLine($"Warning: brain still wrinkly. {ex.Message}");
            }
        }
    }
}
