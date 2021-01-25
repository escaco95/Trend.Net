using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Trend
{
    /// <summary>
    /// 트렌디한 디자인의 버튼.
    /// </summary>
    public class Button : System.Windows.Forms.Button
    {
        /// <summary>
        /// 새 버튼 개체를 생성합니다.
        /// </summary>
        public Button()
        {
            this.FlatStyle = FlatStyle.Flat;
            this.FlatAppearance.BorderSize = 0;
        }


        [DefaultValue(typeof(FlatStyle),"Flat")]
        public new FlatStyle FlatStyle
        {
            get => base.FlatStyle;
            set => base.FlatStyle = value;
        }

    }
}
