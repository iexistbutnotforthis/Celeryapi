using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using System.Threading;
using System.Xml.Linq;


namespace CeleryAPI
{
    public class ExploitApi
    {
        private static string _name;
        private static string _key;
        public static string Name => _name;
        public static string Key => _key;
        public ExploitApi(string name, string key)
        {
            _name = name;
            _key = key;
        }
        public interface ITextContainer
        {
            string Text { get; set; }
        }

        public static string ApiVersion = "V.3.0";
        public static async Task Execute(string source, bool useCustomUnc)
        {
            await BaseFunctions.Execute(source, _name, _key, useCustomUnc);
        }
        public static async Task Inject()
        {
            await BaseFunctions.Inject(_name, _key);
        }
        public static async Task CloseRoblox()
        {
            await BaseFunctions.CloseRoblox(_name, _key);
        }
        public static async Task AutoExecute(string folderPath, int timeout, bool autoExecuteWithCustomUnc)
        {
            await BaseFunctions.AutoExecute(_name, _key, folderPath, timeout, autoExecuteWithCustomUnc);
        }
        public static async Task ChangeExecuterIdentityName(string newname)
        {
            await BaseFunctions.ChangeExecuterIdentityName(newname, _name, _key);
        }
        public static async Task<bool> CheckInjectionStatus()
        {
            return await BaseFunctions.CheckInjectionStats(_name, _key);
        }
        /*public static async Task AutoInject()
        {
            await BaseFunctions.AutoInject(_name, _key);
        }*/
    }


    public class BaseFunctions
    {
        private static string _firebaseDatabaseUrl = "";
        internal static bool incognitoEnabled;
        internal static bool test = false;
        public class RichTextBoxWrapper : ExploitApi.ITextContainer
        {
            private readonly RichTextBox _richTextBox;

            public RichTextBoxWrapper(RichTextBox richTextBox)
            {
                _richTextBox = richTextBox;
            }

            public string Text
            {
                get => _richTextBox.Text;
                set => _richTextBox.Text = value;
            }
        }
        private static async Task<string> GetCustomUncScript()
        {
            string url = "https://raw.githubusercontent.com/iexistbutnotforthis/Celeryapi/main/unc.lua";
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    throw new Exception($"Failed to retrieve Lua script, status code: {response.StatusCode}");
                }
            }
        }
        private static async Task<bool> VerifyKeyAsync(string customKeyName, string providedKey)
        {
	    return true;
        }
        internal static async Task Execute(string source, string name, string key, bool useCustomUnc)
        {
            if (!await VerifyKeyAsync(name, key))
            {
                MessageBox.Show("Sorry! either name or key is incorrect.");
                return;
            }
            string celeryfolder = string.Concat(Path.GetTempPath() + "celery");

            if (Directory.Exists(celeryfolder))
            {
                Console.WriteLine("Celery folder exists");
            }
            else
            {
                Console.WriteLine("Folder doesnt exist! Creating..");
                Directory.CreateDirectory(celeryfolder);

                string filePath = Path.Combine(celeryfolder, "myfile.txt");
                File.Create(filePath).Dispose();
            }
            if (test == true)
            {
                if (useCustomUnc == true)
                {
                    string custom = File.ReadAllText(string.Concat(Path.GetTempPath() + "celery", "\\moreUNC.txt"));
                    string final = custom + source;
                    File.WriteAllText(string.Concat(Path.GetTempPath() + "celery", "\\myfile.txt"), final);
                }
                else
                {
                    File.WriteAllText(string.Concat(Path.GetTempPath() + "celery", "\\myfile.txt"), source);
                }
            }
            else
            {
                MessageBox.Show("Please inject first!");
            }

        }
        internal static async Task Inject(string name, string key)
        {
            if (!await VerifyKeyAsync(name, key))
            {
                MessageBox.Show("Sorry! either name or key is incorrect.");
                return;
            }

            string celeryfolder = string.Concat(Path.GetTempPath() + "celery");

            if (Directory.Exists(celeryfolder))
            {
                Console.WriteLine("Celery folder exists");
            }
            else
            {
                Console.WriteLine("Folder doesnt exist! Creating..");
                Directory.CreateDirectory(celeryfolder);

                string filePath = Path.Combine(celeryfolder, "myfile.txt");
                File.Create(filePath).Dispose();
            }

            try
            {
                bool flag = false;
                if (!(!incognitoEnabled && flag))
                {
                    foreach (Util.ProcInfo item in Util.openProcessesByName("RobloxPlayerBeta.exe"))
                    {
                        if (!WindowsPlayer.isInjected())
                        {
                            switch (WindowsPlayer.injectPlayer(item))
                            {
                                case InjectionStatus.SUCCESS:
                                    flag = true;
                                    Console.WriteLine("API INJECTED");
                                    Thread.Sleep(5000);
                                    test = true;
                                    break;
                                case InjectionStatus.ALREADY_INJECTING:
                                    Thread.Sleep(250);
                                    break;
                                case InjectionStatus.FAILED:
                                    MessageBox.Show("Injection failed! Unknown error.");
                                    break;
                                case InjectionStatus.FAILED_ADMINISTRATOR_ACCESS:
                                    MessageBox.Show("Please run CeleryLauncher.exe as an administrator");
                                    break;
                            }
                            if (!incognitoEnabled && flag)
                            {
                                break;
                            }
                        }
                    }
                }
                if (!flag)
                {
                    MessageBox.Show("Please use Roblox web client");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fatal error: " + ex.ToString());
            }
        }
        internal static async Task CloseRoblox(string name, string key)
        {
            if (!await VerifyKeyAsync(name, key))
            {
                MessageBox.Show("Sorry! either name or key is incorrect.");

                return;
            }

            Process[] processes = Process.GetProcessesByName("RobloxPlayerBeta");

            foreach (Process process in processes)
            {
                try
                {
                    process.Kill();
                    process.WaitForExit();
                    MessageBox.Show("Closed Roblox!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while trying to close Roblox: {ex.Message}");
                }
            }

            if (processes.Length == 0)
            {
                MessageBox.Show("Roblox isnt running?");
            }
        }
        internal static async Task AutoExecute(string name, string key, string folderPath, int timeout, bool autoExecuteWithCustomUnc)
        {
            if (!await VerifyKeyAsync(name, key))
            {
                MessageBox.Show("Sorry! either name or key is incorrect.");

                return;
            }
            Process[] processes = Process.GetProcessesByName("RobloxPlayerBeta");

            foreach (Process process in processes)
            {
                try
                {
                    if (!Directory.Exists(folderPath))
                    {
                        Console.WriteLine("The specified folder does not exist.");
                        return;
                    }
                    Console.WriteLine("Folder exists");
                    string[] txtFiles = Directory.GetFiles(folderPath, "*.txt", SearchOption.AllDirectories);
                    string[] luaFiles = Directory.GetFiles(folderPath, "*.lua", SearchOption.AllDirectories);
                    StringBuilder Conclude = new StringBuilder();
                    foreach (string file in txtFiles)
                    {
                        string compress = File.ReadAllText(file);
                        Conclude.AppendLine(compress);
                    }

                    Console.WriteLine("Lua Files (.lua):");
                    foreach (string file in luaFiles)
                    {
                        string compress = File.ReadAllText(file);
                        Conclude.AppendLine(compress);
                    }

                    string final = Conclude.ToString();
                    System.Threading.Thread.Sleep(timeout);
                    while (true)
                    {
                        if (test == true)
                        {
                            if (autoExecuteWithCustomUnc == true)
                            {
                                Thread.Sleep(timeout);
                                Execute(final, name, key, true);
                                break;
                            }
                            else
                            {
                                Thread.Sleep(timeout);
                                Execute(final, name, key, false);
                                break;
                            }
                        }
                        else
                        {
                            //waiting
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while trying to auto-execute: {ex.Message}");
                }
            }
        }
        internal static async Task ChangeExecuterIdentityName(string executerNewName, string name, string key)
        {
            if (!await VerifyKeyAsync(name, key))
            {
                MessageBox.Show("Sorry! either name or key is incorrect.");

                return;
            }

            string uncScript = await GetCustomUncScript();
            bool contains = uncScript.Contains("MoreUNC");

            if (contains)
            {
                Console.WriteLine("Found identify executer name.");
                string result = uncScript.Replace("MoreUNC", executerNewName);
                bool doubleCheck = result.Contains(executerNewName);
                if (doubleCheck)
                {
                    Console.WriteLine("Swapped UNC name successfully!");
                    File.WriteAllText(string.Concat(Path.GetTempPath() + "celery", "\\moreUNC.txt"), result);
                }
                else
                {
                    Console.WriteLine("Fuck sake");
                }
            }
            else
            {
                Console.WriteLine("The string does not contain the search string.");
            }

        }
        internal static async Task<bool> CheckInjectionStats(string name, string key)
        {
            if (!await VerifyKeyAsync(name, key))
            {
                MessageBox.Show("Sorry! Either name or key is incorrect.");
                return false;
            }

            if (!test)
            {
                Console.WriteLine("Not injected!");
                return false;
            }
            else
            {
                Console.WriteLine("Injected!");
                return true;
            }
        }
        /*
        internal static async Task AutoInject(string name, string key)
        {
            if (!await VerifyKeyAsync(name, key))
            {
                MessageBox.Show("Sorry! either name or key is incorrect.");
                return;
            }


            while (true)
            {

                Process[] processes = Process.GetProcessesByName("RobloxPlayerBeta");
                foreach (Process process in processes)
                {
                    try
                    {
                        Inject(name, key);
                        break;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"An error occurred while trying to inject into Roblox: {ex.Message}");
                        break;
                    }
                }

                if (processes.Length == 0)
                {
                    //
                }
            }
        }
        */


    }
    internal class Imports
    {
        public struct MEMORY_BASIC_INFORMATION
        {
            public int BaseAddress;

            public int AllocationBase;

            public uint AllocationProtect;

            public int RegionSize;

            public uint State;

            public uint Protect;

            public uint Type;
        }

        public static class ConsoleHelper
        {
            public static StreamWriter writer;

            public static FileStream fwriter;

            public static void Initialize(bool alwaysCreateNewConsole = true)
            {
                bool flag = true;
                if (alwaysCreateNewConsole || (AttachConsole(uint.MaxValue) == 0 && (long)Marshal.GetLastWin32Error() != 5))
                {
                    flag = AllocConsole() != 0;
                }
                if (flag)
                {
                    InitializeOutStream();
                    InitializeInStream();
                }
                Console.OutputEncoding = Encoding.UTF8;
            }

            public static void Clear()
            {
                Console.Write("\n\n");
            }

            private static void InitializeOutStream()
            {
                fwriter = CreateFileStream("CONOUT$", 1073741824u, 2u, FileAccess.Write);
                if (fwriter != null)
                {
                    writer = new StreamWriter(fwriter)
                    {
                        AutoFlush = true
                    };
                    Console.SetOut(writer);
                    Console.SetError(writer);
                }
            }

            private static void InitializeInStream()
            {
                FileStream fileStream = CreateFileStream("CONIN$", 2147483648u, 1u, FileAccess.Read);
                if (fileStream != null)
                {
                    Console.SetIn(new StreamReader(fileStream));
                }
            }

            private static FileStream CreateFileStream(string name, uint win32DesiredAccess, uint win32ShareMode, FileAccess dotNetFileAccess)
            {
                SafeFileHandle safeFileHandle = new SafeFileHandle((IntPtr)CreateFileW(name, win32DesiredAccess, win32ShareMode, 0u, 3u, 128u, 0u), ownsHandle: true);
                if (!safeFileHandle.IsInvalid)
                {
                    return new FileStream(safeFileHandle, dotNetFileAccess);
                }
                return null;
            }
        }

        public const uint PAGE_NOACCESS = 1u;

        public const uint PAGE_READONLY = 2u;

        public const uint PAGE_READWRITE = 4u;

        public const uint PAGE_WRITECOPY = 8u;

        public const uint PAGE_EXECUTE = 16u;

        public const uint PAGE_EXECUTE_READ = 32u;

        public const uint PAGE_EXECUTE_READWRITE = 64u;

        public const uint PAGE_EXECUTE_WRITECOPY = 128u;

        public const uint PAGE_GUARD = 256u;

        public const uint PAGE_NOCACHE = 512u;

        public const uint PAGE_WRITECOMBINE = 1024u;

        public const uint MEM_COMMIT = 4096u;

        public const uint MEM_RESERVE = 8192u;

        public const uint MEM_DECOMMIT = 16384u;

        public const uint MEM_RELEASE = 32768u;

        public const uint PROCESS_WM_READ = 16u;

        public const uint PROCESS_ALL_ACCESS = 2035711u;

        private const uint GENERIC_WRITE = 1073741824u;

        private const uint GENERIC_READ = 2147483648u;

        private const uint FILE_SHARE_READ = 1u;

        private const uint FILE_SHARE_WRITE = 2u;

        private const uint OPEN_EXISTING = 3u;

        private const uint FILE_ATTRIBUTE_NORMAL = 128u;

        private const uint ERROR_ACCESS_DENIED = 5u;

        private const uint ATTACH_PARENT = uint.MaxValue;

        public const int EXCEPTION_CONTINUE_EXECUTION = -1;

        public const int EXCEPTION_CONTINUE_SEARCH = 0;

        public const uint STD_OUTPUT_HANDLE = 4294967285u;

        public const int MY_CODE_PAGE = 437;

        public const int SW_HIDE = 0;

        public const int SW_SHOW = 5;

        [DllImport("user32.dll")]
        public static extern int FindWindow(string sClass, string sWindow);

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(int hWnd, int nCmdShow);

        [DllImport("user32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern int MessageBoxA(int hWnd, string sMessage, string sCaption, uint mbType);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int MessageBoxW(int hWnd, string sMessage, string sCaption, uint mbType);

        [DllImport("kernel32.dll")]
        public static extern int GetConsoleWindow();

        [DllImport("kernel32.dll")]
        public static extern ulong OpenProcess(uint dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(ulong hProcess, ulong lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);

        [DllImport("kernel32.dll")]
        public static extern bool WriteProcessMemory(ulong hProcess, ulong lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesWritten);

        [DllImport("kernel32.dll")]
        public static extern bool VirtualProtectEx(ulong hProcess, ulong lpBaseAddress, int dwSize, uint new_protect, ref uint lpOldProtect);

        [DllImport("kernel32.dll")]
        public static extern ulong VirtualQueryEx(ulong hProcess, ulong lpAddress, out MEMORY_BASIC_INFORMATION lpBuffer, uint dwLength);

        [DllImport("kernel32.dll")]
        public static extern ulong VirtualAllocEx(ulong hProcess, ulong lpAddress, int size, uint allocation_type, uint protect);

        [DllImport("kernel32.dll")]
        public static extern ulong VirtualFreeEx(ulong hProcess, ulong lpAddress, int size, uint allocation_type);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern ulong GetModuleHandle(string lpModuleName);

        [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        public static extern ulong GetProcAddress(ulong hModule, string procName);

        [DllImport("kernel32.dll")]
        public static extern uint GetLastError();

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CloseHandle(ulong hObject);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetExitCodeProcess(ulong hProcess, out uint lpExitCode);

        [DllImport("kernel32.dll")]
        public static extern int CreateRemoteThread(ulong hProcess, int lpThreadAttributes, uint dwStackSize, int lpStartAddress, int lpParameter, uint dwCreationFlags, out int lpThreadId);

        [DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern uint GetStdHandle(uint nStdHandle);

        [DllImport("kernel32.dll")]
        public static extern void SetStdHandle(uint nStdHandle, uint handle);

        [DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int AllocConsole();

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern bool SetConsoleTitle(string lpConsoleTitle);

        [DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern uint AttachConsole(uint dwProcessId);

        [DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern uint CreateFileW(string lpFileName, uint dwDesiredAccess, uint dwShareMode, uint lpSecurityAttributes, uint dwCreationDisposition, uint dwFlagsAndAttributes, uint hTemplateFile);

        [DllImport("kernel32.dll")]
        public static extern uint GetCurrentProcessId();

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool FreeConsole();

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern uint CreateFile(string lpFileName, uint dwDesiredAccess, uint dwShareMode, uint lpSecurityAttributes, uint dwCreationDisposition, uint dwFlagsAndAttributes, uint hTemplateFile);
    }
    internal class Util
    {
        public class ProcInfo
        {
            public Process processRef;

            public ulong processId;

            public string processName;

            public string windowName;

            public ulong handle;

            public ulong baseModule;

            private int nothing;

            public ProcInfo()
            {
                processRef = null;
                processId = 0uL;
                handle = 0uL;
            }

            public bool isOpen()
            {
                try
                {
                    if (processRef == null)
                    {
                        return false;
                    }
                    if (processRef.HasExited)
                    {
                        return false;
                    }
                    if (processRef.Id == 0)
                    {
                        return false;
                    }
                    if (processRef.Handle == IntPtr.Zero)
                    {
                        return false;
                    }
                }
                catch (InvalidOperationException)
                {
                    return false;
                }
                catch (Exception)
                {
                    return false;
                }
                if (processId != 0L)
                {
                    return handle != 0;
                }
                return false;
            }

            public Imports.MEMORY_BASIC_INFORMATION getPage(ulong address)
            {
                Imports.MEMORY_BASIC_INFORMATION lpBuffer = default(Imports.MEMORY_BASIC_INFORMATION);
                Imports.VirtualQueryEx(handle, address, out lpBuffer, 28u);
                return lpBuffer;
            }

            public bool isAccessible(ulong address)
            {
                Imports.MEMORY_BASIC_INFORMATION page = getPage(address);
                uint protect = page.Protect;
                if (page.State == 4096)
                {
                    if (protect != 4 && protect != 2 && protect != 64)
                    {
                        return protect == 32;
                    }
                    return true;
                }
                return false;
            }

            public uint setPageProtect(ulong address, int size, uint protect)
            {
                uint lpOldProtect = 0u;
                Imports.VirtualProtectEx(handle, address, size, protect, ref lpOldProtect);
                return lpOldProtect;
            }

            public bool writeByte(ulong address, byte value)
            {
                byte[] array = new byte[1] { value };
                return Imports.WriteProcessMemory(handle, address, array, array.Length, ref nothing);
            }

            public bool writeBytes(ulong address, byte[] bytes, int count = -1)
            {
                return Imports.WriteProcessMemory(handle, address, bytes, (count == -1) ? bytes.Length : count, ref nothing);
            }

            public bool writeString(ulong address, string str, int count = -1)
            {
                char[] array = str.ToCharArray(0, str.Length);
                List<byte> list = new List<byte>();
                char[] array2 = array;
                for (int i = 0; i < array2.Length; i++)
                {
                    byte item = (byte)array2[i];
                    list.Add(item);
                }
                return Imports.WriteProcessMemory(handle, address, list.ToArray(), (count == -1) ? list.Count : count, ref nothing);
            }

            public bool writeWString(ulong address, string str, int count = -1)
            {
                ulong num = address;
                char[] array = str.ToCharArray(0, str.Length);
                foreach (char value in array)
                {
                    writeUInt16(num, Convert.ToUInt16(value));
                    num += 2;
                }
                return true;
            }

            public bool writeInt16(ulong address, short value)
            {
                return Imports.WriteProcessMemory(handle, address, BitConverter.GetBytes(value), 2, ref nothing);
            }

            public bool writeUInt16(ulong address, ushort value)
            {
                return Imports.WriteProcessMemory(handle, address, BitConverter.GetBytes(value), 2, ref nothing);
            }

            public bool writeInt32(ulong address, int value)
            {
                return Imports.WriteProcessMemory(handle, address, BitConverter.GetBytes(value), 4, ref nothing);
            }

            public bool writeUInt32(ulong address, uint value)
            {
                return Imports.WriteProcessMemory(handle, address, BitConverter.GetBytes(value), 4, ref nothing);
            }

            public bool writeFloat(ulong address, float value)
            {
                return Imports.WriteProcessMemory(handle, address, BitConverter.GetBytes(value), 4, ref nothing);
            }

            public bool writeDouble(ulong address, double value)
            {
                return Imports.WriteProcessMemory(handle, address, BitConverter.GetBytes(value), 8, ref nothing);
            }

            public bool writeInt64(ulong address, long value)
            {
                return Imports.WriteProcessMemory(handle, address, BitConverter.GetBytes(value), 8, ref nothing);
            }

            public bool writeUInt64(ulong address, ulong value)
            {
                return Imports.WriteProcessMemory(handle, address, BitConverter.GetBytes(value), 8, ref nothing);
            }

            public byte readByte(ulong address)
            {
                byte[] array = new byte[1];
                Imports.ReadProcessMemory(handle, address, array, 1, ref nothing);
                return array[0];
            }

            public byte[] readBytes(ulong address, int count)
            {
                byte[] array = new byte[count];
                Imports.ReadProcessMemory(handle, address, array, count, ref nothing);
                return array;
            }

            public string readString(ulong address, int count = -1)
            {
                string text = "";
                ulong num = address;
                if (count == -1)
                {
                    for (; num != 512; num += 512)
                    {
                        byte[] array = readBytes(num, 512);
                        byte b;
                        string text2;
                        char c;
                        for (int i = 0; i < array.Length; text2 = text, c = (char)b, text = text2 + c, i++)
                        {
                            b = array[i];
                            switch (b)
                            {
                                case 9:
                                case 10:
                                case 13:
                                case 32:
                                case 33:
                                case 34:
                                case 35:
                                case 36:
                                case 37:
                                case 38:
                                case 39:
                                case 40:
                                case 41:
                                case 42:
                                case 43:
                                case 44:
                                case 45:
                                case 46:
                                case 47:
                                case 48:
                                case 49:
                                case 50:
                                case 51:
                                case 52:
                                case 53:
                                case 54:
                                case 55:
                                case 56:
                                case 57:
                                case 58:
                                case 59:
                                case 60:
                                case 61:
                                case 62:
                                case 63:
                                case 64:
                                case 65:
                                case 66:
                                case 67:
                                case 68:
                                case 69:
                                case 70:
                                case 71:
                                case 72:
                                case 73:
                                case 74:
                                case 75:
                                case 76:
                                case 77:
                                case 78:
                                case 79:
                                case 80:
                                case 81:
                                case 82:
                                case 83:
                                case 84:
                                case 85:
                                case 86:
                                case 87:
                                case 88:
                                case 89:
                                case 90:
                                case 91:
                                case 92:
                                case 93:
                                case 94:
                                case 95:
                                case 96:
                                case 97:
                                case 98:
                                case 99:
                                case 100:
                                case 101:
                                case 102:
                                case 103:
                                case 104:
                                case 105:
                                case 106:
                                case 107:
                                case 108:
                                case 109:
                                case 110:
                                case 111:
                                case 112:
                                case 113:
                                case 114:
                                case 115:
                                case 116:
                                case 117:
                                case 118:
                                case 119:
                                case 120:
                                case 121:
                                case 122:
                                case 123:
                                case 124:
                                case 125:
                                case 126:
                                case 127:
                                    continue;
                            }
                            num = 0uL;
                            break;
                        }
                    }
                }
                else
                {
                    byte[] array = readBytes(num, count);
                    foreach (byte b2 in array)
                    {
                        string text3 = text;
                        char c = (char)b2;
                        text = text3 + c;
                    }
                }
                return text;
            }

            public string readWString(ulong address, int count = -1)
            {
                string text = "";
                ulong num = address;
                if (count == -1)
                {
                    for (; num != 512; num += 512)
                    {
                        byte[] array = readBytes(num, 512);
                        for (int i = 0; i < array.Length; i += 2)
                        {
                            if (array[i] == 0 && array[i + 1] == 0)
                            {
                                num = 0uL;
                                break;
                            }
                            text += Encoding.Unicode.GetString(new byte[2]
                            {
                            array[i],
                            array[i + 1]
                            }, 0, 2);
                        }
                    }
                }
                else
                {
                    byte[] array2 = readBytes(num, count * 2);
                    for (int j = 0; j < array2.Length; j += 2)
                    {
                        text += Encoding.Unicode.GetString(new byte[2]
                        {
                        array2[j],
                        array2[j + 1]
                        }, 0, 2);
                    }
                }
                return text;
            }

            public short readInt16(ulong address)
            {
                byte[] array = new byte[2];
                Imports.ReadProcessMemory(handle, address, array, 2, ref nothing);
                return BitConverter.ToInt16(array, 0);
            }

            public ushort readUInt16(ulong address)
            {
                byte[] array = new byte[2];
                Imports.ReadProcessMemory(handle, address, array, 2, ref nothing);
                return BitConverter.ToUInt16(array, 0);
            }

            public int readInt32(ulong address)
            {
                byte[] array = new byte[4];
                Imports.ReadProcessMemory(handle, address, array, 4, ref nothing);
                return BitConverter.ToInt32(array, 0);
            }

            public uint readUInt32(ulong address)
            {
                byte[] array = new byte[4];
                Imports.ReadProcessMemory(handle, address, array, 4, ref nothing);
                return BitConverter.ToUInt32(array, 0);
            }

            public float readFloat(ulong address)
            {
                byte[] array = new byte[4];
                Imports.ReadProcessMemory(handle, address, array, 4, ref nothing);
                return BitConverter.ToSingle(array, 0);
            }

            public double readDouble(ulong address)
            {
                byte[] array = new byte[8];
                Imports.ReadProcessMemory(handle, address, array, 8, ref nothing);
                return BitConverter.ToDouble(array, 0);
            }

            public long readInt64(ulong address)
            {
                byte[] array = new byte[8];
                Imports.ReadProcessMemory(handle, address, array, 8, ref nothing);
                return BitConverter.ToInt64(array, 0);
            }

            public ulong readUInt64(ulong address)
            {
                byte[] array = new byte[8];
                Imports.ReadProcessMemory(handle, address, array, 8, ref nothing);
                return BitConverter.ToUInt64(array, 0);
            }

            public bool isPrologue(ulong address)
            {
                byte[] array = readBytes(address, 3);
                if (array[0] == 139 && array[1] == byte.MaxValue && array[2] == 85)
                {
                    return true;
                }
                if (address % 16 != 0L)
                {
                    return false;
                }
                if ((array[0] == 82 && array[1] == 139 && array[2] == 212) || (array[0] == 83 && array[1] == 139 && array[2] == 220) || (array[0] == 85 && array[1] == 139 && array[2] == 236) || (array[0] == 86 && array[1] == 139 && array[2] == 244) || (array[0] == 87 && array[1] == 139 && array[2] == byte.MaxValue))
                {
                    return true;
                }
                return false;
            }

            public bool isEpilogue(ulong address)
            {
                byte b = readByte(address);
                switch (b)
                {
                    case 201:
                        return true;
                    case 194:
                    case 195:
                    case 204:
                        {
                            byte b2 = readByte(address - 1);
                            if ((uint)(b2 - 90) > 1u && (uint)(b2 - 93) > 2u)
                            {
                                break;
                            }
                            if (b == 194)
                            {
                                ushort num = readUInt16(address + 1);
                                if (num % 4 == 0 && num > 0)
                                {
                                    _ = 1024;
                                    return true;
                                }
                            }
                            return true;
                        }
                }
                return false;
            }

            private bool isValidCode(ulong address)
            {
                if (readUInt64(address) == 0L)
                {
                    return readUInt64(address + 8) != 0;
                }
                return true;
            }

            public ulong gotoPrologue(ulong address)
            {
                ulong num = address;
                if (isPrologue(num))
                {
                    return num;
                }
                while (!isPrologue(num) && isValidCode(address))
                {
                    num = ((num % 16 == 0L) ? (num - 16) : (num - num % 16));
                }
                return num;
            }

            public ulong gotoNextPrologue(ulong address)
            {
                ulong num = address;
                if (isPrologue(num))
                {
                    num += 16;
                }
                while (!isPrologue(num) && isValidCode(num))
                {
                    num = ((num % 16 != 0L) ? (num + num % 16) : (num + 16));
                }
                return num;
            }
        }

        private static List<ulong> openedHandles = new List<ulong>();

        public static List<ProcInfo> openProcessesByName(string processName)
        {
            List<ProcInfo> list = new List<ProcInfo>();
            Process[] processesByName = Process.GetProcessesByName(processName.Replace(".exe", ""));
            foreach (Process process in processesByName)
            {
                try
                {
                    if (process.Id != 0 && !process.HasExited)
                    {
                        ProcInfo procInfo = new ProcInfo();
                        procInfo.processRef = process;
                        procInfo.baseModule = 0uL;
                        procInfo.handle = 0uL;
                        procInfo.processId = (ulong)process.Id;
                        procInfo.processName = processName;
                        procInfo.windowName = "";
                        list.Add(procInfo);
                    }
                }
                catch (NullReferenceException)
                {
                }
                catch (Exception)
                {
                }
            }
            return list;
        }

        public void flush()
        {
            foreach (ulong openedHandle in openedHandles)
            {
                Imports.CloseHandle(openedHandle);
            }
        }
    }
    internal enum InjectionStatus
    {
        FAILED,
        FAILED_ADMINISTRATOR_ACCESS,
        ALREADY_INJECTING,
        ALREADY_INJECTED,
        SUCCESS
    }
    internal class WindowsPlayer : Util
    {
        private static string injectFileName = "celerywindows.bin";

        public static ProcInfo lastProcInfo;

        public static Process injectorProc;

        private static List<ProcInfo> postInjectedMainPlayer = new List<ProcInfo>();

        private static bool isInjectingMainPlayer = false;

        public static bool isInjected()
        {
            if (injectorProc != null && lastProcInfo != null && lastProcInfo.processRef != null)
            {
                try
                {
                    return !injectorProc.HasExited && !lastProcInfo.processRef.HasExited;
                }
                catch (InvalidOperationException)
                {
                    return false;
                }
            }
            return false;
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetProp(IntPtr hWnd, string lpString, IntPtr hData);

        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string sClass, string sWindow);

        public static void sendScript(string source)
        {
            File.WriteAllText(string.Concat(Path.GetTempPath() + "celery", "\\myfile.txt"), source);
        }

        public static Process ExecuteAsAdmin(string fileName)
        {
            Process process = new Process();
            process.StartInfo.FileName = fileName;
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.Verb = "runas";
            process.Start();
            return process;
        }

        public static InjectionStatus injectPlayer(ProcInfo pinfo)
        {
            if (isInjectingMainPlayer)
            {
                return InjectionStatus.ALREADY_INJECTING;
            }
            if (isInjected())
            {
                return InjectionStatus.ALREADY_INJECTED;
            }
            isInjectingMainPlayer = true;
            FindWindow(null, "Roblox");
            injectorProc = ExecuteAsAdmin(AppDomain.CurrentDomain.BaseDirectory + "CeleryInject.exe");
            lastProcInfo = pinfo;
            isInjectingMainPlayer = false;
            return InjectionStatus.SUCCESS;
        }

        public static List<ProcInfo> getInjectedProcesses()
        {
            List<ProcInfo> list = new List<ProcInfo>();
            if (isInjected())
            {
                list.Add(lastProcInfo);
            }
            return list;
        }
    }

}
