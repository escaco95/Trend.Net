using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Trend
{
    //
    // 공개 클래스 Window
    //
    #region[    CLASS : +Window    ]
    /// <summary>
    /// 트렌디한 디자인의 윈도우를 제공합니다.
    /// </summary>
    public class Window : Form
    {
        //
        // 생성자
        //
        #region[    기본 생성자    ]
        /// <summary>
        /// 새 트렌드 윈도우 개체를 생성합니다.
        /// </summary>
        public Window()
        {
            //
            // 초기화에 영향을 주는 핵심 플래그 초기화.
            //
            _IsLoaded = false;
            //
            // 속성 초기화
            //
            InitializeInheritedProperties();
            InitializeProperties();
            //
            // 이벤트 등록
            //
            Disposed += AtDisposed;
        }
        #endregion
        //
        // 속성 (사용자가 조작 가능한)
        //
        #region[    속성 - 상속받은 속성의 기본값 변경    ]
        /// <summary>
        /// 컨트롤의 자동 크기 조정 모드를 가져오거나 설정합니다.
        /// </summary>
        [DefaultValue(typeof(AutoScaleMode), "Dpi")]
        public new AutoScaleMode AutoScaleMode
        {
            get => base.AutoScaleMode;
            set => base.AutoScaleMode = value;
        }
        /// <summary>
        /// 컨트롤의 기본 폰트를 가져오거나 설정합니다.
        /// </summary>
        [Description("컨트롤의 기본 폰트를 설정합니다.")]
        [DefaultValue(typeof(Font), "Segoe UI, 9pt")]
        public new Font Font
        {
            get => base.Font;
            set => base.Font = value;
        }


        private void InitializeInheritedProperties()
        {
            // 자동 크기 조정 모드를 설정합니다.
            this.AutoScaleMode = AutoScaleMode.Dpi;
            // 기본 폰트를 변경합니다.
            this.Font = new Font("Segoe UI", 9F);
        }
        #endregion
        #region[    속성 - 상속받은 속성 숨기기    ]
        /// <summary>
        /// <see cref="Form.FormBorderStyle"/> 속성은 <see cref="ModernWindow"/> 클래스에서 더 이상 사용되지 않습니다.<br/>
        /// 해당 컨트롤의 고정 속성은 <see cref="FormBorderStyle.None"/> 이며, 크기 조절 및 드래그 가능 여부는 따로 설정할 수 있습니다.
        /// </summary>
        [Browsable(false)]
        [DefaultValue(typeof(FormBorderStyle), "none")]
        public new FormBorderStyle FormBorderStyle
        {
            get => FormBorderStyle.None;
            set { /*Do Nothing*/ }
        }
        #endregion
        #region[    새 속성 - 새로운 속성의 기본값 설정    ]
        private void InitializeProperties()
        {
            _WindowBackdrop = null;
            _WindowAuras = new WindowAuraCollection(this);
            _Resizable = true;
        }
        #endregion
        #region[    새 속성 - 아우라 (콜렉션/백드롭 등)    ]
        /// <summary>
        /// 창 테두리의 각 방향 두께를 가져오거나 설정합니다.
        /// </summary>
        [Description("창 테두리의 각 방향 두께를 설정합니다.")]
        [DefaultValue(typeof(Padding), "10,10,10,10")]
        public Padding AuraThickness
        {
            get => GetAuraThickness();
            set => SetAuraThickness(value);
        }
        private Padding _AuraThickness = new Padding(10);
        private Padding GetAuraThickness() => _AuraThickness;
        private void SetAuraThickness(Padding value)
        {
            if (_AuraThickness.Equals(value)) return;
            _AuraThickness = value;
            UpdateBackdrop();
        }
        /// <summary>
        /// 트렌드 윈도우의 아우라 레이어 콜렉션을 가져옵니다.
        /// </summary>
        public WindowAuraCollection WindowAuras => _WindowAuras;
        internal WindowAuraCollection _WindowAuras;
        internal WindowBackdrop _WindowBackdrop;
        internal int _WindowBackdropBufferWidth = 0;
        internal int _WindowBackdropBufferHeight = 0;
        internal Bitmap _WindowBackdropBuffer = null;
        private void InitializeBackdrop()
        {
            _WindowBackdrop = new WindowBackdrop();
            _WindowBackdrop.Show(this);
            UpdateBackdrop();
        }
        internal void UpdateBackdrop()
        {
            //
            // 상태 유효성 검사
            //
            if (_WindowBackdrop == null
                || _WindowBackdrop.IsDisposed
                || _WindowBackdrop.Disposing) return;


            int width = Width + _AuraThickness.Left + _AuraThickness.Right;
            int height = Height + _AuraThickness.Top + _AuraThickness.Bottom;


            Win32.SetWindowPos(
                _WindowBackdrop.Handle,
                base.Handle,
                base.Left - _AuraThickness.Left,
                base.Top - _AuraThickness.Top,
                width,
                height,
                Win32.SetWindowPosFlags.DoNotActivate);


            bool _visibility = true;
            if (!Visible || WindowState == FormWindowState.Maximized)
                _visibility = false;
            if (_WindowBackdrop.Visible != _visibility)
                _WindowBackdrop.Visible = _visibility;


            // 버퍼 새로고침이 필요한가?
            bool isBufferReallocationRequired = false;
            bool isBufferRedrawRequired = false;
            if (_WindowBackdropBuffer == null)
            {
                isBufferReallocationRequired = true;
            }
            else
            {
                if (_WindowBackdropBufferWidth != width || _WindowBackdropBufferHeight != height)
                    isBufferRedrawRequired = true;
                if (_WindowBackdropBuffer.Width < width || _WindowBackdropBuffer.Height < height)
                    isBufferReallocationRequired = true;
            }

            // 결국 버퍼 새로고침이 필요함!
            if (isBufferReallocationRequired)
            {
                if (_WindowBackdropBuffer != null)
                    _WindowBackdropBuffer.Dispose();

                Bitmap buffer;
                buffer = new Bitmap(width, height, PixelFormat.Format32bppArgb);
                _WindowBackdropBuffer = buffer;
                isBufferRedrawRequired = true;
            }

            if (isBufferRedrawRequired)
            {
                _WindowBackdropBufferWidth = width;
                _WindowBackdropBufferHeight = height;
                using (Graphics g = Graphics.FromImage(_WindowBackdropBuffer))
                {
                    _WindowAuras.Paint(g, width, height);
                }
                _WindowBackdrop.SetBitmap(_WindowBackdropBuffer, 255);
            }
        }
        #endregion
        #region[    새 속성 - 창 관련 (크기/사용자 조작가능 여부 등)    ]
        /// <summary>
        /// 폼의 클라이언트 영역 크기를 가져오거나 설정합니다.
        /// </summary>
        public new Size ClientSize
        {
            get => base.ClientSize;
            set => Size = value;
        }
        /// <summary>
        /// 클라이언트 영역을 컨트롤 상자처럼 사용할 것인지를 결정합니다.<br/>
        /// <br/>
        /// <see langword="true"/> : 컨트롤 배경을 드래그하거나 더블 클릭하여 최대화 및 복원 작업이 가능합니다.<br/>
        /// <see langword="false"/> : 일반적인 폼 배경처럼 동작합니다. (특별한 기능 없음)
        /// </summary>
        [Description("클라이언트 영역을 컨트롤 상자처럼 사용할 것인지를 결정합니다.")]
        [DefaultValue(true)]
        public bool ClientControlBox
        {
            get => GetClientControlBox();
            set => SetClientControlBox(value);
        }


        /// <summary>
        /// 클라이언트 영역을 우클릭하여 시스템 메뉴를 호출할지의 여부를 결정합니다.
        /// </summary>
        [Description("클라이언트 영역을 우클릭하여 시스템 메뉴를 호출할지의 여부를 결정합니다.")]
        [DefaultValue(true)]
        public bool ClientSystemMenu
        {
            get => GetClientSystemMenu();
            set => SetClientSystemMenu(value);
        }
        /// <summary>
        /// 창 크기 조절 테두리의 각 방향 두께를 가져오거나 설정합니다.
        /// </summary>
        [Description("창 크기 조절 테두리의 각 방향 두께를 설정합니다.")]
        [DefaultValue(typeof(Padding), "6,6,6,6")]
        public Padding ResizeGripSize
        {
            get => GetResizeGripSize();
            set => SetResizeGripSize(value);
        }
        /// <summary>
        /// 창 크기 조절이 가능한지의 여부를 가져오거나 설정합니다.
        /// </summary>
        [Description("창 크기 조절이 가능한지의 여부를 설정합니다.")]
        [DefaultValue(true)]
        public bool Resizable
        {
            get => GetResizable();
            set => SetResizable(value);
        }
        private bool _Resizable = true;
        private bool GetResizable() => _Resizable;
        private void SetResizable(bool value)
        {
            if (_Resizable == value) return;
            _Resizable = value;
            if (DesignMode) return;
            if (!_IsLoaded) return;
            Win32.ReleaseCapture();
            if (_Resizable)
            {
                base.FormBorderStyle = FormBorderStyle.Sizable;
            }
            else
            {
                base.FormBorderStyle = FormBorderStyle.None;
            }
            if (_ResizeGrips != null)
                foreach (var sizeGrip in _ResizeGrips)
                    sizeGrip.Visible = _Resizable;
        }
        private Padding _ResizeGripSize = new Padding(6);
        private Padding GetResizeGripSize() => _ResizeGripSize;
        private void SetResizeGripSize(Padding value)
        {
            if (_ResizeGripSize.Equals(value)) return;
            _ResizeGripSize = value;
            if (_ResizeGrips == null) return;
            base.SuspendLayout();
            foreach (var control in _ResizeGrips)
            {
                control.BorderSize = _ResizeGripSize;
            }
            base.ResumeLayout();
        }
        private WindowResizer[] _ResizeGrips = null;
        private void InitializeSizeGrips()
        {
            // UX 기능 : 창 크기 조절기를 동적으로 추가.(디자인 모드 충돌 방지)
            // 추가된 조절기를 가장 앞으로 가져와 Depth 문제를 해결.
            var resizers = new WindowResizer[]
            {
                new WindowResizer(){ Direction = WindowResizerDirection.TopLeft, BorderSize = _ResizeGripSize},
                new WindowResizer(){ Direction = WindowResizerDirection.Top, BorderSize = _ResizeGripSize},
                new WindowResizer(){ Direction = WindowResizerDirection.TopRight, BorderSize = _ResizeGripSize},
                new WindowResizer(){ Direction = WindowResizerDirection.Left, BorderSize = _ResizeGripSize},
                new WindowResizer(){ Direction = WindowResizerDirection.Right, BorderSize = _ResizeGripSize},
                new WindowResizer(){ Direction = WindowResizerDirection.BottomLeft, BorderSize = _ResizeGripSize},
                new WindowResizer(){ Direction = WindowResizerDirection.Bottom, BorderSize = _ResizeGripSize},
                new WindowResizer(){ Direction = WindowResizerDirection.BottomRight, BorderSize = _ResizeGripSize},
            };
            Controls.AddRange(resizers);

            foreach (var control in resizers)
            {
                control.BringToFront();
                control.Visible = _Resizable;
            }

            _ResizeGrips = resizers;
        }
        #endregion
        //
        // 동작 (사용자가 임의로 수정할 수 없는)
        //
        #region[    동작 플래그 - 폼이 로드됨    ]
        /// <summary>
        /// 윈도우가 로드되었는지의 여부를 가져옵니다.
        /// </summary>
        public bool IsLoaded => _IsLoaded;
        private bool _IsLoaded;
        #endregion
        #region[    동작 - WndProc    ]
        protected override void WndProc(ref Message m)
        {
            if (!DesignMode)
            {
                switch ((Win32.WindowMessages)m.Msg)
                {
                    case Win32.WindowMessages.NCCALCSIZE:
                        if (m.WParam.ToInt32() == 1)
                        {
                            int nBorder = 0;

                            Win32.WINDOWPLACEMENT wndpl = new Win32.WINDOWPLACEMENT();

                            wndpl.length = Marshal.SizeOf(wndpl);

                            if (Win32.GetWindowPlacement((int)Handle, ref wndpl))
                            {
                                if (
                                    wndpl.showCmd == Win32.ShowWindowCommands.Maximize
                                    && base.FormBorderStyle != FormBorderStyle.None
                                    )
                                {
                                    nBorder = 8;
                                }
                            }
                            var nccsp = (Win32.NCCALCSIZE_PARAMS)Marshal.PtrToStructure(m.LParam, typeof(Win32.NCCALCSIZE_PARAMS));

                            //We're adjusting the size of the client area here. Right now, the client area is the whole form.
                            //Adding to the Top, Bottom, Left, and Right will size the client area.
                            nccsp.rgrc[0].left += nBorder;      //4-pixel left (resize) border
                            nccsp.rgrc[0].top += nBorder;      //30-pixel top border
                            nccsp.rgrc[0].right -= nBorder;     //4-pixel right (resize) border
                            nccsp.rgrc[0].bottom -= nBorder;    //4-pixel bottom (resize) border

                            //Set the structure back into memory
                            Marshal.StructureToPtr(nccsp, m.LParam, true);

                            m.Result = (IntPtr)0;
                        }
                        return;
                    case Win32.WindowMessages.NCHITTEST:
                        m.Result = (IntPtr)Win32.HitTestValues.HTCLIENT;
                        if (!_ClientControlBox) break;
                        if (!base.MaximizeBox) break;
                        m.Result = (IntPtr)Win32.HitTestValues.HTCAPTION;
                        return;
                    case Win32.WindowMessages.NCRBUTTONUP:
                        if (!_ClientSystemMenu) break;
                        if (!base.MaximizeBox) break;
                        SystemMenu.Show(Handle);
                        return;
                    case Win32.WindowMessages.WINDOWPOSCHANGED:
                        base.WndProc(ref m);
                        UpdateBackdrop();
                        // 최대화 / 최소화 상태 변경됨
                        //base.PerformLayout();
                        BringToFront();
                        Activate();
                        return;
                }
            }
            base.WndProc(ref m);
        }
        #endregion
        #region[    동작 - DesignMode    ]
        /// <summary>
        /// 디자인 모드에서'만' 통용되는 설정을 반영합니다.
        /// </summary>
        private void InitializeDesignModeSettings()
        {
            // 폼 테두리가 없는 상태에서 디자인 작업을 수행할 수 있게 합니다.
            base.FormBorderStyle = FormBorderStyle.None;
        }
        /// <summary>
        /// 실행 모드에서'만' 통용되는 설정을 반영합니다.
        /// </summary>
        private void InitializeNonDesignModeSettings()
        {
            // 리사이즈 가능 여부에 따라 폼 테두리를 업데이트합니다.
            base.FormBorderStyle = (_Resizable)
                ? FormBorderStyle.Sizable
                : FormBorderStyle.None;
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            //
            // 디자인 OR 런타임에 필요한 초기화 작업을 진행합니다.
            //
            if (DesignMode)
            {
                InitializeDesignModeSettings();
            }
            else
            {
                InitializeNonDesignModeSettings();
                InitializeSizeGrips();
                InitializeBackdrop();
            }
            //
            // 윈도우를 '로드됨'으로 마킹합니다.
            //
            _IsLoaded = true;
        }
        #endregion
        #region[    동작 - ClientSystemMenu    ]
        private bool _ClientSystemMenu = true;
        private bool GetClientSystemMenu() => _ClientSystemMenu;
        private void SetClientSystemMenu(bool value)
        {
            if (_ClientSystemMenu.Equals(value)) return;
            _ClientSystemMenu = value;
            Win32.ReleaseCapture();
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (!_ClientSystemMenu) return;
            //if (!m_ClientControlBox) return;
            if (base.MaximizeBox) return;
            if (e.Button != MouseButtons.Right) return;
            SystemMenu.Show(Handle);
        }
        #endregion
        #region[    동작 - ClientControlBox    ]
        private bool _ClientControlBox = true;
        private bool GetClientControlBox() => _ClientControlBox;
        private void SetClientControlBox(bool value)
        {
            if (_ClientControlBox.Equals(value)) return;
            _ClientControlBox = value;
            Win32.ReleaseCapture();
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (!_ClientControlBox) return;
            if (base.MaximizeBox) return;
            if (e.Button != MouseButtons.Left) return;
            Win32.ReleaseCapture();
            Win32.SendMessage(Handle, (int)Win32.WindowMessages.NCLBUTTONDOWN, (int)Win32.HitTestValues.HTCAPTION, 0);
        }
        #endregion

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            if (DesignMode) return;

            _WindowBackdrop?.Close();
        }

        private void AtDisposed(object sender, EventArgs e)
        {
            if (this._WindowBackdropBuffer != null)
                this._WindowBackdropBuffer.Dispose();
        }

    }
    #endregion
    //
    // 내부 클래스 WindowBackdrop
    //
    #region[    CLASS : *WindowBackdrop    ]
    /// <summary>
    /// 트렌드 윈도우의 아우라 개체입니다.
    /// </summary>
    internal sealed class WindowBackdrop : Form
    {
        /// <summary>
        /// 새 물리 아우라 개체를 생성합니다.
        /// </summary>
        internal WindowBackdrop()
        {
            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;
            MaximizeBox = false;
            MinimizeBox = false;

            /*This bit of code makes the form click-through. 
              So you can click forms that are below it in z-space */
            int wl = Win32.GetWindowLong(this.Handle, -20);
            wl = wl | 0x80000 | 0x20;
            Win32.SetWindowLong(this.Handle, -20, wl);
        }


        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x00080000; // This form has to have the WS_EX_LAYERED extended style
                return cp;
            }
        }


        internal void SetBitmap(Bitmap bitmap, byte opacity)
        {
            if (IsDisposed || Disposing) return;

            if (bitmap == null || (bitmap.Width * bitmap.Height < 0))
                throw new Exception();

            if (bitmap.PixelFormat != PixelFormat.Format32bppArgb)
                throw new ApplicationException("The bitmap must be 32ppp with alpha-channel.");

            // 1. Create a compatible DC with screen;
            // 2. Select the bitmap with 32bpp with alpha-channel in the compatible DC;
            // 3. Call the UpdateLayeredWindow.

            IntPtr screenDc = Win32.GetDC(IntPtr.Zero);
            IntPtr memDc = Win32.CreateCompatibleDC(screenDc);
            IntPtr hBitmap = IntPtr.Zero;
            IntPtr oldBitmap = IntPtr.Zero;

            try
            {
                hBitmap = bitmap.GetHbitmap(Color.FromArgb(0));  // grab a GDI handle from this GDI+ bitmap
                oldBitmap = Win32.SelectObject(memDc, hBitmap);

                Size size = new Size(bitmap.Width, bitmap.Height);
                Point pointSource = new Point(0, 0);
                Point topPos = new Point(Left, Top);
                Win32.BLENDFUNCTION blend = new Win32.BLENDFUNCTION
                {
                    BlendOp = Win32.AC_SRC_OVER,
                    BlendFlags = 0,
                    SourceConstantAlpha = opacity,
                    AlphaFormat = Win32.AC_SRC_ALPHA
                };

                Win32.UpdateLayeredWindow(this.Handle, screenDc, ref topPos, ref size, memDc, ref pointSource, 0, ref blend, Win32.ULW_ALPHA);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                Win32.ReleaseDC(IntPtr.Zero, screenDc);
                if (hBitmap != IntPtr.Zero)
                {
                    Win32.SelectObject(memDc, oldBitmap);
                    Win32.DeleteObject(hBitmap);
                }
                Win32.DeleteDC(memDc);
            }
        }
    }
    #endregion
    //
    // 내부 클래스 WindowResizer
    //
    #region[    CLASS : *WindowResizer    ]
    /// <summary>
    /// 컨트롤 크기 조절기 방향.
    /// </summary>
    internal enum WindowResizerDirection
    {
        TopLeft,
        Top,
        TopRight,
        Left,
        Right,
        BottomLeft,
        Bottom,
        BottomRight,
    }
    /// <summary>
    /// 컨트롤 크기 조절기 클래스입니다.
    /// </summary>
    internal sealed class WindowResizer : Panel
    {
        /// <summary>
        /// 크기 조절 가능한 테두리 두께를 가져오거나 설정합니다.<br/>
        /// 두께가 0인 방향은 크기를 조절할 수 없습니다.
        /// </summary>
        [DefaultValue(typeof(Padding), "6,6,6,6")]
        public Padding BorderSize
        {
            get => m_BorderSize;
            set
            {
                if (m_BorderSize.Equals(value)) return;
                m_BorderSize = value;
                UpdateBounds();
            }
        }


        /// <summary>
        /// 크기를 조절할 테두리 방향을 가져오거나 설정합니다.
        /// </summary>
        [DefaultValue(typeof(WindowResizerDirection), "Top")]
        public WindowResizerDirection Direction
        {
            get => m_ResizerDirection;
            set
            {
                if (m_ResizerDirection == value) return;
                m_ResizerDirection = value;
                UpdateDirection();
            }
        }


        private volatile bool m_Visible = true;
        public new bool Visible
        {
            get => m_Visible;
            set
            {
                if (m_Visible.Equals(value)) return;
                m_Visible = value;
                UpdateVisibility();
            }
        }


        private Padding m_BorderSize;
        private WindowResizerDirection m_ResizerDirection;


        public WindowResizer()
        {
            m_BorderSize = new Padding(6);
            m_ResizerDirection = WindowResizerDirection.Top;
            base.Size = new Size(5, 5);

            if (DesignMode) return;

            SetStyle(ControlStyles.Opaque, true);
            UpdateDirection();
        }


        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);

            if (DesignMode) return;

            UpdateBounds();
        }


        private new void UpdateBounds()
        {
            Control parent = Parent;
            if (parent == null) parent = FindForm();
            if (parent == null) return;
            var bound = parent.ClientRectangle;
            switch (m_ResizerDirection)
            {
                case WindowResizerDirection.Top:
                    SetBounds(
bound.Left + m_BorderSize.Left, 0,
bound.Width - m_BorderSize.Left - m_BorderSize.Right, m_BorderSize.Top
); break;
                case WindowResizerDirection.TopLeft:
                    SetBounds(
0, 0,
m_BorderSize.Left, m_BorderSize.Top
); break;
                case WindowResizerDirection.TopRight:
                    SetBounds(
bound.Right - m_BorderSize.Right, 0,
m_BorderSize.Right, m_BorderSize.Top
); break;
                case WindowResizerDirection.Left:
                    SetBounds(
0, m_BorderSize.Top,
m_BorderSize.Left, bound.Height - m_BorderSize.Top - m_BorderSize.Bottom
); break;
                case WindowResizerDirection.Right:
                    SetBounds(
bound.Right - m_BorderSize.Right, m_BorderSize.Top,
m_BorderSize.Right, bound.Height - m_BorderSize.Top - m_BorderSize.Bottom
); break;
                case WindowResizerDirection.Bottom:
                    SetBounds(
m_BorderSize.Left, bound.Bottom - m_BorderSize.Bottom,
bound.Width - m_BorderSize.Left - m_BorderSize.Right, m_BorderSize.Bottom
); break;
                case WindowResizerDirection.BottomLeft:
                    SetBounds(
0, bound.Bottom - m_BorderSize.Bottom,
m_BorderSize.Left, m_BorderSize.Bottom
); break;
                case WindowResizerDirection.BottomRight:
                    SetBounds(
bound.Right - m_BorderSize.Right, bound.Bottom - m_BorderSize.Bottom,
m_BorderSize.Right, m_BorderSize.Bottom
); break;
            }
        }
        private void UpdateCursor()
        {
            Cursor result = Cursor;

            if (FindForm() is Form form)
            {
                if (form.WindowState == FormWindowState.Maximized)
                    result = Cursors.Default;
                else
                {
                    switch (m_ResizerDirection)
                    {
                        case WindowResizerDirection.Top:
                        case WindowResizerDirection.Bottom:
                            result = Cursors.SizeNS;
                            break;
                        case WindowResizerDirection.Left:
                        case WindowResizerDirection.Right:
                            result = Cursors.SizeWE;
                            break;
                        case WindowResizerDirection.TopLeft:
                        case WindowResizerDirection.BottomRight:
                            result = Cursors.SizeNWSE;
                            break;
                        case WindowResizerDirection.TopRight:
                        case WindowResizerDirection.BottomLeft:
                            result = Cursors.SizeNESW;
                            break;
                    }
                }
            }

            if (result == Cursor) return;
            Cursor = result;
        }
        private void UpdateDirection()
        {
            switch (m_ResizerDirection)
            {
                case WindowResizerDirection.Top:
                case WindowResizerDirection.Bottom:
                    Anchor = (m_ResizerDirection == WindowResizerDirection.Top)
                        ? (AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top)
                        : (AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom);
                    break;
                case WindowResizerDirection.Left:
                case WindowResizerDirection.Right:
                    Anchor = (m_ResizerDirection == WindowResizerDirection.Left)
                        ? (AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom)
                        : (AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom);
                    break;
                case WindowResizerDirection.TopLeft:
                case WindowResizerDirection.BottomRight:
                    Anchor = (m_ResizerDirection == WindowResizerDirection.TopLeft)
                        ? (AnchorStyles.Left | AnchorStyles.Top)
                        : (AnchorStyles.Right | AnchorStyles.Bottom);
                    break;
                case WindowResizerDirection.TopRight:
                case WindowResizerDirection.BottomLeft:
                    Anchor = (m_ResizerDirection == WindowResizerDirection.TopRight)
                        ? (AnchorStyles.Right | AnchorStyles.Top)
                        : (AnchorStyles.Left | AnchorStyles.Bottom);
                    break;
            }

            UpdateBounds();
        }


        private void UpdateVisibility()
        {
            bool result = m_Visible;

            if (FindForm() is Form form)
            {
                if (form.WindowState == FormWindowState.Maximized)
                    result = false;
            }

            if (result.Equals(base.Visible)) return;
            base.Visible = result;
        }


        protected override void OnClientSizeChanged(EventArgs e)
        {
            base.OnClientSizeChanged(e);

            if (DesignMode) return;

            UpdateVisibility();
        }


        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle |= 0x00000020;
                return cp;
            }
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (DesignMode) return;
            UpdateCursor();
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
                MoveForm();
        }
        private void MoveForm()
        {
            if (!(FindForm() is Form form)) return;

            if (form.WindowState == FormWindowState.Maximized) return;

            Win32.ReleaseCapture();
            Win32.HitTestValues msg;
            switch (m_ResizerDirection)
            {
                case WindowResizerDirection.Top: msg = Win32.HitTestValues.HTTOP; break;
                case WindowResizerDirection.TopLeft: msg = Win32.HitTestValues.HTTOPLEFT; break;
                case WindowResizerDirection.TopRight: msg = Win32.HitTestValues.HTTOPRIGHT; break;
                case WindowResizerDirection.Left: msg = Win32.HitTestValues.HTLEFT; break;
                case WindowResizerDirection.Right: msg = Win32.HitTestValues.HTRIGHT; break;
                case WindowResizerDirection.Bottom: msg = Win32.HitTestValues.HTBOTTOM; break;
                case WindowResizerDirection.BottomLeft: msg = Win32.HitTestValues.HTBOTTOMLEFT; break;
                case WindowResizerDirection.BottomRight: msg = Win32.HitTestValues.HTBOTTOMRIGHT; break;
                default: return;
            }
            Win32.SendMessage(form.Handle, 0xA1, (int)msg, 0);
        }
    }
    #endregion
    //
    // 래퍼 클래스 WindowAura
    //
    #region[    CLASS : +~WindowAura    ]
    /// <summary>
    /// 윈도우 아우라 레이어링과 관련된 기능을 제공하는 콜렉션입니다.
    /// </summary>
    public sealed class WindowAuraCollection : ICollection<WindowAura>
    {
        /// <summary>
        /// 콜렉션에 포함된 레이어 수를 가져옵니다.
        /// </summary>
        public int Count => _WindowAuras.Count;


        /// <summary>
        /// 아우라 콜렉션이 읽기 전용으로 설정되어 있는지의 여부를 가져옵니다.
        /// </summary>
        public bool IsReadOnly => false;


        /// <summary>
        /// 콜렉션의 맨 위에 새 아우라 레이어를 추가합니다.
        /// </summary>
        /// <param name="item">추가할 레이어.</param>
        public void Add(WindowAura item)
        {
            // 인자 유효성 검사
            if (item == null) throw new ArgumentNullException();


            // 로직 처리
            _WindowAuras.Add(item);


            // TODO 결과 반영
            _Window.UpdateBackdrop();
        }


        /// <summary>
        /// 모든 레이어를 제거합니다.
        /// </summary>
        public void Clear()
        {
            // 로직 처리
            _WindowAuras.Clear();


            // TODO 결과 반영
            _Window.UpdateBackdrop();
        }


        /// <summary>
        /// 콜렉션이 아우라 레이어를 포함하고 있는지의 여부를 반환합니다.
        /// </summary>
        /// <param name="item">찾을 레이어입니다.</param>
        /// <returns>포함하고 있으면 true이고, 그렇지 않으면 false입니다.</returns>
        public bool Contains(WindowAura item)
        {
            return _WindowAuras.Contains(item);
        }


        /// <summary>
        /// 콜렉션을 1차원 배열에 복사합니다.
        /// </summary>
        /// <param name="array">대상 배열.</param>
        /// <param name="arrayIndex">복사가 시작되는 인덱스.</param>
        public void CopyTo(WindowAura[] array, int arrayIndex)
        {
            _WindowAuras.CopyTo(array, arrayIndex);
        }


        /// <summary>
        /// 콜렉션을 반복하는 열거자를 반환합니다.
        /// </summary>
        /// <returns>콜렉션을 반복하는 열거자입니다.</returns>
        public IEnumerator<WindowAura> GetEnumerator()
        {
            return _WindowAuras.GetEnumerator();
        }


        /// <summary>
        /// 콜렉션에서 맨 처음 발견되는 특정 아우라 레이어를 제거합니다.
        /// </summary>
        /// <param name="item">제거할 아우라.</param>
        /// <returns>아우라가 제거되면 true이고, 그렇지 않으면 false입니다.</returns>
        public bool Remove(WindowAura item)
        {
            // 인자 유효성 검사
            if (item == null) throw new ArgumentNullException();


            // 로직 처리
            bool result = _WindowAuras.Remove(item);


            // TODO 결과 반영
            _Window.UpdateBackdrop();


            // Lazy한 결과 반환
            return result;
        }


        /// <summary>
        /// 콜렉션을 반복하는 열거자를 반환합니다.
        /// </summary>
        /// <returns>콜렉션을 반복하는 열거자입니다.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _WindowAuras.GetEnumerator();
        }


        /// <summary>
        /// 콜렉션 소유주 윈도우를 반환합니다.
        /// </summary>
        public Window Window => _Window;


        /// <summary>
        /// 콜렉션 레이어를 그래픽 개체에 그리도록 지시합니다.
        /// </summary>
        /// <param name="g">그래픽 개체.</param>
        /// <param name="width">그려질 너비.</param>
        /// <param name="height">그려질 높이.</param>
        internal void Paint(Graphics g, int width, int height)
        {
            g.Clear(Color.Transparent);
            foreach (WindowAura aura in _WindowAuras)
                aura.OnPaint(g, width, height);
        }


        private Window _Window;
        private List<WindowAura> _WindowAuras;
        internal WindowAuraCollection(Window parent)
        {
            // 인자 유효성 검사
            if (parent == null) throw new ArgumentNullException();


            // 로직 처리
            _Window = parent;
            _WindowAuras = new List<WindowAura>();
        }
    }
    /// <summary>
    /// 트렌드 윈도우의 배경 아우라 레이어입니다.
    /// </summary>
    public abstract class WindowAura
    {
        /// <summary>
        /// 트렌드 윈도우의 아우라가 드로우되면 실행됩니다.
        /// </summary>
        /// <param name="g">그래픽 개체.</param>
        /// <param name="width">아우라 폭.</param>
        /// <param name="height">아우라 높이.</param>
        public abstract void OnPaint(Graphics g, int width, int height);
    }
    /// <summary>
    /// 라이브러리에서 기본으로 제공하는 윈도우 배경 아우라 레이어 샘플입니다.
    /// </summary>
    public static class WindowAuras
    {
        /// <summary>
        /// 검정 그림자 아우라.
        /// </summary>
        public sealed class Shadow : WindowAura
        {
            private static string BaseImage
                = "iVBORw0KGgoAAAANSUhEUgAAAB4AAAAeCAYAAAA7MK6iAAAAwUlEQVR4nO2WsQqDMBCG09ShELBLwKGrk4Obu+//UvGkfyA0PUxKvaM0wjf+93meJGdCCEYDFenPiG0hXxFfE7pC0sxH4j14IxzRV+KQZeVH0jvhiaESjywr52YZpQ9iJKZKRmSjPJs9163DW+8FZmKpZEbWo1bWNSfu8ckmFForWZAdUOtQfAnPPzMTm8KHEXeozYrtiWLbxE3cxCJitQNE7chUvSTUrsXYtfgi8CoXXX1Sufiy9272YuvtafyfeAMMZ4BEDv6r0AAAAABJRU5ErkJggg==";
            internal static readonly Image _AuraBitmap = HelperImage.ConvertFromBase64(BaseImage);


            /// <summary>
            /// 그림자 색상을 가져옵니다.
            /// </summary>
            public Color Color => _Color;
            /// <summary>
            /// 새 그림자 아우라 레이어 개체를 만듭니다.
            /// </summary>
            public Shadow()
            {
                _Color = Color.Black;
                _ColorMatrix = HelperColor.Matrix(_Color);
            }
            /// <summary>
            /// 새 그림자 아우라 레이어 개체를 만듭니다. (그림자 색조 지정)
            /// </summary>
            public Shadow(Color color)
            {
                _Color = color;
                _ColorMatrix = HelperColor.Matrix(_Color);
            }


            public override void OnPaint(Graphics g, int width, int height)
            {
                using (ImageAttributes ia = new ImageAttributes())
                {
                    ia.SetColorMatrix(_ColorMatrix);

                    g.DrawImage(_AuraBitmap, new Rectangle(0, 0, 10, 10), 0, 0, 10, 10, GraphicsUnit.Pixel, ia);
                    g.DrawImage(_AuraBitmap, new Rectangle(width - 10, 0, 10, 10), 20, 0, 10, 10, GraphicsUnit.Pixel, ia);
                    g.DrawImage(_AuraBitmap, new Rectangle(0, height - 10, 10, 10), 0, 20, 10, 10, GraphicsUnit.Pixel, ia);
                    g.DrawImage(_AuraBitmap, new Rectangle(width - 10, height - 10, 10, 10), 20, 20, 10, 10, GraphicsUnit.Pixel, ia);

                    g.DrawImage(_AuraBitmap, new Rectangle(10, 0, width - 20, 10), 10, 0, 10, 10, GraphicsUnit.Pixel, ia);
                    g.DrawImage(_AuraBitmap, new Rectangle(10, height - 10, width - 20, 10), 10, 20, 10, 10, GraphicsUnit.Pixel, ia);

                    g.DrawImage(_AuraBitmap, new Rectangle(0, 10, 10, height - 20), 0, 10, 10, 10, GraphicsUnit.Pixel, ia);
                    g.DrawImage(_AuraBitmap, new Rectangle(width - 10, 10, 10, height - 20), 20, 10, 10, 10, GraphicsUnit.Pixel, ia);
                }
            }


            private ColorMatrix _ColorMatrix;
            private Color _Color;
        }
        /// <summary>
        /// 우아하게 빛나는 아우라.
        /// </summary>
        public sealed class Glow : WindowAura
        {
            private static string BaseImage
                = "iVBORw0KGgoAAAANSUhEUgAAAB4AAAAeCAYAAAA7MK6iAAAAt0lEQVR4nO2WMQ6DMAxFkUpTkqEjUzcOwC26wcb9j+La0kfKkLYOtPHiSG/D/4VgEXdE1FlgInXxJ3rmqqQ/K5aQwNyYyCQlETUBGVVi2fWdGZnHQUZkFE/gnTigcGLmg0zICDXiAbuWgCezMJuSBTUzMgat+ILvtItXql9rJo7I/CqWhkiZWN6Cn9QtiLdMnKjQZC52sYtd/FOx2S/T7JIQzK5Fs0Fgb7Lmo0/pBJoNe3/Fxc14ATu5qPC4rzfzAAAAAElFTkSuQmCC";
            internal static readonly Image _AuraBitmap = HelperImage.ConvertFromBase64(BaseImage);


            /// <summary>
            /// 그림자 색상을 가져옵니다.
            /// </summary>
            public Color Color => _Color;
            /// <summary>
            /// 새 빛나는 아우라 레이어 개체를 만듭니다.
            /// </summary>
            public Glow()
            {
                _Color = Color.Aqua;
                _ColorMatrix = HelperColor.Matrix(_Color);
            }
            /// <summary>
            /// 새 빛나는 아우라 레이어 개체를 만듭니다. (그림자 색조 지정)
            /// </summary>
            public Glow(Color color)
            {
                _Color = color;
                _ColorMatrix = HelperColor.Matrix(_Color);
            }


            public override void OnPaint(Graphics g, int width, int height)
            {
                using (ImageAttributes ia = new ImageAttributes())
                {
                    ia.SetColorMatrix(_ColorMatrix);

                    g.DrawImage(_AuraBitmap, new Rectangle(0, 0, 10, 10), 0, 0, 10, 10, GraphicsUnit.Pixel, ia);
                    g.DrawImage(_AuraBitmap, new Rectangle(width - 10, 0, 10, 10), 20, 0, 10, 10, GraphicsUnit.Pixel, ia);
                    g.DrawImage(_AuraBitmap, new Rectangle(0, height - 10, 10, 10), 0, 20, 10, 10, GraphicsUnit.Pixel, ia);
                    g.DrawImage(_AuraBitmap, new Rectangle(width - 10, height - 10, 10, 10), 20, 20, 10, 10, GraphicsUnit.Pixel, ia);

                    g.DrawImage(_AuraBitmap, new Rectangle(10, 0, width - 20, 10), 10, 0, 10, 10, GraphicsUnit.Pixel, ia);
                    g.DrawImage(_AuraBitmap, new Rectangle(10, height - 10, width - 20, 10), 10, 20, 10, 10, GraphicsUnit.Pixel, ia);

                    g.DrawImage(_AuraBitmap, new Rectangle(0, 10, 10, height - 20), 0, 10, 10, 10, GraphicsUnit.Pixel, ia);
                    g.DrawImage(_AuraBitmap, new Rectangle(width - 10, 10, 10, height - 20), 20, 10, 10, 10, GraphicsUnit.Pixel, ia);
                }
            }


            private ColorMatrix _ColorMatrix;
            private Color _Color;
        }
        /// <summary>
        /// 실선 아우라.
        /// </summary>
        public sealed class Line : WindowAura
        {
            private static string BaseImage
                = "iVBORw0KGgoAAAANSUhEUgAAAB4AAAAeCAYAAAA7MK6iAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAABUSURBVEhL7dRBCgAgCERR7f53NgO3lUGMQfM2toj+JlQzkwotJhzDMAzDlIWzC+R0y2jMqXRYXZyX/L3x4PYuPxcMwzAMwzy/Mq/j54JhGOa3sEgHz8oWKarOxYgAAAAASUVORK5CYIKx2S/T7JIQzK5Fs0Fgb7Lmo0/pBJoNe3/Fxc14ATu5qPC4rzfzAAAAAElFTkSuQmCC";
            internal static readonly Image _AuraBitmap = HelperImage.ConvertFromBase64(BaseImage);

            /// <summary>
            /// 실선 색상을 가져옵니다.
            /// </summary>
            public Color Color => _Color;
            /// <summary>
            /// 새 실선 아우라 레이어 개체를 만듭니다.
            /// </summary>
            public Line()
            {
                _Color = Color.Gray;
                _ColorMatrix = HelperColor.Matrix(_Color);
            }
            /// <summary>
            /// 새 실선 아우라 레이어 개체를 만듭니다. (그림자 색조 지정)
            /// </summary>
            public Line(Color color)
            {
                _Color = color;
                _ColorMatrix = HelperColor.Matrix(_Color);
            }


            public override void OnPaint(Graphics g, int width, int height)
            {
                using (ImageAttributes ia = new ImageAttributes())
                {
                    ia.SetColorMatrix(_ColorMatrix);

                    g.DrawImage(_AuraBitmap, new Rectangle(0, 0, 10, 10), 0, 0, 10, 10, GraphicsUnit.Pixel, ia);
                    g.DrawImage(_AuraBitmap, new Rectangle(width - 10, 0, 10, 10), 20, 0, 10, 10, GraphicsUnit.Pixel, ia);
                    g.DrawImage(_AuraBitmap, new Rectangle(0, height - 10, 10, 10), 0, 20, 10, 10, GraphicsUnit.Pixel, ia);
                    g.DrawImage(_AuraBitmap, new Rectangle(width - 10, height - 10, 10, 10), 20, 20, 10, 10, GraphicsUnit.Pixel, ia);

                    g.DrawImage(_AuraBitmap, new Rectangle(10, 0, width - 20, 10), 10, 0, 10, 10, GraphicsUnit.Pixel, ia);
                    g.DrawImage(_AuraBitmap, new Rectangle(10, height - 10, width - 20, 10), 10, 20, 10, 10, GraphicsUnit.Pixel, ia);

                    g.DrawImage(_AuraBitmap, new Rectangle(0, 10, 10, height - 20), 0, 10, 10, 10, GraphicsUnit.Pixel, ia);
                    g.DrawImage(_AuraBitmap, new Rectangle(width - 10, 10, 10, height - 20), 20, 10, 10, 10, GraphicsUnit.Pixel, ia);
                }
            }


            private ColorMatrix _ColorMatrix;
            private Color _Color;
        }
    }
    #endregion
}
