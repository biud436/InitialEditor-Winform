﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using DarkUI.Config;

namespace Editor
{
    interface IRenderCommand
    {
        void Execute(Graphics g, object[] args);
    }

}
