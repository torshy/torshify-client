using System;
using System.Globalization;
using System.Runtime.InteropServices;

using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Util;

namespace Torshify.Client.Log
{
    public class CustomConsoleColorAppender : AppenderSkeleton
    {
        #region Fields

        public const string ConsoleError = "Console.Error";
        public const string ConsoleOut = "Console.Out";

        private static readonly char[] _windowsNewline = new char[] { '\r', '\n' };

        private LevelMapping _levelMapping;
        private bool _writeToErrorStream;

        #endregion Fields

        #region Constructors

        public CustomConsoleColorAppender()
        {
            this._writeToErrorStream = false;
            this._levelMapping = new LevelMapping();
        }

        #endregion Constructors

        #region Enumerations

        [Flags]
        public enum Colors
        {
            Blue = 1,
            Cyan = 3,
            Green = 2,
            HighIntensity = 8,
            Purple = 5,
            Red = 4,
            White = 7,
            Yellow = 6
        }

        #endregion Enumerations

        #region Properties

        public virtual string Target
        {
            get
            {
                return (this._writeToErrorStream ? "Console.Error" : "Console.Out");
            }
            set
            {
                string strB = value.Trim();
                if (string.Compare("Console.Error", strB, true, CultureInfo.InvariantCulture) == 0)
                {
                    this._writeToErrorStream = true;
                }
                else
                {
                    this._writeToErrorStream = false;
                }
            }
        }

        // Properties
        protected override bool RequiresLayout
        {
            get
            {
                return true;
            }
        }

        #endregion Properties

        #region Methods

        public override void ActivateOptions()
        {
            base.ActivateOptions();
            this._levelMapping.ActivateOptions();
        }

        public void AddMapping(LevelColors mapping)
        {
            this._levelMapping.Add(mapping);
        }

        protected override void Append(LoggingEvent loggingEvent)
        {
            CONSOLE_SCREEN_BUFFER_INFO console_screen_buffer_info;
            IntPtr zero = IntPtr.Zero;
            if (this._writeToErrorStream)
            {
                zero = GetStdHandle(0xfffffff4);
            }
            else
            {
                zero = GetStdHandle(0xfffffff5);
            }
            ushort attributes = 7;
            LevelColors colors = this._levelMapping.Lookup(loggingEvent.Level) as LevelColors;
            if (colors != null)
            {
                attributes = colors.CombinedColor;
            }

            string str = base.RenderLoggingEvent(loggingEvent);
            GetConsoleScreenBufferInfo(zero, out console_screen_buffer_info);
            SetConsoleTextAttribute(zero, attributes);
            char[] buffer = str.ToCharArray();
            int length = buffer.Length;
            bool flag = false;
            if (((length > 1) && (buffer[length - 2] == '\r')) && (buffer[length - 1] == '\n'))
            {
                length -= 2;
                flag = true;
            }

            Console.Write(buffer, 0, length);
            SetConsoleTextAttribute(zero, console_screen_buffer_info.wAttributes);
            if (flag)
            {
                Console.Write(_windowsNewline, 0, 2);
            }
        }

        [DllImport("Kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetConsoleOutputCP();

        [DllImport("Kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool GetConsoleScreenBufferInfo(IntPtr consoleHandle, out CONSOLE_SCREEN_BUFFER_INFO bufferInfo);

        [DllImport("Kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetStdHandle(uint type);

        [DllImport("Kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool SetConsoleTextAttribute(IntPtr consoleHandle, ushort attributes);

        #endregion Methods

        #region Nested Types

        [StructLayout(LayoutKind.Sequential)]
        private struct CONSOLE_SCREEN_BUFFER_INFO
        {
            public COORD dwSize;
            public COORD dwCursorPosition;
            public ushort wAttributes;
            public SMALL_RECT srWindow;
            public COORD dwMaximumWindowSize;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct COORD
        {
            public ushort x;
            public ushort y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SMALL_RECT
        {
            public ushort Left;
            public ushort Top;
            public ushort Right;
            public ushort Bottom;
        }

        public class LevelColors : LevelMappingEntry
        {
            #region Fields

            // Fields
            private ColoredConsoleAppender.Colors m_backColor;
            private ushort m_combinedColor = 0;
            private ColoredConsoleAppender.Colors m_foreColor;

            #endregion Fields

            #region Properties

            // Properties
            public ColoredConsoleAppender.Colors BackColor
            {
                get
                {
                    return this.m_backColor;
                }
                set
                {
                    this.m_backColor = value;
                }
            }

            public ColoredConsoleAppender.Colors ForeColor
            {
                get
                {
                    return this.m_foreColor;
                }
                set
                {
                    this.m_foreColor = value;
                }
            }

            internal ushort CombinedColor
            {
                get
                {
                    return this.m_combinedColor;
                }
            }

            #endregion Properties

            #region Methods

            // Methods
            public override void ActivateOptions()
            {
                base.ActivateOptions();
                this.m_combinedColor = (ushort)(this.m_foreColor + (((int)this.m_backColor) << 4));
            }

            #endregion Methods
        }

        #endregion Nested Types
    }
}