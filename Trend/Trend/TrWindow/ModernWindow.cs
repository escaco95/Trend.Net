using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Trend
{
    //
    // Enums
    //
    #region[    ENUM : CodaFormBackdropStyle    ]
    /// <summary>
    /// <see cref="ModernWindow"/> 클래스의 폼 테두리 스타일을 지정합니다.
    /// </summary>
    public enum CodaFormBackdropStyle
    {
        /// <summary>
        /// 테두리가 없습니다.
        /// </summary>
        None,
        /// <summary>
        /// 각 테두리에 그림자를 설정합니다.
        /// </summary>
        Shadow,
        /// <summary>
        /// 네온처럼 빛나는 테두리를 설정합니다.<br/>
        /// 테두리 색상과 관련한 내용은 <see cref="ModernWindow.FormBorderGlowColor"/> 속성을 참고하세요.
        /// </summary>
        NeonGlow,
        /// <summary>
        /// 사용자가 직접 테두리를 그립니다.<br/>
        /// <see langword="override"/> <see cref="ModernWindow.OnPaintFormBorder(Graphics, int, int)"/>
        /// </summary>
        Custom,
    }
    #endregion
    //
    // ModernWindow 본문
    //
    #region[    CLASS : +ModernWindow    ]
    /// <summary>
    /// 모던 테마가 적용된 테두리 없는 폼.
    /// </summary>
    public class ModernWindow : Form
    {
        //
        // 생성자
        //
        #region[    생성자    ]
        /// <summary>
        /// 클래스의 새 인스턴스를 초기화합니다.
        /// </summary>
        public ModernWindow()
        {
            //
            // 속성 초기화
            //
            InitializeInheritedProperties();
        }
        #endregion
        //
        // 속성
        //
        #region[    속성 초기화    ]
        /// <summary>
        /// 상속받은 기본 속성값을 새로운 값으로 초기화합니다.
        /// </summary>
        private void InitializeInheritedProperties()
        {
            // 자동 크기 조정 모드를 설정합니다.
            this.AutoScaleMode = AutoScaleMode.Dpi;
            // 기본 폰트를 변경합니다.
            this.Font = new Font("Segoe UI", 9F);
        }
        #endregion
        #region[    속성 - 상속된 속성 숨기기    ]
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


        /// <summary>
        /// 컨트롤의 자동 크기 조정 모드를 가져오거나 설정합니다.
        /// </summary>
        [DefaultValue(typeof(AutoScaleMode), "Dpi")]
        public new AutoScaleMode AutoScaleMode
        {
            get => base.AutoScaleMode;
            set => base.AutoScaleMode = value;
        }
        #endregion
        #region[    속성 - 테마    ]
        /// <summary>
        /// 컨트롤의 기본 폰트를 가져오거나 설정합니다.
        /// </summary>
        [Category("CODA - 테마")]
        [Description("컨트롤의 기본 폰트를 설정합니다.")]
        [DefaultValue(typeof(Font), "Segoe UI, 9pt")]
        public new Font Font
        {
            get => base.Font;
            set => base.Font = value;
        }
        /// <summary>
        /// 창 테두리의 각 방향 두께를 가져오거나 설정합니다.
        /// </summary>
        [Category("CODA - 테마")]
        [Description("창 테두리의 각 방향 두께를 설정합니다.")]
        [DefaultValue(typeof(Padding), "10,10,10,10")]
        public Padding FormBorderThickness
        {
            get => GetFormBorderThickness();
            set => SetFormBorderThickness(value);
        }
        /// <summary>
        /// 창 테두리의 발광색을 가져오거나 설정합니다.
        /// </summary>
        [Category("CODA - 테마")]
        [Description("창 테두리의 발광색을 설정합니다. (테두리가 발광 모드일 때만 보임)")]
        [DefaultValue(typeof(Color), "0,122,204")]
        public Color FormBorderGlowColor
        {
            get => GetFormBorderGlowColor();
            set => SetFormBorderGlowColor(value);
        }
        /// <summary>
        /// 창 테두리 스타일을 가져오거나 설정합니다.
        /// </summary>
        [Category("CODA - 테마")]
        [Description("창 테두리 스타일을 설정합니다.")]
        [DefaultValue(typeof(CodaFormBackdropStyle), "NeonGlow")]
        public CodaFormBackdropStyle FormBackdropStyle
        {
            get => GetFormBackdropStyle();
            set => SetFormBackdropStyle(value);
        }
        #endregion
        #region[    속성 - 기능성    ]
        /// <summary>
        /// 클라이언트 영역을 컨트롤 상자처럼 사용할 것인지를 결정합니다.<br/>
        /// <br/>
        /// <see langword="true"/> : 컨트롤 배경을 드래그하거나 더블 클릭하여 최대화 및 복원 작업이 가능합니다.<br/>
        /// <see langword="false"/> : 일반적인 폼 배경처럼 동작합니다. (특별한 기능 없음)
        /// </summary>
        [Category("CODA - 기능성")]
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
        [Category("CODA - 기능성")]
        [Description("클라이언트 영역을 우클릭하여 시스템 메뉴를 호출할지의 여부를 결정합니다.")]
        [DefaultValue(true)]
        public bool ClientSystemMenu
        {
            get => GetClientSystemMenu();
            set => SetClientSystemMenu(value);
        }


        /// <summary>
        /// 창 크기 조절이 가능한지의 여부를 가져오거나 설정합니다.
        /// </summary>
        [Category("CODA - 기능성")]
        [Description("창 크기 조절이 가능한지의 여부를 설정합니다.")]
        [DefaultValue(true)]
        public bool ClientResizable
        {
            get => GetClientResizable();
            set => SetClientResizable(value);
        }


        /// <summary>
        /// 창 크기 조절 테두리의 각 방향 두께를 가져오거나 설정합니다.
        /// </summary>
        [Category("CODA - 기능성")]
        [Description("창 크기 조절 테두리의 각 방향 두께를 설정합니다.")]
        [DefaultValue(typeof(Padding), "6,6,6,6")]
        public Padding ClientResizeBorderThickness
        {
            get => GetClientResizeBorderThickness();
            set => SetClientResizeBorderThickness(value);
        }


        /// <summary>
        /// 최대화 기능의 사용 여부를 가져오거나 설정합니다.
        /// </summary>
        [Category("CODA - 기능성")]
        [Description("최대화 기능의 사용 여부를 설정합니다.")]
        [DefaultValue(true)]
        public new bool MaximizeBox
        {
            get => base.MaximizeBox;
            set => base.MaximizeBox = value;
        }


        /// <summary>
        /// 최소화 기능의 사용 여부를 가져오거나 설정합니다.
        /// </summary>
        [Category("CODA - 기능성")]
        [Description("최소화 기능의 사용 여부를 설정합니다.")]
        [DefaultValue(true)]
        public new bool MinimizeBox
        {
            get => base.MinimizeBox;
            set => base.MinimizeBox = value;
        }


        /// <summary>
        /// 폼의 클라이언트 영역 크기를 가져오거나 설정합니다.
        /// </summary>
        public new Size ClientSize
        {
            get => base.ClientSize;
            set => Size = value;
        }
        #endregion
        //
        // 기능
        //
        #region[    기능 - Loaded (로드 되었는지?)    ]
        /// <summary>
        /// 컨트롤이 로드되었는지의 여부를 가져옵니다.
        /// </summary>
        public bool Loaded
            => IsDisposed
            ? false
            : Disposing
            ? false
            : m_Loaded;
        private bool m_Loaded = false;
        #endregion
        #region[    기능 - ClientControlBox    ]
        private bool m_ClientControlBox = true;
        private bool GetClientControlBox() => m_ClientControlBox;
        private void SetClientControlBox(bool value)
        {
            if (m_ClientControlBox.Equals(value)) return;
            m_ClientControlBox = value;
            Win32.ReleaseCapture();
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (!m_ClientControlBox) return;
            if (base.MaximizeBox) return;
            if (e.Button != MouseButtons.Left) return;
            Win32.ReleaseCapture();
            Win32.SendMessage(Handle, (int)Win32.WindowMessages.NCLBUTTONDOWN, (int)Win32.HitTestValues.HTCAPTION, 0);
        }
        #endregion
        #region[    기능 - ClientSystemMenu    ]
        private bool m_ClientSystemMenu = true;
        private bool GetClientSystemMenu() => m_ClientSystemMenu;
        private void SetClientSystemMenu(bool value)
        {
            if (m_ClientSystemMenu.Equals(value)) return;
            m_ClientSystemMenu = value;
            Win32.ReleaseCapture();
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (!m_ClientSystemMenu) return;
            if (!m_ClientControlBox) return;
            if (base.MaximizeBox) return;
            if (e.Button != MouseButtons.Right) return;
            SystemMenu.Show(Handle);
        }
        #endregion
        #region[    기능 - ClientResizable    ]
        private bool m_ClientResizable = true;
        private bool GetClientResizable() => m_ClientResizable;
        private void SetClientResizable(bool value)
        {
            if (m_ClientResizable.Equals(value)) return;
            m_ClientResizable = value;
            if (DesignMode) return;
            if (!Loaded) return;
            Win32.ReleaseCapture();
            if (m_ClientResizable)
            {
                base.FormBorderStyle = FormBorderStyle.Sizable;
            }
            else
            {
                base.FormBorderStyle = FormBorderStyle.None;
            }
            if (m_SizeGrips != null)
                foreach (var sizeGrip in m_SizeGrips)
                    sizeGrip.Visible = m_ClientResizable;
        }
        private Padding m_ResizeGripBorderSize = new Padding(6);
        private Padding GetClientResizeBorderThickness() => m_ResizeGripBorderSize;
        private void SetClientResizeBorderThickness(Padding value)
        {
            if (m_ResizeGripBorderSize.Equals(value)) return;
            m_ResizeGripBorderSize = value;
            if (m_SizeGrips == null) return;
            base.SuspendLayout();
            foreach (var control in m_SizeGrips)
            {
                control.BorderSize = m_ResizeGripBorderSize;
            }
            base.ResumeLayout();
        }
        private CodaFormResizer[] m_SizeGrips = null;
        private void InitializeSizeGrips()
        {
            // UX 기능 : 창 크기 조절기를 동적으로 추가.(디자인 모드 충돌 방지)
            // 추가된 조절기를 가장 앞으로 가져와 Depth 문제를 해결.
            var resizers = new CodaFormResizer[]
            {
                new CodaFormResizer(){ Direction = CodaFormResizerDirection.TopLeft, BorderSize = m_ResizeGripBorderSize},
                new CodaFormResizer(){ Direction = CodaFormResizerDirection.Top, BorderSize = m_ResizeGripBorderSize},
                new CodaFormResizer(){ Direction = CodaFormResizerDirection.TopRight, BorderSize = m_ResizeGripBorderSize},
                new CodaFormResizer(){ Direction = CodaFormResizerDirection.Left, BorderSize = m_ResizeGripBorderSize},
                new CodaFormResizer(){ Direction = CodaFormResizerDirection.Right, BorderSize = m_ResizeGripBorderSize},
                new CodaFormResizer(){ Direction = CodaFormResizerDirection.BottomLeft, BorderSize = m_ResizeGripBorderSize},
                new CodaFormResizer(){ Direction = CodaFormResizerDirection.Bottom, BorderSize = m_ResizeGripBorderSize},
                new CodaFormResizer(){ Direction = CodaFormResizerDirection.BottomRight, BorderSize = m_ResizeGripBorderSize},
            };
            Controls.AddRange(resizers);

            foreach (var control in resizers)
            {
                control.BringToFront();
                control.Visible = m_ClientResizable;
            }

            m_SizeGrips = resizers;
        }
        #endregion
        //
        // 동작
        //
        #region[    동작 - Paint    ]
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            e.Graphics.Clear(BackColor);
        }
        #endregion
        #region[    동작 - FormBackdrop(Border)    ]
        /**
         * [백드랍 정책]
         * 각종 업데이트 유발 지점 -> UpdateBackdrop -> 백드랍 갱신!
         * 
         *  1. 각 유발 지점은 naive하게 UpdateBackdrop() 호출
         *   - 코드 길이 감소, 유지보수 용이성 증대
         *  2. UpdateBackdrop에서 가능한 모든 유효성 검사 수행
         *  
         * [버퍼 최적화] 
         * 버퍼는 작아지는 방향으로 allocation이 일어나지 않음.
         *  - 메모리 allocation call 감소
         */
        private CodaFormBackdropStyle m_FormBackdropStyle = CodaFormBackdropStyle.NeonGlow;
        private CodaFormBackdropStyle GetFormBackdropStyle() => m_FormBackdropStyle;
        private void SetFormBackdropStyle(CodaFormBackdropStyle value)
        {
            if (m_FormBackdropStyle.Equals(value)) return;
            m_FormBackdropStyle = value;
            UpdateFormBackdrop();
        }


        private Color m_FormBorderGlowColor = Color.FromArgb(0, 122, 204);
        private Color GetFormBorderGlowColor() => m_FormBorderGlowColor;
        private void SetFormBorderGlowColor(Color value)
        {
            if (m_FormBorderGlowColor.Equals(value)) return;
            m_FormBorderGlowColor = value;
            UpdateFormBackdrop();
        }


        private Padding m_FormBorderThickness = new Padding(10);
        private Padding GetFormBorderThickness() => m_FormBorderThickness;
        private void SetFormBorderThickness(Padding value)
        {
            if (m_FormBorderThickness.Equals(value)) return;
            m_FormBorderThickness = value;
            UpdateFormBackdrop();
        }


        /// <summary>
        /// 백드랍 참조용 변수
        /// </summary>
        private CodaFormBackdrop m_FormBackdrop = null;
        /// <summary>
        /// 백드랍 버퍼 참조용 변수
        /// </summary>
        private Bitmap m_FormBackdropBitmap = null;
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            if (DesignMode) return;

            m_FormBackdrop?.Close();
        }


        private void UpdateFormBackdrop()
        {
            if (m_FormBackdrop == null
                || m_FormBackdrop.IsDisposed
                || m_FormBackdrop.Disposing) return;

            int width = Width + m_FormBorderThickness.Left + m_FormBorderThickness.Right;
            int height = Height + m_FormBorderThickness.Top + m_FormBorderThickness.Bottom;

            Win32.SetWindowPos(
                m_FormBackdrop.Handle,
                base.Handle,
                base.Left - m_FormBorderThickness.Left,
                base.Top - m_FormBorderThickness.Top,
                width,
                height,
                Win32.SetWindowPosFlags.DoNotActivate);

            bool _visibility = true;
            if (!Visible || WindowState == FormWindowState.Maximized)
                _visibility = false;
            if (m_FormBackdrop.Visible != _visibility)
                m_FormBackdrop.Visible = _visibility;

            // 버퍼 새로고침이 필요한가?
            bool isBufferReallocationRequired = false;
            if (m_FormBackdropBitmap == null)
            {
                isBufferReallocationRequired = true;
            }
            else
            {
                if (m_FormBackdropBitmap.Width < width || m_FormBackdropBitmap.Height < height)
                    isBufferReallocationRequired = true;
            }

            // 결국 버퍼 새로고침이 필요함!
            if (isBufferReallocationRequired)
            {
                if (m_FormBackdropBitmap != null)
                    m_FormBackdropBitmap.Dispose();

                Bitmap buffer;
                buffer = new Bitmap(width, height, PixelFormat.Format32bppArgb);
                m_FormBackdropBitmap = buffer;
            }

            using (Graphics g = Graphics.FromImage(m_FormBackdropBitmap))
            {
                switch (m_FormBackdropStyle)
                {
                    case CodaFormBackdropStyle.None:
                        g.Clear(Color.Transparent);
                        break;
                    case CodaFormBackdropStyle.Shadow:
                        FormBackdropPaintShadow(g, width, height);
                        break;
                    case CodaFormBackdropStyle.NeonGlow:
                        FormBackdropPaintNeonGlow(g, width, height);
                        break;
                    case CodaFormBackdropStyle.Custom:
                        OnPaintFormBorder(g, width, height);
                        break;
                }
            }
            m_FormBackdrop.SetBitmap(m_FormBackdropBitmap, 255);
        }
        private void FormBackdropPaintNeonGlow(Graphics g, int width, int height)
        {
            g.Clear(Color.Transparent);
            var borderImage = WindowAuras.Glow._AuraBitmap;

            var cm = new ColorMatrix(new float[][]
            {
              new float[] { m_FormBorderGlowColor.R/ 255F, 0, 0, 0, 0},
              new float[] {0, m_FormBorderGlowColor.G / 255F, 0, 0, 0},
              new float[] {0, 0, m_FormBorderGlowColor.B / 255F, 0, 0},
              new float[] {0, 0, 0, 1, 0},
              new float[] {0, 0, 0, 0, 1 }
            });
            var ia = new ImageAttributes();
            ia.SetColorMatrix(cm);

            g.DrawImage(borderImage, new Rectangle(0, 0, 10, 10), 0, 0, 10, 10, GraphicsUnit.Pixel, ia);
            g.DrawImage(borderImage, new Rectangle(width - 10, 0, 10, 10), 20, 0, 10, 10, GraphicsUnit.Pixel, ia);
            g.DrawImage(borderImage, new Rectangle(0, height - 10, 10, 10), 0, 20, 10, 10, GraphicsUnit.Pixel, ia);
            g.DrawImage(borderImage, new Rectangle(width - 10, height - 10, 10, 10), 20, 20, 10, 10, GraphicsUnit.Pixel, ia);

            g.DrawImage(borderImage, new Rectangle(10, 0, width - 20, 10), 10, 0, 10, 10, GraphicsUnit.Pixel, ia);
            g.DrawImage(borderImage, new Rectangle(10, height - 10, width - 20, 10), 10, 20, 10, 10, GraphicsUnit.Pixel, ia);

            g.DrawImage(borderImage, new Rectangle(0, 10, 10, height - 20), 0, 10, 10, 10, GraphicsUnit.Pixel, ia);
            g.DrawImage(borderImage, new Rectangle(width - 10, 10, 10, height - 20), 20, 10, 10, 10, GraphicsUnit.Pixel, ia);
        }
        private void FormBackdropPaintShadow(Graphics g, int width, int height)
        {
            g.Clear(Color.Transparent);
            var borderImage = WindowAuras.Shadow._AuraBitmap;

            var cm = new ColorMatrix(new float[][]
            {
              new float[] { 0, 0, 0, 0, 0},
              new float[] {0, 0, 0, 0, 0},
              new float[] {0, 0, 0, 0, 0},
              new float[] {0, 0, 0, 1, 0},
              new float[] {0, 0, 0, 0, 1 }
            });
            var ia = new ImageAttributes();
            ia.SetColorMatrix(cm);

            g.DrawImage(borderImage, new Rectangle(0, 0, 10, 10), 0, 0, 10, 10, GraphicsUnit.Pixel, ia);
            g.DrawImage(borderImage, new Rectangle(width - 10, 0, 10, 10), 20, 0, 10, 10, GraphicsUnit.Pixel, ia);
            g.DrawImage(borderImage, new Rectangle(0, height - 10, 10, 10), 0, 20, 10, 10, GraphicsUnit.Pixel, ia);
            g.DrawImage(borderImage, new Rectangle(width - 10, height - 10, 10, 10), 20, 20, 10, 10, GraphicsUnit.Pixel, ia);

            g.DrawImage(borderImage, new Rectangle(10, 0, width - 20, 10), 10, 0, 10, 10, GraphicsUnit.Pixel, ia);
            g.DrawImage(borderImage, new Rectangle(10, height - 10, width - 20, 10), 10, 20, 10, 10, GraphicsUnit.Pixel, ia);

            g.DrawImage(borderImage, new Rectangle(0, 10, 10, height - 20), 0, 10, 10, 10, GraphicsUnit.Pixel, ia);
            g.DrawImage(borderImage, new Rectangle(width - 10, 10, 10, height - 20), 20, 10, 10, 10, GraphicsUnit.Pixel, ia);
        }
        /// <summary>
        /// 폼 테두리를 그릴 사용자 정의 방식을 구현합니다.
        /// </summary>
        /// <param name="g">그래픽 개체.</param>
        /// <param name="width">새로 그릴 폼 테두리 영역 폭.</param>
        /// <param name="height">새로 그릴 폼 테두리 영역 높이.</param>
        protected virtual void OnPaintFormBorder(Graphics g, int width, int height)
        {
            g.Clear(Color.Transparent);
        }
        void InitializeFormBackdrop()
        {
            m_FormBackdrop = new CodaFormBackdrop();
            m_FormBackdrop.Show(this);
            UpdateFormBackdrop();
        }
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
                        if (!m_ClientControlBox) break;
                        if (!base.MaximizeBox) break;
                        m.Result = (IntPtr)Win32.HitTestValues.HTCAPTION;
                        return;
                    case Win32.WindowMessages.NCRBUTTONUP:
                        if (!m_ClientSystemMenu) break;
                        if (!base.MaximizeBox) break;
                        SystemMenu.Show(Handle);
                        return;
                    case Win32.WindowMessages.WINDOWPOSCHANGED:
                        base.WndProc(ref m);
                        UpdateFormBackdrop();
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
            base.FormBorderStyle = (m_ClientResizable)
                ? FormBorderStyle.Sizable
                : FormBorderStyle.None;
        }
        protected override void OnLoad(EventArgs e)
        {
            if (DesignMode)
            {
                InitializeDesignModeSettings();
            }
            else
            {
                InitializeNonDesignModeSettings();
                InitializeSizeGrips();
                InitializeFormBackdrop();
            }

            m_Loaded = true;

            base.OnLoad(e);
        }
        #endregion

    }
    #endregion
    //
    // 내부 클래스 CodaFormBackdrop
    //
    #region[    CLASS : *CodaFormBackdrop    ]
    /// <summary>
    /// 폼 백드롭 효과에 사용됩니다.
    /// </summary>
    internal class CodaFormBackdrop : Form
    {
        protected internal CodaFormBackdrop()
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


        public void SetBitmap(Bitmap bitmap, byte opacity)
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


        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x00080000; // This form has to have the WS_EX_LAYERED extended style
                return cp;
            }
        }
    }
    #endregion
    //
    // 내부 클래스 CodaFormResizer
    //
    #region[    CLASS : *CodaFormResizer    ]
    /// <summary>
    /// 컨트롤 크기 조절기 방향.
    /// </summary>
    internal enum CodaFormResizerDirection
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
    internal class CodaFormResizer : Panel
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
        [DefaultValue(typeof(CodaFormResizerDirection), "Top")]
        public CodaFormResizerDirection Direction
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
        private CodaFormResizerDirection m_ResizerDirection;


        public CodaFormResizer()
        {
            m_BorderSize = new Padding(6);
            m_ResizerDirection = CodaFormResizerDirection.Top;
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
                case CodaFormResizerDirection.Top:
                    SetBounds(
bound.Left + m_BorderSize.Left, 0,
bound.Width - m_BorderSize.Left - m_BorderSize.Right, m_BorderSize.Top
); break;
                case CodaFormResizerDirection.TopLeft:
                    SetBounds(
0, 0,
m_BorderSize.Left, m_BorderSize.Top
); break;
                case CodaFormResizerDirection.TopRight:
                    SetBounds(
bound.Right - m_BorderSize.Right, 0,
m_BorderSize.Right, m_BorderSize.Top
); break;
                case CodaFormResizerDirection.Left:
                    SetBounds(
0, m_BorderSize.Top,
m_BorderSize.Left, bound.Height - m_BorderSize.Top - m_BorderSize.Bottom
); break;
                case CodaFormResizerDirection.Right:
                    SetBounds(
bound.Right - m_BorderSize.Right, m_BorderSize.Top,
m_BorderSize.Right, bound.Height - m_BorderSize.Top - m_BorderSize.Bottom
); break;
                case CodaFormResizerDirection.Bottom:
                    SetBounds(
m_BorderSize.Left, bound.Bottom - m_BorderSize.Bottom,
bound.Width - m_BorderSize.Left - m_BorderSize.Right, m_BorderSize.Bottom
); break;
                case CodaFormResizerDirection.BottomLeft:
                    SetBounds(
0, bound.Bottom - m_BorderSize.Bottom,
m_BorderSize.Left, m_BorderSize.Bottom
); break;
                case CodaFormResizerDirection.BottomRight:
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
                        case CodaFormResizerDirection.Top:
                        case CodaFormResizerDirection.Bottom:
                            result = Cursors.SizeNS;
                            break;
                        case CodaFormResizerDirection.Left:
                        case CodaFormResizerDirection.Right:
                            result = Cursors.SizeWE;
                            break;
                        case CodaFormResizerDirection.TopLeft:
                        case CodaFormResizerDirection.BottomRight:
                            result = Cursors.SizeNWSE;
                            break;
                        case CodaFormResizerDirection.TopRight:
                        case CodaFormResizerDirection.BottomLeft:
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
                case CodaFormResizerDirection.Top:
                case CodaFormResizerDirection.Bottom:
                    Anchor = (m_ResizerDirection == CodaFormResizerDirection.Top)
                        ? (AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top)
                        : (AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom);
                    break;
                case CodaFormResizerDirection.Left:
                case CodaFormResizerDirection.Right:
                    Anchor = (m_ResizerDirection == CodaFormResizerDirection.Left)
                        ? (AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom)
                        : (AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom);
                    break;
                case CodaFormResizerDirection.TopLeft:
                case CodaFormResizerDirection.BottomRight:
                    Anchor = (m_ResizerDirection == CodaFormResizerDirection.TopLeft)
                        ? (AnchorStyles.Left | AnchorStyles.Top)
                        : (AnchorStyles.Right | AnchorStyles.Bottom);
                    break;
                case CodaFormResizerDirection.TopRight:
                case CodaFormResizerDirection.BottomLeft:
                    Anchor = (m_ResizerDirection == CodaFormResizerDirection.TopRight)
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
                case CodaFormResizerDirection.Top: msg = Win32.HitTestValues.HTTOP; break;
                case CodaFormResizerDirection.TopLeft: msg = Win32.HitTestValues.HTTOPLEFT; break;
                case CodaFormResizerDirection.TopRight: msg = Win32.HitTestValues.HTTOPRIGHT; break;
                case CodaFormResizerDirection.Left: msg = Win32.HitTestValues.HTLEFT; break;
                case CodaFormResizerDirection.Right: msg = Win32.HitTestValues.HTRIGHT; break;
                case CodaFormResizerDirection.Bottom: msg = Win32.HitTestValues.HTBOTTOM; break;
                case CodaFormResizerDirection.BottomLeft: msg = Win32.HitTestValues.HTBOTTOMLEFT; break;
                case CodaFormResizerDirection.BottomRight: msg = Win32.HitTestValues.HTBOTTOMRIGHT; break;
                default: return;
            }
            Win32.SendMessage(form.Handle, 0xA1, (int)msg, 0);
        }
    }
    #endregion
}
