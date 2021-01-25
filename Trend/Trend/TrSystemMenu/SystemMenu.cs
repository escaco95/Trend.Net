using System;
using System.Windows.Forms;

namespace Trend
{
    /// <summary>
    /// 윈도우 창 시스템 메뉴 개체입니다.
    /// </summary>
    public static class SystemMenu
    {
        /// <summary>
        /// 시스템 메뉴를 호출합니다. (현재 마우스 좌표에서)
        /// </summary>
        /// <param name="handle">시스템 메뉴를 표시할 <see cref="Form"/> 핸들.</param>
        /// <returns>성공 여부.</returns>
        public static bool Show(IntPtr handle) => Show(handle, Cursor.Position.X, Cursor.Position.Y);
        /// <summary>
        /// 시스템 메뉴를 호출합니다. (지정한 좌표에서)
        /// </summary>
        /// <param name="handle">시스템 메뉴를 표시할 <see cref="Form"/> 핸들.</param>
        /// <param name="x">시스템 메뉴를 표시할 위치.</param>
        /// <param name="y">시스템 메뉴를 표시할 위치.</param>
        /// <returns>성공 여부.</returns>
        public static bool Show(IntPtr handle, int x, int y)
        {
            Win32.ReleaseCapture();
            var wMenu = Win32.GetSystemMenu(handle, false);

            uint command = Win32.TrackPopupMenuEx(wMenu,
                Win32.TPM_LEFTBUTTON | Win32.TPM_RETURNCMD,
                x, y, handle, IntPtr.Zero);
            if (command == 0)
                return false;

            Win32.PostMessage(handle, (uint)Win32.WindowMessages.SYSCOMMAND, new IntPtr(command), IntPtr.Zero);
            return true;
        }

    }
}
